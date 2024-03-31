using BepInEx.Configuration;

namespace ScanPlus
{
    public class ConfigManager
    {
        private ConfigEntry<int> _detailLevel;
        private ConfigEntry<bool> _shipUpgrade;
        private ConfigEntry<int> _upgradePrice;

        public int DetailLevel => _detailLevel.Value;
        public bool ShipUpgrade => _shipUpgrade.Value;
        public int UpgradePrice => _upgradePrice.Value;

        private ConfigFile _config;

        public ConfigManager(ConfigFile config)
        {
            _config = config;
        }

        public void LoadConfigurations()
        {
            _detailLevel = _config.Bind("DETAIL", "DetailLevel", 2, "Life Scan Detail -> | 0 = LOW | 1 = MEDIUM | 2 = HIGH | 3 = EXCESSIVE");
            _shipUpgrade = _config.Bind("PREFERENCES", "ShipUpgrade", true, "Enable Ship Upgrade -> | false = No | true = Yes");
            _upgradePrice = _config.Bind("PREFERENCES", "UpgradePrice", 300, "Ship Scanner Upgrade Cost");
        }
    }
}
