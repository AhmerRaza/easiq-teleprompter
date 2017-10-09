using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Xml;

//using System.Xaml;

namespace Teleprompter.Classes {

    public class Playlist : System.Collections.CollectionBase {

        #region Private Variables

        private String lName;
        private String fileName;
        private string lActiveStreamGUID = "";
        private string lViewerStreamGUID = "";
        private String production, productionCompany, client, season, episode;

        #endregion Private Variables

        #region Events

        public event EventHandler<StreamAddedArgs> StreamAddedevent;

        public event EventHandler<EventArgs> NewPlaylistEvent;

        public event EventHandler<StreamRemovedArgs> StreamRemovedEvent;

        public event EventHandler<StreamDetailsChangedArgs> StreamDetailsChangedEvent;

        public event EventHandler<ActiveStreamChangedArgs> ActiveStreamChangedEvent;

        public event EventHandler<ActiveStreamChangedArgs> ViewerStreamChanged;

        public event ForceRefreshDelegate ForceRefreshEvent;

        #endregion Events

        #region Constructor

        public Playlist() {
            Name = "NEW PLAYLIST";
            QStream newStream = new QStream("NEW STREAM");
            ActiveStream = new QStream();
            ViewerStream = new QStream();
            Add(newStream);
        }

        #endregion Constructor

        #region Delegates

        public delegate void ForceRefreshDelegate(object sender);

        #endregion Delegates

        #region Public Methods

        public void Add(QStream stream) {
            int newStreamIndex = 0;
            if (stream.Name != "NEW STREAM") {
                for (int i = 0; i < this.List.Count; i++) {
                    if (((QStream)this.List[i]).Name == "NEW STREAM") {
                        newStreamIndex = i;                        
                    }
                }
            }
            if (this.List.Count > 0) {
                this.List.Insert(newStreamIndex, stream);
            } else {
                this.List.Add(stream);
            }
            ConnectToStream(stream);
            RaiseStreamAddedevent(stream);
        }

        public void Remove(QStream stream) {
            if (this.Contains(stream)) {
                DisconnectFromStream(stream);
                this.List.Remove(stream);
                RaiseStreamRemovedEvent(stream.GUID);
            }
        }

        public void Remove(string guid) {
            if (this.Contains(guid)) { this.Remove(this[guid]); }
        }

        public void Remove(Playlist collect) {
            foreach (QStream stream in collect) {
                Remove(stream);
            }
        }

        public bool Contains(QStream qs) {
            return this.List.Contains(qs);
        }

        public bool Contains(string guid) {
            bool Result = false;
            foreach (QStream qs in this.List) {
                if (qs.GUID == guid) {
                    Result = true;
                    break;
                }
            }
            return Result;
        }

        public int GetIndex(string guid) {
            int Result = -1;
            for (int i = 0; i < this.List.Count; i++) {
                if (this[i].GUID == guid) {
                    Result = i;
                    break;
                }
            }
            return Result;
        }

        #endregion Public Methods

        #region Private Methods

        private void stream_StreamDetailsChangedEvent(object sender, StreamDetailsChangedArgs e) {
            RaiseStreamDetailsChangedEvent(e);
        }

        private void stream_ForceRefreshEvent(object sender) {
            if (ForceRefreshEvent != null) { ForceRefreshEvent(sender); }
        }

        private void RaiseStreamAddedevent(QStream stream) {
            if (StreamAddedevent != null) { StreamAddedevent(this, new StreamAddedArgs(stream)); }
        }

        private void RaiseStreamRemovedEvent(string guid) {
            if (StreamRemovedEvent != null) { StreamRemovedEvent(this, new StreamRemovedArgs(guid)); }
        }

        private void RaiseStreamDetailsChangedEvent(StreamDetailsChangedArgs e) {
            if (StreamDetailsChangedEvent != null) { StreamDetailsChangedEvent(this, e); }
        }

        private void RaiseActiveStreamChangedEvent(QStream stream, QStream oldstream) {
            if (ActiveStreamChangedEvent != null) { ActiveStreamChangedEvent(this, new ActiveStreamChangedArgs(stream, oldstream)); }
        }

        private void RaiseActiveStreamChangedEvent(QStream oldstream) {
            RaiseActiveStreamChangedEvent(this.ActiveStream, oldstream);
        }

        private void RaiseViewerStreamChanged(QStream stream, QStream oldstream) {
            if (ViewerStreamChanged != null) { ViewerStreamChanged(this, new ActiveStreamChangedArgs(stream, oldstream)); }
        }

