using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using ReoGrid.Mvvm;
using ReoGrid.Mvvm.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Helpers;
using TestReoGrid.Models;
using unvell.ReoGrid;

namespace TestReoGrid.ViewModels
{
    public partial class MainWindowMVVMViewModel : BaseViewModel
    {
        private static readonly IMapper _mapper = new Mapper();

        [ObservableProperty]
        private WorksheetModel _sheetModel;

        [ObservableProperty]
        private ObservableCollection<IRecordModel> _paramValues = [];

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="reoGrid"></param>
        [RelayCommand]
        private void LoadReoGrid(ReoGridControl reoGrid)
        {
            ReoGrid = reoGrid;
            Sheet = reoGrid.CurrentWorksheet;
            //InitSetting(reoGrid);

            //Freeze();

            //Data();


            Serial = DataGenerateHelper.CreateNamedRanges(Sheet);
            //DataGenerateHelper.InitSerial(Serial, Sheet);

            LoadData();
        }
        [RelayCommand]
        private void GetSerialData()
        {

        }

        private void LoadData()
        {
            ParamValues?.Clear();
            var count = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).Count();
            for (int i = 0; i < count; i++)
            {
                ParamValues?.Add(new SolutionParamValue2
                {
                    ParamValue = $"{i}",
                });
            }
            var olds = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).SelectMany(x => x.ParamValues);
            foreach (var o in olds)
            {
                var n = _mapper.Map<SolutionParamValue2>(o);
                ParamValues?.Add(n);
            }

            SheetModel = new WorksheetModel(ReoGrid, typeof(SolutionParamValue2), ParamValues);
        }
    }
}
