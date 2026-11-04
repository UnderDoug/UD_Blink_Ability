using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using XRL;
using XRL.CharacterBuilds.Qud;
using XRL.CharacterBuilds.Qud.UI;
using XRL.Rules;
using XRL.UI.Framework;
using XRL.World;
using XRL.World.Loaders;

using static UD_Blink_Mutation.Const;
using static UD_Blink_Mutation.Utils;

namespace UD_Blink_Mutation.Harmony
{
    [HarmonyPatch]
    public static class QudGenotypeModuleWindow_Patches
    {
        public static bool Success { get; private set; }

        public static bool doDebug = true;

        private static bool IsMethodType(Type Type)
        {
            return Type.Name.Contains(nameof(QudGenotypeModuleWindow.GetSelections))
                && AccessTools
                    .GetDeclaredMethods(Type)
                    .Any(m => m.Name == "MoveNext");
        }

        private static Type MethodType => AccessTools.FirstInner(typeof(QudGenotypeModuleWindow), t => IsMethodType(t));

        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
            => AccessTools.Method(
                type: MethodType,
                name: "MoveNext")
            ;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GetSelections_NoPricklePigs_Transpiler(
            IEnumerable<CodeInstruction> Instructions,
            ILGenerator Generator
            )
        {
            string patchMethodName = $"{nameof(QudGenotypeModuleWindow)}.{nameof(QudGenotypeModuleWindow.GetSelections)}()";
            int metricsCheckSteps = 0;

            CodeMatcher codeMatcher = new(Instructions, Generator);

            if (!codeMatcher.Start().MatchStartForward(
                matches: new CodeMatch[]
                {
                    new(OpCodes.Br),
                })
                .IsValid)
            {
                ThisMod.Error($"{patchMethodName}: ({metricsCheckSteps}) {nameof(CodeMatcher.MatchStartForward)} failed to find instructions {OpCodes.Br}");
                return Instructions;
            }
            metricsCheckSteps++;

            if (codeMatcher.Instruction.operand is not Label continueLabel)
            {
                ThisMod.Error($"{patchMethodName}: ({metricsCheckSteps}) {nameof(codeMatcher.Instruction)} failed to cast to {nameof(Label)}");
                return Instructions;
            }
            metricsCheckSteps++;

            if (!codeMatcher.Start().MatchEndForward(new CodeMatch(OpCodes.Stloc_3)).Advance(1).IsValid)
            {
                ThisMod.Error($"{patchMethodName}: ({metricsCheckSteps}) {nameof(CodeMatcher.MatchEndForward)} failed to find instructions {OpCodes.Stloc_3}");
                return Instructions;
            }
            metricsCheckSteps++;

            return codeMatcher
                .Insert(instructions: new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Ldfld, AccessTools.Field(typeof(GenotypeEntry), nameof(GenotypeEntry.CharacterBuilderModules))),
                    new(OpCodes.Ldstr, "NonePlease"),
                    new(OpCodes.Call, AccessTools.Method(typeof(string), "op_Equality", new Type[] { typeof(string), typeof(string) })),
                    new(OpCodes.Brtrue, continueLabel)
                })
                .InstructionEnumeration()
                ;
        }

        [HarmonyCleanup]
        public static Exception GetSelections_NoPricklePigs_Cleanup(MethodBase OriginalMethod, Exception Exception)
        {
            if (OriginalMethod == null)
                return Exception;

            string patchMethodName = $"{nameof(QudGenotypeModuleWindow_Patches)}.{nameof(QudGenotypeModuleWindow.GetSelections)}";

            if (Exception != null)
            {
                Success = false;
                ThisMod.Warn($"Failed to transpile {patchMethodName}{(doDebug ? $": {Exception}" : null)}");
                return null;
            }

            Success = true;
            MetricsManager.LogModInfo(ThisMod, $"Successfully transpiled {patchMethodName}");
            return null;
        }
    }
}
