using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

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
                if (__instance.data?.Entry is not GenotypeEntry { CharacterBuilderModules: not "NonePlease" })
                {
                    var newSelection = __instance
                            ?.genotypes
                            ?.Where(g => g.CharacterBuilderModules != "NonePlease")
                            ?.GetRandomElementCosmetic()
                            ?.Name
                        ?? "Mutated Human"
                        ;

                    __instance.SelectGenotype(newSelection);
                }
            }
            catch (Exception x)
            {
                MetricsManager.LogException($"[{MOD_ID}] {nameof(QudGenotypeModule_Patches)}.{nameof(handleUIEvent_NoPricklePigs_Prefix)}()", x);
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
                if (__result != null
                    && GenotypeFactory.GetGenotypeEntry(__result) is GenotypeEntry { CharacterBuilderModules: "NonePlease" })
                {
                    var newSelection = __instance
                            ?.genotypes
                            ?.Where(g => g.CharacterBuilderModules != "NonePlease")
                            ?.GetRandomElementCosmetic()
                            ?.Name
                        ?? "Mutated Human"
                        ;

                    __instance.SelectGenotype(newSelection);
                }
            }
            catch (Exception x)
            {
                MetricsManager.LogException($"[{MOD_ID}] {nameof(QudGenotypeModule_Patches)}.{nameof(getSelected_NoPricklePigs_Prefix)}()", x);
            }
        }

        [HarmonyPatch(
            declaringType: typeof(QudGenotypeModule),
            methodName: nameof(QudGenotypeModule.SelectGenotype),
            argumentTypes: new Type[]
            {
                typeof(string),
            },
            argumentVariations: new ArgumentType[]
            {
                ArgumentType.Normal,
            })]
        [HarmonyPostfix]
        public static void SelectGenotype_NoPricklePigs_Prefix(ref string id)
        {
            try
            {
                if (id != null
                    && GenotypeFactory.GetGenotypeEntry(id) is GenotypeEntry { CharacterBuilderModules: "NonePlease" })
                {
                    id = GenotypeFactory.Genotypes
                            ?.Where(g => g.CharacterBuilderModules != "NonePlease")
                            ?.GetRandomElementCosmetic()
                            ?.Name
                        ?? "Mutated Human"
                        ;
                }
            }
            catch (Exception x)
            {
                MetricsManager.LogException($"[{MOD_ID}] {nameof(QudGenotypeModule_Patches)}.{nameof(SelectGenotype_NoPricklePigs_Prefix)}()", x);
            }
        }
    }
}
