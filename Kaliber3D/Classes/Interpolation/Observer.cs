using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaliber3D
{
    public delegate void Callback();

    // Subjects, i.e. observables, should define interface internally like follows.
    //public event Callback notifyObserversEvent;
    //// this method is required for calling from derived classes
    //protected void notifyObservers() {
    //    Callback handler = notifyObserversEvent;
    //    if (handler != null) {
    //        handler();
    //    }
    //}
    //public void registerWith(Callback handler) { notifyObserversEvent += handler; }
    //public void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }

    public interface IObservable
    {
        event Callback notifyObserversEvent;
        //void notifyObservers();
        void registerWith(Callback handler);
        void unregisterWith(Callback handler);
    }

    public interface IObserver
    {
        void update();
    }
}
