using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaliber3D
{
    //! base class for classes possibly allowing extrapolation
    // LazyObject should not be here but it is because of the InterpolatedYieldCurve
    public abstract class Extrapolator : LazyObject
    {
        private bool extrapolate_;
        public bool extrapolate { get { return extrapolate_; } set { extrapolate_ = value; } }

        // some extra functionality
        public bool allowsExtrapolation() { return extrapolate_; }                //! tells whether extrapolation is enabled
        public void enableExtrapolation(bool b = true) { extrapolate_ = b; }      //! enable extrapolation in subsequent calls
        public void disableExtrapolation(bool b = true) { extrapolate_ = !b; }    //! disable extrapolation in subsequent calls
    }
}
