using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaliber3D
{
    //! base class for 1-D interpolations.
    /* Classes derived from this class will provide interpolated values from two sequences of equal length,
     * representing discretized values of a variable and a function of the former, respectively. */
    public static partial class Utils
    {
        public static double? toNullable(double val)
        {
            if (val == double.MinValue)
                return null;
            else
                return val;
        }
        public static bool close(double x, double y) { return close(x, y, 0.0001); }
        public static bool close(double x, double y, double n)
        {
            double diff = System.Math.Abs(x - y), tolerance = n * Const.QL_Epsilon;
            return diff <= tolerance * System.Math.Abs(x) && diff <= tolerance * System.Math.Abs(y);
        }
    }

    // this is a redefined collection class to emulate array-type behaviour at initialisation
    // if T is a class then the list is initilized with default constructors instead of null
    public class InitializedList<T> : List<T> where T : new()
    {
        public InitializedList() : base() { }
        public InitializedList(int size)
            : base(size)
        {
            for (int i = 0; i < this.Capacity; i++)
                this.Add(default(T) == null ? new T() : default(T));
        }
        public InitializedList(int size, T value)
            : base(size)
        {
            for (int i = 0; i < this.Capacity; i++)
                this.Add(value);
        }

        // erases the contents without changing the size
        public void Erase()
        {
            for (int i = 0; i < this.Count; i++)
                this[i] = default(T);       // do we need to use "new T()" instead of default(T) when T is class?
        }
    }

    // Interpolation factory
    public interface IInterpolationFactory
    {
        Interpolation interpolate(List<double> xBegin, int size, List<double> yBegin);
        bool global { get; }
        int requiredPoints { get; }
    }

    public abstract class Interpolation : Extrapolator, IValue
    {
        protected Impl impl_;

        public bool empty() { return impl_ == null; }

        public double primitive(double x) { return primitive(x, false); }
        public double primitive(double x, bool allowExtrapolation)
        {
            checkRange(x, allowExtrapolation);
            return impl_.primitive(x);
        }

        public double derivative(double x) { return derivative(x, false); }
        public double derivative(double x, bool allowExtrapolation)
        {
            checkRange(x, allowExtrapolation);
            return impl_.derivative(x);
        }

        public double secondDerivative(double x) { return secondDerivative(x, false); }
        public double secondDerivative(double x, bool allowExtrapolation)
        {
            checkRange(x, allowExtrapolation);
            return impl_.secondDerivative(x);
        }

        public double xMin()
        {
            return impl_.xMin();
        }
        public double xMax()
        {
            return impl_.xMax();
        }
        bool isInRange(double x)
        {
            return impl_.isInRange(x);
        }
        public override void update()
        {
            impl_.update();
        }

        // main method to derive an interpolated point
        public double value(double x) { return value(x, false); }
        public double value(double x, bool allowExtrapolation)
        {
            checkRange(x, allowExtrapolation);
            return impl_.value(x);
        }

        protected void checkRange(double x, bool extrapolate)
        {
            if (!(extrapolate || allowsExtrapolation() || impl_.isInRange(x)))
                throw new ArgumentException("interpolation range is [" + impl_.xMin() + ", " + impl_.xMax()
                                            + "]: extrapolation at " + x + " not allowed");
        }


        // abstract base class interface for interpolation implementations
        protected interface Impl : IValue
        {
            void update();
            double xMin();
            double xMax();
            List<double> xValues();
            List<double> yValues();
            bool isInRange(double d);
            double primitive(double d);
            double derivative(double d);
            double secondDerivative(double d);
        }
        public abstract class templateImpl : Impl
        {
            protected List<double> xBegin_;
            protected List<double> yBegin_;
            protected int size_;

            // this method should be used for initialisation
            public templateImpl(List<double> xBegin, int size, List<double> yBegin)
            {
                xBegin_ = xBegin;
                yBegin_ = yBegin;
                size_ = size;
                if (size < 2)
                    throw new ArgumentException("not enough points to interpolate: at least 2 required, "
                                                + size + " provided");
            }

            public double xMin() { return xBegin_.First(); }
            public double xMax() { return xBegin_[size_ - 1]; }
            public List<double> xValues() { return xBegin_.GetRange(0, size_); }
            public List<double> yValues() { return yBegin_.GetRange(0, size_); }

            public bool isInRange(double x)
            {
                double x1 = xMin(), x2 = xMax();
                return (x >= x1 && x <= x2) || Utils.close(x, x1) || Utils.close(x, x2);
            }

            protected int locate(double x)
            {
                int result = xBegin_.BinarySearch(x);
                if (result < 0)
                    // The upper_bound() algorithm finds the last position in a sequence that value can occupy 
                    // without violating the sequence's ordering
                    // if BinarySearch does not find value the value, the index of the next larger item is returned
                    result = ~result - 1;

                // impose limits. we need the one before last at max or the first at min
                result = Math.Max(Math.Min(result, size_ - 2), 0);
                return result;
            }

            public abstract double value(double d);
            public abstract void update();
            public abstract double primitive(double d);
            public abstract double derivative(double d);
            public abstract double secondDerivative(double d);
        }
    }
}
