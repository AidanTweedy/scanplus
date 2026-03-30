using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;

namespace ScanPlus
{
    public class ConfigManager
    {
        private ConfigEntry<int> _detailLevel;
        private ConfigEntry<bool> _shipUpgrade;
        private ConfigEntry<int> _upgradePrice;
        private ConfigEntry<string> _commandStrings;

        public int DetailLevel => _detailLevel.Value;
        public bool ShipUpgrade => _shipUpgrade.Value;
        public int UpgradePrice => _upgradePrice.Value;
        public HashSet<string> CommandStrings =>
            _commandStrings.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Utils.NormalizeCommand)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToHashSet();

        private ConfigFile _config;

        public ConfigManager(ConfigFile config)
        {
            _config = config;
        }

        private static readonly string[] ScannerCommands =
        {
            "irscan",
            "infrared",
            "infraredscanner",
            "scanlife",
            "scanenemy",
        };

        public void LoadConfigurations()
        {
            _detailLevel = _config.Bind("DETAIL", "DetailLevel", 2, "Life Scan Detail -> | 0 = LOW | 1 = MEDIUM | 2 = HIGH | 3 = EXCESSIVE");
            _shipUpgrade = _config.Bind("PREFERENCES", "ShipUpgrade", true, "Enable Ship Upgrade -> | false = No | true = Yes");
            _upgradePrice = _config.Bind("PREFERENCES", "UpgradePrice", 300, "Ship Scanner Upgrade Cost");
            _commandStrings = _config.Bind("PREFERENCES", "CommandStrings", string.Join(",", ScannerCommands), "Comma-Separated Terminal Commands to Activate IR Scanner");
        }
    }
}
