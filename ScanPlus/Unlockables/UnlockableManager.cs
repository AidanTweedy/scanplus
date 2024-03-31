using LethalLib.Modules;

namespace ScanPlus
{
    public class UnlockableManager
    {
        private const string UpgradeName = "Infrared Scanner";
        private const string UpgradeInfo = "\nUpgrades the ship's scanner with an infrared sensor, allowing for the detection of lifeforms present on the current moon.\n";
        internal Unlockables.RegisteredUnlockable scanner;
        
        public UnlockableManager(){}

        public void AddScannerToStore(int scannerPrice)
        {
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

            Unlockables.RegisterUnlockable(scanUpgrade , storeType: StoreType.ShipUpgrade, price: scannerPrice);
            scanner = Unlockables.registeredUnlockables.Find(u => u.unlockable.unlockableName == UpgradeName);
            

            TerminalNode infoNode = TerminalApi.TerminalApi.CreateTerminalNode(displayText: UpgradeInfo, clearPreviousText: true);
            scanner.itemInfo = infoNode;
        }
        public bool IsScannerUnlocked()
        {
            return scanner?.unlockable?.hasBeenUnlockedByPlayer ?? false;
        }
    }
} 
       