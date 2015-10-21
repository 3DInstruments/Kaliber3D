using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaliber3D
{
    // interface for all value methods
    public interface IValue
    {
        double value(double v);
    }

    public struct Const
    {
        public const double QL_Epsilon = 2.2204460492503131e-016;

        public const double M_SQRT_2 = 0.7071067811865475244008443621048490392848359376887;
        public const double M_1_SQRTPI = 0.564189583547756286948;

        public const double M_LN2 = 0.693147180559945309417;
        public const double M_PI = 3.141592653589793238462643383280;
        public const double M_PI_2 = 1.57079632679489661923;
    }

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12,
        Jan = 1,
        Feb = 2,
        Mar = 3,
        Apr = 4,
        Jun = 6,
        Jul = 7,
        Aug = 8,
        Sep = 9,
        Oct = 10,
        Nov = 11,
        Dec = 12
    };

    //! Units used to describe time periods
    public enum TimeUnit
    {
        Days,
        Weeks,
        Months,
        Years
    };
}
