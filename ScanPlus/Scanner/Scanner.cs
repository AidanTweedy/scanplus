using System;

using System.Data;

using System.Linq;

using System.Text;

using TerminalApi.Events;

namespace ScanPlus
{
    public class Scanner
    {
        internal const string DefaultString = "\nNo life detected.\n\n";
        public int DetailLevel { get; private set; }

        public Scanner(int detailLevel)
        {
            this.DetailLevel = detailLevel;
        }
    
        public string BuildEnemyString()
        {
            var entities = RoundManager.Instance.SpawnedEnemies
                .Where(ai => ai.GetComponentInChildren<ScanNodeProperties>() is not null)
                .GroupBy(ai => ai.enemyType.enemyName)
                .OrderBy(g => g.Key)
                .ToList();

            if (entities.Count == 0)
                return DefaultString;

            int numEntities = entities.SelectMany(group => group).Count();
            int threatLevel = entities
                .SelectMany(group => group)
                .Where(ai => ai.enemyType.isDaytimeEnemy == false)
                .Sum(ai => ai.enemyType.PowerLevel);

            int maxThreatLevel = RoundManager.Instance.currentLevel.maxEnemyPowerCount + RoundManager.Instance.currentLevel.maxOutsideEnemyPowerCount;
            float relativeThreat = (float)threatLevel / maxThreatLevel;
            
            var coloredRelativeThreat = GetColoredPercentage(relativeThreat);
            var nextSpawnTime = GetNextSpawn();

            StringBuilder sb = new();
            sb.Append('\n');

            switch (DetailLevel)
            {
                case 0:
                    sb.Append($"Threat level: {coloredRelativeThreat}");
                    break;
                case 1:
                    sb.Append($"Threat level: {coloredRelativeThreat}\n\n{numEntities} lifeforms detected, totalling a threat level of {threatLevel}.");
                    break;
                case 2:
                    sb.Append($"Threat level: {coloredRelativeThreat}\n\n{numEntities} lifeforms detected, totalling a threat level of {threatLevel}.\n\nLife detected:");
                    foreach (var group in entities)
                    {
                        sb.AppendLine($"\n  {group.Key}: {group.Count()}");
                    }
                    break;
                case 3:
                    sb.Append($"Threat level: {coloredRelativeThreat}\n\n{numEntities} lifeforms detected, totalling a threat level of {threatLevel}.\n\nLife detected:");
                    foreach (var group in entities)
                    {
                        sb.AppendLine($"\n  {group.Key}: {group.Count()}");
                    }
                    sb.AppendLine($"\nNext spawn: {nextSpawnTime}");
                    break;
            }

            sb.AppendLine();
            return sb.ToString();
        }

        private static string GetColoredPercentage(float value)
        {
            float cappedValue = Math.Max(0.0f, Math.Min(1.0f, value));
            int red, green;

            if (cappedValue <= 0.5)
            {
                red = (int)(cappedValue * 2 * 255);
                green = 255;
            }
            else
            {
                red = 255;
                green = (int)((1.0 - cappedValue) * 2 * 255);
            }
            
            string colorCode = $"#{red:X2}{green:X2}00";
            string percentage = (cappedValue * 100).ToString("0.##") + "%";
            
            return $"<color={colorCode}>{percentage}</color>";
        }

        private static string GetNextSpawn()
        {
            var nextSpawn = RoundManager.Instance.enemySpawnTimes.Any(t => t > RoundManager.Instance.timeScript.currentDayTime) ? RoundManager.Instance.enemySpawnTimes.Min() : -1;
            
            if (nextSpawn - RoundManager.Instance.timeScript.currentDayTime >= 60)
                return "Incoming";

            return "Unknown";
        }

        public bool UseUpgradedScan()
        {
            if (!ScanPlus.Instance._configManager.ShipUpgrade)
                return true;
            
            if (ScanPlus.Instance._unlockableManager.IsScannerUnlocked())
                return true;
            
            return false;
        }
    }
}
