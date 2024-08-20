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
using TestReoGrid.Models.ReoGrid.Old;
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

        #region Commands
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="reoGrid"></param>
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


            Sheet.CellDataChanged += Sheet_CellDataChanged;
            //Sheet.AfterRangeCopy += Sheet_AfterRangeCopy;
            Sheet.AfterRangeMove += Sheet_AfterRangeMove;
            //Sheet.AfterCut += Sheet_AfterCut;
            Sheet.BeforeCopyCellContent += Sheet_BeforeCopyCellContent;
            Sheet.AfterCopyCellContent += Sheet_AfterCopyCellContent;

            Sheet.AfterCopy += Sheet_AfterCopy;
            Sheet.BeforeCopy += Sheet_BeforeCopy;

            Sheet.BeforePaste += Sheet_BeforePaste;
            Sheet.AfterPaste += Sheet_AfterPaste;

            Sheet.RangeDataChanged += Sheet_RangeDataChanged;
            //Sheet.CellEditTextChanging += Sheet_CellEditTextChanging;
            //Sheet.AfterCellEdit += Sheet_AfterCellEdit;


        }

       





        /// <summary>
        /// 单元格移动后 触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sheet_AfterRangeMove(object sender, unvell.ReoGrid.Events.CopyOrMoveRangeEventArgs e)
        {

        }

        //private void Sheet_AfterRangeCopy(object sender, unvell.ReoGrid.Events.CopyOrMoveRangeEventArgs e)
        //{

        //}

        private void Sheet_BeforeCopyCellContent(object sender, unvell.ReoGrid.Events.CopyCellContentEventArgs e)
        {

        }
        private void Sheet_AfterCopyCellContent(object sender, unvell.ReoGrid.Events.CopyCellContentEventArgs e)
        {

        }
        /// <summary>
        /// 如果存在readonly 则默认不进行复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sheet_BeforeCopy(object sender, unvell.ReoGrid.Events.BeforeRangeOperationEventArgs e)
        {
            Sheet.IterateCells(e.Range, (row, col, cell) =>
            {
                if (cell.IsReadOnly)
                {
                    e.IsCancelled = true;
                    return false;
                }
                return true;
            });
        }

        private void Sheet_AfterCopy(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {

        }

        private void Sheet_BeforePaste(object sender, unvell.ReoGrid.Events.BeforeRangeOperationEventArgs e)
        {

        }
        private void Sheet_AfterPaste(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            //var xx = Sheet.GetPartialGrid(e.Range);
        }

        private void Sheet_RangeDataChanged(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            //var xx = Sheet.GetRangeData(e.Range);
        }



        private void Sheet_CellDataChanged(object sender, unvell.ReoGrid.Events.CellEventArgs e)
        {
            if (e.Cell is not null)
            {
                var data = e.Cell.Data;
                var col = e.Cell.Column;
                var row = e.Cell.Row;

                var pRow = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).FirstOrDefault(x => x.RowStart == row);
                if (pRow is not null)
                {
                    var cell = pRow.ParamValues.FirstOrDefault(x => x.ColStart == col);
                    if (cell is null)
                    {
                        pRow.ParamValues.Add(new SolutionParamValue()
                        {
                            NameKey = $"{pRow.NameKey}@{col - pRow.ColStart}",
                            RowStart = pRow.RowStart,
                            RowEnd = pRow.RowStart,
                            ColStart = col,
                            ColEnd = col,
                            ParamName = pRow.ParamName,
                            ParamValue = $"{data}",
                        });
                    }
                    else
                    {
                        if (data is null)
                        {
                            pRow.ParamValues.Remove(cell);
                        }
                        else
                        {
                            cell.ParamValue = $"{data}";
                        }
                    }
                }
            }
        }
        //private void Sheet_CellEditCharInputed(object sender, unvell.ReoGrid.Events.CellEditCharInputEventArgs e)
        //{
        //    Sheet_CellDataChanged(null, e);
        //}

        //private void Sheet_AfterCellEdit(object sender, unvell.ReoGrid.Events.CellAfterEditEventArgs e)
        //{
        //    Sheet_CellDataChanged(null, e);
        //}


        //private void Sheet_CellDataChanged(object sender, unvell.ReoGrid.Events.CellEventArgs e)
        //{
        //    if (e.Cell is not null)
        //    {
        //        var data = e.Cell.Data;
        //        var col = e.Cell.Column;
        //        var row = e.Cell.Row;

        //        var pRow = Serial.PropRows.FirstOrDefault(x => x.RowStart == e.Cell.Row);
        //        if (pRow is not null)
        //        {
        //            var cell = pRow.Cells.FirstOrDefault(x => x.ColStart == col);
        //            if (cell is null)
        //            {
        //                pRow.Cells.Add(new ValueCell()
        //                {
        //                    NameKey = $"{pRow.NameKey}@{col - pRow.ColStart}",
        //                    RowStart = pRow.RowStart,
        //                    ColStart = col,
        //                    ParamName = pRow.ParamName,
        //                    ParamValue = $"{data}",
        //                });
        //            }
        //            else
        //            {
        //                if (data is null)
        //                {
        //                    pRow.Cells.Remove(cell);
        //                }
        //                else
        //                {
        //                    cell.ParamValue = $"{data}";
        //                }
        //            }
        //        }
        //    }
        //}



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


        [RelayCommand]
        private void GetSerialData()
        {
            // 结束编辑
            Sheet.EndEdit(EndEditReason.NormalFinish);
            DataGenerateHelper.GetSerialData(Serial, Sheet);
        }


        #endregion Commands

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
