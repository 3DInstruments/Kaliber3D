using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kaliber3D
{
    public partial class UnitSystem
    {
        private static readonly ReadOnlyCollection<UnitLocalization> DefaultLocalizations
            = new ReadOnlyCollection<UnitLocalization>(new List<UnitLocalization>
            {
                new UnitLocalization(typeof (PressureUnit),
                    new[]
                    {
                        new CulturesForEnumValue((int) PressureUnit.Atmosphere,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "atm"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Bar,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "bar"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Centibar,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "cbar"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Decapascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "daPa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Decibar,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "dbar"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Gigapascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "GPa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Hectopascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "hPa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Kilobar,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kbar"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilogramForcePerSquareCentimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kgf/cm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilogramForcePerSquareMeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kgf/m²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilogramForcePerSquareMillimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kgf/mm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilonewtonPerSquareCentimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kN/cm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilonewtonPerSquareMeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kN/m²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilonewtonPerSquareMillimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kN/mm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Kilopascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kPa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilopoundForcePerSquareFoot,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kipf/ft²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.KilopoundForcePerSquareInch,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "kipf/in²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Megabar,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "Mbar"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Megapascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "MPa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Micropascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "μPa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Millibar,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "mbar"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.NewtonPerSquareCentimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "N/cm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.NewtonPerSquareMeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "N/m²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.NewtonPerSquareMillimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "N/mm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Pascal,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "Pa"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.PoundForcePerSquareFoot,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "lb/ft²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.PoundForcePerSquareInch,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "lb/in²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Psi,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "psi"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.TechnicalAtmosphere,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "at"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.TonneForcePerSquareCentimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "tf/cm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.TonneForcePerSquareMeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "tf/m²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.TonneForcePerSquareMillimeter,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "tf/mm²"),
                            }),
                        new CulturesForEnumValue((int) PressureUnit.Torr,
                            new[]
                            {
                                new AbbreviationsForCulture("en-US", "torr"),
                            }),
                    }),
             });
    }
}
