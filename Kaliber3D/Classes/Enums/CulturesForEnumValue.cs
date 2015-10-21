using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kaliber3D
{
    internal class CulturesForEnumValue
    {
        public readonly ReadOnlyCollection<AbbreviationsForCulture> Cultures;
        public readonly int Value;

        public CulturesForEnumValue(int value, IEnumerable<AbbreviationsForCulture> cultures)
        {
            Value = value;
            Cultures = new ReadOnlyCollection<AbbreviationsForCulture>(cultures.ToList());
        }
    }
}