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
        internal static ManualLogSource Log = null!;
        
        internal static Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        private ConfigManager _configManager;
        private UnlockableManager _unlockableManager;
        internal Scanner _scanner;

        private void Awake()
        {
            Log = Logger;

            _configManager = new ConfigManager(Config);
            _configManager.LoadConfigurations();

            _unlockableManager = new UnlockableManager(_configManager);

            _scanner = new Scanner(_configManager, _unlockableManager);

            if (Chainloader.PluginInfos.ContainsKey("TerminalFormatter"))
                harmony.PatchAll(typeof(TFCompatibility));

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
