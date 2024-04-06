using HarmonyLib;

namespace ScanPlus
{
    [HarmonyPatch(typeof(Terminal))]
    public class TFCompatibility
    {
        [HarmonyPostfix, HarmonyPatch("TextPostProcess")]
        public static void TextPostProcessPrefixPostFix(string modifiedDisplayText, TerminalNode node, Terminal __instance)
        {
            if (node.name == "ScanInfo" && ScanPlus.Instance._scanner.UseUpgradedScan() && !__instance.currentText.ToLower().Contains("threat"))
            {
                __instance.currentText += ScanPlus.Instance._scanner.BuildEnemyString();
            }
        }
    }
}
