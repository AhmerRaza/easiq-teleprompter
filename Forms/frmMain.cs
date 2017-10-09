using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Teleprompter
{
    public partial class frmMain : Form
    {
        #region Variables

        private ElementHost ctrlHost;
        public Wpf_Controls.Editor wpfEditorBox;
        private String selectedStreamComboValue = String.Empty;
        private frmFindDialog findDialog;
        private bool changeStreamFromMenu = false;
        private int direction = 0;
        private Image imgPic = null;
        private double editWidth;
        public double lP = 0;
        public double rP = 0;
        private double scrollbarWidth;
        private bool forward = true;
        private volatile int mClick = 0;
        private double currentViewPercentage = 0;
        private Inline selectedMarker = null;
        private volatile bool controllerEvent = false;
        //private Classes.WiiMote wiiMote;

        #endregion Variables

        #region Constructor

        public frmMain()
        {
            InitializeComponent();
            WireEvents();
            scrollbarWidth = SystemParameters.VerticalScrollBarWidth;
            editWidth = (974 / 3) + scrollbarWidth;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            LoadWPF();
            HookMouseControl(this.Controls);
            SetupControls();
            Classes.Controller.LoadFonts();
            LoadFonts();
            cmbSize.SelectedIndex = 2;
        }

        public delegate void dbwDelegate(bool warning);

        public void DisplayBatteryWarning(bool warning)
        {
            if (InvokeRequired)
            {
                this.Invoke(new dbwDelegate(DisplayBatteryWarning), warning);
            }
            else
            {
                String text = "Wiimote Connected" + (warning ? "Warning - battery low" : "");
                if (wiiStatus.Text != text) { wiiStatus.Text = text; }
                Color forecolor = (warning ? Color.Red : Color.Black);
                if (wiiStatus.ForeColor != forecolor) { wiiStatus.ForeColor = forecolor; }
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Classes.Controller.Exit())
            {
                e.Cancel = true;
            }
            else
            {
                //if (wiiMote != null) { wiiMote.Disconnect(); }
                System.Windows.Forms.Application.Exit();
                Environment.Exit(0);
            }
        }

        private void SetupControls()
        {
            Screen primaryScreen = Screen.PrimaryScreen;
            this.Top = (primaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Left = (primaryScreen.WorkingArea.Width - this.Width) / 2;
            lblNotification.BringToFront();
            picUp.Top = (30 * (pnlEditEyeline.Height / 100));
            picDown.Top = (30 * (pnlEditEyeline.Height / 100));
            picUp.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);
            //picDown.Image.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipX);
            picUp.Visible = false;
            picDown.Visible = false;
            pnlSeperator.Visible = false;
            cmbSize.SelectedIndexChanged -= new EventHandler(cmbSize_SelectedIndexChanged);
            cmbSize.SelectedIndex = 2;
            cmbSize.SelectedIndexChanged += new EventHandler(cmbSize_SelectedIndexChanged);
            cmbEyeline.SelectedIndexChanged -= new EventHandler(cmbEyeline_SelectedIndexChanged);
            cmbEyeline.SelectedIndex = 3;
            Classes.Controller.eyeline = 30;
            cmbEyeline.SelectedIndexChanged += new EventHandler(cmbEyeline_SelectedIndexChanged);
            cmbLeft.SelectedIndexChanged -= new EventHandler(cmbLeft_SelectedIndexChanged);
            cmbLeft.SelectedIndex = 0;
            cmbLeft.SelectedIndexChanged += new EventHandler(cmbLeft_SelectedIndexChanged);
            cmbRight.SelectedIndexChanged -= new EventHandler(cmbRight_SelectedIndexChanged);
            cmbRight.SelectedIndex = 0;
            cmbRight.SelectedIndexChanged += new EventHandler(cmbRight_SelectedIndexChanged);
            pnlEditorBox.Width = 345;
            pnlEditorContainer.Width = pnlEditorBox.Width + 25;

            SetNotification();
            DoubleBuffer(this as Control);
        }

        private void LoadWPF()
        {
            ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Fill;
            pnlEditorBox.Controls.Add(ctrlHost);
            wpfEditorBox = new Wpf_Controls.Editor(false);
            wpfEditorBox.InitializeComponent();
            ctrlHost.Child = wpfEditorBox;
            wpfEditorBox.SelectionChangedEvent += wpfEditorBox_SelectionChangedEvent;
            wpfEditorBox.TextChangedEvent += new EventHandler<Wpf_Controls.TextArgs>(wpfEditorBox_TextChangedEvent);
            wpfEditorBox.ScrollChanged += new Wpf_Controls.Editor.ScrollEventHandler(wpfEditorBox_ScrollChanged);
            wpfEditorBox.TogglePreviewEvent += new EventHandler<EventArgs>(wpfEditorBox_TogglePreviewEvent);
            wpfEditorBox.Loaded += new RoutedEventHandler(wpfEditorBox_Loaded);
            wpfEditorBox.MarkerAddedEvent += new EventHandler<Wpf_Controls.MarkerAddArgs>(wpfEditorBox_MarkerAddedEvent);
            wpfEditorBox.PrevMarkerEvent += new EventHandler<EventArgs>(wpfEditorBox_PrevMarkerEvent);
            wpfEditorBox.NextMarkerEvent += new EventHandler<EventArgs>(wpfEditorBox_NextMarkerEvent);
            wpfEditorBox.CheckMarkerEvent += new EventHandler<EventArgs>(wpfEditorBox_CheckMarkerEvent);
            wpfEditorBox.DisablePreviewEvent += new EventHandler<EventArgs>(wpfEditorBox_DisablePreviewEvent);
            wpfEditorBox.MouseEnter += new System.Windows.Input.MouseEventHandler(wpfEditorBox_MouseEnter);
            wpfEditorBox.AdjustFontUpEvent += new EventHandler<EventArgs>(wpfEditorBox_AdjustFontUpEvent);
            wpfEditorBox.AdjustFontDownEvent += new EventHandler<EventArgs>(wpfEditorBox_AdjustFontDownEvent);
            wpfEditorBox.InternalMarkerEvent += wpfEditorBox_InternalMarkerEvent;
        }

        private void wpfEditorBox_InternalMarkerEvent(object sender, Wpf_Controls.IMarkerArgs e)
        {
            GotoInternalMarker(e.markerName);
        }

        private void wpfEditorBox_AdjustFontDownEvent(object sender, EventArgs e)
        {
            AdjustFontSize(-1);
        }

        private void wpfEditorBox_AdjustFontUpEvent(object sender, EventArgs e)
        {
            AdjustFontSize(1);
        }

        private void wpfEditorBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ctrlHost.Focus();
        }

        private void HookMouseControl(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.Name != "pnlEditorBox")
                {
                    c.MouseClick += new MouseEventHandler(c_MouseClick);
                    c.MouseWheel += new MouseEventHandler(c_MouseWheel);
                    HookMouseControl(c.Controls);
                }
            }
        }

        private void DoubleBuffer(Control control)
        {
            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(control, true, null);
            if (control.HasChildren)
            {
                foreach (Control c in control.Controls) { DoubleBuffer(c); }
            }
        }

        #endregion Constructor

        #region Controller Events

        private void WireEvents()
        {
            Classes.Controller.NewPlaylistEvent += new EventHandler<EventArgs>(Controller_NewPlaylistEvent);
            Classes.Controller.StreamChangedEvent += new EventHandler<EventArgs>(Controller_StreamChangedEvent);
            Classes.Controller.FontSizeChangedEvent += new EventHandler<EventArgs>(Controller_FontSizeChangedEvent);
            Classes.Controller.ActiveStreamChangedEvent += new EventHandler<EventArgs>(Controller_ActiveStreamChangedEvent);
            Classes.Controller.SpeedChangedEvent += new EventHandler<EventArgs>(Controller_SpeedChangedEvent);
            Classes.Controller.PlayStateChangedEvent += new EventHandler<EventArgs>(Controller_PlayStateChangedEvent);
            Classes.Controller.PercentageChangedEvent += new EventHandler<EventArgs>(Controller_PercentageChangedEvent);
        }

        public volatile bool viewerLoaded = false;

        private void Controller_NewPlaylistEvent(object sender, EventArgs e)
        {
            RefreshPlaylistTree();
            LoadStreamCombo();
            if (Classes.Controller.playlist != null) { Classes.Controller.playlist.StreamAddedevent += new EventHandler<Classes.StreamAddedArgs>(playlist_StreamAddedevent); }
            ConnectToMarkers();
        }

        private void wpfEditorBox_SelectionChangedEvent(object sender, EventArgs e)
        {
            UpdateWordCount();
        }

        private void Controller_StreamChangedEvent(object sender, EventArgs e)
        {
            if (Classes.Controller.playlist.ActiveStream != null)
            {
                wpfEditorBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "Editor");
                SetMarkerNames();
                UpdateWordCount();
            }
        }

        private void Controller_FontSizeChangedEvent(object sender, EventArgs e)
        {
            wpfEditorBox.ChangeDefaultFontSize();
        }

        private void Controller_ActiveStreamChangedEvent(object sender, EventArgs e)
        {
            if (tvPlaylist.SelectedNode.Tag.ToString() != Classes.Controller.playlist.ActiveStream.GUID)
            {
                foreach (TreeNode tn in tvPlaylist.Nodes[0].Nodes)
                {
                    if (tn.Tag.ToString() == Classes.Controller.playlist.ActiveStream.GUID)
                    {
                        tvPlaylist.SelectedNode = tn;
                        ChangeNode(tn, true);
                        break;
                    }
                }
            }
            else if (Classes.Controller.linkText && tvPlaylist.SelectedNode.Tag.ToString() != Classes.Controller.playlist.ViewerStream.GUID)
            {
                foreach (TreeNode tn in tvPlaylist.Nodes[0].Nodes)
                {
                    if (tn.Tag.ToString() == Classes.Controller.playlist.ViewerStream.GUID)
                    {
                        tvPlaylist.SelectedNode = tn;
                        ChangeNode(tn, true);
                        break;
                    }
                }
            }
            if (Classes.Controller.linkText)
            {
                if (cmbStream.SelectedValue.ToString() != Classes.Controller.playlist.ActiveStream.GUID)
                {
                    cmbStream.SelectedValue = Classes.Controller.playlist.ActiveStream.GUID;
                    FireStream(true);
                }
            }
            stpWordCount.Text = String.Format("Total Word Count: ", wpfEditorBox.WordCount().ToString());
        }

        private void Controller_SpeedChangedEvent(object sender, EventArgs e)
        {
            if ((int)Classes.Controller.playSpeed != trkSpeed.Value)
            {
                trkSpeed.Value = (int)Classes.Controller.playSpeed;
            }
        }

        private void Controller_PlayStateChangedEvent(object sender, EventArgs e)
        {
            picPlay.Visible = !Classes.Controller.isPlaying;
            picPause.Visible = Classes.Controller.isPlaying;
        }

        private void Controller_PercentageChangedEvent(object sender, EventArgs e)
        {
            if (!controllerEvent)
            {
                try
                {
                    trkProgress.Value = (int)Math.Ceiling(Classes.Controller.percentage);
                    if (Classes.Controller.playlist != null && Classes.Controller.playlist.ViewerStream != null && Classes.Controller.isPlaying && Classes.Controller.playlist.ViewerStream.Trans)
                    {
                        if ((trkSpeed.Value < 0 && Classes.Controller.percentage == 0) || (trkSpeed.Value > 0 && Classes.Controller.percentage == 100))
                        {
                            forward = (trkSpeed.Value < 0 ? false : true);
                            TransitionStream(forward);
                        }
                    }
                    if (Classes.Controller.linkText)
                    {
                        SetEditorPercentage();
                    }
                }
                catch { }
            }
        }

        private void playlist_StreamAddedevent(object sender, Classes.StreamAddedArgs e)
        {
            Classes.Controller.SetStreamItems();
            RefreshPlaylistTree();
            LoadStreamCombo();
        }

        #endregion Controller Events

        #region Wpf Events

        private void wpfEditorBox_DisablePreviewEvent(object sender, EventArgs e)
        {
            if (Classes.Controller.flipVisible)
            {
                currentViewPercentage = Classes.Controller.flipViewer.wpfViewBox.Perc;
            }
            chkPreview.Checked = false;
        }

        private void wpfEditorBox_CheckMarkerEvent(object sender, EventArgs e)
        {
            bool markerExists = true;
            foreach (Classes.Marker m in Classes.Controller.playlist.ActiveStream.Markers)
            {
                markerExists = wpfEditorBox.CheckMarkerName(m.InlineName);
                if (!markerExists)
                {
                    break;
                }
            }
            foreach (Classes.Marker m in Classes.Controller.playlist.ActiveStream.SlideMarkers)
            {
                markerExists = wpfEditorBox.CheckMarkerName(m.InlineName);
                if (!markerExists)
                {
                    break;
                }
            }
        }

        private void wpfEditorBox_MarkerAddedEvent(object sender, Wpf_Controls.MarkerAddArgs e)
        {
            AddMarker(e.mType);
        }

        private void wpfEditorBox_NextMarkerEvent(object sender, EventArgs e)
        {
            nextMarkerToolStripMenuItem_Click(sender, e);
        }

        private void wpfEditorBox_PrevMarkerEvent(object sender, EventArgs e)
        {
            prevMarkerToolStripMenuItem_Click(sender, e);
        }

        private void wpfEditorBox_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void wpfEditorBox_TogglePreviewEvent(object sender, EventArgs e)
        {
        }

        private void wpfEditorBox_ScrollChanged(object sender, Wpf_Controls.ScrollArgs args)
        {
            if (Classes.Controller.linkText)
            {
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    Classes.Controller.SetPercentage(args.perc);
                }
            }
        }

        public volatile bool ValidTextEvent = true;

        private void wpfEditorBox_TextChangedEvent(object sender, Wpf_Controls.TextArgs e)
        {
            if (ValidTextEvent)
            {
                if (Classes.Controller.playlist.ActiveStream == null)
                {
                    Classes.Controller.playlist.ActiveStream = Classes.Controller.playlist.Streams[0];
                }
                Classes.Controller.playlist.ActiveStream.RTFContent = e.memorystream;
                Classes.Controller.MatchHandled = false;
            }
            stpWordCount.Text = String.Format("Total Word Count: {0}", wpfEditorBox.WordCount().ToString());
        }

        #endregion Wpf Events

        #region Playlist Tree

        private void RefreshPlaylistTree()
        {
            tvPlaylist.SuspendLayout();
            TreeNode selectedNode = tvPlaylist.SelectedNode;
            String selectedTag = (selectedNode != null ? (selectedNode.Tag != null ? selectedNode.Tag.ToString() : String.Empty) : String.Empty);

            ContextMenu mnuStream = new ContextMenu();
            tvPlaylist.Nodes.Clear();
            mnuStream.MenuItems.Add(new MenuItem("Properties", new EventHandler(mnuStream_Properties_Click)));
            if (!tvPlaylist.Nodes.ContainsKey("PLAYLIST"))
            {
                tvPlaylist.Nodes.Add("PLAYLIST", "");
            }
            tvPlaylist.Nodes["PLAYLIST"].Text = Classes.Controller.playlist.Name;
            tvPlaylist.Nodes["PLAYLIST"].Expand();
            if (Classes.Controller.streamItems != null && Classes.Controller.streamItems.Count > 0)
            {
                if (!Classes.Controller.streamItems.ContainsKey(selectedTag)) { selectedNode = null; }
                foreach (KeyValuePair<String, String> si in Classes.Controller.streamItems)
                {
                    try
                    {
                        if (!tvPlaylist.Nodes["PLAYLIST"].Nodes.ContainsKey(si.Key))
                        {
                            tvPlaylist.Nodes["PLAYLIST"].Nodes.Add(si.Key, "");
                            tvPlaylist.Nodes["PLAYLIST"].Nodes[si.Key].ContextMenu = mnuStream;
                        }
                        tvPlaylist.Nodes["PLAYLIST"].Nodes[si.Key].Tag = si.Key;
                        tvPlaylist.Nodes["PLAYLIST"].Nodes[si.Key].Text = si.Value;
                    }
                    catch { }
                }
                foreach (TreeNode node in tvPlaylist.Nodes["PLAYLIST"].Nodes)
                {
                    try
                    {
                        if (node != null && Classes.Controller.GetStreamByGUID(node.Tag.ToString()) == null)
                        {
                            tvPlaylist.Nodes["PLAYLIST"].Nodes.Remove(node);
                        }
                    }
                    catch { }
                }
                foreach (TreeNode node in tvPlaylist.Nodes["PLAYLIST"].Nodes)
                {
                    try
                    {
                        if (Classes.Controller.playlist.ActiveStream != null && Classes.Controller.playlist.ActiveStream.GUID == node.Tag.ToString())
                        {
                            tvPlaylist.SelectedNode = node;
                            break;
                        }
                    }
                    catch { }
                }
            }
            tvPlaylist.Invalidate();
            tvPlaylist.ResumeLayout();
            if (selectedNode == null)
            {
                tvPlaylist.SelectedNode = tvPlaylist.Nodes["PLAYLIST"].Nodes[0];
                selectedNode = tvPlaylist.SelectedNode;
            }
            OneClick(selectedNode);
        }

        private void ReorderPlaylist()
        {
            List<String> newOrder = new List<string>();
            foreach (TreeNode tn in tvPlaylist.Nodes[0].Nodes)
            {
                if (!String.IsNullOrEmpty(tn.Tag.ToString()))
                {
                    newOrder.Add(tn.Tag.ToString());
                }
            }
            Classes.Controller.ReorderPlaylist(newOrder);
        }

        private void ChangeNode(TreeNode selectedNode, bool firechange)
        {
            if (selectedNode != null)
            {
                TreeNode streamNode = selectedNode;
                String streamGuid = streamNode.Tag.ToString();
                Classes.QStream selectedStream = Classes.Controller.GetStreamByGUID(streamGuid);
                if (selectedStream != null)
                {
                    lblStreamName.Text = selectedStream.Name.Replace("&", "&&");
                    bool testStreams = Classes.Controller.playlist.ActiveStream == selectedStream;
                    Classes.Controller.playlist.ActiveStream = selectedStream;
                    wpfEditorBox.LoadText(selectedStream.RTFContent, "editor");
                    stpWordCount.Text = String.Format("Total Word Count: {0}", wpfEditorBox.WordCount().ToString());
                    SetMarkerNames();
                    //wpfEditorBox.ResizeViewer(Classes.Controller.eFSize, false);
                    wpfEditorBox.ResizeViewer(Classes.Controller.vFSize, false, false);
                    UpdateEditorMarkers();
                    if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
                    {
                        Classes.Controller.bigEditor.wpfViewBox.LoadText(selectedStream.RTFContent, "editor");
                        Classes.Controller.bigEditor.wpfViewBox.ResizeViewer(Classes.Controller.vFSize, false, true);
                    }
                }
            }
        }

        private void RecolorTreeNode()
        {
            try
            {
                String streamGuid = (Classes.Controller.playlist.ViewerStream != null ? Classes.Controller.playlist.ViewerStream.GUID : (cmbStream.SelectedItem != null ? cmbStream.SelectedValue.ToString() : ""));

                foreach (TreeNode tn in tvPlaylist.Nodes[0].Nodes)
                {
                    if (tn.Tag.ToString() == streamGuid)
                    {
                        tn.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        tn.ForeColor = System.Drawing.Color.Black;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, call the ContainsNode
            // method recursively using the parent of the second node.
            return ContainsNode(node1, node2.Parent);
        }

        private void TwoClicks(TreeNode streamNode)
        {
            if (streamNode.Tag != null)
            {
                Classes.QStream qs = Classes.Controller.GetStreamByGUID(streamNode.Tag.ToString());
                if (qs != null) { qs.GetDetailChange(); }
                Classes.Controller.SetStreamByGuid(qs);
            }
        }

        private void OneClick(TreeNode streamNode)
        {
            this.Cursor = Cursors.WaitCursor;
            if (streamNode.Tag != null)
            {
                String streamGuid = streamNode.Tag.ToString();
                Classes.QStream selectedStream = Classes.Controller.GetStreamByGUID(streamGuid);
                if (selectedStream != null && selectedStream != Classes.Controller.playlist.ActiveStream)
                {
                    lblStreamName.Text = selectedStream.Name;
                    if (selectedStream != null)
                    {
                        if (Classes.Controller.playlist.ActiveStream != selectedStream)
                        {
                            Classes.Controller.playlist.ActiveStream = selectedStream;
                            ConnectToMarkers();
                        }
                        wpfEditorBox.LoadText(selectedStream.RTFContent, "editor");
                        wpfEditorBox.SetPercentage(0);
                        stpWordCount.Text = String.Format("Total Word Count: {0}", wpfEditorBox.WordCount().ToString());
                        SetMarkerNames();
                        //wpfEditorBox.ResizeViewer(selectedStream.StreamFontSize < 15 ? Classes.Controller.eFSize : selectedStream.StreamFontSize, false);
                        wpfEditorBox.ResizeViewer(selectedStream.StreamFontSize < 15 ? Classes.Controller.vFSize : selectedStream.StreamFontSize * 3, false, false);
                        UpdateEditorMarkers();
                        if (Classes.Controller.playlist.ViewerStream != null && Classes.Controller.playlist.ViewerStream == Classes.Controller.playlist.ActiveStream)
                        {
                            chkPreview.Checked = Classes.Controller.playlist[Classes.Controller.playlist.ViewerStream.GUID].linked;
                        }
                        else
                        {
                            chkPreview.Checked = false;
                        }
                    }
                }
                if (selectedStream != null && selectedStream.StreamFontSize != 0)
                {
                    Classes.Controller.SetFontSizes(selectedStream.StreamFontSize);
                    try
                    {
                        cmbSize.SelectedItem = selectedStream.StreamFontSize.ToString();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                    //wpfEditorBox.ResizeViewer(Classes.Controller.eFSize, false);
                    wpfEditorBox.ResizeViewer(Classes.Controller.vFSize, false, false);
                }
                ChangeNode(streamNode, true);
            }
            this.Cursor = Cursors.Arrow;
        }

        #endregion Playlist Tree

        #region Markers

        public void AddMarker(int type)
        {
            double mx, my;
            int lineNumber = 478;
            if (type != 3)
            {
                try
                {
                    List<String> markerNames = new List<string>();
                    if (type == 1)
                    {
                        foreach (Classes.Marker m in Classes.Controller.playlist.ActiveStream.Markers) { markerNames.Add(m.Name); }
                    }
                    else
                    {
                        foreach (Classes.Marker m in Classes.Controller.playlist.ActiveStream.SlideMarkers) { markerNames.Add(m.Name); }
                    }
                    lineNumber = 487;
                    Classes.Marker marker = new Classes.Marker(type, (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible ? Classes.Controller.bigEditor.wpfViewBox.getSelectedText() : wpfEditorBox.getSelectedText()));
                    int mAdd = 0;
                    if (marker == null)
                    {
                        System.Windows.Forms.MessageBox.Show("No marker generated!");
                        return;
                    }
                    else if (String.IsNullOrEmpty(marker.Name))
                    {
                        marker.Name = "zzz";
                    }
                    String oName = marker.Name;
                    while (markerNames.Contains(marker.Name))
                    {
                        marker.Name = oName + "_" + mAdd.ToString();
                        mAdd++;
                    }
                    lineNumber = 495;
                    if (!String.IsNullOrEmpty(marker.Name))
                    {
                        int si = Classes.Controller.playlist.GetIndex(Classes.Controller.playlist.ActiveStream.GUID);
                        lineNumber = 499;
                        if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
                        {
                            Classes.Controller.bigEditor.wpfViewBox.AddMarker(type, marker.InlineName, out mx, out my);
                        }
                        else
                        {
                            wpfEditorBox.AddMarker(type, marker.InlineName, out mx, out my);
                        }
                        lineNumber = 503;
                        marker.MX = mx;
                        marker.MY = my;
                        if (type == 1)
                        {
                            Classes.Controller.playlist.ActiveStream.Markers.Add(marker, true, false);
                        }
                        else
                        {
                            Classes.Controller.playlist.ActiveStream.SlideMarkers.Add(marker, true, false);
                        }
                        lineNumber = 511;
                        Classes.Controller.playlist.Streams[si].Markers = Classes.Controller.playlist.ActiveStream.Markers;
                        Classes.Controller.playlist.Streams[si].SlideMarkers = Classes.Controller.playlist.ActiveStream.SlideMarkers;
                        lineNumber = 513;
                        SetMarkerNames();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + " - " + lineNumber.ToString());
                }
            }
            else
            {
                frmCameraProp camProp = new frmCameraProp();
                if (camProp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    String camName = camProp.CameraName;
                    wpfEditorBox.AddMarker(type, camName, out mx, out my);
                    if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible) { Classes.Controller.bigEditor.wpfViewBox.AddMarker(type, camName, out mx, out my); }
                }
            }
        }

        private void ConnectToMarkers()
        {
            if (Classes.Controller.playlist.ActiveStream != null)
            {
                Classes.Controller.playlist.ActiveStream.Markers.MarkerAddedEvent -= new EventHandler<Classes.MarkerChangedArgs>(Markers_MarkerAddedEvent);
                Classes.Controller.playlist.ActiveStream.Markers.MarkerRemovedEvent -= new EventHandler<Classes.MarkerChangedArgs>(Markers_MarkerRemovedEvent);
                Classes.Controller.playlist.ActiveStream.Markers.MarkerAddedEvent += new EventHandler<Classes.MarkerChangedArgs>(Markers_MarkerAddedEvent);
                Classes.Controller.playlist.ActiveStream.Markers.MarkerRemovedEvent += new EventHandler<Classes.MarkerChangedArgs>(Markers_MarkerRemovedEvent);
                Classes.Controller.playlist.ActiveStream.SlideMarkers.MarkerAddedEvent -= new EventHandler<Classes.MarkerChangedArgs>(SMarkers_MarkerAddedEvent);
                Classes.Controller.playlist.ActiveStream.SlideMarkers.MarkerRemovedEvent -= new EventHandler<Classes.MarkerChangedArgs>(SMarkers_MarkerRemovedEvent);
                Classes.Controller.playlist.ActiveStream.SlideMarkers.MarkerAddedEvent += new EventHandler<Classes.MarkerChangedArgs>(SMarkers_MarkerAddedEvent);
                Classes.Controller.playlist.ActiveStream.SlideMarkers.MarkerRemovedEvent += new EventHandler<Classes.MarkerChangedArgs>(SMarkers_MarkerRemovedEvent);
            }
        }

        public void UpdateMarkers()
        {
            int tempMarkerID = Classes.Controller.playlist.ViewerStream.MarkerID;
            //int tempMarkerID = (cmbMarker.SelectedItem != null ? (cmbMarker.SelectedIndex : 0);
            int tempSlideID = (cmbSlide.SelectedItem != null ? cmbSlide.SelectedIndex : 0);
            cmbMarker.SelectedIndexChanged -= new EventHandler(cmbMarker_SelectedIndexChanged);
            List<Classes.MarkerComboList> mList = new List<Classes.MarkerComboList>();
            List<Classes.MarkerComboList> mList2 = new List<Classes.MarkerComboList>();
            if (Classes.Controller.playlist.ViewerStream == null)
            {
                Classes.Controller.playlist.ViewerStream = new Classes.QStream();
            }
            else
            {
                mList = Classes.Controller.playlist.ViewerStream.Markers.GetComboList();
                mList2 = Classes.Controller.playlist.ViewerStream.SlideMarkers.GetComboList();
            }
            cmbMarker.DataSource = null;
            cmbMarker.Items.Clear();
            cmbMarker.ValueMember = "value";
            cmbMarker.DisplayMember = "key";
            cmbMarker.DataSource = mList;
            try { cmbMarker.SelectedIndex = tempMarkerID; } catch { cmbMarker.SelectedIndex = -1; }
            try { cmbSlide.SelectedIndex = tempSlideID; } catch { cmbSlide.SelectedIndex = -1; }
            cmbMarker.SelectedIndexChanged += new EventHandler(cmbMarker_SelectedIndexChanged);

            cmbSlide.SelectedIndexChanged -= new EventHandler(cmbSlide_SelectedIndexChanged);
            cmbSlide.DataSource = null;
            cmbSlide.Items.Clear();
            cmbSlide.ValueMember = "value";
            cmbSlide.DisplayMember = "key";
            cmbSlide.DataSource = mList2;
            cmbSlide.SelectedIndexChanged += new EventHandler(cmbSlide_SelectedIndexChanged);
        }

        private void UpdateEditorMarkers()
        {
            cmbEditMarker.SelectedIndexChanged -= new EventHandler(cmbEditMarker_SelectedIndexChanged);
            List<Classes.MarkerComboList> mList = new List<Classes.MarkerComboList>();
            List<Classes.MarkerComboList> mList2 = new List<Classes.MarkerComboList>();
            if (Classes.Controller.playlist.ActiveStream != null)
            {
                mList = Classes.Controller.playlist.ActiveStream.Markers.GetComboList();
                mList2 = Classes.Controller.playlist.ActiveStream.SlideMarkers.GetComboList();
            }
            cmbEditMarker.DataSource = null;
            cmbEditMarker.Items.Clear();
            cmbEditMarker.ValueMember = "value";
            cmbEditMarker.DisplayMember = "key";
            cmbEditMarker.DataSource = mList;
            cmbEditMarker.SelectedIndexChanged += new EventHandler(cmbEditMarker_SelectedIndexChanged);

            cmbEditSlide.SelectedIndexChanged -= new EventHandler(cmbEditSlide_SelectedIndexChanged);
            cmbEditSlide.DataSource = null;
            cmbEditSlide.Items.Clear();
            cmbEditSlide.ValueMember = "value";
            cmbEditSlide.DisplayMember = "key";
            cmbEditSlide.DataSource = mList2;
            cmbEditSlide.SelectedIndexChanged += new EventHandler(cmbEditSlide_SelectedIndexChanged);
        }

        public void SetMarkerNames()
        {
            List<String> markerNames = new List<string>();
            List<String> smarkerNames = new List<string>();
            foreach (Classes.Marker m in Classes.Controller.playlist.ActiveStream.Markers) { markerNames.Add(m.InlineName); }
            foreach (Classes.Marker m in Classes.Controller.playlist.ActiveStream.SlideMarkers) { smarkerNames.Add(m.InlineName); }
            wpfEditorBox.SetMarkerNames(markerNames, 1);
            wpfEditorBox.SetMarkerNames(smarkerNames, 2);
            if (Classes.Controller.bigEditor != null)
            {
                try
                {
                    Classes.Controller.bigEditor.wpfViewBox.SetMarkerNames(markerNames, 1);
                    Classes.Controller.bigEditor.wpfViewBox.SetMarkerNames(smarkerNames, 2);
                }
                catch { }
            }
            if (chkPreview.Checked)
            {
                //Classes.Controller
            }
        }

        private void Markers_MarkerRemovedEvent(object sender, Classes.MarkerChangedArgs e)
        {
            List<Classes.MarkerComboList> mListEdit = (e.isActive ? Classes.Controller.playlist.ActiveStream.Markers.GetComboList() : Classes.Controller.playlist.ViewerStream.Markers.GetComboList());
            List<Classes.MarkerComboList> mListView = (e.isActive ? Classes.Controller.playlist.ActiveStream.Markers.GetComboList() : Classes.Controller.playlist.ViewerStream.Markers.GetComboList());
            if (e.isActive)
            {
                cmbEditMarker.SelectedIndexChanged -= new EventHandler(cmbEditMarker_SelectedIndexChanged);
                cmbEditMarker.DataSource = null;
                cmbEditMarker.Items.Clear();
                cmbEditMarker.ValueMember = "value";
                cmbEditMarker.DisplayMember = "key";
                cmbEditMarker.DataSource = mListEdit;
                cmbEditMarker.SelectedItem = null;
                cmbEditMarker.SelectedIndexChanged += new EventHandler(cmbEditMarker_SelectedIndexChanged);
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    cmbMarker.SelectedIndexChanged -= new EventHandler(cmbMarker_SelectedIndexChanged);
                    String selectedMarkerGUID = String.Empty;
                    if (cmbMarker.SelectedItem != null) { selectedMarkerGUID = cmbMarker.SelectedValue.ToString(); }
                    cmbMarker.DataSource = null;
                    cmbMarker.Items.Clear();
                    cmbMarker.ValueMember = "value";
                    cmbMarker.DisplayMember = "key";
                    cmbMarker.DataSource = mListView;
                    if (!String.IsNullOrEmpty(selectedMarkerGUID))
                    {
                        try
                        {
                            cmbMarker.SelectedValue = selectedMarkerGUID;
                        }
                        catch { }
                    }
                    cmbMarker.SelectedIndexChanged += new EventHandler(cmbMarker_SelectedIndexChanged);
                }
            }
        }

        private void Markers_MarkerAddedEvent(object sender, Classes.MarkerChangedArgs e)
        {
            List<String> unorderedNames = new List<string>();
            foreach (Classes.Marker um in Classes.Controller.playlist.ActiveStream.Markers)
            {
                unorderedNames.Add(um.InlineName);
            }
            List<String> orderedNames = new List<string>();
            if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
            {
                orderedNames = Classes.Controller.bigEditor.wpfViewBox.GetMarkerOrder(unorderedNames, 1);
            }
            else
            {
                orderedNames = wpfEditorBox.GetMarkerOrder(unorderedNames, 1);
            }
            Classes.Controller.playlist.ActiveStream.OrderMarkers(orderedNames);
            ConnectToMarkers();
            List<Classes.MarkerComboList> mListEdit = Classes.Controller.playlist.ActiveStream.Markers.GetComboList();
            List<Classes.MarkerComboList> mListView = Classes.Controller.playlist.ViewerStream.Markers.GetComboList();
            if (e.isActive)
            {
                wpfEditorBox.GotoMarker(Classes.Controller.playlist.ActiveStream.Markers[e.GUID].InlineName);
                cmbEditMarker.SelectedIndexChanged -= new EventHandler(cmbEditMarker_SelectedIndexChanged);
                String selectedEditMarkerValue = (cmbEditMarker.SelectedItem != null ? cmbEditMarker.SelectedValue.ToString() : String.Empty);
                cmbEditMarker.DataSource = null;
                cmbEditMarker.Items.Clear();
                cmbEditMarker.ValueMember = "value";
                cmbEditMarker.DisplayMember = "key";
                cmbEditMarker.DataSource = mListEdit;
                cmbEditMarker.SelectedValue = e.GUID;
                if (!String.IsNullOrEmpty(selectedEditMarkerValue)) { cmbEditMarker.SelectedValue = selectedEditMarkerValue; }
                cmbEditMarker.SelectedIndexChanged += new EventHandler(cmbEditMarker_SelectedIndexChanged);
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    cmbMarker.SelectedIndexChanged -= new EventHandler(cmbMarker_SelectedIndexChanged);
                    String selectedMarkerGUID = String.Empty;
                    if (cmbMarker.SelectedItem != null) { selectedMarkerGUID = cmbMarker.SelectedValue.ToString(); }
                    cmbMarker.DataSource = null;
                    cmbMarker.Items.Clear();
                    cmbMarker.ValueMember = "value";
                    cmbMarker.DisplayMember = "key";
                    cmbMarker.DataSource = mListView;
                    if (!String.IsNullOrEmpty(selectedMarkerGUID))
                    {
                        try
                        {
                            cmbMarker.SelectedValue = selectedMarkerGUID;
                        }
                        catch { }
                    }
                    cmbMarker.SelectedIndexChanged += new EventHandler(cmbMarker_SelectedIndexChanged);
                }
            }
        }

        private void SMarkers_MarkerRemovedEvent(object sender, Classes.MarkerChangedArgs e)
        {
            List<Classes.MarkerComboList> mListEdit = (e.isActive ? Classes.Controller.playlist.ActiveStream.SlideMarkers.GetComboList() : Classes.Controller.playlist.ViewerStream.SlideMarkers.GetComboList());
            List<Classes.MarkerComboList> mListView = (e.isActive ? Classes.Controller.playlist.ActiveStream.SlideMarkers.GetComboList() : Classes.Controller.playlist.ViewerStream.SlideMarkers.GetComboList());
            if (e.isActive)
            {
                cmbEditSlide.SelectedIndexChanged -= new EventHandler(cmbEditSlide_SelectedIndexChanged);
                cmbEditSlide.DataSource = null;
                cmbEditSlide.Items.Clear();
                cmbEditSlide.ValueMember = "value";
                cmbEditSlide.DisplayMember = "key";
                cmbEditSlide.DataSource = mListEdit;
                cmbEditSlide.SelectedItem = null;
                cmbEditSlide.SelectedIndexChanged += new EventHandler(cmbEditSlide_SelectedIndexChanged);
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    cmbSlide.SelectedIndexChanged -= new EventHandler(cmbSlide_SelectedIndexChanged);
                    String selectedMarkerGUID = String.Empty;
                    if (cmbSlide.SelectedItem != null) { selectedMarkerGUID = cmbSlide.SelectedValue.ToString(); }
                    cmbSlide.DataSource = null;
                    cmbSlide.Items.Clear();
                    cmbSlide.ValueMember = "value";
                    cmbSlide.DisplayMember = "key";
                    cmbSlide.DataSource = mListView;
                    if (!String.IsNullOrEmpty(selectedMarkerGUID))
                    {
                        try
                        {
                            cmbSlide.SelectedValue = selectedMarkerGUID;
                        }
                        catch { }
                    }
                    cmbSlide.SelectedIndexChanged += new EventHandler(cmbSlide_SelectedIndexChanged);
                }
            }
        }

        private void SMarkers_MarkerAddedEvent(object sender, Classes.MarkerChangedArgs e)
        {
            List<String> unorderedNames = new List<string>();
            foreach (Classes.Marker um in Classes.Controller.playlist.ActiveStream.SlideMarkers) { unorderedNames.Add(um.InlineName); }
            List<String> orderedNames = wpfEditorBox.GetMarkerOrder(unorderedNames, 2);
            Classes.Controller.playlist.ActiveStream.OrderMarkers(orderedNames);
            ConnectToMarkers();
            List<Classes.MarkerComboList> mListEdit = Classes.Controller.playlist.ActiveStream.SlideMarkers.GetComboList();
            List<Classes.MarkerComboList> mListView = Classes.Controller.playlist.ViewerStream.SlideMarkers.GetComboList();
            if (e.isActive)
            {
                wpfEditorBox.GotoMarker(Classes.Controller.playlist.ActiveStream.SlideMarkers[e.GUID].InlineName);
                cmbEditSlide.SelectedIndexChanged -= new EventHandler(cmbEditSlide_SelectedIndexChanged);
                String selectedEditMarkerValue = (cmbEditSlide.SelectedItem != null ? cmbEditSlide.SelectedValue.ToString() : String.Empty);
                cmbEditSlide.DataSource = null;
                cmbEditSlide.Items.Clear();
                cmbEditSlide.ValueMember = "value";
                cmbEditSlide.DisplayMember = "key";
                cmbEditSlide.DataSource = mListEdit;
                cmbEditSlide.SelectedValue = e.GUID;
                if (!String.IsNullOrEmpty(selectedEditMarkerValue)) { cmbEditSlide.SelectedValue = selectedEditMarkerValue; }
                cmbEditSlide.SelectedIndexChanged += new EventHandler(cmbEditSlide_SelectedIndexChanged);
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    cmbSlide.SelectedIndexChanged -= new EventHandler(cmbSlide_SelectedIndexChanged);
                    String selectedMarkerGUID = String.Empty;
                    if (cmbSlide.SelectedItem != null) { selectedMarkerGUID = cmbSlide.SelectedValue.ToString(); }
                    cmbSlide.DataSource = null;
                    cmbSlide.Items.Clear();
                    cmbSlide.ValueMember = "value";
                    cmbSlide.DisplayMember = "key";
                    cmbSlide.DataSource = mListView;
                    if (!String.IsNullOrEmpty(selectedMarkerGUID))
                    {
                        try
                        {
                            cmbSlide.SelectedValue = selectedMarkerGUID;
                        }
                        catch { }
                    }
                    cmbSlide.SelectedIndexChanged += new EventHandler(cmbSlide_SelectedIndexChanged);
                }
            }
        }

        #endregion Markers

        #region Control Events

        private void menuNewPlaylist_Click(object sender, EventArgs e)
        {
            Classes.Controller.NewPlaylist(false);
        }

        private void menuOpenPlaylist_Click(object sender, EventArgs e)
        {
            Classes.Controller.NewPlaylist(true);
        }

        private void importStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Classes.Controller.ImportStream();
            //ConnectToMarkers();
        }

        private void menuSavePlaylist_Click(object sender, EventArgs e)
        {
            wpfEditorBox.GetContent();
            Classes.Controller.SavePlaylist(false);
        }

        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            wpfEditorBox.GetContent();
            Classes.Controller.SavePlaylist(true);
            RefreshPlaylistTree();
        }

        private void mnuPlaylistProperties_Click(object sender, EventArgs e)
        {
            fmPlaylistProp pplaylist = new fmPlaylistProp();
            pplaylist.PlayList = Classes.Controller.playlist;
            if (pplaylist.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Classes.Controller.playlist = pplaylist.PlayList;
                RefreshPlaylistTree();
            }
        }

        private void mnuExport_Click(object sender, EventArgs e)
        {
            Classes.Controller.ExportToTXT();
        }

        private void menuNewStream_Click(object sender, EventArgs e)
        {
            Classes.Controller.AddNewStream();
        }

        private void mnuStreamProperties_Click(object sender, EventArgs e)
        {
            fmStreamProp streamProp = new fmStreamProp();
            streamProp.Stream = Classes.Controller.playlist.ActiveStream;
            if (streamProp.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Classes.Controller.playlist.ActiveStream = streamProp.Stream;
                Classes.Controller.SetStreamByGuid(streamProp.Stream);
                RefreshPlaylistTree();
                LoadStreamCombo();
            }
        }

        private void mnuDeleteStream_Click(object sender, EventArgs e)
        {
            Classes.Controller.RemoveStream();
        }

        private void mnuFontSizes_Click(object sender, EventArgs e)
        {
            frmSettings settingF = new frmSettings();
            settingF.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Classes.Controller.Exit();
        }

        #region Find and Replace

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findDialog = new frmFindDialog();
            wpfEditorBox.SelectionChangedEvent += wpfEditorBox_SelectionChangedEvent;
            findDialog.FormClosed += new FormClosedEventHandler(findDialog_FormClosed);
            findDialog.FindEvent += new EventHandler<EventArgs>(findDialog_FindEvent);
            findDialog.TextChangedEvent += findDialog_TextChangedEvent;
            findDialog.ReplaceEvent += findDialog_ReplaceEvent;
            findDialog.TopMost = true;
            findDialog.Show();
            findDialog.Left = this.Right - findDialog.Width - 20;
            findDialog.Top = this.Height - findDialog.Height;
        }

        private void findDialog_TextChangedEvent(object sender, EventArgs e)
        {
            if (findDialog.checkUp != searchUp || findDialog.findString != searchString || findDialog.replaceString != replaceString || findDialog.matchCase != matchCase || findDialog.wholeWord != wholeWord)
            {
                searchUp = findDialog.checkUp;
                searchString = findDialog.findString;
                replaceString = findDialog.replaceString;
                matchCase = findDialog.matchCase;
                wholeWord = findDialog.wholeWord;
                wpfEditorBox.foundIdx = 0;
                newSearch = true;
            }
        }

        private TextPointer currentPointer;

        private void findDialog_FindEvent(object sender, EventArgs e)
        {
            if (wpfEditorBox.CaretPosition != wpfEditorBox.sStartPointer)
            {
                newSearch = true;
                wpfEditorBox.foundIdx = 0;
            }
            if (newSearch)
            {
                FindAndReplace(findDialog.findString, String.Empty, false, findDialog.checkUp);
                newSearch = false;
            }
            else
            {
                FindNext(findDialog.replaceString, false);
            }
        }

        private void findDialog_ReplaceEvent(object sender, EventArgs e)
        {
            if (wpfEditorBox.CaretPosition != wpfEditorBox.sStartPointer)
            {
                newSearch = true;
                wpfEditorBox.foundIdx = 0;
            }
            if (newSearch)
            {
                FindAndReplace(findDialog.findString, findDialog.replaceString, true, findDialog.checkUp);
                newSearch = false;
            }
            else
            {
                FindNext(findDialog.replaceString, true);
            }
        }

        private bool newSearch = true;
        private bool searchUp = false;
        private bool matchCase = false;
        private bool wholeWord = false;
        private String searchString = String.Empty;
        private String replaceString = String.Empty;

        private void FindAndReplace(String keyword, String newString, bool replace, bool up)
        {
            wpfEditorBox.FindText(keyword, up, newString, replace, matchCase, wholeWord);
        }

        private void FindNext(String newString, bool replace)
        {
            wpfEditorBox.FindNext(newString, replace);
        }

        private void findDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            wpfEditorBox.SelectionChangedEvent -= wpfEditorBox_SelectionChangedEvent;
            wpfEditorBox.resetSelection();
        }

        #endregion Find and Replace

        private void menuFonts_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Font f = fontDialog.Font;
                System.Drawing.FontStyle fStyle = f.Style;
                System.Windows.Media.FontFamily mfont = new System.Windows.Media.FontFamily(f.Name);

                wpfEditorBox.ChangeFontFamily(mfont);
            }
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColor(1);
        }

        private void btnWhite_Click(object sender, EventArgs e)
        {
            UpdateColor(1);
        }

        private void yellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColor(2);
        }

        private void btnYellow_Click(object sender, EventArgs e)
        {
            UpdateColor(2);
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColor(3);
        }

        private void btnGreen_Click(object sender, EventArgs e)
        {
            UpdateColor(3);
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColor(4);
        }

        private void btnRed_Click(object sender, EventArgs e)
        {
            UpdateColor(4);
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateColor(5);
        }

        private void btnBlue_Click(object sender, EventArgs e)
        {
            UpdateColor(5);
        }

        public void UpdateColor(int color)
        {
            wpfEditorBox.ChangeTextColor(color);
            if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible) { Classes.Controller.bigEditor.wpfViewBox.ChangeTextColor(color); }
        }

        private void mnuFontCaps1_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ChangeCase(true);
            wpfEditorBox.GetContent();
        }

        private void mnuFontCaps2_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ChangeCase(false);
            wpfEditorBox.GetContent();
        }

        private void mnuPlaceMarker_Click(object sender, EventArgs e)
        {
            AddMarker(1);
        }

        private void mnuRemoveAllMarkers_Click(object sender, EventArgs e)
        {
            Classes.MarkerCollection markers = Classes.Controller.playlist.ActiveStream.Markers;
            List<String> markerNames = new List<string>();
            foreach (Classes.Marker marker in markers)
            {
                markerNames.Add(marker.InlineName);
            }
            wpfEditorBox.RemoveMarkers(markerNames);
            Classes.Controller.playlist.ActiveStream.Markers.RemoveAll(true);

            Classes.MarkerCollection smarkers = Classes.Controller.playlist.ActiveStream.SlideMarkers;
            List<String> smarkerNames = new List<string>();
            foreach (Classes.Marker marker in smarkers)
            {
                smarkerNames.Add(marker.InlineName);
            }
            wpfEditorBox.RemoveMarkers(smarkerNames);
            Classes.Controller.playlist.ActiveStream.SlideMarkers.RemoveAll(true);
        }

        private void insertSlideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMarker(2);
        }

        private void insertCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMarker(3);
        }

        private void mnuViewerGotoEdit_Click(object sender, EventArgs e)
        {
            SetEditorPercentage();
        }

        private void mnuPreviousStream_Click(object sender, EventArgs e)
        {
            ChangeStream(-1);
        }

        private void mnuNextStream_Click(object sender, EventArgs e)
        {
            ChangeStream(1);
        }

        public void ChangeStream(int direction)
        {
            if (direction == -1)
            {
                if (tvPlaylist.SelectedNode.Index >= 1)
                {
                    tvPlaylist.SelectedNode = tvPlaylist.Nodes[0].Nodes[tvPlaylist.SelectedNode.Index - 1];
                    ChangeNode(tvPlaylist.SelectedNode, Classes.Controller.linkText);
                }
            }
            else
            {
                if (tvPlaylist.SelectedNode.Index < tvPlaylist.Nodes[0].Nodes.Count - 1)
                {
                    tvPlaylist.SelectedNode = tvPlaylist.Nodes[0].Nodes[tvPlaylist.SelectedNode.Index + 1];
                    ChangeNode(tvPlaylist.SelectedNode, Classes.Controller.linkText);
                }
            }
            if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
            {
                changeStreamFromMenu = true;
                try
                {
                    cmbStream.SelectedIndex = cmbStream.SelectedIndex + direction;
                }
                catch { }
            }
        }

        public void ChangeMarkers(int direction)
        {
            if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
            {
                chkPreview.Checked = true;
            }
            if (cmbEditMarker.Items.Count > 0)
            {
                if (direction == -1 && cmbEditMarker.SelectedItem != null && cmbEditMarker.SelectedIndex > 0)
                {
                    cmbEditMarker.SelectedIndex -= 1;
                }
                else if (direction == 1)
                {
                    if (cmbEditMarker.SelectedItem == null)
                    {
                        cmbEditMarker.SelectedIndex = 0;
                    }
                    else if (cmbEditMarker.SelectedIndex < cmbEditMarker.Items.Count - 1)
                    {
                        cmbEditMarker.SelectedIndex += 1;
                    }
                }
            }
        }

        public delegate void ChangeMarkerDelegate(int dir);

        public void ChangeMarkersFromController(int direction)
        {
            if (InvokeRequired)
            {
                this.Invoke(new ChangeMarkerDelegate(ChangeMarkersFromController), direction);
            }
            else
            {
                if (direction < 0 && cmbMarker.SelectedItem != null && cmbMarker.SelectedIndex > 0)
                {
                    cmbMarker.SelectedIndex -= 1;
                }
                else if (direction > 0)
                {
                    if (cmbMarker.SelectedItem == null)
                    {
                        cmbMarker.SelectedIndex = 0;
                    }
                    else if (cmbMarker.SelectedIndex < cmbMarker.Items.Count - 1)
                    {
                        cmbMarker.SelectedIndex += 1;
                    }
                }
                if (Classes.Controller.playlist.ActiveStream.GUID == Classes.Controller.playlist.ViewerStream.GUID)
                {
                    cmbEditMarker.SelectedIndex = cmbMarker.SelectedIndex;
                    if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible && Classes.Controller.flipVisible)
                    {
                        Classes.Controller.bigEditor.wpfViewBox.SetPercentage(Classes.Controller.flipViewer.wpfViewBox.Perc);
                    }
                }
                Thread.Sleep(500);
            }
        }

        public void prevMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cmbEditMarker.Items.Count > 0)
            {
                if (cmbEditMarker.SelectedItem != null && cmbEditMarker.SelectedIndex > 0)
                {
                    cmbEditMarker.SelectedIndex -= 1;
                }
            }
        }

        public void nextMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cmbEditMarker.Items.Count > 0)
            {
                if (cmbEditMarker.SelectedItem == null)
                {
                    cmbEditMarker.SelectedIndex = 0;
                }
                else if (cmbEditMarker.SelectedIndex < cmbEditMarker.Items.Count - 1)
                {
                    cmbEditMarker.SelectedIndex += 1;
                }
            }
        }

        private void prevSlideToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void nextSlideToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void mnuViewerPlay_Click(object sender, EventArgs e)
        {
            TogglePlay();
        }

        private void mnuUpdateViewer_Click(object sender, EventArgs e)
        {
            if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
            {
                SendViewerStream(sender);
                int gotoPerc = (int)Math.Ceiling(wpfEditorBox.GetPercentage());
                Classes.Controller.SetPercentage(wpfEditorBox.GetPercentage());
                trkProgress.Value = gotoPerc;
            }
        }

        private void mnuRefreshViewer_Click(object sender, EventArgs e)
        {
            if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
            {
                double voffset = Classes.Controller.flipViewer.wpfViewBox.VOffset;
                SendViewerStream(sender);
                //Classes.Controller.flipViewer.wpfViewBox.VOffset = voffset;
                //if (Classes.Controller.exViewer != null) {
                //    Classes.Controller.exViewer.wpfViewBox.VOffset = voffset;
                //}
                Classes.Controller.scrollComplete = true;
            }
        }

        private void mnuLinkEditor_Click(object sender, EventArgs e)
        {
            chkPreview.Checked = !chkPreview.Checked;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (chkPreview.Checked)
            {
                prevMarkerToolStripMenuItem_Click(sender, e);
            }
            else
            {
                if (cmbMarker.Items.Count > 0)
                {
                    if (cmbMarker.SelectedItem != null && cmbMarker.SelectedIndex > 0)
                    {
                        cmbMarker.SelectedIndex -= 1;
                    }
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (chkPreview.Checked)
            {
                nextMarkerToolStripMenuItem_Click(sender, e);
            }
            else
            {
                if (cmbMarker.Items.Count > 0)
                {
                    if (cmbMarker.SelectedItem == null)
                    {
                        cmbMarker.SelectedIndex = 0;
                    }
                    else if (cmbMarker.SelectedIndex < cmbMarker.Items.Count - 1)
                    {
                        cmbMarker.SelectedIndex += 1;
                    }
                }
            }
        }

        private void previousStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeStreamFromMenu = true;
            if (cmbStream.SelectedIndex >= 1)
            {
                cmbStream.SelectedIndex = cmbStream.SelectedIndex - 1;
            }
        }

        private void nextStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeStreamFromMenu = true;
            if (cmbStream.SelectedIndex < cmbStream.Items.Count - 1)
            {
                cmbStream.SelectedIndex = cmbStream.SelectedIndex + 1;
            }
        }

        private void playerSpeedIncreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trkSpeed.Value < trkSpeed.Maximum)
            {
                trkSpeed.Value++;
            }
        }

        private void playerSpeedDecreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trkSpeed.Value > trkSpeed.Minimum)
            {
                trkSpeed.Value--;
            }
        }

        private void mnuFlipH_Click(object sender, EventArgs e)
        {
            chkFlipH.Checked = !chkFlipH.Checked;
        }

        private void mnuFlipV_Click(object sender, EventArgs e)
        {
            chkFlipV.Checked = !chkFlipV.Checked;
        }

        private void mnuHideScreen_Click(object sender, EventArgs e)
        {
            chkBlackout.Checked = !chkBlackout.Checked;
        }

        private void mnuTimer_Click(object sender, EventArgs e)
        {
            chkCountup.Checked = !chkCountup.Checked;
        }

        private void logoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkLogo.Checked = !chkLogo.Checked;
        }

        private void mnuHelpCommands_Click(object sender, EventArgs e)
        {
            Process.Start("Help.pdf");
        }

        private void mnuHelpStart_Click(object sender, EventArgs e)
        {
            Process.Start("EasiQ Teleprompter Quick start basics.pdf");
        }

        private void picEQ_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.easiq.co.za");
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (wpfEditorBox.lastCommand == "changecase")
            {
                wpfEditorBox.UndoCase();
            }
            else
            {
                wpfEditorBox.rtfEditor.Undo();
            }
            wpfEditorBox.GetContent();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            wpfEditorBox.rtfEditor.Redo();
            wpfEditorBox.GetContent();
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ToggleBold();
            wpfEditorBox.GetContent();
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ToggleItalic();
            wpfEditorBox.GetContent();
        }

        private void btnUnderline_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ToggleUnderline();
            wpfEditorBox.GetContent();
        }

        private void btnAllBold_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ToggleAllBold();
            wpfEditorBox.GetContent();
        }

        private void btnRegular_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ToggleAllRegular();
            wpfEditorBox.GetContent();
        }

        private void btnAllCaps_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ChangeCase(true);
            wpfEditorBox.GetContent();
        }

        private void btnSentence_Click(object sender, EventArgs e)
        {
            wpfEditorBox.ChangeCase(false);
            wpfEditorBox.GetContent();
        }

        private void cmbEditMarker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThreadSafeEditMarker();
        }

        private delegate void ThreadSafeMarkerDelegate();

        private void ThreadSafeEditMarker()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ThreadSafeMarkerDelegate(ThreadSafeEditMarker));
            }
            else
            {
                if (cmbEditMarker.SelectedItem != null && Classes.Controller.playlist.ActiveStream.Markers != null)
                {
                    int selectedIndex = cmbEditMarker.SelectedIndex;
                    String markerName = Classes.Controller.playlist.ActiveStream.Markers[cmbEditMarker.SelectedValue.ToString()].InlineName;
                    selectedMarker = (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible ? Classes.Controller.bigEditor.wpfViewBox.GotoMarker(markerName) : wpfEditorBox.GotoMarker(markerName));
                    if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible) { Classes.Controller.linkText = true; }
                    if (selectedMarker != null)
                    {
                        if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
                        {
                            Classes.Controller.bigEditor.wpfViewBox.ScrollToMarker(selectedMarker);
                        }
                        if (Classes.Controller.linkText)
                        {
                            cmbMarker.SelectedIndexChanged -= new EventHandler(cmbMarker_SelectedIndexChanged);
                            wpfEditorBox.ScrollChanged -= wpfEditorBox_ScrollChanged;
                            Classes.Controller.PercentageChangedEvent -= Controller_PercentageChangedEvent;
                            cmbMarker.SelectedIndex = selectedIndex;
                            Classes.Controller.linkText = false;
                            Classes.Controller.ChangeMarker(markerName);
                            wpfEditorBox.ScrollToMarker(selectedMarker);
                            Classes.Controller.linkText = true;
                            cmbMarker.SelectedIndexChanged += new EventHandler(cmbMarker_SelectedIndexChanged);
                            Classes.Controller.PercentageChangedEvent += Controller_PercentageChangedEvent;
                            wpfEditorBox.ScrollChanged += wpfEditorBox_ScrollChanged;
                        }
                        else
                        {
                            wpfEditorBox.ScrollToMarker(selectedMarker);
                        }

                        selectedMarker = null;
                    }
                }
            }
        }

        public void GotoInternalMarker(String markerName)
        {
            Classes.Marker m = null;
            int mi = -1;
            if (Classes.Controller.playlist.ActiveStream.Markers != null)
            {
                cmbEditMarker.SelectedIndexChanged -= cmbEditMarker_SelectedIndexChanged;
                for (int i = 0; i < cmbEditMarker.Items.Count; i++)
                {
                    Classes.MarkerComboList mcl = (Classes.MarkerComboList)cmbEditMarker.Items[i];
                    if (markerName == Classes.Controller.playlist.ActiveStream.Markers[mcl.value].InlineName)
                    {
                        cmbEditMarker.SelectedValue = mcl;
                        m = Classes.Controller.playlist.ActiveStream.Markers[mcl.value];
                        mi = i;
                        break;
                    }
                }
                cmbEditMarker.SelectedIndexChanged += cmbEditMarker_SelectedIndexChanged;
                if ((Classes.Controller.linkText || (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)) && m != null)
                {
                    cmbMarker.SelectedIndexChanged -= new EventHandler(cmbMarker_SelectedIndexChanged);
                    wpfEditorBox.ScrollChanged -= wpfEditorBox_ScrollChanged;
                    Classes.Controller.PercentageChangedEvent -= Controller_PercentageChangedEvent;
                    cmbMarker.SelectedIndex = mi;
                    Classes.Controller.linkText = false;
                    Classes.Controller.ChangeMarker(markerName);
                    Classes.Controller.linkText = true;
                    cmbMarker.SelectedIndexChanged += new EventHandler(cmbMarker_SelectedIndexChanged);
                    Classes.Controller.PercentageChangedEvent += Controller_PercentageChangedEvent;
                    wpfEditorBox.ScrollChanged += wpfEditorBox_ScrollChanged;
                }
            }
        }

        private void cmbEditSlide_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEditSlide.SelectedItem != null && Classes.Controller.playlist.ActiveStream.SlideMarkers != null)
            {
                int selectedIndex = cmbEditSlide.SelectedIndex;

                Classes.Marker marker = Classes.Controller.playlist.ActiveStream.SlideMarkers[cmbEditSlide.SelectedValue.ToString()];

                String markerName = Classes.Controller.playlist.ActiveStream.SlideMarkers[cmbEditSlide.SelectedValue.ToString()].InlineName;
                selectedMarker = wpfEditorBox.GotoMarker(markerName);
                //if (Classes.Controller.bigEditor.Visible) { Classes.Controller.bigEditor.wpfViewBox.GotoMarker(markerName); }
                if (selectedMarker != null)
                {
                    wpfEditorBox.ScrollToMarker(selectedMarker);
                    selectedMarker = null;
                }
                if (Classes.Controller.linkText)
                {
                    cmbSlide.SelectedIndexChanged -= new EventHandler(cmbSlide_SelectedIndexChanged);
                    cmbSlide.SelectedIndex = cmbEditSlide.SelectedIndex;
                    cmbSlide.SelectedIndexChanged += new EventHandler(cmbSlide_SelectedIndexChanged);
                    Classes.Controller.linkText = false;
                    Classes.Controller.ChangeMarker(markerName);
                    Classes.Controller.linkText = true;
                }
            }
        }

        private void btnEditMarker_Click(object sender, EventArgs e)
        {
            if (cmbEditMarker.SelectedItem != null)
            {
                Classes.Marker marker = Classes.Controller.playlist.ActiveStream.Markers[cmbEditMarker.SelectedIndex];
                marker.GetDetails(String.Empty, true, cmbEditMarker.SelectedIndex);
                UpdateEditorMarkers();
                SetMarkerNames();
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    UpdateMarkers();
                    Classes.Controller.UpdateViewerMarkers();
                }
            }
        }

        private void btnDeleteMarker_Click(object sender, EventArgs e)
        {
            if (cmbEditMarker.SelectedItem != null)
            {
                Classes.Marker marker = Classes.Controller.playlist.ActiveStream.Markers[cmbEditMarker.SelectedIndex];
                List<String> oneMarker = new List<string>();
                oneMarker.Add(marker.InlineName);
                wpfEditorBox.RemoveMarkers(oneMarker);
                Classes.Controller.playlist.ActiveStream.Markers.Remove(marker, true);
                UpdateEditorMarkers();
                SetMarkerNames();
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream)
                {
                    UpdateMarkers();
                    Classes.Controller.flipViewer.wpfViewBox.RemoveMarkers(oneMarker);
                    Classes.Controller.UpdateViewerMarkers();
                }
            }
        }

        private void btnEditSlide_Click(object sender, EventArgs e)
        {
            if (cmbEditSlide.SelectedItem != null)
            {
                Classes.Marker marker = Classes.Controller.playlist.ActiveStream.SlideMarkers[cmbEditSlide.SelectedIndex];
                marker.GetDetails(String.Empty);
                UpdateEditorMarkers();
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream) { UpdateMarkers(); }
            }
        }

        private void btnDeleteSlide_Click(object sender, EventArgs e)
        {
            if (cmbEditSlide.SelectedItem != null)
            {
                Classes.Marker marker = Classes.Controller.playlist.ActiveStream.SlideMarkers[cmbEditSlide.SelectedIndex];
                List<String> oneMarker = new List<string>();
                oneMarker.Add(marker.InlineName);
                wpfEditorBox.RemoveMarkers(oneMarker);
                Classes.Controller.playlist.ActiveStream.SlideMarkers.Remove(marker, true);
                UpdateEditorMarkers();
                if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream) { UpdateMarkers(); }
            }
        }

        private double actualProgress = 0;

        private void trkProgress_Scroll(object sender, EventArgs e)
        {
            Classes.Controller.SetPercentage(trkProgress.Value);
        }

        private void cmbStream_SelectedIndexChanged(object sender, EventArgs e)
        {
            FireStream(changeStreamFromMenu);
            if (changeStreamFromMenu) { changeStreamFromMenu = false; }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendViewerStream(sender);
        }

        private void picPlay_Click(object sender, EventArgs e)
        {
            TogglePlay();
        }

        private void picPause_Click(object sender, EventArgs e)
        {
            TogglePlay();
        }

        private void cmbMarker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMarker.SelectedItem != null)
            {
                Classes.Controller.playlist.ViewerStream.MarkerID = cmbMarker.SelectedIndex;
                String markerName = Classes.Controller.playlist.ViewerStream.Markers[cmbMarker.SelectedIndex].InlineName;
                Classes.Controller.ChangeMarker(markerName);
                if (Classes.Controller.linkText)
                {
                    Classes.Controller.PercentageChangedEvent -= Controller_PercentageChangedEvent;
                    cmbEditMarker.SelectedIndexChanged -= new EventHandler(cmbEditMarker_SelectedIndexChanged);
                    cmbEditMarker.SelectedIndex = cmbMarker.SelectedIndex;
                    cmbEditMarker.SelectedIndexChanged += new EventHandler(cmbEditMarker_SelectedIndexChanged);
                    Classes.Controller.linkText = false;
                    selectedMarker = wpfEditorBox.GotoMarker(markerName);
                    if (selectedMarker != null)
                    {
                        wpfEditorBox.ScrollToMarker(selectedMarker);
                        selectedMarker = null;
                    }
                    Classes.Controller.linkText = true;
                    Classes.Controller.PercentageChangedEvent += Controller_PercentageChangedEvent;
                }
            }
        }

        private void picPrevMarker_Click(object sender, EventArgs e)
        {
            if (cmbMarker.SelectedItem != null && cmbMarker.SelectedIndex > 0)
            {
                cmbMarker.SelectedIndex -= 1;
            }
        }

        private void picNextMarker_Click(object sender, EventArgs e)
        {
            if (cmbMarker.SelectedItem == null)
            {
                cmbMarker.SelectedIndex = 0;
            }
            else if (cmbMarker.SelectedIndex < cmbMarker.Items.Count - 1)
            {
                cmbMarker.SelectedIndex += 1;
            }
        }

        private void trkSpeed_Scroll(object sender, EventArgs e)
        {
            if (trkSpeed.Focused)
            {
                trkSpeed.Value += direction;
            }
            lblSpeed.Text = trkSpeed.Value.ToString();
            Classes.Controller.ChangeSpeed(trkSpeed.Value);
            if (trkSpeed.Value >= 0)
            {
                picUp.Visible = false;
                picDown.Visible = true;
            }
            else
            {
                picUp.Visible = true;
                picDown.Visible = false;
            }
        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            String ssize = cmbSize.SelectedItem.ToString();
            Classes.Controller.SetFontSizes(int.Parse(ssize));
            if (Classes.Controller.playlist.ActiveStream == Classes.Controller.playlist.ViewerStream || Classes.Controller.playlist.ViewerStream == null)
            {
                //wpfEditorBox.ResizeViewer(Classes.Controller.eFSize, true);
                wpfEditorBox.ResizeViewer(Classes.Controller.vFSize, true, false);
                System.Windows.Forms.Application.DoEvents();
                SetEditorPercentage();
                if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible)
                {
                    Classes.Controller.bigEditor.wpfViewBox.ResizeViewer(Classes.Controller.vFSize, true, true);
                    Classes.Controller.bigEditor.wpfViewBox.SetPercentage(Classes.Controller.flipViewer.wpfViewBox.Perc);
                }
            }
            foreach (Classes.QStream stream in Classes.Controller.playlist)
            {
                stream.StreamFontSize = int.Parse(ssize);
            }
        }

        private void chkFlipH_CheckedChanged(object sender, EventArgs e)
        {
            SetFlipper();
            SetNotification();
        }

        private void chkFlipV_CheckedChanged(object sender, EventArgs e)
        {
            SetFlipper();
            SetNotification();
        }

        private void chkBlackout_CheckedChanged(object sender, EventArgs e)
        {
            Classes.Controller.Blackout(chkBlackout.Checked);
            SetNotification();
        }

        private void cmbSlide_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSlide.SelectedItem != null)
            {
                String markerName = Classes.Controller.playlist.ViewerStream.SlideMarkers[cmbSlide.SelectedIndex].InlineName;
                Classes.Controller.ChangeMarker(markerName);
                if (Classes.Controller.linkText)
                {
                    cmbEditSlide.SelectedIndexChanged -= new EventHandler(cmbEditSlide_SelectedIndexChanged);
                    cmbEditSlide.SelectedIndex = cmbSlide.SelectedIndex;
                    cmbEditSlide.SelectedIndexChanged += new EventHandler(cmbEditSlide_SelectedIndexChanged);
                    Classes.Controller.linkText = false;
                    selectedMarker = wpfEditorBox.GotoMarker(markerName);
                    if (selectedMarker != null)
                    {
                        wpfEditorBox.ScrollToMarker(selectedMarker);
                        selectedMarker = null;
                    }
                    Classes.Controller.linkText = true;
                }
            }
        }

        private void picPrevSlide_Click(object sender, EventArgs e)
        {
        }

        private void picNextSlide_Click(object sender, EventArgs e)
        {
        }

        private void chkCountup_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk.Checked)
            {
                if (chkLogo.Checked) { chkLogo.Checked = false; }
                chkBlackout.Checked = true;
                if (chk == chkCountup)
                {
                    chkCountdown.CheckedChanged -= new EventHandler(chkCountup_CheckedChanged);
                    chkCountdown.Checked = false;
                    chkCountdown.CheckedChanged += new EventHandler(chkCountup_CheckedChanged);
                }
                else
                {
                    chkCountup.CheckedChanged -= new EventHandler(chkCountup_CheckedChanged);
                    chkCountup.Checked = false;
                    chkCountup.CheckedChanged += new EventHandler(chkCountup_CheckedChanged);
                }
            }
            else
            {
                chkBlackout.Checked = false;
            }
            Classes.Controller.ToggleCounter(chk.Checked);
            SetNotification();
        }

        private void txtCountup_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            Classes.Controller.ChangeCounter(txt.Text);
        }

        private void picPlayCountup_Click(object sender, EventArgs e)
        {
            ToggleCountUp();
        }

        private void picPauseCountup_Click(object sender, EventArgs e)
        {
            ToggleCountUp();
        }

        private void btnResetCountup_Click(object sender, EventArgs e)
        {
            txtCountup.Text = "00:00:00";
            if (Classes.Controller.countupPlaying) { ToggleCountUp(); }
        }

        private void tmrCount_Tick(object sender, EventArgs e)
        {
            String[] timerBits = txtCountup.Text.Split(new String[] { ":" }, StringSplitOptions.None);
            int[] iTmrBits = new int[3];
            iTmrBits[0] = int.Parse(timerBits[0]);
            iTmrBits[1] = int.Parse(timerBits[1]);
            iTmrBits[2] = int.Parse(timerBits[2]);
            if (iTmrBits[2] + 1 >= 60)
            {
                timerBits[2] = "00";
                iTmrBits[1]++;
            }
            else
            {
                iTmrBits[2]++;
                if (iTmrBits[2] < 10)
                {
                    timerBits[2] = "0" + iTmrBits[2].ToString();
                }
                else
                {
                    timerBits[2] = iTmrBits[2].ToString();
                }
            }
            if (iTmrBits[1] + 1 >= 60)
            {
                timerBits[1] = "00";
                iTmrBits[0]++;
            }
            else
            {
                if (iTmrBits[1] < 10)
                {
                    timerBits[1] = "0" + iTmrBits[1].ToString();
                }
                else
                {
                    timerBits[1] = iTmrBits[1].ToString();
                }
            }
            if (iTmrBits[0] < 10)
            {
                timerBits[0] = "0" + iTmrBits[0].ToString();
            }
            else
            {
                timerBits[0] = iTmrBits[0].ToString();
            }
            txtCountup.Text = String.Format("{0}:{1}:{2}", timerBits[0], timerBits[1], timerBits[2]);
        }

        private void tmrCountdown_Tick(object sender, EventArgs e)
        {
            if (txtCountdown.Text != "00:00:00")
            {
                String[] timerBits = txtCountdown.Text.Split(new String[] { ":" }, StringSplitOptions.None);
                int[] iTmrBits = new int[3];
                iTmrBits[0] = int.Parse(timerBits[0]);
                iTmrBits[1] = int.Parse(timerBits[1]);
                iTmrBits[2] = int.Parse(timerBits[2]);
                if (iTmrBits[2] - 1 < 0)
                {
                    timerBits[2] = "59";
                    iTmrBits[1]--;
                }
                else
                {
                    iTmrBits[2]--;
                    if (iTmrBits[2] < 10)
                    {
                        timerBits[2] = "0" + iTmrBits[2].ToString();
                    }
                    else
                    {
                        timerBits[2] = iTmrBits[2].ToString();
                    }
                }
                if (iTmrBits[1] < 0)
                {
                    timerBits[1] = "59";
                    iTmrBits[0]--;
                }
                else
                {
                    if (iTmrBits[1] < 10)
                    {
                        timerBits[1] = "0" + iTmrBits[1].ToString();
                    }
                    else
                    {
                        timerBits[1] = iTmrBits[1].ToString();
                    }
                }
                if (iTmrBits[0] < 10)
                {
                    timerBits[0] = "0" + iTmrBits[0].ToString();
                }
                else
                {
                    timerBits[0] = iTmrBits[0].ToString();
                }
                txtCountdown.Text = String.Format("{0}:{1}:{2}", timerBits[0], timerBits[1], timerBits[2]);
            }
        }

        private void picPlayCountdown_Click(object sender, EventArgs e)
        {
            ToggleCountDown();
        }

        private void picPauseCountdown_Click(object sender, EventArgs e)
        {
            ToggleCountDown();
        }

        private void btnResetCountdown_Click(object sender, EventArgs e)
        {
            txtCountdown.Text = "00:00:00";
            if (Classes.Controller.countdownPlaying) { ToggleCountDown(); }
        }

        private void chkLogo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLogo.Checked)
            {
                if (chkCountdown.Checked) { chkCountdown.Checked = false; }
                if (chkCountup.Checked) { chkCountup.Checked = false; }
            }
            chkBlackout.Checked = chkLogo.Checked;
            Classes.Controller.ShowLogo = chkLogo.Checked;
            SetNotification();
        }

        private void picLogo_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String fileName = ofd.FileName;
                if (File.Exists(fileName))
                {
                    imgPic = new Bitmap(fileName);
                    picLogo.Image = (Image)imgPic.Clone();
                    Classes.Controller.SetImage(imgPic);
                }
            }
        }

        private void mnuStream_Properties_Click(object sender, EventArgs e)
        {
            ShowStreamProperties();
        }

        private void countdownTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkCountdown.Checked = !chkCountdown.Checked;
        }

        private void mnuExportTXT_Click(object sender, EventArgs e)
        {
            Classes.Controller.ExportToTXT();
        }

        private void cmbEyeline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEyeline.SelectedItem != null)
            {
                int eyeline = int.Parse(cmbEyeline.SelectedItem.ToString());
                Classes.Controller.ChangeEyeline(eyeline);
                ChangeEyeline();
            }
        }

        public void c_MouseWheel(object sender, MouseEventArgs e)
        {
            if (sender.GetType() == typeof(frmEditor) || this.ActiveControl.GetType() != typeof(System.Windows.Forms.Integration.ElementHost))
            {
                Control c = sender as Control;
                int numberOfTextLinesToMove = SystemInformation.MouseWheelScrollLines;
                try
                {
                    TrackBar trk = sender as TrackBar;
                    if (trk == trkProgress) { return; }
                }
                catch { }
                if (e.Delta < 0 && trkSpeed.Value > trkSpeed.Minimum)
                {
                    trkSpeed.Value -= 1;
                }
                else if (e.Delta > 0 && trkSpeed.Value < trkSpeed.Maximum)
                {
                    trkSpeed.Value += 1;
                }

                if (!trkSpeed.Focused) { trkSpeed.Value += direction; }
                if (chkPreview.Checked)
                {
                    if (trkSpeed.Value >= 0)
                    {
                        picUp.Visible = false;
                        picDown.Visible = true;
                    }
                    else
                    {
                        picUp.Visible = true;
                        picDown.Visible = false;
                    }
                }
                HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
                ee.Handled = true;
            }
        }

        public int ChangeSpeedFromBigEditor()
        {
            return trkSpeed.Value;
        }

        public delegate void UpdateSpeedDelegate(int dir);

        public void UpdateSpeedFromController(int direction)
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateSpeedDelegate(UpdateSpeedFromController), direction);
            }
            else
            {
                int mySpeed = ChangeSpeedFromBigEditor(direction);
                if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible) { Classes.Controller.bigEditor.ChangeSpeedFromController(mySpeed); }
                Thread.Sleep(500);
            }
        }

        public int ChangeSpeedFromBigEditor(int direction)
        {
            if (direction == 666)
            {
                trkSpeed.Value *= -1;
                Thread.Sleep(500);
            }
            else if (direction < 0 && trkSpeed.Value > trkSpeed.Minimum)
            {
                trkSpeed.Value -= 1;
            }
            else if (direction > 0 && trkSpeed.Value < trkSpeed.Maximum)
            {
                trkSpeed.Value += 1;
            }
            else if (direction == 0)
            {
                trkSpeed.Value = 0;
            }
            //if (!trkSpeed.Focused) { trkSpeed.Value += direction; }
            return trkSpeed.Value;
        }

        public int GetSpeed
        {
            get { return trkSpeed.Value; }
            set { trkSpeed.Value = value; }
        }

        private void c_MouseClick(object sender, MouseEventArgs e)
        {
            object obj = sender;
            if (obj.GetType() != typeof(PictureBox) && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TogglePlay();
            }
        }

        private void trkSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (lblSpeed.Text != trkSpeed.Value.ToString())
            {
                lblSpeed.Text = trkSpeed.Value.ToString();
                Classes.Controller.ChangeSpeed(trkSpeed.Value);
            }
        }

        private void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPreview.Checked)
            {
                //pnlSeperator.Visible = true;
                if (Classes.Controller.playSpeed >= 0)
                {
                    picUp.Visible = false;
                    picDown.Visible = true;
                }
                else
                {
                    picUp.Visible = true;
                    picDown.Visible = false;
                }
                ChangeEditSize(false);
                trkSpeed.Focus();
                wpfEditorBox.trigger = true;
            }
            else
            {
                wpfEditorBox.trigger = false;
                picUp.Visible = false;
                picDown.Visible = false;
                ChangeEditSize(true);
                Classes.Controller.prevlinkPerc = 0;
            }
            Classes.Controller.linkText = chkPreview.Checked;
            if (Classes.Controller.playlist.ViewerStream == Classes.Controller.playlist.ActiveStream)
            {
                Classes.Controller.playlist.ViewerStream.linked = chkPreview.Checked;
                Classes.Controller.playlist[Classes.Controller.playlist.ViewerStream.GUID].linked = chkPreview.Checked;
                if (chkPreview.Checked)
                {
                    wpfEditorBox.Perc = Classes.Controller.flipViewer.wpfViewBox.Perc;
                }
            }
            else if (chkPreview.Checked && Classes.Controller.playlist.ViewerStream != Classes.Controller.playlist.ActiveStream)
            {
                Classes.Controller.playlist.ActiveStream = Classes.Controller.playlist.ViewerStream;
                wpfEditorBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "editor");
                //wpfEditorBox.ResizeViewer(Classes.Controller.eFSize, false);
                wpfEditorBox.ResizeViewer(Classes.Controller.vFSize, false, false);
                SetEditorPercentage();
                chkPreview.Checked = true;
            }
        }

        public void BigEditorSwitch(bool switchMe)
        {
            chkPreview.Checked = switchMe;
        }

        private void cmbLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLeft.SelectedItem != null)
            {
                lP = double.Parse(cmbLeft.SelectedItem.ToString());
                ResizeEditor();
            }
        }

        private void cmbRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRight.SelectedItem != null)
            {
                rP = double.Parse(cmbRight.SelectedItem.ToString());
                ResizeEditor();
            }
        }

        public void LeftFromBig(int direction)
        {
            if (direction > 0)
            {
                if (cmbLeft.SelectedItem == null || cmbLeft.SelectedIndex < 0)
                {
                    cmbLeft.SelectedIndex = 1;
                }
                else if (cmbLeft.SelectedIndex < cmbLeft.Items.Count - 1)
                {
                    cmbLeft.SelectedIndex++;
                }
            }
            else
            {
                if (cmbLeft.SelectedItem != null && cmbLeft.SelectedIndex > 0)
                {
                    cmbLeft.SelectedIndex--;
                }
            }
        }

        public void RightFromBig(int direction)
        {
            if (direction > 0)
            {
                if (cmbRight.SelectedItem == null || cmbRight.SelectedIndex < 0)
                {
                    cmbRight.SelectedIndex = 1;
                }
                else if (cmbRight.SelectedIndex < cmbRight.Items.Count - 1)
                {
                    cmbRight.SelectedIndex++;
                }
            }
            else
            {
                if (cmbRight.SelectedItem != null && cmbRight.SelectedIndex > 0)
                {
                    cmbRight.SelectedIndex--;
                }
            }
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                TogglePlay();
            }
        }

        private void picEQ_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void picEQ_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void frmMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TogglePlay();
            }
        }

        private void tvPlaylist_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, System.Windows.Forms.DragDropEffects.Move);
        }

        private void tvPlaylist_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            System.Drawing.Point targetPoint = tvPlaylist.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = tvPlaylist.GetNodeAt(targetPoint);
            int tNodeIdx = targetNode.Index;
            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            int dNodeIdx = draggedNode.Index;
            // Confirm that the node at the drop location is not the dragged node or a descendant of
            // the dragged node.
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current location and add it to
                // the node at the drop location.
                if (e.Effect == System.Windows.Forms.DragDropEffects.Move)
                {
                    tvPlaylist.Nodes[0].Nodes.RemoveAt(dNodeIdx);
                    tvPlaylist.Nodes[0].Nodes.Insert(tNodeIdx, draggedNode);
                }

                // Expand the node at the location to show the dropped node.
                tvPlaylist.Nodes[0].Expand();
            }
            ReorderPlaylist();
        }

        private void tvPlaylist_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void tvPlaylist_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            System.Drawing.Point targetPoint = tvPlaylist.PointToClient(new System.Drawing.Point(e.X, e.Y));

            // Select the node at the mouse position.
            tvPlaylist.SelectedNode = tvPlaylist.GetNodeAt(targetPoint);
        }

        private void cmbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            String fontFamily = cmbFont.SelectedItem.ToString();
            wpfEditorBox.ChangeFontFamily(fontFamily);
        }

        private void tvPlaylist_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mClick++;
                tmrTreeClicker.Enabled = true;
            }
        }

        private void tmrTreeClicker_Tick(object sender, EventArgs e)
        {
            tmrTreeClicker.Enabled = false;
            TreeNode streamNode = tvPlaylist.SelectedNode;
            if (mClick == 1)
            {
                mClick = 0;
                OneClick(streamNode);
            }
            else
            {
                mClick = 0;
                TwoClicks(streamNode);
            }
        }

        #endregion Control Events

        #region Class Methods

        public void BigEditorShortcuts(int shortcutType)
        {
            switch (shortcutType)
            {
                case 1:
                    chkBlackout.Checked = !chkBlackout.Checked;
                    break;

                case 2:
                    chkFlipH.Checked = !chkFlipH.Checked;
                    break;

                case 3:
                    chkFlipV.Checked = !chkFlipV.Checked;
                    break;

                case 4:
                    chkLogo.Checked = !chkLogo.Checked;
                    break;
            }
        }

        private void SetNotification()
        {
            lblNotification.Visible = false;
            if (chkFlipH.Checked)
            {
                lblNotification.Text = "Flip H";
                lblNotification.Visible = true;
            }
            if (chkFlipV.Checked)
            {
                lblNotification.Text = "Flip V";
                lblNotification.Visible = true;
            }
            if (chkFlipH.Checked && chkFlipV.Checked)
            {
                lblNotification.Text = "Flip H && V";
                lblNotification.Visible = true;
            }
            if (chkBlackout.Checked)
            {
                lblNotification.Text = "Blackout";
                lblNotification.Visible = true;
            }
            if (chkLogo.Checked)
            {
                lblNotification.Text = "Logo";
                lblNotification.Visible = true;
            }
            if (chkCountup.Checked || chkCountdown.Checked)
            {
                lblNotification.Text = "Timer";
                lblNotification.Visible = true;
            }
            lblNotification.Left = pnlEditorContainer.Width - 20 - lblNotification.Width;
        }

        private void SetFlipper()
        {
            Classes.Controller.eyeline = int.Parse(cmbEyeline.SelectedItem.ToString());
            Classes.Controller.SetFlipper(chkFlipH.Checked, chkFlipV.Checked);
        }

        public delegate void ToggleDelegate();

        public void TogglePlay()
        {
            if (InvokeRequired)
            {
                this.Invoke(new ToggleDelegate(TogglePlay));
            }
            else
            {
                Classes.Controller.TogglePlay();
                if (chkPreview.Checked && Classes.Controller.isPlaying) { trkSpeed.Focus(); }
                Thread.Sleep(500);
            }
        }

        private void ToggleCountUp()
        {
            Classes.Controller.countupPlaying = !Classes.Controller.countupPlaying;
            picPlayCountup.Visible = !Classes.Controller.countupPlaying;
            picPauseCountup.Visible = Classes.Controller.countupPlaying;
            tmrCountup.Enabled = Classes.Controller.countupPlaying;
        }

        private void ToggleCountDown()
        {
            Classes.Controller.countdownPlaying = !Classes.Controller.countdownPlaying;
            picPlayCountdown.Visible = !Classes.Controller.countdownPlaying;
            picPauseCountdown.Visible = Classes.Controller.countdownPlaying;
            tmrCountdown.Enabled = Classes.Controller.countdownPlaying;
        }

        private void SetEditorPercentage()
        {
            if (cmbStream.SelectedItem == null)
            {
                cmbStream.SelectedValue = Classes.Controller.playlist.ActiveStream.GUID;
            }
            if (cmbStream.SelectedItem != null)
            {
                if (Classes.Controller.playlist.ActiveStream.GUID != cmbStream.SelectedValue.ToString())
                {
                    Classes.Controller.ChangeActiveStream(cmbStream.SelectedValue.ToString());
                    chkPreview.Checked = (Classes.Controller.playlist.ViewerStream != null ? Classes.Controller.playlist[Classes.Controller.playlist.ViewerStream.GUID].linked : false);
                    UpdateEditorMarkers();
                }
                wpfEditorBox.SetPercentage(Classes.Controller.percentage);
            }
        }

        public void TransitionStream(bool isForward)
        {
            changeStreamFromMenu = true;
            if (isForward)
            {
                if (cmbStream.SelectedIndex < cmbStream.Items.Count - 1)
                {
                    cmbStream.SelectedIndex = cmbStream.SelectedIndex + 1;
                }
            }
            else
            {
                if (cmbStream.SelectedIndex >= 1)
                {
                    cmbStream.SelectedIndex = cmbStream.SelectedIndex - 1;
                }
            }
            try { cmbMarker.SelectedIndex = 0; } catch { }
        }

        private void FireStream(bool fireme)
        {
            if (cmbStream.SelectedItem != null && ((cmbStream.SelectedValue.ToString() != selectedStreamComboValue) || fireme))
            {
                selectedStreamComboValue = cmbStream.SelectedValue.ToString();
                Classes.Controller.playlist.ViewerStream = Classes.Controller.playlist[selectedStreamComboValue];
                if (Classes.Controller.playlist.ViewerStream == Classes.Controller.playlist.ActiveStream) { Classes.Controller.playlist.ViewerStream.RTFContent = wpfEditorBox.UpdateContent(); }
                UpdateMarkers();
                if (Classes.Controller.linkText && ((Classes.Controller.playlist.ActiveStream.GUID != selectedStreamComboValue) || fireme))
                {
                    if (Classes.Controller.playlist.ActiveStream.GUID != selectedStreamComboValue)
                    {
                        Classes.Controller.ChangeActiveStream(selectedStreamComboValue);
                        UpdateEditorMarkers();
                    }
                    SendViewerStream(btnSend);
                }
                else if (fireme)
                {
                    Classes.Controller.playlist.ViewerStream = Classes.Controller.playlist[selectedStreamComboValue];
                    SendViewerStream(btnSend);
                }
            }
        }

        public void SendViewerStream(object sender)
        {
            double percentage = 0;
            trkSpeed.Value = 0;
            Classes.Controller.ChangeSpeed(0);
            bool isButton = false;
            if (sender.GetType() == typeof(Button))
            {
                if (!String.IsNullOrEmpty(selectedStreamComboValue) && Classes.Controller.streamItems.ContainsKey(selectedStreamComboValue))
                {
                    Classes.QStream selectedStream = Classes.Controller.GetStreamByGUID(selectedStreamComboValue);
                    if (selectedStream == Classes.Controller.playlist.ActiveStream) { selectedStream.RTFContent = wpfEditorBox.UpdateContent(); }
                    if (Classes.Controller.playlist.ViewerStream != selectedStream)
                    {
                        Classes.Controller.playlist.ViewerStream = selectedStream;
                    }
                    percentage = (forward ? 0 : 100);
                    isButton = true;
                }
            }
            else
            {
                isButton = false;
                ToolStripMenuItem mnuSender = sender as ToolStripMenuItem;
                Classes.Controller.playlist.ActiveStream.RTFContent = wpfEditorBox.UpdateContent();
                Classes.Controller.playlist[Classes.Controller.playlist.ActiveStream.GUID] = Classes.Controller.playlist.ActiveStream;
                if (mnuSender.Name == "mnuUpdateViewer")
                {// - change position
                    percentage = wpfEditorBox.GetPercentage();
                }
                else
                {
                    percentage = Classes.Controller.getViewerPercentage();
                }
                if (Classes.Controller.playlist.ActiveStream != null)
                {
                    Classes.Controller.playlist.ViewerStream = Classes.Controller.playlist.ActiveStream;
                    UpdateMarkers();
                }
            }
            RecolorTreeNode();
            Classes.Controller.playlist.ViewerStream.linked = chkPreview.Checked;
            Classes.Controller.playlist[Classes.Controller.playlist.ViewerStream.GUID].linked = chkPreview.Checked;
            Classes.Controller.SendStreamToViewer(percentage);
            System.Windows.Forms.Application.DoEvents();
            Classes.Controller.SetPercentage(percentage);
            trkProgress.Value = (int)percentage;
            if ((isButton && !Classes.Controller.isPlaying) || (!isButton && Classes.Controller.isPlaying)) { TogglePlay(); }
            if (isButton) { try { cmbMarker.SelectedIndex = 0; } catch { } }
        }

        private void LoadStreamCombo()
        {
            cmbStream.SelectedIndexChanged -= new EventHandler(cmbStream_SelectedIndexChanged);
            bool hasSelectedItem;
            if (cmbStream.SelectedItem != null)
            {
                selectedStreamComboValue = cmbStream.SelectedValue.ToString();
                hasSelectedItem = true;
            }
            else
            {
                selectedStreamComboValue = String.Empty;
                hasSelectedItem = false;
            }
            cmbStream.DataSource = new BindingSource(Classes.Controller.streamItems, null);
            cmbStream.DisplayMember = "Value";
            cmbStream.ValueMember = "Key";
            if (!String.IsNullOrEmpty(selectedStreamComboValue) && hasSelectedItem && Classes.Controller.streamItems.ContainsKey(selectedStreamComboValue))
            {
                cmbStream.SelectedValue = selectedStreamComboValue;
            }
            else
            {
                cmbStream.SelectedIndex = 0;
                selectedStreamComboValue = cmbStream.SelectedValue.ToString();
            }
            Classes.Controller.playlist.ViewerStream = Classes.Controller.playlist[selectedStreamComboValue];
            UpdateMarkers();
            cmbStream.SelectedIndexChanged += new EventHandler(cmbStream_SelectedIndexChanged);
        }

        private void ShowStreamProperties()
        {
            foreach (TreeNode node in tvPlaylist.Nodes["PLAYLIST"].Nodes)
            {
                if (node.IsSelected)
                {
                    Classes.QStream qs = Classes.Controller.GetStreamByGUID(node.Tag.ToString());
                    if (qs != null) { qs.GetDetailChange(); }
                    Classes.Controller.SetStreamByGuid(qs);
                }
            }
        }

        private void ChangeEyeline()
        {
            int idx = cmbEyeline.SelectedIndex;
            double pos = idx * 25.6;
            int posEye = (int)Math.Ceiling(pos);
            int eyeline = Classes.Controller.eyeline;
            picUp.Top = posEye;//(int)top;
            picDown.Top = posEye;
            if (picUp.Top >= pnlEditEyeline.Height)
            {
                picUp.Top -= 17;
                picDown.Top -= 17;
            }
            System.Windows.Forms.Application.DoEvents();
        }

        private void ChangeEditSize(bool full)
        {
            double viewPerc = Classes.Controller.flipViewer.wpfViewBox.Perc;
            pnlEditorBox.SuspendLayout();
            pnlBorder.SuspendLayout();
            if (full)
            {
                pnlEditorBox.Height = 515;
                wpfEditorBox.ResizeEditor(515);
                pnlEditorContainer.BackColor = System.Drawing.Color.Black;
            }
            else
            {
                pnlEditorBox.Height = 258;
                wpfEditorBox.ResizeEditor(256);
                pnlEditorContainer.BackColor = panel1.BackColor;
            }
            pnlBorder.Visible = full;
            pnlBorder.ResumeLayout();
            pnlEditorBox.ResumeLayout();
            Classes.Controller.flipViewer.wpfViewBox.Perc = viewPerc;
        }

        public void ResizeEditor()
        {
            double maxWidth = wpfEditorBox.iniPageWidth;
            double minWidth = maxWidth / 3;
            double maxMargin = minWidth * 2;
            cmbLeft.SelectedIndexChanged -= cmbLeft_SelectedIndexChanged;
            cmbRight.SelectedIndexChanged -= cmbRight_SelectedIndexChanged;
            int left = 0;
            int right = 0;
            try
            {
                left = (int)lP;
            }
            catch { }
            try
            {
                right = (int)rP;
            }
            catch { }
            if (left + right > 100)
            {
                right = 100 - left;
                rP = right;
                cmbRight.SelectedItem = right.ToString();
            }
            double leftMargin = (left > 0 ? maxMargin * left / 100 : 0);
            double rightMargin = (right > 0 ? maxMargin * right / 100 : 0);
            double newWidth = maxWidth - leftMargin - rightMargin;
            wpfEditorBox.SetPageWidth(newWidth);

            if (leftMargin > 0)
            {
                pnlEditorBox.Width = (pnlEditorContainer.Width - 25 - (int)leftMargin) + 15;
                pnlSeperator.Width = pnlEditorBox.Width;
                pnlBorder.Width = pnlEditorContainer.Width - pnlEditorBox.Width;
                pnlEditEyeline.Width = pnlBorder.Width;
                picUp.Left = pnlEditEyeline.Width - picUp.Width;
                picDown.Left = picUp.Left;
                pnlEditorBox.Left = pnlEditorContainer.Width - pnlEditorBox.Width;
                pnlSeperator.Left = pnlEditorBox.Left;
            }
            else
            {
                pnlEditorBox.Width = 345;
                pnlSeperator.Width = 355;
                pnlBorder.Width = 25;
                pnlEditEyeline.Width = pnlBorder.Width;
                picUp.Left = 0;
                picDown.Left = picUp.Left;
                pnlEditorBox.Left = 25;
                pnlSeperator.Left = 25;
            }
            Classes.Controller.ChangeViewerWidth(lP, right);
            cmbLeft.SelectedIndexChanged += cmbLeft_SelectedIndexChanged;
            cmbRight.SelectedIndexChanged += cmbRight_SelectedIndexChanged;
        }

        private void stpWordCount_TextChanged(object sender, EventArgs e)
        {
            //double wps = 3;
            //double wordCount = wpfEditorBox.WordCount();
            //double secLength = wordCount / wps;
            //TimeSpan t = TimeSpan.FromSeconds(secLength);
            //string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            //stpLength.Text = String.Format("Estimated Length: {0}", answer);
        }

        private delegate void UpdateWordCountDelegate();

        private void UpdateWordCount()
        {
            if (InvokeRequired)
            {
                this.Invoke(new UpdateWordCountDelegate(UpdateWordCount));
            }
            else
            {
                double wps = 3;
                double wordCount = wpfEditorBox.WordCount();
                if (!Classes.Controller.isPlaying) { stpWordCount.Text = "Total Word Count: " + wordCount.ToString(); }
                double secLength = wordCount / wps;
                TimeSpan t = TimeSpan.FromSeconds(secLength);
                string answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                stpLength.Text = String.Format("Estimated Length: {0}", answer);
            }
        }

        private void LoadFonts()
        {
            cmbFont.SelectedIndexChanged -= new EventHandler(cmbFont_SelectedIndexChanged);
            if (Classes.Controller.fonts == null) { Classes.Controller.LoadFonts(); }
            cmbFont.Items.Clear();
            foreach (String font in Classes.Controller.fonts)
            {
                cmbFont.Items.Add(font);
            }
            try
            {
                cmbFont.SelectedItem = Properties.Settings.Default.defaultFont;
            }
            catch
            {
                cmbFont.SelectedItem = "Arial";
            }
            cmbFont.SelectedIndexChanged += new EventHandler(cmbFont_SelectedIndexChanged);
        }

        #endregion Class Methods

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutEasiQ abt = new AboutEasiQ();
            abt.ShowDialog();
        }

        private void chkBigEditor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBigEditor.Checked)
            {
                trkProgress.Scroll -= trkProgress_Scroll;
                SendViewerStream(mnuUpdateViewer);
                System.Windows.Forms.Application.DoEvents();
                //while (!Classes.Controller.viewerLoaded) { }
                int gotoPerc = (int)Math.Ceiling(wpfEditorBox.GetPercentage());

                double viewPerc = (Classes.Controller.flipVisible ? Classes.Controller.flipViewer.wpfViewBox.Perc : wpfEditorBox.Perc);
                Classes.Controller.ShowBigEditor(true);
                Classes.Controller.bigEditor.wpfViewBox.Perc = Classes.Controller.editorPerc;
                System.Windows.Forms.Application.DoEvents();

                trkProgress.Value = (int)Classes.Controller.editorPerc;
                Classes.Controller.SetPercentage(trkProgress.Value);
                Classes.Controller.bigEditor.wpfViewBox.be = false;
                Classes.Controller.bigEditor.wpfViewBox.executeScroll = true;
                System.Windows.Forms.Application.DoEvents();
                if (Classes.Controller.flipVisible) { Classes.Controller.flipViewer.wpfViewBox.Perc = viewPerc; }
                Classes.Controller.bigEditor.SetPercentage(viewPerc);
                System.Windows.Forms.Application.DoEvents();
                trkProgress.Scroll += trkProgress_Scroll;
                SetMarkerNames();
                Classes.Controller.bigEditor.updateViewer(true);
                Classes.Controller.isPlaying = true;
            }
            else
            {
                chkPreview.Checked = false;
            }
        }

        public delegate void SetPercFromControllerDelegate(int direction);

        public void SetPercFromController(int direction)
        {
            Classes.Controller.PercentageChangedEvent -= Controller_PercentageChangedEvent;
            if (InvokeRequired)
            {
                this.Invoke(new SetPercFromControllerDelegate(SetPercFromController), direction);
            }
            else
            {
                if (Classes.Controller.bigEditor == null || !Classes.Controller.bigEditor.Visible)
                {
                    wpfEditorBox.ScrollEnd(direction);
                }
                trkProgress.Value = direction;
                Classes.Controller.SetPercentage(direction);
            }
            Classes.Controller.PercentageChangedEvent += Controller_PercentageChangedEvent;
            controllerEvent = false;
        }

        public void ChangeProgress(int progress)
        {
            trkProgress.Value = progress;
        }

        public bool changed = false;

        public void toggleBigBox()
        {
            chkBigEditor.Checked = false;
            wpfEditorBox.LoadText(Classes.Controller.playlist.ActiveStream.RTFContent, "editor");
            //wpfEditorBox.ResizeViewer(Classes.Controller.eFSize, false);
            wpfEditorBox.ResizeViewer(Classes.Controller.vFSize, false, false);
            SetMarkerNames();
            System.Windows.Forms.Application.DoEvents();
            wpfEditorBox.Focus();
            if (!changed)
            {
                chkPreview.Checked = true;
                chkPreview.Checked = false;
                changed = true;
            }
        }

        private void zoomEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkBigEditor.Checked = !chkBigEditor.Checked;
        }

        private void zoomEditorToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            chkBigEditor.Checked = !chkBigEditor.Checked;
        }

        private void decreaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdjustFontSize(-1);
        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdjustFontSize(1);
        }

        public delegate void AdjustDelegate(int adjust);

        public void AdjustFontSize(int adjustment)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AdjustDelegate(AdjustFontSize), adjustment);
            }
            else
            {
                if (adjustment < 0 && cmbSize.SelectedIndex > 0)
                {
                    cmbSize.SelectedIndex -= 1;
                }
                else if (adjustment > 0 && cmbSize.SelectedIndex < cmbSize.Items.Count - 1)
                {
                    cmbSize.SelectedIndex += 1;
                }
                Thread.Sleep(500);
            }
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            trkSpeed.Focus();
        }

        private void pnlEditorBox_MouseEnter(object sender, EventArgs e)
        {
            ctrlHost.Focus();
        }

        private void tvPlaylist_MouseEnter(object sender, EventArgs e)
        {
            trkSpeed.Focus();
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            trkSpeed.Focus();
        }

        private void chkLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    String fileName = ofd.FileName;
                    if (File.Exists(fileName))
                    {
                        imgPic = new Bitmap(fileName);
                        picLogo.Image = (Image)imgPic.Clone();
                        Classes.Controller.SetImage(imgPic);
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void licensingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Forms.frmActivation activationF = new Forms.frmActivation(Classes.Controller.myLicense);
            //if (activationF.ShowDialog() == System.Windows.Forms.DialogResult.OK) { Classes.Controller.myLicense = activationF.myLicense; }
        }

        private void launchControllerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //wiiMote = new Classes.WiiMote();
            //System.Windows.Forms.MessageBox.Show("Please press 1 + 2 on the controller simultaneously, then click OK");
            //if (!wiiMote.Connect())
            //{
            //    if (System.Windows.Forms.MessageBox.Show("No wiimote found! Launch dummy controller?", "WiiMote", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            //    {
            //        Forms.DummyController dc = new Forms.DummyController();
            //        dc.Show();
            //    }
            //}
            //else
            //{
            //    wiiStatus.Text = "Wiimote connected";
            //}
        }
    }
}