using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public partial class PL_Exp_Channel : PLBase, IReoModel
    {
        /// <summary>
        /// 实验Id
        /// </summary>
        public long Exp_Id { get; set; }
        /// <summary>
        /// Cycle的Id
        /// </summary>
        public long? Cycle_Id { get; set; }
        /// <summary>
        /// 通道序号
        /// </summary>
        public int Channel_No { get; set; }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string Channel_Name { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        [ObservableProperty]
        private bool _is_Selected;


        public string NameKey { get; set; }

        public int RowStart { get; set; }
        public int RowEnd { get; set; }

        public int ColStart { get; set; }
        public int ColEnd { get; set; }

        public int Rows => RowEnd - RowStart + 1;
        public int Cols => ColEnd - ColStart + 1;
    }
}
