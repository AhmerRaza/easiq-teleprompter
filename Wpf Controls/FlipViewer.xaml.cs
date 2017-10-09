using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Teleprompter.Wpf_Controls {

    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class FlipViewer : Grid {

        public delegate void ScrollEventHandler(object sender, ScrollArgs args);

        private delegate void ScrollDelegate();

        private delegate void ScrollDirectDelegate(double cpos);

        public event ScrollEventHandler ScrollChanged;

        public event EventHandler<EventArgs> ExtentEvent;

        public event EventHandler<TransitionArgs> TransitionEvent;

        public event EventHandler<EventArgs> LoadedCompleteEvent;

        private const double lineHeight = 22.996;
        private String controlSent = "FlipViewer";
        public double bottomPos, cpos;
        private double p;
        private MemoryStream myMS;
        private double fontSize;
        private double maxPerc = 0;
        private bool isVisible = true;
        private double defaultSize = 974;

        public bool IsVisible {
            get { return isVisible; }
            set {
                isVisible = value;
                rtfEditor.Visibility = (isVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
            }
        }

        private double voffset = 0;

        public double VOffset {
            get { return rtfEditor.VerticalOffset; }
            set { ScrollDirect(value); }
        }

        public double[] ExtentHeight {
            get { return new double[] { rtfEditor.ExtentHeight, oflineHeight }; }
        }

        public double Perc {
            get {
                double vo = rtfEditor.VerticalOffset;
                double vh = rtfEditor.ViewportHeight;
                double eh = rtfEditor.ExtentHeight;
                double perc = 0;
                cpos = vo;
                perc = (eh != 0 ? vo / eh * 100 : 0);
                return perc;
            }
            set {
                p = value;
                double vo = rtfEditor.VerticalOffset;
                double eh = rtfEditor.ExtentHeight;
                double bp = bottomPos;
                cpos = (p * eh) / 100;
                ScrollDirect(cpos);
            }
        }

        public volatile int offset;

        public void ScrollDirect(double cppos) {
            rtfEditor.ScrollToVerticalOffset(cppos);
            Classes.Controller.scrollComplete = true;
        }

        public void ScrollDirectFromController(double newOffset) {
            cpos += newOffset;
            ScrollDirect(cpos);
        }

        private void GotoText(double pos) {
            int start = 0;
            double tPos = 0;
            TextPointer tp = rtfEditor.Document.ContentStart;
            TextPointer tpC = tp;
            while (tPos < pos) {
                tpC = GetPositionAtOffset(tp, start, LogicalDirection.Forward);
                tPos = tpC.GetCharacterRect(LogicalDirection.Forward).Top;
                start++;
            }
            FrameworkContentElement e = tpC.Parent as FrameworkContentElement;
            if (e != null) { e.BringIntoView(); }
        }

        private int GetOffsetInTextLength(TextPointer pointer1, TextPointer pointer2) {
            if (pointer1 == null || pointer2 == null) { return 0; }
            TextRange tr = new TextRange(pointer1, pointer2);
            return tr.Text.Length;
        }

        private TextPointer GetPositionAtOffset(TextPointer startingPoint, int offset, LogicalDirection direction) {
            TextPointer binarySearchPoint1 = null;
            TextPointer binarySearchPoint2 = null;

            // setup arguments appropriately
            if (direction == LogicalDirection.Forward) {
                binarySearchPoint2 = this.rtfEditor.Document.ContentEnd;
                if (offset < 0) { offset = Math.Abs(offset); }
            }

            if (direction == LogicalDirection.Backward) {
                binarySearchPoint2 = this.rtfEditor.Document.ContentStart;
                if (offset > 0) { offset = -offset; }
            }

            // setup for binary search
            bool isFound = false;
            TextPointer resultTextPointer = null;

            int offset2 = Math.Abs(GetOffsetInTextLength(startingPoint, binarySearchPoint2));
            int halfOffset = direction == LogicalDirection.Backward ? -(offset2 / 2) : offset2 / 2;

            binarySearchPoint1 = startingPoint.GetPositionAtOffset(halfOffset, direction);
            int offset1 = Math.Abs(GetOffsetInTextLength(startingPoint, binarySearchPoint1));

            // binary search loop

            while (isFound == false) {
                if (Math.Abs(offset1) == Math.Abs(offset)) {
                    isFound = true;
                    resultTextPointer = binarySearchPoint1;
                } else
                    if (Math.Abs(offset2) == Math.Abs(offset)) {
                        isFound = true;
                        resultTextPointer = binarySearchPoint2;
                    } else {
                        if (Math.Abs(offset) < Math.Abs(offset1)) {
                            // this is simple case when we search in the 1st half
                            binarySearchPoint2 = binarySearchPoint1;
                            offset2 = offset1;

                            halfOffset = direction == LogicalDirection.Backward ? -(offset2 / 2) : offset2 / 2;

                            binarySearchPoint1 = startingPoint.GetPositionAtOffset(halfOffset, direction);
                            offset1 = Math.Abs(GetOffsetInTextLength(startingPoint, binarySearchPoint1));
                        } else {
                            // this is more complex case when we search in the 2nd half
                            int rtfOffset1 = startingPoint.GetOffsetToPosition(binarySearchPoint1);
                            int rtfOffset2 = startingPoint.GetOffsetToPosition(binarySearchPoint2);
                            int rtfOffsetMiddle = (Math.Abs(rtfOffset1) + Math.Abs(rtfOffset2)) / 2;
                            if (direction == LogicalDirection.Backward) {
                                rtfOffsetMiddle = -rtfOffsetMiddle;
                            }

                            TextPointer binarySearchPointMiddle = startingPoint.GetPositionAtOffset(rtfOffsetMiddle, direction);
                            int offsetMiddle = GetOffsetInTextLength(startingPoint, binarySearchPointMiddle);

                            // two cases possible
                            if (Math.Abs(offset) < Math.Abs(offsetMiddle)) {
                                // 3rd quarter of search domain
                                binarySearchPoint2 = binarySearchPointMiddle;
                                offset2 = offsetMiddle;
                            } else {
                                // 4th quarter of the search domain
                                binarySearchPoint1 = binarySearchPointMiddle;
                                offset1 = offsetMiddle;
                            }
                        }
                    }
            }

            return resultTextPointer;
        }

        public FlipViewer() {
            InitializeComponent();
            rtfEditor.Foreground = (Properties.Settings.Default.defaultFore == "White" ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black);
            rtfEditor.Background = (Properties.Settings.Default.defaultBack == "White" ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black);
        }

        public void ChangeDefaultFontSize() {
            rtfEditor.FontSize = Properties.Settings.Default.vfsize;
        }

        private void rtfEditor_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                if (TogglePreviewEvent != null) { TogglePreviewEvent(this, new EventArgs()); }
            }
        }

        private void rtfEditor_PreviewKeyUp(object sender, KeyEventArgs e) {
            if ((e.Key == Key.Space) || (e.Key == Key.D1 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) || e.Key == Key.Enter || e.Key == Key.OemQuestion ||
                e.Key == Key.OemComma || e.Key == Key.OemPeriod || e.Key == Key.OemSemicolon) {
                TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
                MemoryStream ms = new MemoryStream();
                textRange.Save(ms, DataFormats.Rtf);
                if (controlSent == "Editor") {
                    if (TextChangedEvent != null) { TextChangedEvent(this, new TextArgs(ms)); }
                }
            }
        }

        private volatile bool textLoading = false;

        private void rtfEditor_TextChanged(object sender, TextChangedEventArgs e) {
            if (!textLoading) {
                bottomPos = rtfEditor.Document.ContentEnd.GetCharacterRect(LogicalDirection.Forward).Top;
            }
        }

        private bool flipMePlease;

        public void LoadText(MemoryStream ms, String fromWhere, bool flipMe) {
            flipMePlease = flipMe;
            Dispatcher.BeginInvoke(new Action(
                delegate() {
                    textLoading = true;
                    controlSent = fromWhere;
                    FlowDocument fd = new FlowDocument();
                    TextRange textRange = new TextRange(fd.ContentStart, fd.ContentEnd);
                    if (ms == null) {
                        textRange.Text = "";
                    } else {
                        textRange.Load(ms, DataFormats.Rtf);
                    }
                    rtfEditor.Document = fd;
                    controlSent = "FlipViewer";
                    myMS = ms;
                    IsVisible = isVisible;
                    rtfEditor.Document.PageWidth = defaultSize;
                    List<String> markerNames = new List<string>();
                    List<String> smarkerNames = new List<string>();
                    foreach (Classes.Marker m in Classes.Controller.playlist.ViewerStream.Markers) { markerNames.Add(m.InlineName); }
                    foreach (Classes.Marker m in Classes.Controller.playlist.ViewerStream.SlideMarkers) { smarkerNames.Add(m.InlineName); }
                    SetMarkerNames(markerNames, 1);
                    SetMarkerNames(smarkerNames, 2);
                    textLoading = false;
                    ResizeViewer(Classes.Controller.vFSize, false);
                    if (flipMe) {
                        Flipper(Classes.Controller.flipX, Classes.Controller.flipY, Classes.Controller.flipB, Classes.Controller.flipViewer.myHeight);
                    }
                    if (LoadedCompleteEvent != null) { LoadedCompleteEvent(this, new EventArgs()); }
                }
            ), null);
        }

        public void LoadText(Classes.QStream stream, bool flipMe) {
            flipMePlease = flipMe;
            Dispatcher.BeginInvoke(new Action(
                delegate() {
                    textLoading = true;
                    rtfEditor.Document = stream.Document;
                    controlSent = "FlipViewer";
                    myMS = stream.RTFContent;
                    IsVisible = isVisible;
                    rtfEditor.Document.PageWidth = defaultSize;
                    List<String> markerNames = new List<string>();
                    List<String> smarkerNames = new List<string>();
                    foreach (Classes.Marker m in stream.Markers) { markerNames.Add(m.InlineName); }
                    foreach (Classes.Marker m in stream.SlideMarkers) { smarkerNames.Add(m.InlineName); }
                    SetMarkerNames(markerNames, 1);
                    SetMarkerNames(smarkerNames, 2);
                    textLoading = false;
                    ResizeViewer(Classes.Controller.vFSize, false);
                    if (flipMe) { Flipper(Classes.Controller.flipX, Classes.Controller.flipY, Classes.Controller.flipB, Classes.Controller.flipViewer.myHeight); }
                    if (LoadedCompleteEvent != null) { LoadedCompleteEvent(this, new EventArgs()); }
                }
            ), null);
        }

        public void ReloadText() {
            LoadText(myMS, "FlipViewer", flipMePlease);
            ResizeViewer(fontSize, false);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            cpos = 0;
        }

        private void rtfEditor_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            //Classes.Controller.scrollComplete = Classes.Controller.isPlaying;
            GetPercentage();
            Classes.Controller.scrollComplete = Classes.Controller.isPlaying;
        }

        private void rtfEditor_SelectionChanged(object sender, RoutedEventArgs e) {
            GetPercentage();
        }

        private void GetPercentage() {
            double vo = rtfEditor.VerticalOffset;
            double vh = rtfEditor.ViewportHeight;
            double eh = rtfEditor.ExtentHeight;
            double perc = 0;
            cpos = vo;
            perc = (eh != 0 ? vo / eh * 100 : 0);
            bool perc100 = IsAt100();
            perc = (perc100 ? 100 : perc);
            if (perc == 0 || perc100) {
                if (ExtentEvent != null) { ExtentEvent(this, new EventArgs()); }
            }
            if (ScrollChanged != null && !markerChange) { ScrollChanged(this, new ScrollArgs(perc)); } else { markerChange = false; }
        }

        public bool IsAt100() {
            bool isat100 = false;
            // get the vertical scroll position
            double dVer = rtfEditor.VerticalOffset;

            //get the vertical size of the scrollable content area
            double dViewport = rtfEditor.ViewportHeight;

            //get the vertical size of the visible content area
            double dExtent = bottomPos;// rtfEditor.ExtentHeight;

            if (dVer != 0 && dVer + dViewport == dExtent) { isat100 = true; }
            if (isat100) {
                Classes.Controller.TogglePlay();
            }
            return isat100;
        }

        private double oflineHeight = 0;
        public bool isFromFont;

        public void ResizeViewer(double size, bool fromFont) {
            isFromFont = fromFont;
            fontSize = size;
            using (var d = Dispatcher.DisableProcessing()) {
                TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
                textRange.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                textRange.ApplyPropertyValue(Run.BaselineAlignmentProperty, "Center");
                Rect rect = rtfEditor.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                double height = rect.Height;
                double imgHeight = height * 0.75;
                height = height * 0.9;
                oflineHeight = height;
                ResizeImages(imgHeight);
                double myHeight = rtfEditor.Height;
                rtfEditor.SetValue(Paragraph.LineHeightProperty, height);
                if (bottomPos != rtfEditor.ExtentHeight) { bottomPos = rtfEditor.ExtentHeight; }
            }
            isFromFont = false;
        }

        private void ResizeImages(double newHeight) {
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines) {
                        if (inline is InlineUIContainer) {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            uiContainer.BaselineAlignment = BaselineAlignment.Bottom;
                            if (uiContainer.Child is Image) {
                                Image img = (Image)uiContainer.Child;
                                img.Height = newHeight;
                                img.Width = newHeight / 2;
                            }
                        }
                    }
                }
            }
        }

        public event EventHandler<TextArgs> TextChangedEvent;

        public event EventHandler<EventArgs> TogglePreviewEvent;

        private void Grid_MouseEnter(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.None;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        public String GetText() {
            TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            return textRange.Text;
        }

        private double cx, cy;
        public bool fx, fy, fb;

        public void Flipper(bool x, bool y, bool both, double parentHeight) {
            using (var d = Dispatcher.DisableProcessing()) {
                double rtfWidth = rtfEditor.Width;
                double rtfHeight = rtfEditor.Height;
                cx = rtfWidth / 2;
                cy = rtfHeight / 2;
                if ((!x && !y && !both) && (fx || fy)) {
                    rtfEditor.RenderTransform = new ScaleTransform(1, 1, cx, cy);
                    fx = false;
                    fy = false;
                    fb = false;
                } else if (both) {
                    fx = false;
                    fy = false;
                    fb = true;
                    rtfEditor.RenderTransform = new RotateTransform(180, cx, cy);
                } else if (x) {
                    fx = true;
                    fy = false;
                    fb = false;
                    rtfEditor.RenderTransform = new ScaleTransform(-1, 1, cx, cy);
                } else if (y) {
                    fx = false;
                    fy = true;
                    fb = false;
                    rtfEditor.RenderTransform = new ScaleTransform(1, -1, cx, cy);
                }
            }
        }

        private void rtfEditor_MouseEnter(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.None;
        }

        private void rtfEditor_MouseLeave(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private double opos = 0;

        public void Scroll() {
            //if (rtfEditor.Dispatcher.CheckAccess()) {
            using (var d = Dispatcher.DisableProcessing()) {
                if (cpos >= 0 && cpos < bottomPos) {
                    cpos += offset;

                    rtfEditor.ScrollToVerticalOffset(cpos);
                }
                double vh = rtfEditor.ViewportHeight;
                if (cpos <= 0 || cpos + (vh - oflineHeight) >= bottomPos) {
                    cpos = (cpos <= 0 ? 0 : cpos = bottomPos);
                    if ((Classes.Controller.playSpeed < 0 && cpos == 0) || (Classes.Controller.playSpeed > 0 && cpos == bottomPos)) {
                        //MessageBox.Show(cpos != 0 ? "I am at the bottom" + cpos.ToString() : "I am at the top");

                        Classes.Controller.scrollComplete = true;
                        Classes.Controller.TogglePlay();
                    }
                }
            }
            // } else {
            //     rtfEditor.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new ScrollDelegate(Scroll));
            // }
        }

        public void FireTransition(bool isForward) {
            if (TransitionEvent != null) { TransitionEvent(this, new TransitionArgs(isForward)); }
        }

        public Inline GotoMarker(String name) {
            Inline ri = null;
            bool foundMe = false;
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    var paragraph = block as Paragraph;
                    foreach (Inline i in paragraph.Inlines) {
                        //MessageBox.Show("expected = " + name + "; actual = " + i.Name);
                        if (name == i.Name) {
                            //i.BringIntoView();
                            ri = i;
                            foundMe = true;
                            break;
                        }
                    }
                }
                if (foundMe) { break; }
            }
            return ri;
        }

        private bool markerChange = false;

        public void ScrollToMarker(Inline i) {
            markerChange = true;
            double absStart = rtfEditor.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward).Top;
            double markerStart = i.ContentStart.GetCharacterRect(LogicalDirection.Forward).Top;
            int itpCurrent = (int)Math.Floor(markerStart);

            if (itpCurrent < 0) {
                i.BringIntoView();
            } else {
                if (absStart < 0) {
                    absStart *= -1;
                    itpCurrent += (int)absStart;
                }
                ScrollDirect(itpCurrent);
            }
        }

        public void SetMarkerNames(List<String> names, int markerType) {
            List<String> intMarkerNames = names;
            int nI = 0;
            Image testImage = Classes.Controls.MarkerImage();
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines) {
                        if (inline is InlineUIContainer) {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            if (uiContainer.Child is Image && nI < names.Count) {
                                int mType = GetElementRTF(uiContainer);
                                if (markerType == mType) {
                                    inline.Name = names[nI];
                                    //MessageBox.Show(inline.Name + " set");
                                    nI++;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SetPageWidth(double pageWidth) {
            defaultSize = pageWidth;
            rtfEditor.Document.PageWidth = pageWidth;
        }

        private int GetElementRTF(InlineUIContainer ic) {
            TextRange tr = new TextRange(ic.ContentStart, ic.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Rtf);
            string rtfText = ASCIIEncoding.Default.GetString(ms.ToArray());
            int imageType = 0;
            if (rtfText.Contains(Classes.Controller.MarkerRTF) || rtfText.Contains(Classes.Controller.MarkerRTF2) || rtfText.Contains(Classes.Controller.WhiteRTF) || rtfText.Contains(Classes.Controller.XPRtf) || rtfText.Contains(Classes.Controller.XPRtf2)) {
                imageType = 1;
                //MessageBox.Show("type 1");
            } else if (rtfText.Contains(Classes.Controller.SlideMarkerRTF)) {
                imageType = 2;
                //MessageBox.Show("type 2");
            } else {
                //MessageBox.Show(rtfText);
            }
            return imageType;
        }

        public void ChangePageWidth(double width) {
            rtfEditor.Document.PageWidth = width;
            rtfEditor.Width = width;
        }

        public void RemoveMarkers(List<String> names) {
            List<Inline> removeC = new List<Inline>();
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    var paragraph = block as Paragraph;
                    InlineCollection c = paragraph.Inlines;
                    foreach (Inline i in c) {
                        if (!String.IsNullOrEmpty(i.Name)) {
                            bool here = true;
                        }

                        try {
                            foreach (String mName in names) {
                                if (mName == i.Name) {
                                    removeC.Add(i);
                                    break;
                                }
                            }
                        } catch { }
                    }
                }
            }
            foreach (Inline i in removeC) { RemoveMarker(i); }
        }

        private void RemoveMarker(Inline i) {
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    var paragraph = block as Paragraph;
                    if (paragraph.Inlines.Contains(i)) {
                        paragraph.Inlines.Remove(i);

                        break;
                    }
                }
            }
        }
    }
}