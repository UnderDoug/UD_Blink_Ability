using HarmonyLib;

using System;
using System.Collections.Generic;

using XRL;
using XRL.CharacterBuilds.Qud;
using XRL.Rules;
using XRL.World;
using XRL.World.Loaders;

using static UD_Blink_Mutation.Const;
using static UD_Blink_Mutation.Utils;

namespace UD_Blink_Mutation.Harmony
{
    [HarmonyPatch]
    public static class QudGenotypeModule_Patches
    {
        public static bool doDebug = true;

        [HarmonyPatch(
            declaringType: typeof(QudGenotypeModule),
            methodName: nameof(QudGenotypeModule.handleUIEvent),
            argumentTypes: new Type[]
            {
                typeof(string),
                typeof(object)
            },
            argumentVariations: new ArgumentType[]
            {
                ArgumentType.Normal,
                ArgumentType.Normal
            })]
        [HarmonyPrefix]
        public static void handleUIEvent_NoPricklePigs_Prefix(ref QudGenotypeModule __instance, ref object __result)
        {
            try
            {
                if (__instance.data?.Entry is GenotypeEntry genotypeEntry
                    && genotypeEntry.CharacterBuilderModules == "NonePlease")
                    __instance.data.Genotype = "Mutated Human";
            }
            catch (Exception x)
            {
                MetricsManager.LogException(
                    $"[{MOD_ID}] {nameof(QudGenotypeModule_Patches)}." +
                    $"{nameof(handleUIEvent_NoPricklePigs_Prefix)}()",
                    x);
            }
        }

        [HarmonyPatch(
            declaringType: typeof(QudGenotypeModule),
            methodName: nameof(QudGenotypeModule.getSelected))]
        [HarmonyPostfix]
        public static void getSelected_NoPricklePigs_Prefix(ref QudGenotypeModule __instance, ref string __result)
        {
            try
            {
                if (GenotypeFactory.GetGenotypeEntry(__result) is GenotypeEntry genotypeEntry
                    && genotypeEntry.CharacterBuilderModules == "NonePlease")
                    __result = "Mutated Human";
            }
            catch (Exception x)
            {
                MetricsManager.LogException(
                    $"[{MOD_ID}] {nameof(QudGenotypeModule_Patches)}." +
                    $"{nameof(getSelected_NoPricklePigs_Prefix)}()",
                    x);
            }
        }
    }
}
