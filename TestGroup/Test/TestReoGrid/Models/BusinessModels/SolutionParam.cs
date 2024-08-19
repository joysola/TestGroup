using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public partial class SolutionParam : ObservableObject, IReoModel
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



        public string NameKey { get; set; }

        public int RowStart { get; set; }
        public int RowEnd { get; set; }

        public int ColStart { get; set; }
        public int ColEnd { get; set; }

        public int Rows => RowEnd - RowStart + 1;
        public int Cols => ColEnd - ColStart + 1;
    }

    

   
}
