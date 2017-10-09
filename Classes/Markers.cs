using System;
using System.Collections.Generic;
using System.Text;

namespace Teleprompter.Classes {

    public class Marker {

        #region Private Variables

        private string lGUID = Guid.NewGuid().ToString();
        private double mX = 0;
        private double mY = 0;
        private string lName = "";
        private String inlineName = "";
        private int mType = 0;

        #endregion Private Variables

        #region Constructor

        public Marker(double x, double y, string name, int markType) {
            MX = x;
            MY = y;
            mType = markType;
            if (!String.IsNullOrEmpty(name)) {
                this.Name = name.Replace("{", "").Replace("}", "");
            } else {
                if (mType == 1) {
                    GetDetails(String.Empty);
                } else {
                    GetSlideDetails(String.Empty);
                }
            }
        }

        public Marker(int markType, String autoname) {
            mType = markType;
            autoname = autoname.Trim().Replace(" ", "_");
            if (markType == 1) {
                GetDetails(autoname);
            } else {
                GetSlideDetails(autoname);
            }
        }

        public Marker(bool useDefaults) {
        }

        #endregion Constructor

        #region Public Methods

        public bool GetDetails(String autoname) {
            bool Result = false;
            fmMarkerProp mprop = new fmMarkerProp(this, (String.IsNullOrEmpty(autoname) ? this.Name : autoname));
            try {
                if (mprop.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    this.Name = mprop.Marker.Name;
                    Result = true;
                } else {
                    this.Name = String.Empty;
                }
            } catch {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.tempMarker)) {
                    this.Name = Properties.Settings.Default.tempMarker;
                    Properties.Settings.Default.tempMarker = String.Empty;
                    Properties.Settings.Default.Save();
                    Result = true;
                } else {
                    this.Name = String.Empty;
                }
            }
            return Result;
        }

        public bool GetDetails(String autoname, bool rename, int idx) {
            bool Result = false;

            fmMarkerProp mprop = (rename ? new fmMarkerProp(this, idx, (String.IsNullOrEmpty(autoname) ? this.Name : autoname), rename) : new fmMarkerProp(this, (String.IsNullOrEmpty(autoname) ? this.Name : autoname)));
            try {
                if (mprop.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    this.Name = mprop.Marker.Name;
                    Result = true;
                } else {
                    this.Name = String.Empty;
                }
            } catch {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.tempMarker)) {
                    this.Name = Properties.Settings.Default.tempMarker;
                    Properties.Settings.Default.tempMarker = String.Empty;
                    Properties.Settings.Default.Save();
                    Result = true;
                } else {
                    this.Name = String.Empty;
                }
            }
            return Result;
        }

        public bool GetSlideDetails(String autoname) {
            bool Result = false;
            fmSlideProp mprop = new fmSlideProp(this, Controller.playlist.ActiveStream.SlideMarkers, autoname);
            if (mprop.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                this.Name = mprop.Marker.Name;
                Result = true;
            } else {
                this.Name = String.Empty;
            }
            return Result;
        }

        #endregion Public Methods

        #region Properties

        public string GUID {
            get { return lGUID; }
            set { lGUID = value; }
        }

        public double MX {
            get { return mX; }
            set { mX = value; }
        }

        public double MY {
            get { return mY; }
            set { mY = value; }
        }

        public string Name {
            get { return lName; }
            set {
                if (lName != value) {
                    string temp = lName;
                    lName = value;
                    InlineName = value;
                }
            }
        }

        public String InlineName {
            get { return inlineName; }
            set {
                string s = value;
                var sb = new StringBuilder();

                foreach (char c in s) {
                    if (!char.IsPunctuation(c)) {
                        sb.Append(c);
                    } else {
                        sb.Append("point");
                    }
                }
                inlineName = (mType == 1 ? "m" : "s") + sb.ToString().Replace(" ", "");
                if (inlineName.Length > 8) { inlineName = inlineName.Substring(0, 8); }
            }
        }

        public int MarkerType {
            get { return mType; }
            set {
                mType = value;
                if (mType == 1 && inlineName.StartsWith("s")) { inlineName = inlineName.Replace("s", "m"); }
                if (mType == 2 && inlineName.StartsWith("m")) { inlineName = inlineName.Replace("m", "s"); }
            }
        }

        #endregion Properties
    }

    public class MarkerCollection : System.Collections.CollectionBase {

        #region Events

        public event EventHandler<MarkerChangedArgs> MarkerAddedEvent;

        public event EventHandler<MarkerChangedArgs> MarkerRemovedEvent;

        #endregion Events

        #region Public Methods

        public bool Add(Marker marker, bool isActive, bool setmeup) {
            if (!this.List.Contains(marker)) {
                this.List.Add(marker);
                if (MarkerAddedEvent != null) { MarkerAddedEvent(this, new MarkerChangedArgs(marker.GUID, isActive, marker.MarkerType)); }
                return true;
            } else {
                return false;
            }
        }

        public void Remove(Marker marker, bool isActive) {
            if (this.Contains(marker)) {
                this.List.Remove(marker);
                if (MarkerRemovedEvent != null) { MarkerRemovedEvent(this, new MarkerChangedArgs("", isActive, marker.MarkerType)); }
            }
        }

        public void Remove(string guid, bool isActive) {
            foreach (Marker marker in this.List) {
                if (marker.GUID == guid) {
                    Remove(this[guid], isActive);
                    break;
                }
            }
        }

        public void RemoveAll(bool isActive) {
            this.Clear();
            int markerType = 0;
            if (this.List.Count > 0) {
                Marker m = (Marker)this.List[0];
                markerType = m.MarkerType;
            }
            if (MarkerRemovedEvent != null) { MarkerRemovedEvent(this, new MarkerChangedArgs("", isActive, markerType)); }
        }

        public void Remove(bool isActive) {
            Marker[] myList = new Marker[this.List.Count];
            this.List.CopyTo(myList, 0);
            foreach (Marker marker in myList) {
                Remove(marker.GUID, isActive);
            }
        }

        public bool Contains(Marker marker) {
            return this.List.Contains(marker);
        }

        public bool Contains(string guid) {
            bool Result = false;
            foreach (Marker marker in this.List) {
                if (marker.GUID == guid) {
                    Result = true;
                    break;
                }
            }
            return Result;
        }

        public void bubbleSort() {
            Marker[] oldmarkers = new Marker[this.List.Count];
            for (int om = 0; om < oldmarkers.Length; om++) {
                oldmarkers[om] = (Marker)this.List[om];
            }
            int x = oldmarkers.Length;
            int i;
            int j;
            Marker temp;
            for (i = (x - 1); i >= 0; i--) {
                for (j = 1; j <= i; j++) {
                    if (oldmarkers[j - 1].MX > oldmarkers[j].MX && oldmarkers[j - 1].MY > oldmarkers[j].MY) {
                        temp = oldmarkers[j - 1];
                        oldmarkers[j - 1] = oldmarkers[j];
                        oldmarkers[j] = temp;
                    }
                }
            }
            this.List.Clear();
            foreach (Marker m in oldmarkers) {
                this.List.Add(m);
            }
        }

        public List<MarkerComboList> GetComboList() {
            List<MarkerComboList> mList = new List<MarkerComboList>();
            for (int i = 0; i < this.List.Count; i++) {
                Marker m = (Marker)this.List[i];
                MarkerComboList ml = new MarkerComboList();
                ml.key = m.Name;
                ml.value = m.GUID;
                mList.Add(ml);
            }
            return mList;
        }

        #endregion Public Methods

        #region Properties

        public Marker this[int index] {
            get { return (Marker)this.List[index]; }
            set { this.List[index] = value; }
        }

        public Marker this[string guid] {
            get {
                Marker Result = null;
                foreach (Marker marker in this.List) {
                    if (marker.GUID == guid) {
                        Result = marker;
                        break;
                    }
                }
                return Result;
            }
            set {
                for (int i = 0; i < this.List.Count; i++) {
                    if (((Marker)this.List[i]).GUID == guid) {
                        this.List[i] = value;
                        break;
                    }
                }
            }
        }

        #endregion Properties
    }

    public class MarkerComboList {

        public String key { get; set; }

        public String value { get; set; }
    }
}