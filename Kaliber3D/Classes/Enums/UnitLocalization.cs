using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kaliber3D
{
    internal class UnitLocalization
    {
        public readonly ReadOnlyCollection<CulturesForEnumValue> EnumValues;
        public readonly Type UnitEnumType;

        public UnitLocalization(Type unitEnumType, IEnumerable<CulturesForEnumValue> enumValues)
        {
            UnitEnumType = unitEnumType;
            EnumValues = new ReadOnlyCollection<CulturesForEnumValue>(enumValues.ToList());
        }
    }
}