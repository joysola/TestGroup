using CommunityToolkit.Mvvm.ComponentModel;
using ReoGrid.Mvvm.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    /// <summary>
    /// 变量参数值
    /// </summary>
    public partial class SolutionParamValue2 : ObservableObject, IReoModel, IRecordModel
    {
        public int RowIndex { get; set; }
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


        public string NameKey { get; set; }

        public int RowStart { get; set; }
        public int RowEnd { get; set; }

        public int ColStart { get; set; }
        public int ColEnd { get; set; }

        public int Rows => RowEnd - RowStart + 1;
        public int Cols => ColEnd - ColStart + 1;
    }
}
