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
 * 文件名称: UIDatetimePicker.cs
 * 文件说明: 日期时间选择框
 * 当前版本: V3.0
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-07-06: V2.2.6 重写下拉窗体，缩短创建时间
 * 2020-08-07: V2.2.7 可编辑输入，日期范围控制以防止出错
 * 2020-09-16: V2.2.7 更改滚轮选择时间的方向
******************************************************************************/

using System;
using System.ComponentModel;
using System.Linq;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    public sealed partial class UIDatetimePicker : UIDropControl
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UIDatetimePicker
            // 
            this.Name = "UIDatetimePicker";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 30, 0);
            this.SymbolDropDown = 61555;
            this.SymbolNormal = 61555;
            this.ButtonClick += new System.EventHandler(this.UIDatetimePicker_ButtonClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        [DefaultValue(false)]
        [Description("日期输入时，是否可空显示"), Category("SunnyUI")]
        public bool CanEmpty { get; set; }

        [DefaultValue(false)]
        [Description("日期输入时，显示今日按钮"), Category("SunnyUI")]
        public bool ShowToday { get; set; }


        public UIDatetimePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;
            Text = Value.ToString(DateFormat);
            Width = 200;
            Watermark = dateFormat;
            EditorLostFocus += UIDatePicker_LostFocus;
            TextChanged += UIDatePicker_TextChanged;
            MaxLength = 19;
        }

        private void UIDatePicker_TextChanged(object sender, EventArgs e)
        {
            if (Text.Length <= 0)
                return;

            if (!"1234567890".Any(m => m == Text[Text.Length - 1]))
            {
                Text = Text.Remove(Text.Length - 1);
                SelectionStart = Text.Length;
            }

            if (Text.Length < dateFormat.Length
                && "/: -".Any(m => m == dateFormat[Text.Length - 1])
                && Text[Text.Length - 1] != dateFormat[Text.Length - 1])
            {
                Text = $"{Text.Substring(0, Text.Length - 1)}{dateFormat[Text.Length - 1]}{Text[Text.Length - 1]}";
                SelectionStart = Text.Length;
            }

            if (Text.Length == MaxLength)
            {
                try
                {
                    DateTime dt = Text.ToDateTime(DateFormat);
                    Value = dt;
                }
                catch
                {
                    Value = DateTime.Now.Date;
                }
            }
        }

        private void UIDatePicker_LostFocus(object sender, EventArgs e)
        {
            if (Text.IsNullOrEmpty())
            {
                if (CanEmpty) return;
            }

            try
            {
                DateTime dt = Text.ToDateTime(DateFormat);
                Value = dt;
            }
            catch
            {
                Value = DateTime.Now.Date;
            }
        }

        public delegate void OnDateTimeChanged(object sender, DateTime value);


        public event OnDateTimeChanged ValueChanged;

        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            Value = (DateTime)value;
            Text = Value.ToString(dateFormat);
            Invalidate();
            ValueChanged?.Invoke(this, Value);
        }

        private readonly UIDateTimeItem item = new UIDateTimeItem();

        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        [Description("选中日期时间"), Category("SunnyUI")]
        public DateTime Value
        {
            get => item.Date;
            set
            {
                if (value < new DateTime(1900, 1, 1))
                    value = new DateTime(1900, 1, 1);
                Text = value.ToString(dateFormat);
                item.Date = value;
            }
        }

        private void UIDatetimePicker_ButtonClick(object sender, EventArgs e)
        {
            if (DateTime.TryParse(Text, out DateTime dt))
                Value = dt;
            else
                Value = DateTime.Now;

            item.Date = Value;
            item.ShowToday = ShowToday;
            item.PrimaryColor = RectColor;
            item.Translate();
            ItemForm.Show(this);
        }

        private string dateFormat = "dd/MM/yyyy HH:mm:ss";

        [Description("日期格式化掩码"), Category("SunnyUI")]
        [DefaultValue("dd/MM/yyyy HH:mm:ss")]
        public string DateFormat
        {
            get => dateFormat;
            set
            {
                dateFormat = value;
                Text = Value.ToString(dateFormat);
                MaxLength = dateFormat.Length;
            }
        }
    }
}
