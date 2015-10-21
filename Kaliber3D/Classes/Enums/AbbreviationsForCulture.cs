using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Kaliber3D
{
    internal class AbbreviationsForCulture
    {
        public readonly ReadOnlyCollection<string> Abbreviations;
        public readonly CultureInfo Cult;

        public AbbreviationsForCulture(string cultureName, params string[] abbreviations)
        {
            Cult = new CultureInfo(cultureName);
            Abbreviations = new ReadOnlyCollection<string>(abbreviations.ToList());
        }
    }
}