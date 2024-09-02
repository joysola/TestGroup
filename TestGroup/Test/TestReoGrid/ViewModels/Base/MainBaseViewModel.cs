using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TestReoGrid.Models;
using TestReoGrid.Models.Enums;
using unvell.ReoGrid.Data;
using unvell.ReoGrid;

namespace TestReoGrid.ViewModels
{
    public partial class MainBaseViewModel : ObservableRecipient
    {
        private const string DilutionFormula = "Dilute";
        /// <summary>
        /// 小数位数
        /// </summary>
        private const int Digital = 3;

        private Color _mainTextColor;
        private Color _dangerColor;
        private Color _lightDangerColor;

        private ToolTip _toolTip = new()
        {
            Foreground = (Brush)Application.Current.Resources["PL_DangerBrush"],
        };


        private OperEnum _operStatus;


        #region Properties
        [ObservableProperty]
        private PL_Exp_Dsgn_Inject _solutionInject = new()
        {
            Concentration = 5.0,
            Molecular_Weight = 100,
            UnitType = UnitTypeEnum.Mole,
        };


        [ObservableProperty]
        private ReoGridControl _reoGrid;

        [ObservableProperty]
        private Worksheet _sheet;

        //[ObservableProperty]
        //private AutoColumnFilter _propFilter;


        [ObservableProperty]
        private Dictionary<string, string> _filterDict = new()
        {
            [nameof(PL_Exp_Dsgn_Inject.Solution_Name)] = "Name",
            [nameof(PL_Exp_Dsgn_Inject.Concentration)] = "Conc.",
            [nameof(PL_Exp_Dsgn_Inject.Molecular_Weight)] = "MW",
            [nameof(PL_Exp_Dsgn_Inject.Info)] = "Info",
            [nameof(PL_Exp_Dsgn_Inject.Mass_concentration)] = "Mass Conc.",
        };

        [ObservableProperty]
        private string _selectedFilterCol;

        [ObservableProperty]
        private double _dilutionRatio = 2;

        #endregion Properties



    }
}
