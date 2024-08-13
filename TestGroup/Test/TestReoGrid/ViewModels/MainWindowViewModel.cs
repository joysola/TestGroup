using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TestReoGrid.Helpers;
using TestReoGrid.Models;
using unvell.ReoGrid;
using unvell.ReoGrid.Data;

namespace TestReoGrid
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
        private const string Channels = "Channels";
        private const string Props = "Props";

        [ObservableProperty]
        private ReoGridControl _reoGrid;

        [ObservableProperty]
        private Worksheet _sheet;

        [ObservableProperty]
        private AutoColumnFilter _propFilter;

        [ObservableProperty]
        private ObservableCollection<string> _filterColumns = ["Conc.", "Mass Conc.", "Name", "Info", "MW"];

        [ObservableProperty]
        private string _selectedFilterCol;

        [ObservableProperty]
        private NamedRange _channelsRange;


        [ObservableProperty]
        private NamedRange _propRange;

        /// <summary>
        /// Serial 命名Range集合
        /// </summary>
        public SerialRange Serial { get; set; }


        [RelayCommand]
        private void LoadReoGrid(ReoGridControl reoGrid)
        {
            ReoGrid = reoGrid;
            Sheet = reoGrid.CurrentWorksheet;
            InitSetting(reoGrid);
            //RowCol();
            Freeze();
           // CreateRanges();
            Data();

            //
            Serial = DataGenerateHelper.CreateNamedRanges(Sheet);
            DataGenerateHelper.InitSerial(Serial, Sheet);
        }



        /// <summary>
        /// WPF 没有相应的界面
        /// </summary>
        [RelayCommand]
        private void SetFilter()
        {
            PropFilter?.Detach();
            PropFilter = Sheet.CreateColumnFilter("B", "B", 0, unvell.ReoGrid.Data.AutoColumnFilterUI.NoGUI);
            var selectedItems = PropFilter.Columns["B"].SelectedTextItems;
            selectedItems.Clear();
            if (SelectedFilterCol is { Length: > 0 })
            {
                selectedItems.AddRange([SelectedFilterCol]);
            }
            else
            {
                selectedItems.AddRange(FilterColumns);
            }
            PropFilter.Apply();
        }


        private void InitSetting(ReoGridControl reoGrid)
        {
            reoGrid.SheetTabNewButtonVisible = false;
            reoGrid.SheetTabVisible = false;
            //reoGrid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowSheetTabControl, false);

            var sheet = reoGrid.CurrentWorksheet;
            sheet.Name = "Serial";
        }


        private void RowCol()
        {
            //Sheet.SetRows(8);
            //Sheet.SetCols(10);


            var channelHeader = Sheet.GetColumnHeader(0);
            channelHeader.Text = "Channel";

            var propHeader = Sheet.GetColumnHeader(1);
            propHeader.Text = "Properties";
            propHeader.IsAutoWidth = true;

        }


        private void Freeze()
        {
            // 冻结到序号2列
            Sheet.FreezeToCell(0, 2, FreezeArea.Left);
            //Sheet.Unfreeze();
        }


        private void Data()
        {
            //var channels = new List<string>();
            //for (int i = 0; i < 8; i++)
            //{
            //    channels.Add($"Channel{i + 1}");
            //}
            //Sheet["A1:A8"] = channels;
            //Sheet["B1:B8"] = new object[] { "Conc.", "Conc.", "Conc.", "Conc.", "Mass Conc.", "Name", "Info", "MW" };
        }



        private void CreateRanges()
        {
            ChannelsRange = Sheet.DefineNamedRange(Channels, "A1:A8");
            ChannelsRange.IsReadonly = true;
            ChannelsRange.Comment = "123";

            var channels = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                channels.Add($"Channel{i + 1}");
            }
            ChannelsRange.Data = channels;


            PropRange = Sheet.DefineNamedRange(Props, "B1:B8");
            PropRange.IsReadonly = true;

            PropRange.Data = new object[] { "Conc.", "Conc.", "Conc.", "Conc.", "Mass Conc.", "Name", "Info", "MW" }; ;
        }



    }
}
