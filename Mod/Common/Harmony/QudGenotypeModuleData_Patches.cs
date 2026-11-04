/*using HarmonyLib;

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
    public static class QudGenotypeModuleData_Patches
    {
        public static bool doDebug = true;

        [HarmonyPatch(typeof(QudGenotypeModuleData))]
        [HarmonyPatch(new[] { typeof(string) }, new[] { ArgumentType.Normal })]
        [HarmonyPrefix]
        public static void ctor_NoPricklePigs_Prefix(ref string id)
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
                MetricsManager.LogException($"[{MOD_ID}] {nameof(QudGenotypeModuleData_Patches)}.{nameof(ctor_NoPricklePigs_Prefix)}()", x);
            }
        }
    }
}*/