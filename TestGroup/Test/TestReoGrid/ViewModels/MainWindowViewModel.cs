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
using unvell.ReoGrid;
using unvell.ReoGrid.Data;

namespace TestReoGrid
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
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

        [RelayCommand]
        private void LoadReoGrid(ReoGridControl reoGrid)
        {
            ReoGrid = reoGrid;
            Sheet = reoGrid.CurrentWorksheet;
            InitSetting(reoGrid);
            RowCol();
            Data();
        }



        /// <summary>
        /// WPF 没有相应的界面
        /// </summary>
        [RelayCommand]
        private void SetFilter()
        {
            var slectedItems = PropFilter.Columns["B"].SelectedTextItems;
            slectedItems.Clear();
            slectedItems.AddRange([SelectedFilterCol]);
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
            Sheet.SetRows(8);
            Sheet.SetCols(10);


            var channelHeader = Sheet.GetColumnHeader(0);
            channelHeader.Text = "Channel";

            var propHeader = Sheet.GetColumnHeader(1);
            propHeader.Text = "Properties";
            propHeader.IsAutoWidth = true;

            PropFilter = Sheet.CreateColumnFilter("B", "B", 0, unvell.ReoGrid.Data.AutoColumnFilterUI.NoGUI);
        }

        private void Data()
        {
            //Sheet["A1:A8"]
            var channels = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                channels.Add($"Channel{i + 1}");
            }
            Sheet["A1:A8"] = channels;
            Sheet["B1:B8"] = new object[] { "Conc.", "Conc.", "Conc.", "Conc.", "Mass Conc.", "Name", "Info", "MW" };
        }
    }
}
