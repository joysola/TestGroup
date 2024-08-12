using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public partial class SolutionParam : ObservableObject
    {
        /// <summary>
        /// inject的属性名称
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// inject的属性别名
        /// </summary>
        public string ParamAlias { get; set; }
        /// <summary>
        /// inject的IsVar属性的名称
        /// </summary>
        public string IsVarName { get; set; }
        /// <summary>
        /// 校验的资源字典key
        /// </summary>
        [ObservableProperty]
        private string _validationKey;
        /// <summary>
        /// 变量类型
        /// </summary>
        public VarTypeEnum VarType { get; set; }
        /// <summary>
        /// 变量值的类型
        /// </summary>
        public Type ParamType { get; set; }

        /// <summary>
        /// 变量参数值集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SolutionParamValue> _paramValues = new();
    }

    /// <summary>
    /// 变量参数值
    /// </summary>
    public partial class SolutionParamValue : ObservableObject
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        [ObservableProperty]
        private string _paramValue;
        /// <summary>
        /// 是否有错
        /// </summary>
        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isEnabled;
    }

    public enum VarTypeEnum
    {
        None = 0,
        /// <summary>
        /// 溶液自身属性变量
        /// </summary>
        Solution = 5,
        /// <summary>
        /// 机器变量
        /// </summary>
        Machine = 10,
    }
}
