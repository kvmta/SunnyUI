﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2020 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@qq.com
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
 * 当前版本: V2.2
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-06-03: V2.2.5 增加多行，增加滚动条
******************************************************************************/

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sunny.UI
{
    [DefaultEvent("TextChanged")]
    [DefaultProperty("Text")]
    public sealed partial class UITextBox : UIPanel
    {
        private readonly UIEdit edit = new UIEdit();
        private readonly UIScrollBar bar = new UIScrollBar();

        public UITextBox()
        {
            InitializeComponent();

            ShowText = false;
            Font = UIFontColor.Font;

            edit.Left = 3;
            edit.Top = 3;
            edit.Text = String.Empty;
            edit.BorderStyle = BorderStyle.None;
            edit.TextChanged += EditTextChanged;
            edit.KeyDown += EditOnKeyDown;
            edit.KeyUp += EditOnKeyUp;
            edit.KeyPress += EditOnKeyPress;
            edit.MouseEnter += Edit_MouseEnter;

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
        }

        private void Bar_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void Edit_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.IBeam;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (bar.Visible)
            {
                var si = ScrollBarInfo.GetInfo(Handle);
                if (e.Delta > 10)
                {
                    if (si.nPos > 0)
                    {
                        ScrollBarInfo.ScrollUp(Handle);
                    }
                }
                else if (e.Delta < -10)
                {
                    if (si.nPos < si.ScrollMax)
                    {
                        ScrollBarInfo.ScrollDown(Handle);
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

                edit.ScrollBars = value ? ScrollBars.Vertical : ScrollBars.None;
                bar.Visible = multiline;

                SizeChange();
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
            edit.Select(start, length);
        }

        public void ScrollToCaret()
        {
            edit.ScrollToCaret();
        }

        private void EditOnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(sender, e);
        }

        private void EditOnKeyDown(object sender, KeyEventArgs e)
        {
            KeyDown?.Invoke(sender, e);
        }

        private void EditOnKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(sender, e);
        }

        [DefaultValue(null)]
        public string Watermark
        {
            get => edit.Watermark;
            set => edit.Watermark = value;
        }

        public void SelectAll()
        {
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

        private void EditTextChanged(object s, EventArgs e)
        {
            TextChanged?.Invoke(this, e);
            SetScrollInfo();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            edit.Font = Font;
            Invalidate();
        }

        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            SizeChange();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
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
                //bar.Visible = si.ScrollMax > 0 && si.nMax > 0 && si.nPage > 0;
                bar.Value = si.nPos;
            }
            else
            {
                bar.Maximum = si.ScrollMax;
                //bar.Visible = false;
            }
        }

        private int MiniHeight;

        private void SizeChange()
        {
            UIEdit edt = new UIEdit();
            edt.Font = edit.Font;
            edt.Invalidate();
            MiniHeight = edt.Height;
            edt.Dispose();

            if (!multiline)
            {
                Height = MiniHeight;
                edit.Top = (Height - edit.Height) / 2;
                edit.Left = 4;
                edit.Width = Width - 8;
            }
            else
            {
                if (Height < MiniHeight)
                {
                    Height = MiniHeight;
                }

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
        public char PasswordChar
        {
            get => edit.PasswordChar;
            set => edit.PasswordChar = value;
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => edit.ReadOnly;
            set
            {
                edit.ReadOnly = value;
                edit.BackColor = Color.White;
            }
        }

        [Description("输入类型"), Category("自定义")]
        [DefaultValue(UIEditType.String)]
        public UIEditType Type
        {
            get => edit.Type;
            set => edit.Type = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，能输入的最大值
        /// </summary>
        [Description("当InputType为数字类型时，能输入的最大值。")]
        [DefaultValue(int.MaxValue)]
        public double Maximum
        {
            get => edit.MaxValue;
            set => edit.MaxValue = value;
        }

        /// <summary>
        /// 当InputType为数字类型时，能输入的最小值
        /// </summary>
        [Description("当InputType为数字类型时，能输入的最小值。")]
        [DefaultValue(int.MinValue)]
        public double Minimum
        {
            get => edit.MinValue;
            set => edit.MinValue = value;
        }

        [DefaultValue(false)]
        public bool HasMaximum
        {
            get => edit.HasMaxValue;
            set => edit.HasMaxValue = value;
        }

        [DefaultValue(false)]
        public bool HasMinimum
        {
            get => edit.HasMinValue;
            set => edit.HasMinValue = value;
        }

        [DefaultValue(0.00)]
        public double DoubleValue
        {
            get => edit.DoubleValue;
            set => edit.DoubleValue = value;
        }

        [DefaultValue(0)]
        public int IntValue
        {
            get => edit.IntValue;
            set => edit.IntValue = value;
        }

        [CategoryAttribute("文字"), Browsable(true)]
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
        [DefaultValue(2)]
        public int DecLength
        {
            get => edit.DecLength;
            set => edit.DecLength = Math.Max(value, 0);
        }

        [DefaultValue(false)]
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
            if (uiColor.IsCustom()) return;

            edit.BackColor = fillColor = Color.White;
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
            edit.BackColor = color;
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
                    if (!currentItemText.Equals("ListItems"))
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
    }
}