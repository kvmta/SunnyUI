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
 * 文件名称: UIDatePicker.cs
 * 文件说明: 日期选择框
 * 当前版本: V3.0
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-08-07: V2.2.7 可编辑输入，日期范围控制以防止出错
******************************************************************************/

using System;
using System.ComponentModel;
using System.Linq;

namespace Sunny.UI
{
    [ToolboxItem(true)]
    [DefaultProperty("Value")]
    [DefaultEvent("ValueChanged")]
    public sealed partial class UIDatePicker : UIDropControl
    {
        public delegate void OnDateTimeChanged(object sender, DateTime value);

        public UIDatePicker()
        {
            InitializeComponent();
            Value = DateTime.Now;
            Watermark = dateFormat;
            MaxLength = 10;
            EditorLostFocus += UIDatePicker_LostFocus;
            TextChanged += UIDatePicker_TextChanged;
        }

        [DefaultValue(false)]
        [Description("日期输入时，是否可空显示"), Category("SunnyUI")]
        public bool CanEmpty { get; set; }

        [DefaultValue(false)]
        [Description("日期输入时，显示今日按钮"), Category("SunnyUI")]
        public bool ShowToday { get; set; }

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
                && "/-".Any(m => m == dateFormat[Text.Length - 1])
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
                    Value = DateTime.Now;
                    Text = null;
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

        public event OnDateTimeChanged ValueChanged;

        protected override void ItemForm_ValueChanged(object sender, object value)
        {
            Value = (DateTime)value;
            Text = Value.ToString(dateFormat);
            Invalidate();
            ValueChanged?.Invoke(this, Value);
        }

        private readonly UIDateItem item = new UIDateItem();

        protected override void CreateInstance()
        {
            ItemForm = new UIDropDown(item);
        }

        [Description("选中日期"), Category("SunnyUI")]
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
            item.Date = Value;
            item.ShowToday = ShowToday;
            item.PrimaryColor = RectColor;
            item.Translate();
            ItemForm.Show(this);
        }

        private string dateFormat = "dd/MM/yyyy";

        [Description("日期格式化掩码"), Category("SunnyUI")]
        [DefaultValue("dd/MM/yyyy")]
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