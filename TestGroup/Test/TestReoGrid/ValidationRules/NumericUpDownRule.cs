using HandyControl.Properties.Langs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace TestReoGrid.ValidationRules
{
    /// <summary>
    /// 流速验证规则
    /// </summary>
    public class NumericUpDownRule : ValidationRule
    {
        public double MaxValue { get; set; } = 9999;
        public double MinValue { get; set; } = 1;
        /// <summary>
        /// 是否存在小数
        /// </summary>
        public bool HasDecimal { get; set; } = true;
        /// <summary>
        /// 数据是否可以为Null
        /// </summary>
        public bool CanbeNull { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is BindingExpression be && be.ResolvedSource is not null)
            {
                var type = be.ResolvedSource.GetType();
                var prop = type.GetProperty(be.ResolvedSourcePropertyName);
                var val = prop.GetValue(be.ResolvedSource);
                if (int.TryParse($"{val}", out int flowRate) &&
                    MaxValue >= flowRate && MinValue <= flowRate)
                {
                    return ValidationResult.ValidResult;
                }
            }
            else
            {
                // 可空则为空或字符串为空值认为正确
                if (CanbeNull)
                {
                    if (value is null || value is string valueStr && string.IsNullOrWhiteSpace(valueStr))
                    {
                        return ValidationResult.ValidResult;
                    }
                }

                if (HasDecimal)
                {
                    // 防止第一次输入小数点就校验通过，再经过String2NullableConverter时回调赋值再次进入校验
                    var valueStr = $"{value}";
                    var dotIndex = valueStr.IndexOf('.'); // 小数点的位置索引
                    if (double.TryParse(valueStr, out double num) && MaxValue >= num && MinValue <= num)
                    {
                        if (dotIndex > -1) // 1. 包含小数点
                        {
                            // 1-1. 小数点后有数字
                            if (dotIndex != valueStr.Length - 1)
                            {
                                if (valueStr.EndsWith('0')) // 1-1-1. 0.00 =》正在输入
                                {
                                    // do nothing
                                }
                                else // 1-1-2. 0.01=》数字成功
                                {
                                    return ValidationResult.ValidResult;
                                }
                            }
                            // 1-2. 小数点在末尾=》等待输入
                        }
                        else // 2. 整数
                        {
                            return ValidationResult.ValidResult;
                        }
                    }
                    //if (!$"{value}".EndsWith(".") && double.TryParse($"{value}", out double flowRateD) && MaxValue >= flowRateD && MinValue <= flowRateD)
                    //{
                    //    return ValidationResult.ValidResult;
                    //}
                }
                else
                {
                    if (int.TryParse($"{value}", out int flowRate) && MaxValue >= flowRate && MinValue <= flowRate)
                    {
                        return ValidationResult.ValidResult;
                    }
                }
            }
            return new ValidationResult(false, /*Lang.FormatError*/$"value shoule be between {MinValue} and {MaxValue}.");
        }
    }
}
