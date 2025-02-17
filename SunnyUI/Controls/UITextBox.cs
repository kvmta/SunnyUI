﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2021 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@QQ.Com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * 文件名称: UITextBox.cs
 * 文件说明: 输入框
 * 当前版本: V3.0
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-06-03: V2.2.5 增加多行，增加滚动条
 * 2020-09-03: V2.2.7 增加FocusedSelectAll属性，激活时全选。
 * 2021-04-18: V3.0.3 增加ShowScrollBar属性，单独控制垂直滚动条
******************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    public sealed partial class UITextBox : UIPanel, ISymbol
    {
        private readonly UIEdit edit = new UIEdit();
        private readonly UIScrollBar bar = new UIScrollBar();

        public UITextBox()
        {
            InitializeComponent();
            SetStyleFlags();
            CalcEditHeight();
            Height = MinHeight;
            ShowText = false;
            Font = UIFontColor.Font;
            Padding = new Padding(0);

            edit.Top = (Height - edit.Height) / 2;
            edit.Left = 4;
            edit.Width = Width - 8;
            edit.Text = String.Empty;
            edit.BorderStyle = BorderStyle.None;
            edit.TextChanged += EditTextChanged;
            edit.KeyDown += EditOnKeyDown;
            edit.KeyUp += EditOnKeyUp;
            edit.KeyPress += EditOnKeyPress;
            edit.MouseEnter += Edit_MouseEnter;
            edit.Click += Edit_Click;
            edit.DoubleClick += Edit_DoubleClick;
            edit.Leave += Edit_Leave;
            edit.Validated += Edit_Validated;
            edit.Validating += Edit_Validating;
            edit.GotFocus += Edit_GotFocus;
            edit.LostFocus += Edit_LostFocus;

            edit.Invalidate();
            Controls.Add(edit);
            fillColor = Color.White;
            Width = 150;

            bar.Parent = this;
            bar.Dock = DockStyle.None;
            bar.Style = UIStyle.Custom;
            bar.Visible = false;
            bar.ValueChanged += Bar_ValueChanged;
            edit.MouseWheel += OnMouseWheel;
            bar.MouseEnter += Bar_MouseEnter;
            TextAlignment = ContentAlignment.MiddleLeft;

            SizeChange();

            editCursor = Cursor;
            TextAlignmentChange += UITextBox_TextAlignmentChange;
        }

        private void Edit_LostFocus(object sender, EventArgs e)
        {
            LostFocus?.Invoke(this, e);
        }

        private void Edit_GotFocus(object sender, EventArgs e)
        {
            GotFocus?.Invoke(this, e);
        }

        private void Edit_Validating(object sender, CancelEventArgs e)
        {
            Validating?.Invoke(this, e);
        }

        public new EventHandler GotFocus;
        public new EventHandler LostFocus;
        public new CancelEventHandler Validating;
        public new event EventHandler Validated;

        private void Edit_Validated(object sender, EventArgs e)
        {
            Validated?.Invoke(this, e);
        }

        public new void Focus()
        {
            base.Focus();
            edit.Focus();
        }

        [Browsable(false)]
        public TextBox TextBox => edit;

        private void Edit_Leave(object sender, EventArgs e)
        {
            Leave?.Invoke(this, e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            edit.BackColor = Enabled ? Color.White : FillDisableColor;
            edit.Enabled = Enabled;
        }

        public override bool Focused
        {
            get => edit.Focused;
        }

        [DefaultValue(false)]
        [Description("激活时选中全部文字"), Category("SunnyUI")]
        public bool FocusedSelectAll
        {
            get => edit.FocusedSelectAll;
            set => edit.FocusedSelectAll = value;
        }

        private void UITextBox_TextAlignmentChange(object sender, ContentAlignment alignment)
        {
            if (edit == null) return;
            if (alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.MiddleLeft ||
                alignment == ContentAlignment.BottomLeft)
                edit.TextAlign = HorizontalAlignment.Left;

            if (alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.MiddleCenter ||
                alignment == ContentAlignment.BottomCenter)
                edit.TextAlign = HorizontalAlignment.Center;

            if (alignment == ContentAlignment.TopRight || alignment == ContentAlignment.MiddleRight ||
                alignment == ContentAlignment.BottomRight)
                edit.TextAlign = HorizontalAlignment.Right;
        }

        private void Edit_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        public new event EventHandler DoubleClick;
        public new event EventHandler Click;

        private void Edit_Click(object sender, EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        protected override void OnCursorChanged(EventArgs e)
        {
            base.OnCursorChanged(e);
            edit.Cursor = Cursor;
        }

        private Cursor editCursor;

        private void Bar_MouseEnter(object sender, EventArgs e)
        {
            editCursor = Cursor;
            Cursor = Cursors.Default;
        }

        private void Edit_MouseEnter(object sender, EventArgs e)
        {
            Cursor = editCursor;
            if (FocusedSelectAll)
            {
                SelectAll();
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (bar != null && bar.Visible && edit != null)
            {
                var si = ScrollBarInfo.GetInfo(edit.Handle);
                if (e.Delta > 10)
                {
                    if (si.nPos > 0)
                    {
                        ScrollBarInfo.ScrollUp(edit.Handle);
                    }
                }
                else if (e.Delta < -10)
                {
                    if (si.nPos < si.ScrollMax)
                    {
                        ScrollBarInfo.ScrollDown(edit.Handle);
                    }
                }
            }

            SetScrollInfo();
        }

        private void Bar_ValueChanged(object sender, EventArgs e)
        {
            if (edit != null)
            {
                ScrollBarInfo.SetScrollValue(edit.Handle, bar.Value);
            }
        }

        private bool multiline;

        [DefaultValue(false)]
        public bool Multiline
        {
            get => multiline;
            set
            {
                multiline = value;
                edit.Multiline = value;
                // edit.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
                // bar.Visible = multiline;

                if (value && Type != UIEditType.String)
                {
                    Type = UIEditType.String;
                }

                SizeChange();
            }
        }

        private bool showScrollBar;

        [DefaultValue(false)]
        [Description("显示垂直滚动条"), Category("SunnyUI")]
        public bool ShowScrollBar
        {
            get => showScrollBar;
            set
            {
                value = value && Multiline;
                showScrollBar = value;
                if (value)
                {
                    edit.ScrollBars = ScrollBars.Vertical;
                    bar.Visible = true;
                }
                else
                {
                    edit.ScrollBars = ScrollBars.None;
                    bar.Visible = false;
                }
            }
        }

        [DefaultValue(true)]
        public bool WordWarp
        {
            get => edit.WordWrap;
            set => edit.WordWrap = value;
        }

        public void Select(int start, int length)
        {
            edit.Focus();
            edit.Select(start, length);
        }

        public void ScrollToCaret()
        {
            edit.ScrollToCaret();
        }

        private void EditOnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        private void EditOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoEnter?.Invoke(this, e);
            }

            KeyDown?.Invoke(this, e);
        }

        public event EventHandler DoEnter;

        private void EditOnKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        [DefaultValue(null)]
        [Description("水印文字"), Category("SunnyUI")]
        public string Watermark
        {
            get => edit.Watermark;
            set => edit.Watermark = value;
        }

        public void SelectAll()
        {
            edit.Focus();
            edit.SelectAll();
        }

        public void CheckMaxMin()
        {
            edit.CheckMaxMin();
        }

        [Browsable(true)]
        public new event EventHandler TextChanged;

        public new event KeyEventHandler KeyDown;

        public new event KeyEventHandler KeyUp;

        public new event KeyPressEventHandler KeyPress;

        public new event EventHandler Leave;

        private void EditTextChanged(object s, EventArgs e)
        {
            TextChanged?.Invoke(this, e);
            SetScrollInfo();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            edit.Font = Font;
            CalcEditHeight();
            SizeChange();
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            SizeChange();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            SizeChange();
        }

        public void SetScrollInfo()
        {
            if (bar == null)
            {
                return;
            }

            var si = ScrollBarInfo.GetInfo(edit.Handle);
            if (si.ScrollMax > 0)
            {
                bar.Maximum = si.ScrollMax;
                bar.Value = si.nPos;
            }
            else
            {
                bar.Maximum = si.ScrollMax;
            }
        }

        private int MinHeight;
        private int MaxHeight;

        private void CalcEditHeight()
        {
            TextBox edt = new();
            edt.Font = edit.Font;
            MinHeight = edt.PreferredHeight;
            edt.BorderStyle = BorderStyle.None;
            MaxHeight = edt.PreferredHeight * 2 + MinHeight - edt.PreferredHeight;
            edt.Dispose();
        }

        private void SizeChange()
        {
            if (!multiline)
            {
                if (Height < MinHeight) Height = MinHeight;
                if (Height > MaxHeight) Height = MaxHeight;

                edit.Top = (Height - edit.Height) / 2;
                if (icon == null && Symbol == 0)
                {
                    edit.Left = 4 + Padding.Left;
                    edit.Width = Width - 8 - Padding.Left - Padding.Right;
                }
                else
                {
                    if (icon != null)
                    {
                        edit.Left = 4 + iconSize + Padding.Left;
                        edit.Width = Width - 8 - iconSize - Padding.Left - Padding.Right;
                    }
                    else if (Symbol > 0)
                    {
                        edit.Left = 4 + SymbolSize + Padding.Left;
                        edit.Width = Width - 8 - SymbolSize - Padding.Left - Padding.Right;
                    }
                }
            }
            else
            {
                edit.Top = 3;
                edit.Height = Height - 6;
                edit.Left = 1;
                edit.Width = Width - 2;
                bar.Top = 2;
                bar.Width = ScrollBarInfo.VerticalScrollBarWidth();
                bar.Left = Width - bar.Width - 1;
                bar.Height = Height - 4;
                bar.BringToFront();

                SetScrollInfo();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            edit.Focus();
        }

        public void Clear()
        {
            edit.Clear();
        }

        [DefaultValue('\0')]
        [Description("密码掩码"), Category("SunnyUI")]
        public char PasswordChar
        {
            get => edit.PasswordChar;
            set => edit.PasswordChar = value;
        }

        [DefaultValue(false)]
        [Description("是否只读"), Category("SunnyUI")]
        public bool ReadOnly
        {
            get => edit.ReadOnly;
            set
            {
                edit.ReadOnly = value;
                edit.BackColor = Enabled ? Color.White : FillDisableColor;
            }
        }

        [Description("输入类型"), Category("SunnyUI")]
        [DefaultValue(UIEditType.String)]
        public UIEditType Type
        {
            get => edit.Type;
            set => edit.Type = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，能输入的最大值
        /// </summary>
        [Description("当InputType为数字类型时，能输入的最大值。"), Category("SunnyUI")]
        [DefaultValue(int.MaxValue)]
        public double Maximum
        {
            get => edit.MaxValue;
            set => edit.MaxValue = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，能输入的最小值
        /// </summary>
        [Description("当InputType为数字类型时，能输入的最小值。"), Category("SunnyUI")]
        [DefaultValue(int.MinValue)]
        public double Minimum
        {
            get => edit.MinValue;
            set => edit.MinValue = value;
        }

        [DefaultValue(false)]
        [Description("是否判断最大值显示"), Category("SunnyUI")]
        public bool MaximumEnabled
        {
            get => HasMaximum;
            set => HasMaximum = value;
        }

        [DefaultValue(false)]
        [Description("是否判断最小值显示"), Category("SunnyUI")]
        public bool MinimumEnabled
        {
            get => HasMinimum;
            set => HasMinimum = value;
        }

        [DefaultValue(false), Browsable(false)]
        [Description("是否判断最大值显示"), Category("SunnyUI")]
        public bool HasMaximum
        {
            get => edit.HasMaxValue;
            set => edit.HasMaxValue = value;
        }

        [DefaultValue(false), Browsable(false)]
        [Description("是否判断最小值显示"), Category("SunnyUI")]
        public bool HasMinimum
        {
            get => edit.HasMinValue;
            set => edit.HasMinValue = value;
        }

        [DefaultValue(0.00)]
        [Description("浮点返回值"), Category("SunnyUI")]
        public double DoubleValue
        {
            get => edit.DoubleValue;
            set => edit.DoubleValue = value;
        }

        [DefaultValue(0)]
        [Description("整形返回值"), Category("SunnyUI")]
        public int IntValue
        {
            get => edit.IntValue;
            set => edit.IntValue = value;
        }

        [Description("文本返回值"), Category("SunnyUI")]
        [Browsable(true)]
        [DefaultValue("")]
        public override string Text
        {
            get => edit.Text;
            set => edit.Text = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，小数位数。
        /// </summary>
        [Description("当InputType为数字类型时，小数位数。")]
        [DefaultValue(2), Category("SunnyUI")]
        public int DecLength
        {
            get => edit.DecLength;
            set => edit.DecLength = Math.Max(value, 0);
        }

        [DefaultValue(false)]
        [Description("整形或浮点输入时，是否可空显示"), Category("SunnyUI")]
        public bool CanEmpty
        {
            get => edit.CanEmpty;
            set => edit.CanEmpty = value;
        }

        public void Empty()
        {
            if (edit.CanEmpty)
                edit.Text = "";
        }

        public bool IsEmpty => edit.Text == "";

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ActiveControl = edit;
        }

        [DefaultValue(32767)]
        public int MaxLength
        {
            get => edit.MaxLength;
            set => edit.MaxLength = Math.Max(value, 1);
        }

        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            edit.BackColor = fillColor = Enabled ? Color.White : FillDisableColor;
            edit.ForeColor = foreColor = UIFontColor.Primary;

            if (bar != null)
            {
                bar.ForeColor = uiColor.PrimaryColor;
                bar.HoverColor = uiColor.ButtonFillHoverColor;
                bar.PressColor = uiColor.ButtonFillPressColor;
                bar.FillColor = Color.White;
            }

            Invalidate();
        }

        protected override void AfterSetForeColor(Color color)
        {
            base.AfterSetForeColor(color);
            edit.ForeColor = color;
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            edit.BackColor = Enabled ? color : FillDisableColor;
        }

        public enum UIEditType
        {
            /// <summary>
            /// 字符串
            /// </summary>
            String,

            /// <summary>
            /// 整数
            /// </summary>
            Integer,

            /// <summary>
            /// 浮点数
            /// </summary>
            Double
        }

        [DefaultValue(false)]
        public bool AcceptsReturn
        {
            get => edit.AcceptsReturn;
            set => edit.AcceptsReturn = value;
        }

        [DefaultValue(AutoCompleteMode.None), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode
        {
            get => edit.AutoCompleteMode;
            set => edit.AutoCompleteMode = value;
        }

        [
            DefaultValue(AutoCompleteSource.None),
            TypeConverterAttribute(typeof(TextBoxAutoCompleteSourceConverter)),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource
        {
            get => edit.AutoCompleteSource;
            set => edit.AutoCompleteSource = value;
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            Localizable(true),
            Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => edit.AutoCompleteCustomSource;
            set => edit.AutoCompleteCustomSource = value;
        }

        [DefaultValue(CharacterCasing.Normal)]
        public CharacterCasing CharacterCasing
        {
            get => edit.CharacterCasing;
            set => edit.CharacterCasing = value;
        }

        public void Paste(string text)
        {
            edit.Paste(text);
        }

        internal class TextBoxAutoCompleteSourceConverter : EnumConverter
        {
            public TextBoxAutoCompleteSourceConverter(Type type) : base(type)
            {
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                StandardValuesCollection values = base.GetStandardValues(context);
                ArrayList list = new ArrayList();
                int count = values.Count;
                for (int i = 0; i < count; i++)
                {
                    string currentItemText = values[i].ToString();
                    if (currentItemText != null && !currentItemText.Equals("ListItems"))
                    {
                        list.Add(values[i]);
                    }
                }

                return new StandardValuesCollection(list);
            }
        }

        [DefaultValue(false)]
        public bool AcceptsTab
        {
            get => edit.AcceptsTab;
            set => edit.AcceptsTab = value;
        }

        [DefaultValue(false)]
        public bool EnterAsTab
        {
            get => edit.EnterAsTab;
            set => edit.EnterAsTab = value;
        }

        [DefaultValue(true)]
        public bool ShortcutsEnabled
        {
            get => edit.ShortcutsEnabled;
            set => edit.ShortcutsEnabled = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanUndo
        {
            get => edit.CanUndo;
        }

        [DefaultValue(true)]
        public bool HideSelection
        {
            get => edit.HideSelection;
            set => edit.HideSelection = value;
        }

        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            MergableProperty(false),
            Localizable(true),
            Editor("System.Windows.Forms.Design.StringArrayEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))
        ]
        public string[] Lines
        {
            get => edit.Lines;
            set => edit.Lines = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get => edit.Modified;
            set => edit.Modified = value;
        }

        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int PreferredHeight
        {
            get => edit.PreferredHeight;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedText
        {
            get => edit.SelectedText;
            set => edit.SelectedText = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get => edit.SelectionLength;
            set => edit.SelectionLength = value;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart
        {
            get => edit.SelectionStart;
            set => edit.SelectionStart = value;
        }

        [Browsable(false)]
        public int TextLength
        {
            get => edit.TextLength;
        }

        public void AppendText(string text)
        {
            edit.AppendText(text);
        }

        public void ClearUndo()
        {
            edit.ClearUndo();
        }

        public void Copy()
        {
            edit.Copy();
        }

        public void Cut()
        {
            edit.Cut();
        }

        public void Paste()
        {
            edit.Paste();
        }

        public char GetCharFromPosition(Point pt)
        {
            return edit.GetCharFromPosition(pt);
        }

        public int GetCharIndexFromPosition(Point pt)
        {
            return edit.GetCharIndexFromPosition(pt);
        }

        public int GetLineFromCharIndex(int index)
        {
            return edit.GetLineFromCharIndex(index);
        }

        public Point GetPositionFromCharIndex(int index)
        {
            return edit.GetPositionFromCharIndex(index);
        }

        public int GetFirstCharIndexFromLine(int lineNumber)
        {
            return edit.GetFirstCharIndexFromLine(lineNumber);
        }

        public int GetFirstCharIndexOfCurrentLine()
        {
            return edit.GetFirstCharIndexOfCurrentLine();
        }

        public void DeselectAll()
        {
            edit.DeselectAll();
        }

        public void Undo()
        {
            edit.Undo();
        }

        private Image icon;
        [Description("图标"), Category("SunnyUI")]
        [DefaultValue(null)]
        public Image Icon
        {
            get => icon;
            set
            {
                icon = value;
                SizeChange();
                Invalidate();
            }
        }

        private int iconSize = 24;
        [Description("图标大小(方形)"), Category("SunnyUI"), DefaultValue(24)]
        public int IconSize
        {
            get => iconSize;
            set
            {
                iconSize = Math.Min(MinHeight, value);
                SizeChange();
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (multiline) return;
            if (icon != null)
            {
                e.Graphics.DrawImage(icon, new Rectangle(4, (Height - iconSize) / 2, iconSize, iconSize), new Rectangle(0, 0, icon.Width, icon.Height), GraphicsUnit.Pixel);
            }
            else if (Symbol != 0)
            {
                e.Graphics.DrawFontImage(Symbol, SymbolSize, SymbolColor, new Rectangle(4 + symbolOffset.X, (Height - SymbolSize) / 2 + 1 + symbolOffset.Y, SymbolSize, SymbolSize), SymbolOffset.X, SymbolOffset.Y);
            }
        }

        public Color _symbolColor = UIFontColor.Primary;
        [DefaultValue(typeof(Color), "48, 48, 48")]
        [Description("字体图标颜色"), Category("SunnyUI")]
        public Color SymbolColor
        {
            get => _symbolColor;
            set
            {
                _symbolColor = value;
                Invalidate();
            }
        }

        private int _symbol = 0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(UIImagePropertyEditor), typeof(UITypeEditor))]
        [DefaultValue(0)]
        [Description("字体图标"), Category("SunnyUI")]
        public int Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                SizeChange();
                Invalidate();
            }
        }

        private int _symbolSize = 24;

        [DefaultValue(24)]
        [Description("字体图标大小"), Category("SunnyUI")]
        public int SymbolSize
        {
            get => _symbolSize;
            set
            {
                _symbolSize = Math.Max(value, 16);
                _symbolSize = Math.Min(value, MinHeight);
                SizeChange();
                Invalidate();
            }
        }

        private Point symbolOffset = new Point(0, 0);

        [DefaultValue(typeof(Point), "0, 0")]
        [Description("字体图标的偏移位置"), Category("SunnyUI")]
        public Point SymbolOffset
        {
            get => symbolOffset;
            set
            {
                symbolOffset = value;
                Invalidate();
            }
        }
    }
}