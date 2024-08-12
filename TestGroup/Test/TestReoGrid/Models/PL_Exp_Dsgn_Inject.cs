using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Expression.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Helpers;
using TestReoGrid.Models.Enums;
using unvell.ReoGrid.IO.OpenXML.Schema;

namespace TestReoGrid.Models
{
    public partial class PL_Exp_Dsgn_Inject : PLBase
    {
        /// <summary>
        /// 实验Id
        /// </summary>
        public long Exp_Id { get; set; }
        /// <summary>
        /// 设计步骤对应Id
        /// </summary>
        public long Step_Id { get; set; }

        #region machine properties
        /// <summary>
        /// 溶液类型
        /// </summary>
        [ObservableProperty]
        private SolutionTypeEnum _solution_Type;

        partial void OnSolution_TypeChanged(SolutionTypeEnum value)
        {
            //OnPropertyChanged(nameof(Solution_Type_No));
            //OnPropertyChanged(nameof(Display));
        }
        /// <summary>
        /// 同种溶液类型序号
        /// </summary>
        [ObservableProperty]
        private int _solution_No = -1;
        partial void OnSolution_NoChanged(int value)
        {
            //OnPropertyChanged(nameof(Solution_Type_No));
            //OnPropertyChanged(nameof(Display));
        }

        /// <summary>
        /// 报告点的注释
        /// </summary>
        [ObservableProperty]
        private string _comment;
        ///// <summary>
        ///// 等待的执行时间s
        ///// </summary>
        //[ObservableProperty]
        //private int _wait_Timespan;
        /// <summary>
        /// 流速μl/min
        /// </summary>
        [ObservableProperty]
        private int _flow_Rate;
        partial void OnFlow_RateChanged(int value)
        {
            Volume = (int)Math.Round((double)value / 60 * Contact_Time, 0, MidpointRounding.AwayFromZero); // 四舍五入
        }
        /// <summary>
        /// 流路 0001：A      0002：B     0003：AB
        /// </summary>
        [ObservableProperty]
        private string _flow_Path;
        /// <summary>
        /// 注射容量0001~9999
        /// </summary>
        [ObservableProperty]
        private int _volume;
        /// <summary>
        /// 接触时间
        /// </summary>
        [ObservableProperty]
        private int _contact_Time;

        partial void OnContact_TimeChanged(int value)
        {
            Volume = (int)Math.Round((double)Flow_Rate / 60 * value, 0, MidpointRounding.AwayFromZero); // 四舍五入
        }

        /// <summary>
        /// 设置气泡(0001~0003) 
        /// </summary>
        [ObservableProperty]
        private int _bubble;
        /// <summary>
        /// 设置预洗 0000：关闭预清洗     0001：开启预清洗
        /// </summary>
        public bool Is_Cleaned_Before { get; set; }
        /// <summary>
        /// 设置后洗 0000：关闭后清洗     0001：开启后清洗
        /// </summary>
        public bool Is_Cleaned_After { get; set; }
        /// <summary>
        /// 设置Koff时间（0000~9999）ms
        /// </summary>
        [ObservableProperty]
        private int _koff_TimeSpan;
        #endregion machine properties

        #region solution properties

        [ObservableProperty]
        private string _solution_Name;

        [ObservableProperty]
        [property: Obsolete]
        private double? _pH;
        /// <summary>
        /// 摩尔浓度（nM）/ (ng/ml)
        /// </summary>
        [ObservableProperty]
        private double? _concentration;
        partial void OnConcentrationChanged(double? value)
        {
            //if (value.HasValue && Molecular_Weight.HasValue)
            //    Mass_concentration = value.Value * Molecular_Weight.Value / 1000;
            //else
            //    Mass_concentration = null;
            Mass_concentration = MassMoleUnitHelper.GetMoleMassConc(value, Molecular_Weight, UnitType);
        }
        /// <summary>
        /// info
        /// </summary>
        [ObservableProperty]
        private string _info;
        /// <summary>
        /// 分子量(kD)
        /// </summary>
        [ObservableProperty]
        private int? _molecular_Weight;
        partial void OnMolecular_WeightChanged(int? value)
        {
            //if (value.HasValue && Concentration.HasValue)
            //    Mass_concentration = value.Value * Concentration.Value / 1000;
            //else
            //    Mass_concentration = null;
            Mass_concentration = MassMoleUnitHelper.GetMoleMassConc(Concentration, value, UnitType);
        }
        /// <summary>
        /// 质量浓度（ug/ml）/ (μM)
        /// </summary>
        [ObservableProperty]
        private double? _mass_concentration;
        #endregion solution properties

        ///// <summary>
        ///// Mix状态
        ///// </summary>
        //[ObservableProperty]
        //private MixStatusEnum? _mix_Status;
        /// <summary>
        /// Mix溶液A
        /// </summary>
        [ObservableProperty]
        private string _mix_SolutionA;
        /// <summary>
        /// Mix溶液B
        /// </summary>
        [ObservableProperty]
        private string _mix_SolutionB;
        /// <summary>
        /// Mix比例
        /// </summary>
        [ObservableProperty]
        private int? _mix_Fraction;

        /// <summary>
        /// 类型
        /// </summary>
        [ObservableProperty]
        private UnitTypeEnum? _unitType;
        partial void OnUnitTypeChanged(UnitTypeEnum? oldValue, UnitTypeEnum? newValue)
        {
            Mass_concentration = MassMoleUnitHelper.GetMoleMassConc(Concentration, Molecular_Weight, newValue);
        }
    }
}
