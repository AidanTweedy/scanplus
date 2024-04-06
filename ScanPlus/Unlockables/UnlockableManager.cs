using System;
using System.Linq;

using HarmonyLib;

using UnityEngine;

namespace ScanPlus
{
    public class UnlockableManager
    {
        public static UnlockableManager Instance { get; private set; }
        private ConfigManager ConfigManager;
        private const string UpgradeName = "Infrared Scanner";
        private const string UpgradeInfo = "\nUpgrades the ship's scanner with an infrared sensor, allowing for the detection of lifeforms present on the current moon.\n";
        private int UpgradePrice;
        internal UnlockableItem ScanUpgrade = new UnlockableItem
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
        
        public UnlockableManager(ConfigManager _configManager)
        {
            Instance = this;
            ConfigManager = _configManager;

            if (ConfigManager.ShipUpgrade == true)
            {
                ScanPlus.Log.LogInfo($"{PluginInfo.PLUGIN_GUID}: adding scanner unlockable to store.");
                ScanPlus.Harmony.PatchAll(typeof(UnlockableManager));
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Terminal), "Awake")]
        public static void Patch_TerminalAwake(Terminal __instance)
        {
            try
            {
                Instance?.AddScannerToStore(__instance);
            } catch(Exception e)
            {
               ScanPlus.Log.LogInfo($"{PluginInfo.PLUGIN_GUID}: error occurred registering scanner upgrade: {e}"); 
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Terminal), "TextPostProcess")]
        public static void Patch_TextPostProcess(ref string modifiedDisplayText, TerminalNode node)
        {
            try
            {
                if (modifiedDisplayText.Contains("[buyableItemsList]") && modifiedDisplayText.Contains("[unlockablesSelectionList]")) {
                    int index = modifiedDisplayText.IndexOf(@":");

                    string upgradeLine = $"\n* {UpgradeName}    //    Price: ${Instance.UpgradePrice}";
                    modifiedDisplayText = modifiedDisplayText.Insert(index + 1, upgradeLine);
                }

            } catch(Exception e)
            {
                ScanPlus.Log.LogError(e);
            }
            
        }

        private static TerminalKeyword CreateKeyword(string word, TerminalKeyword defaultVerb)
        {
            TerminalKeyword keyword = ScriptableObject.CreateInstance<TerminalKeyword>();
            keyword.name = word;
            keyword.word = word;
            keyword.isVerb = false;
            keyword.accessTerminalObjects = false;
            keyword.defaultVerb = defaultVerb;

            return keyword;
        }  

        public void AddScannerToStore(Terminal terminal)
        {
            int scannerUnlockableID = -1;

            UnlockablesList unlockablesList = StartOfRound.Instance.unlockablesList;
            int index = unlockablesList.unlockables.FindIndex(unlockable => unlockable.unlockableName == UpgradeName);

            if (index == -1)
            {

                var buyKeyword = terminal.terminalNodes.allKeywords.First(keyword => keyword.word == "buy");
                var cancelPurchaseNode = buyKeyword.compatibleNouns[0].result.terminalOptions[1].result;
                var infoKeyword = terminal.terminalNodes.allKeywords.First(keyword => keyword.word == "info");

                var keyword = CreateKeyword($"{UpgradeName.ToLowerInvariant().Replace(" ", "")}", buyKeyword);

                UpgradePrice = ConfigManager.UpgradePrice;

                unlockablesList.unlockables.Capacity++;
                unlockablesList.unlockables.Add(ScanUpgrade);
                scannerUnlockableID = unlockablesList.unlockables.FindIndex(unlockable => unlockable.unlockableName == UpgradeName);

                ScanPlus.Log.LogInfo($"{UpgradeName} added to unlockable list at index {scannerUnlockableID}");

                TerminalNode buyNode2 = ScriptableObject.CreateInstance<TerminalNode>();
                buyNode2.name = $"{UpgradeName.Replace(" ", "-")}BuyNode2";
                buyNode2.displayText = $"Ordered {UpgradeName}! Your new balance is [playerCredits].\n\n";
                buyNode2.clearPreviousText = true;
                buyNode2.maxCharactersToType = 15;
                buyNode2.buyItemIndex = -1;
                buyNode2.shipUnlockableID = scannerUnlockableID;
                buyNode2.buyUnlockable = true;
                buyNode2.creatureName = UpgradeName;
                buyNode2.isConfirmationNode = false;
                buyNode2.itemCost = UpgradePrice;

                TerminalNode buyNode1 = ScriptableObject.CreateInstance<TerminalNode>();
                buyNode1.name = $"{UpgradeName.Replace(" ", "-")}BuyNode1";
                buyNode1.displayText = $"You have requested to order {UpgradeName}.\nTotal cost of item: [totalCost].\n\nPlease CONFIRM or DENY.\n\n";
                buyNode1.clearPreviousText = true;
                buyNode1.maxCharactersToType = 15;
                buyNode1.shipUnlockableID = scannerUnlockableID;
                buyNode1.itemCost = UpgradePrice;
                buyNode1.creatureName = UpgradeName;
                buyNode1.overrideOptions = true;
                buyNode1.terminalOptions = new[]
                {
                    new CompatibleNoun()
                    {
                        noun = terminal.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "confirm"),
                        result = buyNode2
                    },
                    new CompatibleNoun()
                    {
                        noun = terminal.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "deny"),
                        result = cancelPurchaseNode
                    }
                };

                string infoText = UpgradeInfo;

                TerminalNode itemInfo = ScriptableObject.CreateInstance<TerminalNode>();
                itemInfo.name = $"{UpgradeName.Replace(" ", "-")}InfoNode";
                itemInfo.displayText = infoText;
                itemInfo.clearPreviousText = true;
                itemInfo.maxCharactersToType = 25;

                var allKeywords = terminal.terminalNodes.allKeywords.ToList();
                allKeywords.Add(keyword);
                terminal.terminalNodes.allKeywords = allKeywords.ToArray();

                var nouns = buyKeyword.compatibleNouns.ToList();
                nouns.Add(new CompatibleNoun()
                {
                    noun = keyword,
                    result = buyNode1
                });
                buyKeyword.compatibleNouns = nouns.ToArray();

                var itemInfoNouns = infoKeyword.compatibleNouns.ToList();
                itemInfoNouns.Add(new CompatibleNoun()
                {
                    noun = keyword,
                    result = itemInfo
                });
                infoKeyword.compatibleNouns = itemInfoNouns.ToArray();
               
                ScanPlus.Log.LogInfo($"{PluginInfo.PLUGIN_GUID}: successfully added {UpgradeName} to the store.");
            } else
            {
                scannerUnlockableID = index;
            }
        }
        
        public bool IsScannerUnlocked()
        {
            return ScanUpgrade?.hasBeenUnlockedByPlayer ?? false;
        }
    }
} 