        private void RaiseViewerStreamChanged(QStream oldstream) {
            RaiseViewerStreamChanged(this.ViewerStream, oldstream);
        }

        private void DisconnectFromStream(QStream stream) {
            stream.StreamDetailsChangedEvent -= new EventHandler<StreamDetailsChangedArgs>(stream_StreamDetailsChangedEvent);
            stream.ForceRefreshEvent -= new QStream.ForceRefreshDelegate(stream_ForceRefreshEvent);
        }

        private void ConnectToStream(QStream stream) {
            DisconnectFromStream(stream);
            stream.StreamDetailsChangedEvent += new EventHandler<StreamDetailsChangedArgs>(stream_StreamDetailsChangedEvent);
            stream.ForceRefreshEvent += new QStream.ForceRefreshDelegate(stream_ForceRefreshEvent);
        }

        #endregion Private Methods

        #region Public Properties

        public List<QStream> Streams {
            get {
                int countme = this.List.Count;
                List<QStream> streams = new List<QStream>();
                for (int i = 0; i < countme; i++) {
                    streams.Add((QStream)this.List[i]);
                }
                return streams;
            }
        }

        public String Name {
            get { return lName; }
            set { lName = value; }
        }

        public String FileName {
            get { return fileName; }
            set { fileName = value; }
        }

        public QStream this[int index] {
            get { return (QStream)this.List[index]; }
            set { this.List[index] = value; }
        }

        public QStream this[string guid] {
            get {
                QStream Result = null;
                foreach (QStream qs in this.List) {
                    if (qs.GUID == guid) {
                        Result = qs;
                        break;
                    }
                }
                return Result;
            }
            set {
                for (int i = 0; i < this.List.Count; i++) {
                    QStream qs = (QStream)this.List[i];
                    if (qs.GUID == guid) {
                        this.List[i] = value;
                        break;
                    }
                }
            }
        }

        public QStream ActiveStream {
            get { return this[lActiveStreamGUID]; }
            set {
                if (this.Contains(value) && lActiveStreamGUID != value.GUID) {
                    string oldguid = lActiveStreamGUID;
                    lActiveStreamGUID = value.GUID;
                    RaiseActiveStreamChangedEvent(this[oldguid]);
                    this.ActiveStream.RaiseStreamWordCountChangedEvent();
                }
            }
        }

        public QStream ViewerStream {
            get { return this[lViewerStreamGUID]; }
            set {
                if (this.Contains(value) && lViewerStreamGUID != value.GUID) {
                    string oldguid = lViewerStreamGUID;
                    lViewerStreamGUID = value.GUID;
                    RaiseViewerStreamChanged(this[oldguid]);
                }
            }
        }

        public String ActiveStreamGUID {
            get { return lActiveStreamGUID; }
            set {
                if (this.Contains(value)) {
                    string oldguid = lViewerStreamGUID;
                    lActiveStreamGUID = value;
                    RaiseActiveStreamChangedEvent(this[oldguid]);
                    this.ActiveStream.RaiseStreamWordCountChangedEvent();
                }
            }
        }

        public String ViewerStreamGUID {
            get { return lViewerStreamGUID; }
            set {
                if (this.Contains(value) && lViewerStreamGUID != value) {
                    string oldguid = lViewerStreamGUID;
                    lViewerStreamGUID = value;
                    RaiseViewerStreamChanged(this[oldguid]);
                }
            }
        }

        public String Production {
            get { return production; }
            set { production = value; }
        }

        public String ProductionCompany {
            get { return productionCompany; }
            set { productionCompany = value; }
        }

        public String Client {
            get { return client; }
            set { client = value; }
        }

        public String Season {
            get { return season; }
            set { season = value; }
        }

        public String Episode {
            get { return episode; }
            set { episode = value; }
        }

