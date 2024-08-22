using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TestReoGrid.Helpers;
using TestReoGrid.Models;
using TestReoGrid.Models.Enums;
using TestReoGrid.Models.ReoGrid.Old;
using unvell.ReoGrid;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.Graphics;

namespace TestReoGrid
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
        private const string Channels = "Channels";
        private const string Props = "Props";
        private RangeBorderStyle _dangerRangeBdStyle = new()
        {
            Color = ((Brush)Application.Current.Resources["PL_DangerBrush"]).ToReoColor(),
            Style = BorderLineStyle.Solid
        };
        [ObservableProperty]
        private ReoGridControl _reoGrid;

        [ObservableProperty]
        private Worksheet _sheet;

        [ObservableProperty]
        private AutoColumnFilter _propFilter;

        //[ObservableProperty]
        //private ObservableCollection<string> _filterColumns = ["Conc.", "Mass Conc.", "Name", "Info", "MW"];


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
        private NamedRange _channelsRange;


        [ObservableProperty]
        private NamedRange _propRange;

        /// <summary>
        /// Serial 命名Range集合
        /// </summary>
        public SerialRange Serial { get; set; }


        private OperEnum _operStatus;

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
            //Data();

            Serial = DataGenerateHelper.CreateNamedRanges(Sheet);
            DataGenerateHelper.InitSerial(Serial, Sheet);

            //Sheet.AfterRangeCopy += Sheet_AfterRangeCopy;
            //Sheet.AfterCut += Sheet_AfterCut;
            //Sheet.BeforeCopyCellContent += Sheet_BeforeCopyCellContent;
            //Sheet.AfterCopyCellContent += Sheet_AfterCopyCellContent;

            Sheet.CellDataChanged += Sheet_CellDataChanged;

            Sheet.BeforeRangeMove += Sheet_BeforeRangeMove;
            Sheet.AfterRangeMove += Sheet_AfterRangeMove;

            Sheet.AfterCopy += Sheet_AfterCopy;
            Sheet.BeforeCopy += Sheet_BeforeCopy;

            Sheet.BeforePaste += Sheet_BeforePaste;
            Sheet.AfterPaste += Sheet_AfterPaste;


            Sheet.RangeDataChanged += Sheet_RangeDataChanged;
            Sheet.BeforeDeleteCellContent += Sheet_BeforeDeleteCellContent;

            //Sheet.RowsDeleted += Sheet_RowsDeleted;
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
                selectedItems.AddRange([FilterDict[SelectedFilterCol]]);
            }
            else
            {
                selectedItems.AddRange(FilterDict.Values);
            }
            PropFilter.Apply();
        }

        /// <summary>
        /// 测试
        /// </summary>
        [RelayCommand]
        private void GetSerialData()
        {
            // 结束编辑
            Sheet.EndEdit(EndEditReason.NormalFinish);
            var xx = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
            var yy = xx.SelectMany(x => x.ParamValues).ToList();
            //DataGenerateHelper.GetSerialData(Serial, Sheet);
        }

        /// <summary>
        /// 删除某个属性
        /// </summary>
        [RelayCommand]
        private void DeleteProp()
        {
            _operStatus = OperEnum.DeleteRow;
            if (SelectedFilterCol is { Length: > 0 })
            {
                var allSPs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
                foreach (var ch in Serial.SolutionChannels)
                {
                    var sp = ch.SolutionParamList.FirstOrDefault(x => x.ParamName == SelectedFilterCol);
                    ch.SolutionParamList.Remove(sp);
                    allSPs.Remove(sp);

                    Sheet.DeleteRows(sp.RowStart, 1);

                    var row = sp.RowStart;
                    // 更新sp
                    var lowRows = allSPs.Where(x => x.RowStart > row).ToList();
                    foreach (var lrow in lowRows)
                    {
                        lrow.RowStart--;
                        lrow.RowEnd--;
                        foreach (var val in lrow.ParamValues)
                        {
                            val.RowStart--;
                            val.RowEnd--;
                        }
                    }
                    // 更新ch
                    ch.RowEnd--;
                    var lowChs = Serial.SolutionChannels.Where(x => x.RowStart > row).ToList();
                    foreach (var lch in lowChs)
                    {
                        lch.RowStart--;
                        lch.RowEnd--;
                    }
                }
            }
            _operStatus = OperEnum.None;
        }

        /// <summary>
        /// 新增某个属性
        /// </summary>
        [RelayCommand]
        private void AddProp()
        {
            _operStatus = OperEnum.InsertRow;
            if (SelectedFilterCol is { Length: > 0 })
            {
                var allSPs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
                foreach (var ch in Serial.SolutionChannels)
                {
                    var sp = DataGenerateHelper.CreateSP(SelectedFilterCol); // 必须重新生成一个
                    var existeSP = ch.SolutionParamList.FirstOrDefault(x => x.ParamName == SelectedFilterCol);
                    if (existeSP is null)
                    {
                        var row = ch.RowEnd + 1;
                        sp.RowStart = row;
                        sp.RowEnd = row;

                        sp.ColStart = ch.ColStart + 1;
                        sp.ColEnd = sp.ColStart;

                        ch.SolutionParamList.Add(sp);
                        Sheet.InsertRows(row, 1);

                        var propCell = Sheet.CreateAndGetCell(sp.RowStart, sp.ColStart);
                        propCell.Data = sp.ParamAlias;
                        propCell.IsReadOnly = true;


                        var lowSPs = allSPs.Where(x => x.RowStart >= row).ToList();
                        foreach (var lrow in lowSPs)
                        {
                            lrow.RowStart++;
                            lrow.RowEnd++;
                            foreach (var val in lrow.ParamValues)
                            {
                                val.RowStart++;
                                val.RowEnd++;
                            }
                        }
                        allSPs.Add(sp);

                        ch.RowEnd++;
                        var lowChs = Serial.SolutionChannels.Where(x => x.RowStart >= row).ToList();
                        foreach (var lch in lowChs)
                        {
                            lch.RowStart++;
                            lch.RowEnd++;
                        }

                        // 更新channel range
                        Sheet.UndefineNamedRange(ch.NameKey);
                        var newRange = Sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
                        newRange.Merge();
                        newRange.IsReadonly = true;
                    }
                }
            }
            _operStatus = OperEnum.None;
        }



        #endregion Commands



        #region Operation

        #region Move
        private void Sheet_BeforeRangeMove(object sender, unvell.ReoGrid.Events.BeforeCopyOrMoveRangeEventArgs e)
        {
            if (_operStatus is OperEnum.None)
            {
                _operStatus = OperEnum.Move;
            }
        }
        /// <summary>
        /// 单元格移动后 触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sheet_AfterRangeMove(object sender, unvell.ReoGrid.Events.CopyOrMoveRangeEventArgs e)
        {
            if (_operStatus is OperEnum.Move)
            {
                // from
                Sheet.IterateCells(e.FromRange, (row, col, cell) =>
                {
                    Serial.SolutionChannels.RemoveSoluParamValue(row, col);
                    return true;
                });

                // to
                Sheet.IterateCells(e.ToRange, (row, col, cell) =>
                {
                    var soluCh = Serial.SolutionChannels.FirstOrDefault(x => row >= x.RowStart && row <= x.RowEnd);
                    if (soluCh is not null)
                    {
                        var sp = Serial.SolutionChannels.FindSoluParam(row, col);
                        var spv = sp.FindSoluParamValue(row, col);
                        sp?.ParamValues?.Remove(spv);

                        spv = new SolutionParamValue
                        {
                            RowStart = row,
                            RowEnd = row,
                            ColStart = col,
                            ColEnd = col,
                            ParamValue = $"{cell.Data}",
                            ParamName = sp.ParamName,
                        };
                        sp.ParamValues.Add(spv);
                    }
                    return true;
                });

                _operStatus = OperEnum.None;
            }
        }
        #endregion Move

        #region Copy Paste
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

            if (!e.IsCancelled)
            {
                //_sourceValues.Clear();
                _operStatus = OperEnum.Copy;
            }
        }

        private void Sheet_AfterCopy(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            switch (_operStatus)
            {
                case OperEnum.Copy:
                case OperEnum.Cut:
                    break;
            }
        }

        private void Sheet_BeforePaste(object sender, unvell.ReoGrid.Events.BeforeRangeOperationEventArgs e)
        {
            Sheet.SelectRange(e.Range); // 重点，否则after就没有这个区域了
            // 可以拿到 目标位置的range
            switch (_operStatus)
            {
                case OperEnum.Copy:
                    break;
                case OperEnum.Cut:
                    break;
            }
        }

        private void Sheet_AfterPaste(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            Sheet.IterateCells(e.Range, (row, col, cell) =>
            {
                var spv = new SolutionParamValue
                {
                    RowStart = row,
                    RowEnd = row,
                    ColStart = col,
                    ColEnd = col,
                    ParamValue = $"{cell.Data}",
                };
                var sp = Serial.SolutionChannels.FindSoluParam(row, col);
                var oldSpv = sp.FindSoluParamValue(row, col);
                sp?.ParamValues?.Remove(oldSpv); // 目标位置已经有值，则先删除

                if (sp is not null)
                {
                    spv.ParamName = sp.ParamName;
                    sp.ParamValues.Add(spv);
                }

                return true;
            });


            // 没啥用
            _operStatus = OperEnum.None;
        }



        #endregion Copy Paste

        private void Sheet_RangeDataChanged(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            switch (_operStatus)
            {
                case OperEnum.Copy:
                    Sheet.IterateCells(e.Range, (row, col, cell) =>
                    {
                        Serial.SolutionChannels.RemoveSoluParamValue(row, col);

                        return true;
                    });
                    break;

                case OperEnum.Delete:
                    Sheet.IterateCells(e.Range, (row, col, cell) =>
                    {
                        Serial.SolutionChannels.RemoveSoluParamValue(row, col);

                        return true;
                    });
                    _operStatus = OperEnum.None;
                    break;
            }
        }

        /// <summary>
        /// 删除单元格操作先进入此方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sheet_BeforeDeleteCellContent(object sender, unvell.ReoGrid.Events.DeleteCellContentEventArgs e)
        {
            if (_operStatus is OperEnum.None && !e.Cell.IsReadOnly)
            {
                _operStatus = OperEnum.Delete;
            }
        }


        private void Sheet_CellDataChanged(object sender, unvell.ReoGrid.Events.CellEventArgs e)
        {
            if (e.Cell is not null && _operStatus is OperEnum.None)
            {
                var data = e.Cell.Data;
                var col = e.Cell.Column;
                var row = e.Cell.Row;


                var sp = Serial.SolutionChannels.FindSoluParam(row, col);
                //var pRow = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).FirstOrDefault(x => x.RowStart == row);
                if (sp is not null)
                {
                    //var cell = pRow.ParamValues.FirstOrDefault(x => x.ColStart == col);
                    var spv = sp.FindSoluParamValue(row, col);
                    if (spv is null)
                    {
                        sp.ParamValues.Add(new SolutionParamValue()
                        {
                            //NameKey = $"{pRow.NameKey}@{col - pRow.ColStart}",
                            RowStart = sp.RowStart,
                            RowEnd = sp.RowStart,
                            ColStart = col,
                            ColEnd = col,
                            ParamName = sp.ParamName,
                            ParamValue = $"{data}",
                        });
                    }
                    else
                    {
                        if (data is null)
                        {
                            sp.ParamValues.Remove(spv);
                        }
                        else
                        {
                            spv.ParamValue = $"{data}";
                        }
                    }
                }
            }
        }

        #endregion Operation

        #region Style
        public void SetDangerStyle(Cell cell)
        {
            if (cell is not null)
            {
                cell.Border.All = new RangeBorderStyle()
                {
                    Color = ((Brush)Application.Current.Resources["PL_DangerBrush"]).ToReoColor(),
                    Style = BorderLineStyle.Solid,
                };
            }
        }
        public void RestoreStyle(Cell cell)
        {
            if (cell is not null)
            {
                cell.Border.All = RangeBorderStyle.Empty;
            }
        }

       

        public void SetDangerBorder(int row, int col, int rows, int cols)
        {
            Sheet.SetRangeBorders(row, col, rows, cols, BorderPositions.InsideAll, _dangerRangeBdStyle);
        }

        public void RestoreBorder(int row, int col, int rows, int cols)
        {
            Sheet.RemoveRangeBorders(new RangePosition(row, col, rows, cols), BorderPositions.InsideAll);
        }


        #endregion Style

        #region Init Reogrid
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


        //private void Data()
        //{
        //var channels = new List<string>();
        //for (int i = 0; i < 8; i++)
        //{
        //    channels.Add($"Channel{i + 1}");
        //}
        //Sheet["A1:A8"] = channels;
        //Sheet["B1:B8"] = new object[] { "Conc.", "Conc.", "Conc.", "Conc.", "Mass Conc.", "Name", "Info", "MW" };
        //}



        //private void CreateRanges()
        //{
        //    ChannelsRange = Sheet.DefineNamedRange(Channels, "A1:A8");
        //    ChannelsRange.IsReadonly = true;
        //    ChannelsRange.Comment = "123";

        //    var channels = new List<string>();
        //    for (int i = 0; i < 8; i++)
        //    {
        //        channels.Add($"Channel{i + 1}");
        //    }
        //    ChannelsRange.Data = channels;


        //    PropRange = Sheet.DefineNamedRange(Props, "B1:B8");
        //    PropRange.IsReadonly = true;

        //    PropRange.Data = new object[] { "Conc.", "Conc.", "Conc.", "Conc.", "Mass Conc.", "Name", "Info", "MW" }; ;
        //}

        #endregion Init Reogrid

    }
}
