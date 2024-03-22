using HarmonyLib;

namespace ScanPlus
{
    [HarmonyPatch(typeof(Terminal))]
    public class TFCompatibility
    {
        [HarmonyPostfix, HarmonyPriority(Priority.Last)]
        [HarmonyPatch("TextPostProcess")]
        private static void TextPostProcessPrefixPostFix(string modifiedDisplayText, TerminalNode node, Terminal __instance)
        {
            if (node.name == "ScanInfo")
            {
                __instance.currentText += ScanPlus.BuildEnemyCountString();
            }
        }
    }
}