        public string XML {
            get {
                string Result = "";
                Result += "<playlist>";
                Result += "<name>" + this.Name.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;") + "</name>";
                Result += "<production>" + this.Production + "</production>";
                Result += "<productioncompany>" + this.ProductionCompany + "</productioncompany>";
                Result += "<client>" + this.Client + "</client>";
                Result += "<season>" + this.Season + "</season>";
                Result += "<episode>" + this.Episode + "</episode>";
                Result += "<streamlist>";
                for (int i = 0; i < this.List.Count; i++) {
                    Result += ((QStream)this.List[i]).XML;
                }
                Result += "</streamlist>";
                Result += "</playlist>";
                XmlDocument doc = new XmlDocument();
                try {
                    doc.LoadXml(Result);
                } catch {
                    Result = String.Empty;
                }
                return Result;
            }
            set {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(value);
                foreach (XmlElement xmlPlaylist in xmlDoc.ChildNodes) {
                    if (xmlPlaylist.Name == "playlist") {
                        foreach (XmlElement xmlMeta in xmlPlaylist.ChildNodes) {
                            switch (xmlMeta.Name) {
                                case "name":
                                    if (this.Name == "NEW PLAYLIST") { this.Name = xmlMeta.InnerText.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&"); }
                                    break;

                                case "production": this.Production = xmlMeta.InnerText; break;
                                case "productioncompany": this.ProductionCompany = xmlMeta.InnerText; break;
                                case "client": this.Client = xmlMeta.InnerText; break;
                                case "season": this.Season = xmlMeta.InnerText; break;
                                case "episode": this.Episode = xmlMeta.InnerText; break;
                                case "streamlist":
                                    this.List.Clear();
                                    foreach (XmlElement xmlStream in xmlMeta.ChildNodes) {
                                        QStream stream = new QStream();
                                        stream.XML = xmlStream.OuterXml;
                                        this.Add(stream);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public string AddImportStream {
            set {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(value);
                QStream stream = new QStream();
                stream.XML = xmlDoc.OuterXml;
                this.Add(stream);
            }
        }

        #endregion Public Properties
    }

    public class QStream {

        #region Variables

        private int lWordsPerSecond = 0;
        public bool linked = false;
        public bool autotext = false;
        private String lGUID = String.Empty;
        private String lName = String.Empty;
        private bool trans = false;
        private bool hideSlide = false;
        private String pptName = String.Empty;
        private bool isPpt = false;
        private MarkerCollection lMarkers = new MarkerCollection();
        private MarkerCollection sMarkers = new MarkerCollection();
        private int streamFontSize = 0;
        private MemoryStream rtfContent = new MemoryStream();
        private bool hasDefaultColor = false;
        private System.Drawing.Color defaultColor = System.Drawing.Color.White;
        private FlowDocument document = new FlowDocument();
        private double perc = 0;
        public Form parent;
        public int MarkerID = 0;

        #endregion Variables

        #region Properties

        public int WordsPerSecond {
            get { return lWordsPerSecond; }
            set { lWordsPerSecond = value; }
        }

        public string GUID {
            get { return lGUID; }
        }

        public string Name {
            get { return lName; }
            set {
                if (lName != value) {
                    lName = value;
                }
            }
        }

        public bool Trans {
            get { return trans; }
            set { trans = value; }
        }

        public bool HideSlides {
            get { return hideSlide; }
            set { hideSlide = value; }
        }

        public string PPTName {
            get { return pptName; }
            set { pptName = value; }
        }

        public bool IsPPT {
            get { return isPpt; }
            set { isPpt = value; }
        }

        public MarkerCollection Markers {
            get { return lMarkers; }
            set { lMarkers = value; }
        }

        public MarkerCollection SlideMarkers {
            get { return sMarkers; }
            set { sMarkers = value; }
        }

        public int StreamFontSize {
            get { return streamFontSize; }
            set { streamFontSize = value; }
        }

        public MemoryStream RTFContent {
            get { return rtfContent; }
            set {
                rtfContent = value;
                CreateDocument();
            }
        }

        public FlowDocument Document {
            get { return document; }
            set { document = value; }
        }

        public double Percentage {
            get { return perc; }
            set { perc = value; }
        }

        public string XML {
            get {
                string Result = "";
                Result += "<stream>";
                Result += "<wps>" + this.WordsPerSecond.ToString() + "</wps>";
                Result += "<guid>" + this.GUID + "</guid>";
                Result += "<name>" + this.Name.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;") + "</name>";
                Result += "<fsize>" + streamFontSize + "</fsize>";
                Result += "<trans>" + this.Trans + "</trans>";
                if (this.isPpt) {
                    Result += "<ippt>" + isPpt.ToString() + "</ippt>";
                    Result += "<ppt>" + pptName + "</ppt>";
                    Result += "<hideslide>" + HideSlides.ToString() + "</hideslide>";
                }
                String content = Controller.iso.GetString(rtfContent.ToArray()).Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
                //content = Controller.CleanContents(content);
                Result += "<rtf>" + content + "</rtf>"; //
                Result += "<markers>";
                List<String> mNames = new List<string>();
                foreach (Marker m in this.Markers) {
                    if (!mNames.Contains(m.Name)) {
                        Result += "<marker>";
                        Result += "<name>" + m.Name + "</name>";
                        Result += "<guid>" + m.GUID + "</guid>";
                        Result += "<type>" + m.MarkerType + "</type>";
                        Result += "</marker>";
                        mNames.Add(m.Name);
                    }
                }
                foreach (Marker m in this.SlideMarkers) {
                    Result += "<marker>";
                    Result += "<name>" + m.Name + "</name>";
                    Result += "<guid>" + m.GUID + "</guid>";
                    Result += "<type>" + m.MarkerType + "</type>";
                    Result += "</marker>";
                }
                Result += "</markers>";
                Result += "</stream>";
                return Result;
            }
            set {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(value);
                foreach (XmlElement xmlStream in xmlDoc.ChildNodes) {
                    if (xmlStream.Name == "stream") {
                        foreach (XmlElement xmlMeta in xmlStream.ChildNodes) {
                            switch (xmlMeta.Name) {
                                case "guid":
                                    lGUID = xmlMeta.InnerText;
                                    break;

                                case "wps":
                                    this.WordsPerSecond = System.Convert.ToInt32(xmlMeta.InnerText);
                                    break;

                                case "name":
                                    this.Name = xmlMeta.InnerText;
                                    break;

                                case "fsize":
                                    streamFontSize = (int.TryParse(xmlMeta.InnerText, out streamFontSize) ? streamFontSize : 1);
                                    if (streamFontSize > 20) { streamFontSize = 1; }
                                    break;

                                case "ippt":
                                    isPpt = bool.Parse(xmlMeta.InnerText);
                                    break;

                                case "ppt":
                                    pptName = xmlMeta.InnerText;
                                    break;

                                case "hideslide":
                                    HideSlides = bool.Parse(xmlMeta.InnerText);
                                    break;

                                case "rtf":
                                    String rtfText = xmlMeta.InnerText;
                                    rtfText = rtfText.Replace("'steveamp'", "&");
                                    rtfText = rtfText.Replace("'steveamp'gt;", "&gt;").Replace("'steveamp'lt;", "&lt;");
                                    rtfText = rtfText.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");

                                    Encoding iso = Encoding.GetEncoding("ISO-8859-1");
                                    Encoding utf8 = Encoding.UTF8;
                                    byte[] utfBytes = utf8.GetBytes(rtfText);
                                    String tMsg = utf8.GetString(utfBytes);
                                    byte[] isoBytes = Controller.iso.GetBytes(rtfText);
                                    string msg = Controller.iso.GetString(isoBytes);

                                    System.Windows.Forms.RichTextBox rtfTest = new System.Windows.Forms.RichTextBox();
                                    rtfTest.Rtf = msg;
                                    String actualText = rtfTest.Text;

                                    RTFContent = new MemoryStream(isoBytes);

                                    hasDefaultColor = false;
                                    break;

                                case "text":
                                    String rtfT = xmlMeta.InnerText;
                                    rtfT = rtfT.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");

                                    rtfContent = new MemoryStream(Classes.Controller.iso.GetBytes(rtfT));
                                    hasDefaultColor = true;
                                    defaultColor = System.Drawing.Color.White;
                                    break;

                                case "trans":
                                    this.Trans = bool.Parse(xmlMeta.InnerText);
                                    break;

                                case "markers":
                                    this.Markers = new MarkerCollection();
                                    this.SlideMarkers = new MarkerCollection();
                                    foreach (XmlElement mEle in xmlMeta.ChildNodes) {
                                        Marker m = new Marker(false);
                                        m.MarkerType = 1;
                                        foreach (XmlElement mDet in mEle.ChildNodes) {
                                            if (mDet.Name == "name") { m.Name = mDet.InnerText; }
                                            if (mDet.Name == "guid") { m.GUID = mDet.InnerText; }
                                            if (mDet.Name == "type") { m.MarkerType = int.Parse(mDet.InnerText); }
                                        }
                                        if (m.MarkerType == 1) {
                                            this.Markers.Add(m, false, false);
                                        } else {
                                            this.SlideMarkers.Add(m, false, false);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    //SortoutText();
                    //CheckMarkers();
                }
            }
        }

        #endregion Properties

        #region Delegates

        public delegate void ForceRefreshDelegate(object sender);

        #endregion Delegates

        #region Events

        public event ForceRefreshDelegate ForceRefreshEvent;

        public event EventHandler<StreamDetailsChangedArgs> StreamDetailsChangedEvent;

        public event EventHandler<StreamDetailsChangedArgs> MarkerEvent;

        public event EventHandler<StreamDetailsChangedArgs> MarkerChangedEvent;

        public event EventHandler<StreamDetailsChangedArgs> SlideChangedEvent;

        public event EventHandler<ViewerTextChangedArgs> ViewerTextChangedEvent;

        public event EventHandler<StreamWordCountChangedArgs> StreamWordCountChangedEvent;

        #endregion Events

        #region Constructor

        public QStream(String myName) {
            Name = myName;
            lGUID = Guid.NewGuid().ToString();
            Markers = new MarkerCollection();
            SlideMarkers = new MarkerCollection();
            RTFContent = UpdateContent();
        }

        public QStream() {
            Markers = new MarkerCollection();
            SlideMarkers = new MarkerCollection();
        }

        #endregion Constructor

        #region Class Methods

        public void OrderMarkers(List<String> orderedNames) {
            MarkerCollection mc = new MarkerCollection();
            MarkerCollection smc = new MarkerCollection();
            foreach (String om in orderedNames) {
                foreach (Marker m in Markers) {
                    if (m.InlineName == om && !mc.Contains(m)) {
                        mc.Add(m, true, false);
                    }
                }
                foreach (Marker sm in SlideMarkers) {
                    if (sm.InlineName == om && !smc.Contains(sm)) {
                        smc.Add(sm, true, false);
                    }
                }
            }
            if (mc.Count > 0) { Markers = mc; }
            if (smc.Count > 0) { SlideMarkers = smc; }
        }

        public String RTF_RTFText() {
            return Controller.iso.GetString(rtfContent.ToArray());
        }

        private void SaveTempStream(TextRange tr) {
            FileStream fs = new FileStream("Test.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            tr.Save(fs, System.Windows.DataFormats.Xaml);
        }

        private void RaiseStreamDetailsChangedEvent() {
            if (StreamDetailsChangedEvent != null) { StreamDetailsChangedEvent(this, new StreamDetailsChangedArgs(this)); }
        }

        private void RaiseArbStreamEvent() {
            if (MarkerEvent != null) { MarkerEvent(this, new StreamDetailsChangedArgs(this)); }
        }

        private void RaiseArbMarkerEvent() {
            if (MarkerChangedEvent != null) { MarkerChangedEvent(this, new StreamDetailsChangedArgs(this)); }
        }

        private void RaiseSlideMarkerEvent() {
            if (SlideChangedEvent != null) { SlideChangedEvent(this, new StreamDetailsChangedArgs(this)); }
        }

        private void RaiseViewerTextChangedEvent() {
            if (ViewerTextChangedEvent != null) { ViewerTextChangedEvent(this, new ViewerTextChangedArgs()); }
        }

        private void RaiseStreamWordCountChangedEvent(int count) {
            if (StreamWordCountChangedEvent != null) {
                StreamWordCountChangedEvent(this, new StreamWordCountChangedArgs(count, this.WordsPerSecond));
            }
        }

        public void RaiseStreamWordCountChangedEvent() {
            if (rtfContent == null) { rtfContent = new MemoryStream(); }
            string rtfText = Controller.iso.GetString(rtfContent.ToArray());
            string[] fields = rtfText.Split(new char[] { ' ' });
            RaiseStreamWordCountChangedEvent(fields.Length);
        }

        private void CreateDocument() {
            FlowDocument to = new FlowDocument();
            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
            range2.Load(rtfContent, DataFormats.Rtf);
            Document = to;
        }

        private MemoryStream CreateRichTextBoxWithText(String text) {
            System.Windows.Controls.RichTextBox rtf = new System.Windows.Controls.RichTextBox();
            MemoryStream m = new System.IO.MemoryStream(Controller.iso.GetBytes(text));
            TextRange textRange = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
            textRange.Load(m, DataFormats.Rtf);
            MemoryStream mx = new MemoryStream();
            textRange.Save(mx, System.Windows.DataFormats.Rtf);
            return mx;
        }

        private MemoryStream CreateRichTextBoxWithText(MemoryStream m) {
            System.Windows.Controls.RichTextBox rtf = new System.Windows.Controls.RichTextBox();
            TextRange textRange = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
            textRange.Load(m, DataFormats.Rtf);
            MemoryStream mx = new MemoryStream();
            textRange.Save(mx, System.Windows.DataFormats.Rtf);
            return mx;
        }

        public MemoryStream UpdateContent() {
            System.Windows.Controls.RichTextBox rtfEditor = new System.Windows.Controls.RichTextBox();
            TextRange tr = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Rtf);
            return ms;
        }

        public void GetDetailChange() {
            fmStreamProp sp = new fmStreamProp();
            sp.Stream = this;
            if (sp.ShowDialog() == DialogResult.OK) {
                RaiseStreamDetailsChangedEvent();
            }
        }

        #endregion Class Methods
    }
}