using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Teleprompter.Wpf_Controls {

    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Grid {

        #region Public Events

        public delegate void ScrollEventHandler(object sender, ScrollArgs args);

        public event EventHandler<MarkerAddArgs> MarkerAddedEvent;

        public event EventHandler<EventArgs> PrevMarkerEvent;

        public event EventHandler<EventArgs> NextMarkerEvent;

        public event EventHandler<EventArgs> CheckMarkerEvent;

        public event EventHandler<MarkerRemovedArgs> MarkerRemovedEvent;

        public event EventHandler<EventArgs> DisablePreviewEvent;

        public event ScrollEventHandler ScrollChanged;

        public event EventHandler<TextArgs> TextChangedEvent;

        public event EventHandler<EventArgs> TogglePreviewEvent;

        public event EventHandler<EventArgs> EscapeEvent;

        public event EventHandler<EventArgs> ToggleSwitchEvent;

        public event EventHandler<EventArgs> ForceToggleSwitchEvent;

        public event EventHandler<EventArgs> BigEditUpdateEvent;

        public event EventHandler<EventArgs> ForceScrollOffEvent;

        public event EventHandler<EventArgs> AdjustFontUpEvent;

        public event EventHandler<EventArgs> AdjustFontDownEvent;

        public event EventHandler<IMarkerArgs> InternalMarkerEvent;

        public event EventHandler<EventArgs> SelectionChangedEvent;

        #endregion Public Events

        #region Private Delegates

        private delegate void ScrollDirectDelegate(double cpos);

        #endregion Private Delegates

        #region Public Variables

        public bool trigger = false;
        public double bottomPos, cpos;
        public double fontSize = 25;
        public bool pasted, formatFlag;
        public String lastCommand;
        public double iniPageWidth;

        #endregion Public Variables

        #region Private Variables

        private Dictionary<int[], String> markerList;
        private double p;
        private const double lineHeight = 22.996;
        private String controlSent = "Editor";
        private List<InlineUIContainer> intMarkers;
        private MemoryStream undoMS;
        private List<String> intMarkerNames;
        private TextPointer tp;
        private double beforePaste = 0;
        private double afterPaste = 0;
        private volatile bool markerScroll = false;
        private volatile Inline currentMarker = null;
        private bool isDelete;

        #endregion Private Variables

        #region Public Properties

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
                cpos = (p * eh) / 100;
                ScrollDirect(cpos);
                cpos = rtfEditor.VerticalOffset;
            }
        }

        public FlowDocument GetDocument {
            get {
                return CloneDocument();
            }
        }

        #endregion Public Properties

        #region Constructor

        public bool be = false;
        private String mySender = "";

        public Editor(bool bigeditor) {
            InitializeComponent();
            tp = rtfEditor.GetPositionFromPoint(new Point(0, 0), true);
            if (bigeditor) {
                be = true;
                mySender = "big";
            } else {
                mySender = "small";
            }
            formatFlag = false;
            DataObject.AddPastingHandler(rtfEditor, MyPasteCommand);
            CommandManager.AddPreviewCanExecuteHandler(rtfEditor, onPreviewCanExecute);
            CommandManager.AddPreviewExecutedHandler(rtfEditor, onPreviewExecuted);
            rtfEditor.KeyDown += rtfEditor_KeyDown;
            rtfEditor.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, CutExecuted, CanCut));
            rtfEditor.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, CutExecuted, CanCut));
            rtfEditor.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, CutExecuted, CanCut));
            rtfEditor.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, CutExecuted, CanCut));
            rtfEditor.Foreground = (Properties.Settings.Default.defaultFore == "White" ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black);
            rtfEditor.Background = (Properties.Settings.Default.defaultBack == "White" ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.Black);
            iniPageWidth = (974 / 3) + 7;
            SetPageWidth(iniPageWidth);
            isDelete = false;
        }

        private void rtfEditor_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Back || e.Key == Key.Delete) {
                isDelete = true;
            } else {
                isDelete = false;
            }
        }

        #endregion Constructor

        #region Class Events

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            cpos = 0;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.IBeam;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void rtfEditor_PreviewKeyDown(object sender, KeyEventArgs e) {
            isDelete = false;
            if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                if (TogglePreviewEvent != null) { TogglePreviewEvent(this, new EventArgs()); }
            } else if (e.Key == Key.Back || e.Key == Key.Delete) {
                isDelete = true;
            }
        }

        private void rtfEditor_PreviewKeyUp(object sender, KeyEventArgs e) {
            if ((e.Key == Key.Space) || (e.Key == Key.D1 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) || e.Key == Key.Enter || e.Key == Key.OemQuestion ||
                e.Key == Key.OemComma || e.Key == Key.OemPeriod || e.Key == Key.OemSemicolon || e.Key == Key.Back || e.Key == Key.Delete ||
                (e.Key == Key.B && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) || (e.Key == Key.I && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                || (e.Key == Key.U && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)) {
                if (e.Key == Key.Back || e.Key == Key.Delete) {
                    isDelete = true;
                } else {
                    isDelete = false;
                }
                GetContent();
            } else if ((e.Key == Key.X || e.Key == Key.Z || e.Key == Key.Y) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                GetContent();
            }
        }

        private void rtfEditor_TextChanged(object sender, TextChangedEventArgs e) {
            foreach (TextChange change in e.Changes) {
                if (change.RemovedLength > 0 && isDelete) {
                    List<String> removeMarkerNames = new List<string>();
                    foreach (KeyValuePair<int[], String> markerPos in markerList) {
                        int sDel = change.Offset;
                        int rLength = change.RemovedLength;
                        int eDel = sDel + rLength;
                        int sMark = markerPos.Key[0];
                        int eMark = markerPos.Key[1];
                        String mName = markerPos.Value;
                        while (sDel <= eDel) {
                            if (sDel == sMark && rLength >= 0) {
                                if (MarkerRemovedEvent != null) { MarkerRemovedEvent(this, new MarkerRemovedArgs(mName)); }
                                break;
                            }
                            sDel++;
                            rLength--;
                        }
                    }
                }
            }
            isDelete = false;
            GetImages();
            if (pasted) {
                pasted = false;
                ResizeViewer(Classes.Controller.vFSize, false, (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible));
                afterPaste = rtfEditor.VerticalOffset;
                if (afterPaste != beforePaste) {
                    cpos = beforePaste;
                    ScrollDirect(cpos);
                }
                GetContent();
            }

            bottomPos = rtfEditor.Document.ContentEnd.GetCharacterRect(LogicalDirection.Forward).Top;
            Object obj1 = rtfEditor.Document;
            if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible && Classes.Controller.BigEditorLoaded) {
                if (trigger && DisablePreviewEvent != null) { DisablePreviewEvent(this, new EventArgs()); }
                if (ForceToggleSwitchEvent != null) { ForceToggleSwitchEvent(this, new EventArgs()); }
                if (BigEditUpdateEvent != null) { BigEditUpdateEvent(this, new EventArgs()); }
            } else if (Classes.Controller.bigEditor == null || !Classes.Controller.bigEditor.Visible) {
                if (trigger && DisablePreviewEvent != null) { DisablePreviewEvent(this, new EventArgs()); }
                if (ForceToggleSwitchEvent != null) { ForceToggleSwitchEvent(this, new EventArgs()); }
                if (BigEditUpdateEvent != null) { BigEditUpdateEvent(this, new EventArgs()); }
            }
        }

        public bool executeScroll = false;

        private void rtfEditor_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            if (executeScroll) {
                if (!isFromFont && !be) {
                    if (!markerChange) {
                        if (ScrollChanged != null) { ScrollChanged(this, new ScrollArgs(GetPercentage2())); }
                    } else {
                        markerChange = false;
                    }
                } else {
                    isFromFont = false;
                }
            } else if (!executeScroll && ((be && mySender == "small" && Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible == true) || (!be && mySender == "big"))) {
            } else {
                if (!isFromFont && !be) {
                    if (!markerChange) {
                        if (ScrollChanged != null) { ScrollChanged(this, new ScrollArgs(GetPercentage2())); }
                    } else {
                        markerChange = false;
                    }
                } else {
                    isFromFont = false;
                }
            }
        }

        private void rtfEditor_SelectionChanged(object sender, RoutedEventArgs e) {
            if (SelectionChangedEvent != null) { SelectionChangedEvent(this, new EventArgs()); }
            caretPos = rtfEditor.Selection.Start;
        }

        public TextPointer CaretPosition {
            get {
                return rtfEditor.Selection.Start;
            }
        }

        #endregion Class Events

        #region Find Text

        public void FoundText(String texttobefound, bool up) {
            rtfEditor.SelectionBrush = Brushes.LightSkyBlue;
            if (tp == null) { tp = rtfEditor.GetPositionFromPoint(new Point(1, 1), true); }
            TextRange tr = GetTextRangeFromPosition(ref tp, texttobefound, up);
            if (tr != null) {
                Paragraph para = tr.Start.Paragraph;
                if (para != null) {
                    para.BringIntoView();
                }
                rtfEditor.Selection.Select(tr.Start, tr.End);
            }
        }

        public TextRange GetTextRangeFromPosition(ref TextPointer position, String input, bool up) {
            TextRange textRange = null;
            while (position != null) {
                if (position.CompareTo(rtfEditor.Document.ContentEnd) == 0) {
                    break;
                }

                if (position.GetPointerContext((!up ? LogicalDirection.Forward : LogicalDirection.Backward)) == TextPointerContext.Text) {
                    String textRun = position.GetTextInRun((!up ? LogicalDirection.Forward : LogicalDirection.Backward));
                    StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase;
                    Int32 indexInRun = textRun.IndexOf(input, stringComparison);

                    if (indexInRun >= 0) {
                        position = position.GetPositionAtOffset(indexInRun);
                        TextPointer nextPointer = position.GetPositionAtOffset(input.Length);
                        textRange = new TextRange(position, nextPointer);
                        position = position.GetPositionAtOffset(textRun.Length);
                        break;
                    } else {
                        position = position.GetNextContextPosition((!up ? LogicalDirection.Forward : LogicalDirection.Backward));
                    }
                } else {
                    position = position.GetNextContextPosition((!up ? LogicalDirection.Forward : LogicalDirection.Backward));
                }
            }

            return textRange;
        }

        private TextPointer current;
        private TextPointer caretPos;
        public TextPointer sStartPointer;

        public void FindText(String keyword, bool up, String newString, bool replace, bool matchCase, bool wholeWord) {
            bool found = false;
            FindAllInstances(keyword, up, matchCase, wholeWord);
            if (foundWords.Count == 0) {
                MessageBox.Show("Text not found in selection", "Find and Replace", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (up) { foundWords.Reverse(); }
            FindNext(newString, replace);
        }

        public void FindNext(String newString, bool replace) {
            try {
                if (foundIdx < foundWords.Count) {
                    rtfEditor.Selection.Select(foundWords[foundIdx].Start, foundWords[foundIdx].End);
                    Paragraph para = foundWords[foundIdx].Start.Paragraph;
                    if (para != null) { para.BringIntoView(); }
                    if (replace) {
                        rtfEditor.Selection.Text = newString;
                        foundWords[foundIdx].Text = newString;
                    }
                    sStartPointer = rtfEditor.Selection.Start;
                    rtfEditor.Focus();
                    foundIdx++;
                } else {
                    MessageBox.Show("No more instances found", "Find and Replace", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } catch { }
        }

        private List<TextRange> foundWords;
        public int foundIdx;
        public TextPointer currentPointer;

        private void FindAllInstances(String keyword, bool up, bool matchCase, bool wholeWord) {
            foundWords = new List<TextRange>();
            TextRange text;
            if (up) {
                currentPointer = (rtfEditor.Selection.Start != null ? rtfEditor.Selection.Start : rtfEditor.Document.ContentEnd);
                text = new TextRange(rtfEditor.Document.ContentStart, currentPointer);
            } else {
                currentPointer = (rtfEditor.Selection.End != null ? rtfEditor.Selection.End : rtfEditor.Document.ContentStart);
                text = new TextRange(currentPointer, rtfEditor.Document.ContentEnd);
            }
            LogicalDirection ld = LogicalDirection.Forward;
            currentPointer = text.Start.GetInsertionPosition((up ? LogicalDirection.Backward : LogicalDirection.Forward));
            while (currentPointer != null) {
                StringComparison stringComparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                string textInRun = currentPointer.GetTextInRun(ld);
                if (!string.IsNullOrWhiteSpace(textInRun)) {
                    int index = textInRun.IndexOf(keyword, stringComparison);

                    while (index != -1) {
                        TextPointer selectionStart = currentPointer.GetPositionAtOffset(index, LogicalDirection.Forward);
                        TextPointer selectionEnd = selectionStart.GetPositionAtOffset(keyword.Length, LogicalDirection.Forward);
                        TextRange selection = new TextRange(selectionStart, selectionEnd);
                        bool foundMe = false;
                        if (wholeWord) {
                            if (IsWholeWord(selection)) { foundMe = true; }
                        } else {
                            foundMe = true;
                        }
                        if (foundMe && !foundWords.Contains(selection)) { foundWords.Add(selection); }
                        index = textInRun.IndexOf(keyword, index + 1, stringComparison);
                    }
                }
                currentPointer = currentPointer.GetNextContextPosition(ld);
            }
        }

        private Boolean IsWordChar(Char character) {
            return Char.IsLetterOrDigit(character) || character == '_';
        }

        /// <summary>
        /// Tests if the string within the specified<see cref="TextRange"/>instance is a word.
        /// </summary>
        /// <param name="textRange"><see cref="TextRange"/>instance to test</param>
        /// <returns>test result</returns>
        private Boolean IsWholeWord(TextRange textRange) {
            Char[] chars = new Char[1];
            if (textRange.Start.CompareTo(rtfEditor.Document.ContentStart) == 0 || textRange.Start.IsAtLineStartPosition) {
                textRange.End.GetTextInRun(LogicalDirection.Forward, chars, 0, 1);
                return !IsWordChar(chars[0]);
            } else if (textRange.End.CompareTo(rtfEditor.Document.ContentEnd) == 0) {
                textRange.Start.GetTextInRun(LogicalDirection.Backward, chars, 0, 1);
                return !IsWordChar(chars[0]);
            } else {
                textRange.End.GetTextInRun(LogicalDirection.Forward, chars, 0, 1);
                if (!IsWordChar(chars[0])) {
                    textRange.Start.GetTextInRun(LogicalDirection.Backward, chars, 0, 1);
                    return !IsWordChar(chars[0]);
                }
            }
            return false;
        }

        #endregion Find Text

        #region Scroll Engine

        public void ScrollEnd(double cpos) {
            this.SetPercentage(cpos);
        }

        public void ScrollDirect(double cpos) {
            if (rtfEditor.Dispatcher.CheckAccess()) {
                using (var d = Dispatcher.DisableProcessing()) { rtfEditor.ScrollToVerticalOffset(cpos); }
            } else {
                rtfEditor.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new ScrollDirectDelegate(ScrollDirect), cpos);
            }
        }

        public void ScrollDirectFromController(double newOffset) {
            cpos += newOffset;
            ScrollDirect(cpos);
        }

        #endregion Scroll Engine

        #region Formatting

        private void onPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e) {
            if (e.Command == ApplicationCommands.Paste) {
                e.CanExecute = true;
                e.Handled = true;
                beforePaste = rtfEditor.VerticalOffset;
            } else if (e.Command == ApplicationCommands.Delete) {
                isDelete = true;
            }
        }

        private void onPreviewExecuted(object sender, ExecutedRoutedEventArgs e) {
            if (e.Command == ApplicationCommands.Paste) {
                String contents = Clipboard.GetText();
                pasted = true;
                e.Handled = true;
                rtfEditor.Paste();
                afterPaste = rtfEditor.VerticalOffset;
                if (beforePaste != afterPaste) {
                    ScrollDirect(beforePaste);
                }
            } else if (e.Command == ApplicationCommands.Cut) {
                CheckCutText();
                e.Handled = true;
                rtfEditor.Cut();
                GetImages();
            } else if (e.Command == ApplicationCommands.Delete) {
                isDelete = true;
            }
        }

        private void MyPasteCommand(object sender, DataObjectEventArgs e) {
            String contents = Clipboard.GetText();
            pasted = true;
        }

        private void CanCut(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = false;
        }

        private void BlockTheCommand(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = false;
        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = false;
        }

        private void CutExecuted(object sender, ExecutedRoutedEventArgs e) {
            if (e.Command == ApplicationCommands.Cut) {
                CheckCutText();
                ApplicationCommands.Cut.Execute(null, rtfEditor);
                GetImages();
            } else if (e.Command == ApplicationCommands.Undo) {
                ApplicationCommands.Undo.Execute(null, rtfEditor);
            } else if (e.Command == ApplicationCommands.Redo) {
                ApplicationCommands.Redo.Execute(null, rtfEditor);
            } else if (e.Command == ApplicationCommands.Delete) {
                isDelete = true;
                ApplicationCommands.Delete.Execute(null, rtfEditor);
            }
            GetContent();
        }

        private void CheckCutText() {
            List<String> removeMarkerNames = new List<string>();
            foreach (KeyValuePair<int[], String> markerPos in markerList) {
                int sDel = rtfEditor.Document.ContentStart.GetOffsetToPosition(rtfEditor.Selection.Start);
                int eDel = rtfEditor.Document.ContentStart.GetOffsetToPosition(rtfEditor.Selection.End);
                int mDel;
                if (sDel > eDel) {
                    mDel = sDel;
                    sDel = eDel;
                    eDel = sDel;
                }
                int rLength = eDel - sDel;
                int sMark = markerPos.Key[0];
                int eMark = markerPos.Key[1];
                String mName = markerPos.Value;
                while (sDel <= eDel) {
                    if (sDel == sMark && rLength >= 0) {
                        if (MarkerRemovedEvent != null) { MarkerRemovedEvent(this, new MarkerRemovedArgs(mName)); }
                        break;
                    }
                    sDel++;
                    rLength--;
                }
            }
        }

        public void UndoCase() {
            double myPerc = GetPercentage();
            LoadText(undoMS, "editor");
            SetPercentage(myPerc);
            lastCommand = String.Empty;
            GetContent();
        }

        public void ToggleBold() {
            EditingCommands.ToggleBold.Execute(null, rtfEditor);
            GetContent();
        }

        public void ToggleAllBold() {
            TextPointer tsp = rtfEditor.Selection.Start;
            TextPointer tep = rtfEditor.Selection.End;
            TextRange textRange;
            if (tsp == tep) {
                textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            } else {
                textRange = new TextRange(rtfEditor.Selection.Start, rtfEditor.Selection.End);
            }
            if (!textRange.IsEmpty) { textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold); }
            rtfEditor.Selection.Select(tsp, tep);
        }

        public void ToggleAllRegular() {
            TextPointer tsp = rtfEditor.Selection.Start;
            TextPointer tep = rtfEditor.Selection.End;
            TextRange textRange;
            if (tsp == tep) {
                textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            } else {
                textRange = new TextRange(rtfEditor.Selection.Start, rtfEditor.Selection.End);
            }

            if (!textRange.IsEmpty) { textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular); }
            rtfEditor.Selection.Select(tsp, tep);
        }

        public void ToggleItalic() {
            EditingCommands.ToggleItalic.Execute(null, rtfEditor);
            GetContent();
        }

        public void ToggleUnderline() {
            EditingCommands.ToggleUnderline.Execute(null, rtfEditor);
            GetContent();
        }

        public void ChangeCase(bool upper) {
            undoMS = GetUndoContent();
            lastCommand = "changecase";
            String selectedText = rtfEditor.Selection.Text;
            if (String.IsNullOrEmpty(selectedText)) {
                try {
                    Plan2(upper);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Case change error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else {
                if (upper) {
                    rtfEditor.Selection.Text = rtfEditor.Selection.Text.ToUpper();
                } else {
                    String[] punctuation = new String[] { "!", ";", ":", ".", "?" };
                    String lastChar = "";
                    String outputString = "";
                    for (int i = 0; i < rtfEditor.Selection.Text.Length; i++) {
                        String cChar = rtfEditor.Selection.Text.Substring(i, 1);
                        if (i == 0) {
                            outputString += cChar.ToUpper();
                        } else {
                            if (punctuation.Contains(cChar)) { lastChar = cChar; }
                            if (punctuation.Contains(lastChar) && cChar != lastChar && cChar != " ") {
                                lastChar = "";
                                cChar = cChar.ToUpper();
                            } else {
                                cChar = cChar.ToLower();
                            }
                            outputString += cChar;
                        }
                    }
                    rtfEditor.Selection.Text = outputString;
                }
            }
            GetContent();
        }

        public void ChangeTextColor(int color) {
            SolidColorBrush brushColor;
            switch (color) {
                case 1:
                    brushColor = Brushes.White;
                    break;

                case 2:
                    brushColor = Brushes.Yellow;
                    break;

                case 3:
                    brushColor = Brushes.Lime;
                    break;

                case 4:
                    brushColor = Brushes.Red;
                    break;

                case 5:
                    brushColor = Brushes.Blue;
                    break;

                default:
                    brushColor = Brushes.White;
                    break;
            }
            TextPointer tsp = rtfEditor.Selection.Start;
            TextPointer tep = rtfEditor.Selection.End;
            TextRange textRange;
            if (tsp != tep) {
                textRange = new TextRange(rtfEditor.Selection.Start, rtfEditor.Selection.End);
                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, brushColor);
                GetContent();
            }
        }

        private void Plan2(bool upper) {
            FlowDocument forConversion = MakeACopy2(rtfEditor.Document);
            ToUpperFlowDocument(forConversion, upper);
            rtfEditor.Document = forConversion;
        }

        private void ToUpperFlowDocument(FlowDocument doc, bool upper) {
            List<TextElement> elements = new List<TextElement>();
            //rtfEditor.Selection
            foreach (TextElement element in doc.Blocks) {
                elements.Add(element);
            }
            foreach (TextElement element in elements) {
                if (upper) {
                    ToUpperTextElement(element);
                } else {
                    ToSentenceCase(element);
                }
            }
        }

        private void ToUpperTextElement(TextElement element) {
            Run run = element as Run;
            if (run == null) {
                List<object> children = new List<object>();
                foreach (object child in LogicalTreeHelper.GetChildren(element)) {
                    children.Add(child);
                }
                foreach (object child in children) {
                    TextElement elem = child as TextElement;
                    if (elem != null) {
                        ToUpperTextElement(elem);
                    }
                }
            } else {
                run.Text = run.Text.ToUpper();
            }
        }

        private void ToSentenceCase(TextElement element) {
            String[] punctuation = new String[] { "!", ";", ":", ".", "?" };
            Run run = element as Run;
            if (run == null) {
                List<object> children = new List<object>();
                foreach (object child in LogicalTreeHelper.GetChildren(element)) {
                    children.Add(child);
                }
                foreach (object child in children) {
                    TextElement elem = child as TextElement;
                    if (elem != null) {
                        ToSentenceCase(elem);
                    }
                }
            } else {
                String lastChar = "";
                String outputString = "";
                for (int i = 0; i < run.Text.Length; i++) {
                    String cChar = run.Text.Substring(i, 1);
                    if (i == 0) {
                        outputString += cChar.ToUpper();
                    } else {
                        if (punctuation.Contains(cChar)) { lastChar = cChar; }
                        if (punctuation.Contains(lastChar) && cChar != lastChar && cChar != " ") {
                            lastChar = "";
                            cChar = cChar.ToUpper();
                        } else {
                            cChar = cChar.ToLower();
                        }
                        outputString += cChar;
                    }
                }
                run.Text = outputString;
            }
        }

        public void ChangeFontFamily(FontFamily fontF) {
            if (rtfEditor.Selection.Start == rtfEditor.Selection.End) {
                rtfEditor.SelectAll();
            }
            TextSelection text = rtfEditor.Selection;
            if (!text.IsEmpty) {
                text.ApplyPropertyValue(RichTextBox.FontFamilyProperty, fontF);
            }
        }

        public void ChangeDefaultFontSize() {
            rtfEditor.FontSize = Properties.Settings.Default.efsize;
        }

        public void ChangeFontFamily(String fontFamily) {
            TextPointer tsp = rtfEditor.Selection.Start;
            TextPointer tep = rtfEditor.Selection.End;
            TextRange textRange;
            if (tsp == tep) {
                textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            } else {
                textRange = new TextRange(rtfEditor.Selection.Start, rtfEditor.Selection.End);
            }

            if (!textRange.IsEmpty) { textRange.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily); }
            rtfEditor.Selection.Select(tsp, tep);
        }

        public void ResizeEditor(double height) {
            //double currentPerc = GetPercentage2();
            //rtfEditor.Height = height;
            //double newPercentage = GetPercentage2();
        }

        public void ResizeEditor(double height, double width) {
            //double currentPerc = GetPercentage2();
            rtfEditor.Height = height;
            rtfEditor.Width = width;
            //double newPercentage = GetPercentage2();
        }

        private bool isFromFont;
        private bool bigEditor;

        public void ResizeViewer(double size, bool fromFont, bool bigEdit) {
            isFromFont = fromFont;
            bigEditor = bigEdit;
            size = (bigEdit ? size : size / 3);
            rtfEditor.FontSize = size;
            fontSize = size;
            using (var d = Dispatcher.DisableProcessing()) {
                try {
                    TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
                    textRange.ApplyPropertyValue(TextElement.FontSizeProperty, size);

                    textRange.ApplyPropertyValue(Run.BaselineAlignmentProperty, "Center");
                    double rectHeight = GetLineHeight(size);
                    double imgSize = rectHeight * 0.75;
                    double height = rectHeight * 0.9;
                    double cHeight = 10;

                    ResizeImages(imgSize); //change - examine
                    rtfEditor.SetValue(Paragraph.LineHeightProperty, height);
                    rtfEditor.ContextMenu.SetValue(Paragraph.LineHeightProperty, cHeight);
                    textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentStart);
                } catch { }
                formatFlag = true;
            }
        }

        private double GetLineHeight(double size) {
            TextRange textRange = new TextRange(rtfTemp.Document.ContentStart, rtfTemp.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            Rect rect = rtfTemp.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward);
            double height = rect.Height;
            if (size > 59) {
                switch ((int)size) {
                    case 60: height = 68.993333333333339; break;
                    case 69: height = 79.343333333333334; break;
                    case 78: height = 89.693333333333342; break;
                    case 87: height = 100.4; break;
                    case 96: height = 110.39; break;
                    case 105: height = 120.74; break;
                    case 114: height = 131.09; break;
                    case 120: height = 137.98666666666668; break;
                    case 132: height = 151.78666666666669; break;
                    case 141: height = 162.13666666666666; break;
                    case 150: height = 172.48666666666668; break;
                    case 159: height = 182.83333333333337; break;
                    case 168: height = 193.18333333333334; break;
                    case 177: height = 203.53333333333336; break;
                    case 186: height = 213.88333333333335; break;
                }
            }
            return height;
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
                                img.Width = img.Height / 2;
                                GetImages();
                            }
                        }
                    }
                }
            }
        }

        private void GetImages() {
            markerList = new Dictionary<int[], string>();
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines) {
                        if (inline is InlineUIContainer) {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            uiContainer.BaselineAlignment = BaselineAlignment.Bottom;
                            if (uiContainer.Child is Image) {
                                TextPointer tpS = rtfEditor.Document.ContentStart;
                                int offset1 = tpS.GetOffsetToPosition(uiContainer.ContentStart);
                                int offset2 = tpS.GetOffsetToPosition(uiContainer.ContentEnd);
                                int[] offset = new int[] { offset1, offset2 };
                                if (!markerList.Keys.Contains(offset) && !markerList.Values.Contains(uiContainer.Name) && !String.IsNullOrEmpty(uiContainer.Name)) {
                                    markerList.Add(offset, uiContainer.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void changescrollbar() {
            rtfEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        #endregion Formatting

        #region Document

        public String GetText() {
            TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            return textRange.Text;
        }

        public void LoadText(MemoryStream ms, String fromWhere) {
            controlSent = fromWhere;
            FlowDocument fd = new FlowDocument();
            //TextRange textRange = new TextRange(fd.ContentStart, fd.ContentEnd);
            if (ms == null) {
                rtfEditor.Document = fd;
            } else {
                String wtf = Classes.Controller.iso.GetString(ms.ToArray());
                ms.Position = 0;
                rtfEditor.SelectAll();
                try {
                    rtfEditor.Selection.Load(ms, DataFormats.Rtf);
                } catch (Exception ex) {
                    String msg = ex.Message;
                }
                int currentOffset = 0;
                TextPointer currentPosition = rtfEditor.Document.ContentStart.GetPositionAtOffset(currentOffset, LogicalDirection.Forward);
                if (currentPosition != null) {
                    rtfEditor.Selection.Select(currentPosition, currentPosition);
                }
            }
            controlSent = "Editor";
        }

        private double GetFlowDocHeight() {
            Paragraph p = rtfEditor.Document.Blocks.FirstBlock as Paragraph;
            double lineHeight = p.LineHeight;
            int lineNumber;
            if (!String.IsNullOrEmpty(rtfEditor.Selection.Text)) {
                rtfEditor.Selection.Start.GetLineStartPosition(-int.MaxValue, out lineNumber);
            } else {
                rtfEditor.CaretPosition.GetLineStartPosition(-int.MaxValue, out lineNumber);
            }
            lineNumber *= -1;
            double skipHeight = (double.IsNaN(lineNumber * lineHeight) ? 0 : lineNumber * lineHeight);

            return skipHeight;
        }

        public int WordCount() {
            TextRange textRange = new TextRange(rtfEditor.CaretPosition, rtfEditor.Document.ContentEnd);
            String s = textRange.Text;
            return Regex.Matches(s, @"[\w']+").Count;
        }

        public void GetContent() {
            TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            textRange.Save(ms, DataFormats.Rtf);
            if (controlSent == "Editor") {
                if (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible && Classes.Controller.BigEditorLoaded) {
                    if (ForceToggleSwitchEvent != null) { ForceToggleSwitchEvent(this, new EventArgs()); }
                    if (TextChangedEvent != null) { TextChangedEvent(this, new TextArgs(ms)); }
                } else if (Classes.Controller.bigEditor == null || !Classes.Controller.bigEditor.Visible) {
                    if (ForceToggleSwitchEvent != null) { ForceToggleSwitchEvent(this, new EventArgs()); }
                    if (TextChangedEvent != null) { TextChangedEvent(this, new TextArgs(ms)); }
                }
            }
        }

        private FlowDocument CloneDocument() {
            TextRange range = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            MemoryStream stream = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(range, stream);
            range.Save(stream, DataFormats.Rtf);
            FlowDocument to = new FlowDocument();
            TextRange range2 = new TextRange(to.ContentEnd, to.ContentEnd);
            range2.Load(stream, DataFormats.Rtf);
            return to;
        }

        public MemoryStream GetUndoContent() {
            TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            textRange.Save(ms, DataFormats.Rtf);
            return ms;
        }

        public MemoryStream UpdateContent() {
            TextRange textRange = new TextRange(rtfEditor.Document.ContentStart, rtfEditor.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            textRange.Save(ms, DataFormats.Rtf);
            return ms;
        }

        private int GetElementRTF(InlineUIContainer ic) {
            TextRange tr = new TextRange(ic.ContentStart, ic.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Rtf);
            string rtfText = ASCIIEncoding.Default.GetString(ms.ToArray());
            int imageType = 0;
            if (rtfText.Contains(Classes.Controller.MarkerRTF) || rtfText.Contains(Classes.Controller.MarkerRTF2) || rtfText.Contains(Classes.Controller.WhiteRTF) || rtfText.Contains(Classes.Controller.XPRtf) || rtfText.Contains(Classes.Controller.XPRtf2)) {
                imageType = 1;
            } else if (rtfText.Contains(Classes.Controller.SlideMarkerRTF)) {
                imageType = 2;
            } else {
                rtfText = rtfText.Replace(@"{\rtf1\ansi\ansicpg1252\uc1\htmautsp\deff2{\fonttbl{\f0\fcharset0 Times New Roman;}{\f2\fcharset0 Tahoma;}}{\colortbl\red0\green0\blue0;\red255\green255\blue255;}\loch\hich\dbch\pard\plain\ltrpar\itap0{\lang1033\loch\f2\fs16\cf1\ltrch {\fs38 }{\fs112 {\*\shppict{\pict\picwgoal159\pichgoal318\pngblip", "");
                if (rtfText == Classes.Controller.XPRtf) {
                    MessageBox.Show("WTF!");
                } else {
                    List<String> rtfs = new List<string>();
                    rtfs.Add(rtfText);
                    WriteLog(rtfs);
                }
            }
            return imageType;
        }

        private void WriteLog(List<String> contents) {
            String fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            File.WriteAllLines(fileName, contents);
        }

        public String getSelectedText() {
            return rtfEditor.Selection.Text;
        }

        private static FlowDocument MakeACopy2(FlowDocument doc) {
            TextRange range = new TextRange(doc.ContentStart, doc.ContentEnd);
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Rtf);
            stream.Position = 0;

            FlowDocument copy = new FlowDocument();
            range = new TextRange(copy.ContentStart, copy.ContentEnd);
            range.Load(stream, DataFormats.Rtf);
            return copy;
        }

        public void resetSelection() {
            rtfEditor.SelectionBrush = Brushes.Blue;
        }

        public void UndoAction() {
            rtfEditor.Undo();
        }

        public void SetPageWidth(double pageWidth) {
            rtfEditor.Document.PageWidth = pageWidth;
        }

        public void AddLines() {
        }

        #endregion Document

        #region Percentage

        public int GetAltQPercentage() {
            int someBigNumber = int.MaxValue;// for example int.MaxValue this number must be bigger then lines count
            int lineMoved;
            rtfEditor.CaretPosition.GetLineStartPosition(someBigNumber, out lineMoved);
            int currentLineNumber = -lineMoved;
            return currentLineNumber;
        }

        public double GetPercentage() {
            double vo = GetFlowDocHeight();
            if (vo == 0) {
                vo = rtfEditor.VerticalOffset;
            }
            //vo = actPos;
            double vh = rtfEditor.ViewportHeight;
            double eh = rtfEditor.ExtentHeight;
            double perc = 0;
            cpos = vo;
            perc = (eh != 0 ? vo / eh * 100 : 0);
            return perc;
        }

        public double GetPercentage2() {
            double vo = rtfEditor.VerticalOffset;
            //vo = actPos;
            double vh = rtfEditor.ViewportHeight;
            double eh = rtfEditor.ExtentHeight;
            double perc = 0;
            cpos = vo;
            perc = (eh != 0 ? vo / eh * 100 : 0);
            return perc;
        }

        public double GetExHeight {
            get { return rtfEditor.ExtentHeight; }
        }

        public void SetPercentage(double perc) {
            double vo = rtfEditor.VerticalOffset;
            double eh = rtfEditor.ExtentHeight;
            if (eh != 0) {
                cpos = (perc * eh) / 100;
                ScrollDirect(cpos);
                cpos = rtfEditor.VerticalOffset;
            }
        }

        #endregion Percentage

        #region Markers

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

        public Inline GotoMarker(String name) {
            Inline ri = null;
            bool foundMe = false;
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    var paragraph = block as Paragraph;
                    foreach (Inline i in paragraph.Inlines) {
                        if (name == i.Name) {
                            //i.BringIntoView();
                            ri = i;
                            break;
                        }
                    }
                }
                if (foundMe) {
                    break;
                }
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
            try {
                this.rtfEditor.CaretPosition = i.ContentStart;
            } catch { }
        }

        public void SetMarkerNames(List<String> names, int markerType) {
            intMarkerNames = names;
            intMarkers = new List<InlineUIContainer>();
            int nI = 0;
            Image testImage = Classes.Controls.MarkerImage();
            Classes.Controller.markerPositions = new Dictionary<string, Inline>();
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines) {
                        if (inline is InlineUIContainer) {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            if (uiContainer.Child is Image && nI < names.Count) {
                                int mType = GetElementRTF(uiContainer);
                                if (markerType == mType) {
                                    Classes.Controller.markerPositions.Add(names[nI], inline);
                                    inline.Name = names[nI];
                                    nI++;
                                    intMarkers.Add(uiContainer);
                                }
                            }
                        }
                    }
                }
            }
            if (Classes.Controller.markerPositions.Count > 0) { AddMarkersToContext(); }
        }

        private void DummyFunction() {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true)) {
                //use either one of the below
                Visual visi = rtfEditor as Visual;

                pd.PrintVisual(rtfEditor as Visual, "Print Visual");
                pd.PrintDocument((((IDocumentPaginatorSource)rtfEditor.Document).DocumentPaginator), "Print Document");
            }
        }

        public void SaveAsImage(string filename) {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            int widthLocal = (int)ActualWidth;
            int heightLocal = (int)ActualHeight;
            RenderTargetBitmap bmp = new RenderTargetBitmap(widthLocal, heightLocal, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(rtfEditor as Visual);

            string extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension)) {
                throw new ArgumentException("File name is not valid. Extension not supplied.");
            }

            BitmapEncoder encoder;
            switch (extension.ToLower()) {
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;

                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;

                case ".jpg":
                case ".jpeg":
                    encoder = new JpegBitmapEncoder();
                    break;

                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;

                default:
                    throw new ArgumentException("filename");
            }

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stream = File.Create(filename)) {
                encoder.Save(stream);
            }
        }

        private void CreateMenuItem(String headerText) {
            MenuItem mItem = new MenuItem();
            mItem.Header = headerText;
            mItem.IsEnabled = false;
            rtfEditor.ContextMenu.Items.Add(mItem);
        }

        public void AddHotkeys() {
            CreateMenuItem("Scroll: F2");
            CreateMenuItem("Font size: Ctrl L/R");
            CreateMenuItem("Margin Increase: Alt+Shift+ L/R");
            CreateMenuItem("Margin Decrease: Alt+Ctrl+ L/R");
            CreateMenuItem("Flip H: Alt+H");
            CreateMenuItem("Flip V: Alt+V");
            CreateMenuItem("Blackout: Alt+B");
            CreateMenuItem("Logo: Alt+L");
            CreateMenuItem("Exit: Esc");

        }

        private void AddMarkersToContext() {
            MenuItem markerItem = null;
            foreach (MenuItem m in rtfEditor.ContextMenu.Items) {
                if (m.Header == "Markers") {
                    m.Items.Clear();
                    markerItem = m;
                    break;
                }
            }
            if (markerItem == null) {
                markerItem = new MenuItem();
                markerItem.Header = "Markers";
                rtfEditor.ContextMenu.Items.Add(markerItem);
            }
            foreach (KeyValuePair<String, Inline> m in Classes.Controller.markerPositions) {
                MenuItem mItem = new MenuItem();
                mItem.Header = m.Key.Substring(1, m.Key.Length - 1).Replace("point", ".");
                RoutedUICommand markerCommand = new RoutedUICommand(m.Key, m.Key + "cmd", typeof(Editor));
                this.CommandBindings.Add(new CommandBinding(markerCommand, new ExecutedRoutedEventHandler(CommandBinding_GotoMarker)));
                mItem.Command = markerCommand;
                markerItem.Items.Add(mItem);
            }
        }

        private void CommandBinding_GotoMarker(object sender, ExecutedRoutedEventArgs e) {
            RoutedUICommand ric = (RoutedUICommand)e.Command;
            String name = ric.Name.Replace("cmd", "");

            Inline i = GotoMarker(name);
            if (i != null) {
                ScrollToMarker(i);
            }
            if (InternalMarkerEvent != null) { InternalMarkerEvent(this, new IMarkerArgs(name)); }
        }

        public bool CheckMarkerName(String markerName) {
            bool found = false;
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    Paragraph paragraph = (Paragraph)block;
                    foreach (Inline inline in paragraph.Inlines) {
                        if (inline is InlineUIContainer) {
                            InlineUIContainer uiContainer = (InlineUIContainer)inline;
                            if (uiContainer.Child is Image) {
                                String imgName = (uiContainer.Child as Image).Name;
                                if (imgName == markerName) {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (found) { break; }
                }
            }
            return found;
        }

        public List<String> GetMarkerOrder(List<String> names, int markerType) {
            List<String> orderedNames = new List<string>();
            foreach (var block in rtfEditor.Document.Blocks) {
                if (block is Paragraph) {
                    var paragraph = block as Paragraph;
                    foreach (Inline i in paragraph.Inlines) {
                        if (names.Contains(i.Name)) {
                            orderedNames.Add(i.Name);
                        }
                    }
                }
            }
            return orderedNames;
        }

        public void AddMarker(int markerType, String name, out double mx, out double my) {
            mx = 0;
            my = 0;
            Image image = null;
            switch (markerType) {
                case 1:
                    image = Classes.Controls.MarkerImage();
                    break;

                case 2:
                    image = Classes.Controls.SlideImage();
                    break;
            }
            if (markerType != 3 && image != null) {
                fontSize = Classes.Controller.vFSize;
                image.Height = fontSize;
                TextPointer tp = null;
                if (String.IsNullOrEmpty(rtfEditor.Selection.Text)) {
                    tp = rtfEditor.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
                } else {
                    tp = rtfEditor.Selection.Start;
                }
                InlineUIContainer bic = new InlineUIContainer(image, tp);

                bic.Name = name;
                bic.Child = image;
                bic.FontSize = fontSize;
                bic.Child = image;
                //GetElementRTF(bic);
                bic.BaselineAlignment = BaselineAlignment.Center;
                mx = tp.GetCharacterRect(LogicalDirection.Forward).Left;
                my = tp.GetCharacterRect(LogicalDirection.Forward).Top;

                rtfEditor.CaretPosition = bic.ContentEnd;
                rtfEditor.CaretPosition.InsertTextInRun(" ");
                bool bigEdit = (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible);
                ResizeViewer(fontSize, false, bigEdit);
                GetContent();
                GetImages();
            } else {
                int originalOffset = rtfEditor.Document.ContentStart.GetOffsetToPosition(rtfEditor.CaretPosition);
                TextRange tr = new TextRange(rtfEditor.CaretPosition, rtfEditor.CaretPosition.GetInsertionPosition(LogicalDirection.Backward));
                TextPointer tp = rtfEditor.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
                String camMarkerName = "(" + name + ")";
                int camLength = camMarkerName.Length;
                int newOffset1 = originalOffset + camLength;
                int newOffset2 = newOffset1 + 1;
                tr.Text = camMarkerName + " ";
                tr.ApplyPropertyValue(TextElement.FontFamilyProperty, "Arial");
                tr.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Properties.Settings.Default.defaultCamera);

                TextPointer tp1 = tr.End.GetPositionAtOffset(-1, LogicalDirection.Forward);
                TextPointer tp2 = tr.End;// rtfEditor.Document.ContentStart.GetPositionAtOffset(newOffset2, LogicalDirection.Forward);
                TextRange tr1 = new TextRange(tp1, tp2);
                tr1.ApplyPropertyValue(TextElement.FontFamilyProperty, "Arial");
                tr1.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                tr1.ApplyPropertyValue(TextElement.ForegroundProperty, Properties.Settings.Default.defaultFore);
                rtfEditor.CaretPosition = tp2;
                ResizeViewer(Classes.Controller.vFSize, false, (Classes.Controller.bigEditor != null && Classes.Controller.bigEditor.Visible));
                GetContent();
            }
        }

        #endregion Markers

        #region Command Binding

        private void CommandBinding_AddMarker(object sender, ExecutedRoutedEventArgs e) {
            if (MarkerAddedEvent != null) { MarkerAddedEvent(this, new MarkerAddArgs(1)); }
        }

        private void CommandBinding_AddSlideMarker(object sender, ExecutedRoutedEventArgs e) {
            if (MarkerAddedEvent != null) { MarkerAddedEvent(this, new MarkerAddArgs(2)); }
        }

        private void CommandBinding_AddCameraMarker(object sender, ExecutedRoutedEventArgs e) {
            if (MarkerAddedEvent != null) { MarkerAddedEvent(this, new MarkerAddArgs(3)); }
        }

        private void CommandBinding_ColorWhite(object sender, ExecutedRoutedEventArgs e) {
            ChangeTextColor(1);
        }

        private void CommandBinding_ColorYellow(object sender, ExecutedRoutedEventArgs e) {
            ChangeTextColor(2);
        }

        private void CommandBinding_ColorGreen(object sender, ExecutedRoutedEventArgs e) {
            ChangeTextColor(3);
        }

        private void CommandBinding_ColorRed(object sender, ExecutedRoutedEventArgs e) {
            ChangeTextColor(4);
        }

        private void CommandBinding_ColorBlue(object sender, ExecutedRoutedEventArgs e) {
            ChangeTextColor(5);
        }

        private void CommandBinding_AllCaps(object sender, ExecutedRoutedEventArgs e) {
            ChangeCase(true);
        }

        private void CommandBinding_SentenceCaps(object sender, ExecutedRoutedEventArgs e) {
            ChangeCase(false);
        }

        private void CommandBinding_PrevMarker(object sender, ExecutedRoutedEventArgs e) {
            if (PrevMarkerEvent != null) { PrevMarkerEvent(this, new EventArgs()); }
        }

        private void CommandBinding_NextMarker(object sender, ExecutedRoutedEventArgs e) {
            if (NextMarkerEvent != null) { NextMarkerEvent(this, new EventArgs()); }
        }

        private void CommandBinding_Escape(object sender, ExecutedRoutedEventArgs e) {
            if (EscapeEvent != null) { EscapeEvent(this, new EventArgs()); }
        }

        private void CommandBinding_ToggleScroll(object sender, ExecutedRoutedEventArgs e) {
            if (ToggleSwitchEvent != null) { ToggleSwitchEvent(this, new EventArgs()); }
        }

        private void CommandBinding_AdjustFontUp(object sender, ExecutedRoutedEventArgs e) {
            if (AdjustFontUpEvent != null) { AdjustFontUpEvent(this, new EventArgs()); }
        }

        private void CommandBinding_AdjustFontDown(object sender, ExecutedRoutedEventArgs e) {
            if (AdjustFontDownEvent != null) { AdjustFontDownEvent(this, new EventArgs()); }
        }

        #endregion Command Binding

        #region Background Worker

        private BackgroundWorker m_oWorker;
        private FlowDocument testDocument;

        private void StartBackgroundWorker() {
            m_oWorker = new BackgroundWorker();
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
        }

        private void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) {
                testDocument = null;
            } else if (e.Error != null) {
                testDocument = null;
            }
        }

        private void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            //throw new NotImplementedException();
        }

        private void m_oWorker_DoWork(object sender, DoWorkEventArgs e) {
            throw new NotImplementedException();
        }

        #endregion Background Worker

        private void rtfEditor_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (ForceScrollOffEvent != null) { ForceScrollOffEvent(this, new EventArgs()); }
        }

        private void CommandBinding_Bold(object sender, ExecutedRoutedEventArgs e) {
            ToggleBold();
            e.Handled = true;
        }

        private void CommandBinding_Italic(object sender, ExecutedRoutedEventArgs e) {
            ToggleItalic();
            e.Handled = true;
        }
    }

    public static class Command {
        public static readonly RoutedUICommand AddMarker = new RoutedUICommand("Add marker", "AddMarker", typeof(Editor));
        public static readonly RoutedUICommand AddSlideMarker = new RoutedUICommand("Add slide marker", "AddSlideMarker", typeof(Editor));
        public static readonly RoutedUICommand AddCameraMarker = new RoutedUICommand("Add camera marker", "AddCameraMarker", typeof(Editor));
        public static readonly RoutedUICommand ColorWhite = new RoutedUICommand("ColorWhite", "ColorWhite", typeof(Editor));
        public static readonly RoutedUICommand ColorYellow = new RoutedUICommand("ColorYellow", "ColorYellow", typeof(Editor));
        public static readonly RoutedUICommand ColorGreen = new RoutedUICommand("ColorGreen", "ColorGreen", typeof(Editor));
        public static readonly RoutedUICommand ColorRed = new RoutedUICommand("ColorRed", "ColorRed", typeof(Editor));
        public static readonly RoutedUICommand ColorBlue = new RoutedUICommand("ColorBlue", "ColorBlue", typeof(Editor));
        public static readonly RoutedUICommand AllCaps = new RoutedUICommand("AllCaps", "AllCaps", typeof(Editor));
        public static readonly RoutedUICommand SentenceCaps = new RoutedUICommand("SentenceCaps", "SentenceCaps", typeof(Editor));
        public static readonly RoutedUICommand PrevMarker = new RoutedUICommand("PrevMarker", "PrevMarker", typeof(Editor));
        public static readonly RoutedUICommand NextMarker = new RoutedUICommand("NextMarker", "NextMarker", typeof(Editor));
        public static readonly RoutedUICommand Escape = new RoutedUICommand("Escape", "Escape", typeof(Editor));
        public static readonly RoutedUICommand ToggleScroll = new RoutedUICommand("ToggleScroll", "ToggleScroll", typeof(Editor));
        public static readonly RoutedUICommand AdjustFontDown = new RoutedUICommand("AdjustFontDown", "AdjustFontDown", typeof(Editor));
        public static readonly RoutedUICommand AdjustFontUp = new RoutedUICommand("AdjustFontUp", "AdjustFontUp", typeof(Editor));
    }
}