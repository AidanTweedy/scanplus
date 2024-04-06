﻿﻿using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;

using HarmonyLib;

namespace ScanPlus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TerminalFormatter", BepInDependency.DependencyFlags.SoftDependency)]
    public class ScanPlus : BaseUnityPlugin
    {
        public static ScanPlus Instance { get; private set; }
        internal static ManualLogSource Log = null!;
        
        internal static Harmony Harmony = new(PluginInfo.PLUGIN_GUID);
        private ConfigManager _configManager;
        private UnlockableManager _unlockableManager;
        internal Scanner _scanner;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            _configManager = new ConfigManager(Config);
            _configManager.LoadConfigurations();

            _unlockableManager = new UnlockableManager(_configManager);

            _scanner = new Scanner(_configManager, _unlockableManager);

            Harmony.PatchAll(typeof(ScanPlus));

            if (Chainloader.PluginInfos.ContainsKey("TerminalFormatter"))
                Harmony.PatchAll(typeof(TFCompatibility));

            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loaded");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
        public static void Patch_ParsePlayerSentence(ref TerminalNode __result)
        {
            if (__result.name == "ScanInfo" && Instance._scanner.UseUpgradedScan())
            {
                var delimiter = "\n";
                __result.displayText = __result.displayText.Split(delimiter)[0] + delimiter + Instance._scanner.BuildEnemyString();
            }
        }
    }
}
