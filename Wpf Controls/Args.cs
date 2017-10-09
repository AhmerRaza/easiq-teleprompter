using System;
using System.IO;

namespace Teleprompter.Wpf_Controls {

    public class TextArgs : EventArgs {
        public MemoryStream memorystream;

        public TextArgs(MemoryStream memoryStream) {
            memorystream = memoryStream;
        }
    }

    public class IMarkerArgs : EventArgs {
        public String markerName;

        public IMarkerArgs(String mName) {
            markerName = mName;
        }
    }

    public class ScrollArgs : EventArgs {

        public double perc { get; set; }

        public ScrollArgs(double Perc) {
            perc = Perc;
        }
    }

    public class MarkerArgs : EventArgs {

        public double x { get; set; }

        public double y { get; set; }

        public MarkerArgs(double X, double Y) {
            x = X;
            y = Y;
        }
    }

    public class MarkerAddArgs : EventArgs {

        public int mType { get; set; }

        public MarkerAddArgs(int MTYPE) {
            mType = MTYPE;
        }
    }

    public class MarkerRemovedArgs : EventArgs {

        public String markerName { get; set; }

        public MarkerRemovedArgs(String mName) {
            markerName = mName;
        }
    }

    public class TransitionArgs : EventArgs {

        public bool forward { get; set; }

        public TransitionArgs(bool isForward) {
            forward = isForward;
        }
    }
}