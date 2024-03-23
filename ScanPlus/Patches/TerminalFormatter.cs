using HarmonyLib;

namespace ScanPlus
{
    [HarmonyPatch(typeof(Terminal))]
    public class TFCompatibility
    {
        [HarmonyPatch("TextPostProcess")]
        public static void TextPostProcessPrefixPostFix(string modifiedDisplayText, TerminalNode node, Terminal __instance)
        {
            if (node.name == "ScanInfo" && ScanPlus.UseUpgradedScan())
            {
                __instance.currentText += ScanPlus.BuildEnemyCountString();
            }
        }
    }
}
