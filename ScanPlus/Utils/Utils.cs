using System.Data;
using System.Linq;

namespace ScanPlus
{
    public static class Utils
    {
        public static string NormalizeCommand(string input)
        {
            return new string(
                input.ToLowerInvariant()
                    .Where(c => !char.IsWhiteSpace(c) && c != '-' && c != '_')
                    .ToArray()
            );
        }
    }
}
