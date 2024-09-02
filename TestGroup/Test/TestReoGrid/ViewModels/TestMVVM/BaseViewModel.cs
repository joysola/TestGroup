using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models.ReoGrid.Old;
using unvell.ReoGrid.Data;
using unvell.ReoGrid;

namespace TestReoGrid.ViewModels
{
    public partial class BaseViewModel : ObservableRecipient
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


        protected void InitSetting(ReoGridControl reoGrid)
        {
            reoGrid.SheetTabNewButtonVisible = false;
            reoGrid.SheetTabVisible = false;
            //reoGrid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowSheetTabControl, false);

            var sheet = reoGrid.CurrentWorksheet;
            sheet.Name = "Serial";
        }


        protected void RowCol()
        {
            //Sheet.SetRows(8);
            //Sheet.SetCols(10);


            var channelHeader = Sheet.GetColumnHeader(0);
            channelHeader.Text = "Channel";

            var propHeader = Sheet.GetColumnHeader(1);
            propHeader.Text = "Properties";
            propHeader.IsAutoWidth = true;

        }


        protected void Freeze()
        {
            // 冻结到序号2列
            Sheet.FreezeToCell(0, 2, FreezeArea.Left);
            //Sheet.Unfreeze();
        }


        protected void Data()
        {
            //var channels = new List<string>();
            //for (int i = 0; i < 8; i++)
            //{
            //    channels.Add($"Channel{i + 1}");
            //}
            //Sheet["A1:A8"] = channels;
            //Sheet["B1:B8"] = new object[] { "Conc.", "Conc.", "Conc.", "Conc.", "Mass Conc.", "Name", "Info", "MW" };
        }



        protected void CreateRanges()
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
