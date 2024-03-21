﻿using System;
using System.Data;
using System.Linq;
using System.Text;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using LethalLib.Modules;

using TerminalApi.Events;

namespace ScanPlus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("atomic.terminalapi")]
    public class ScanPlus : BaseUnityPlugin
    {
        private static ManualLogSource _log = null!;
        private static ConfigEntry<int>
            config_DetailLevel,
            config_ShipUpgrade;
        private static int m_detailLevel;
        private static bool m_shipUpgrade;

        private static Unlockables.RegisteredUnlockable scanner;

        private const string DefaultString = "\nNo life detected.\n\n";

        private const string UpgradeName = "Infrared Scanner";
        private const string UpgradeInfo = "\nUpgrades the ship's scanner with an infrared sensor, allowing for the detection of lifeforms present on the current moon.\n";

        private void Awake()
        {
            _log = Logger;

            ConfigFile();

            Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} is loaded");

            Events.TerminalParsedSentence += OnTerminalParsedSentence;

            if (!m_shipUpgrade)
                return;

            var scanUpgrade = new UnlockableItem
            {
                unlockableName = UpgradeName,
                spawnPrefab = false,
                alwaysInStock = true,
                prefabObject = null,
                unlockableType = 1,
                IsPlaceable = false,
                maxNumber = 1,
                canBeStored = false,
                alreadyUnlocked = false
            };

            Unlockables.RegisterUnlockable(scanUpgrade , storeType: StoreType.ShipUpgrade, price: 1);
            scanner = Unlockables.registeredUnlockables.Find(u => u.unlockable.unlockableName == UpgradeName);
            

            TerminalNode infoNode = TerminalApi.TerminalApi.CreateTerminalNode(displayText: UpgradeInfo, clearPreviousText: true);
            
            scanner.itemInfo = infoNode;
        }

        private void ConfigFile()
        {
            //Setup Config File
            config_DetailLevel = Config.Bind("DETAIL", "DetailLevel", 2, "Life Scan Detail -> | 0 = LOW | 1 = MEDIUM | 2 = HIGH | 3 = EXCESSIVE");
            config_ShipUpgrade = Config.Bind("PREFERENCES", "ShipUpgrade", 1, "Enable Ship Upgrade -> | 0 = false | 1 = true");

            //Load Config File
            m_detailLevel = config_DetailLevel.Value;
            m_shipUpgrade = Convert.ToBoolean(config_ShipUpgrade.Value);
        }

        private static void OnTerminalParsedSentence(object sender, Events.TerminalParseSentenceEventArgs e)
        {
            if (e.ReturnedNode.name != "ScanInfo")
                return;

            if (m_shipUpgrade && !scanner.unlockable.hasBeenUnlockedByPlayer)
                return;

            e.ReturnedNode.displayText = e.ReturnedNode.displayText.Split('\n')[0] + '\n' + BuildEnemyCountString();
        }

        private static string BuildEnemyCountString()
        {
            var entities = FindObjectsOfType<EnemyAI>()
                .Where(ai => ai.GetComponentInChildren<ScanNodeProperties>() is not null)
                .GroupBy(ai => ai.enemyType.enemyName)
                .OrderBy(g => g.Key)
                .ToList();

            if (entities.Count == 0)
                return DefaultString;

            StringBuilder sb = new();
            sb.Append('\n');

            int numEntities = RoundManager.Instance.numberOfEnemiesInScene;
            int threatLevel = RoundManager.Instance.currentEnemyPower;
            int maxThreatLevel = RoundManager.Instance.currentLevel.maxEnemyPowerCount;
            string relativeThreat = $"{(float)threatLevel / maxThreatLevel * 100:N1}%";

            switch (m_detailLevel)
            {
                case 0:
                    sb.Append($"Threat level: {relativeThreat}");
                    break;
                case 1:
                    sb.Append($"{numEntities} lifeforms have been detected, totalling a threat level of {threatLevel}.");
                    break;
                case 2:
                    sb.Append("Life detected:");
                    foreach (var group in entities)
                    {
                        sb.AppendLine($"\n  {group.Key}: {group.Count()}");
                    }
                    break;
                case 3:
                    sb.Append($"Threat level: {relativeThreat}\n\n{numEntities} lifeforms detected, totalling a threat level of {threatLevel}.\n\nLife detected:");
                    foreach (var group in entities)
                    {
                        sb.AppendLine($"\n  {group.Key}: {group.Count()}");
                    }
                    break;
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
