﻿using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;

using HarmonyLib;

using TerminalApi.Events;

namespace ScanPlus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi")]
    [BepInDependency("TerminalFormatter", BepInDependency.DependencyFlags.SoftDependency)]
    public class ScanPlus : BaseUnityPlugin
    {
        public static ScanPlus Instance { get; private set; }
        private static ManualLogSource _log = null!;
        internal ConfigManager _configManager;
        internal UnlockableManager _unlockableManager;
        internal Scanner _scanner;
        private static Harmony harmony = new(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            _log = Logger;

            _configManager = new ConfigManager(Config);
            _configManager.LoadConfigurations();

            if (_configManager.ShipUpgrade == true)
            {
                _unlockableManager = new UnlockableManager();
                _unlockableManager.AddScannerToStore(_configManager.UpgradePrice);
            }
            
            _scanner = new Scanner(_configManager.DetailLevel);

            if (Chainloader.PluginInfos.ContainsKey("TerminalFormatter"))
            {
                Logger.LogInfo($"{PluginInfo.PLUGIN_GUID}: applying compatibility patch for TerminalFormatter");
                
                var original = AccessTools.Method(typeof(Terminal), "TextPostProcess");
                var postfix = new HarmonyMethod(typeof(TFCompatibility).GetMethod("TextPostProcessPrefixPostFix"));
            
                harmony.Patch(original, null, postfix);
            }

            Events.TerminalParsedSentence += OnTerminalParsedSentence;

            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loaded");
        }

        private void OnTerminalParsedSentence(object sender, Events.TerminalParseSentenceEventArgs e)
        {
            if (e.ReturnedNode.name == "ScanInfo" && _scanner.UseUpgradedScan())
            {
                var delimiter = "\n";
                e.ReturnedNode.displayText = e.ReturnedNode.displayText.Split(delimiter)[0] + delimiter + _scanner.BuildEnemyString();
            }
        }
    }
}
