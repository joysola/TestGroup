using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TestReoGrid.DataFormats;
using TestReoGrid.Helpers;
using TestReoGrid.Models;
using TestReoGrid.Models.Enums;
using TestReoGrid.Models.ReoGrid.Old;
using unvell.ReoGrid;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Graphics;

namespace TestReoGrid
{
    public partial class MainWindowViewModel : ObservableRecipient
    {
        private ToolTip _toolTip = new()
        {
            Foreground = (Brush)Application.Current.Resources["PL_DangerBrush"],
        };

        //private const string Channels = "Channels";
        //private const string Props = "Props";

        private RangeBorderStyle _dangerRangeBdStyle = new()
        {
            Color = ((Brush)Application.Current.Resources["PL_DangerBrush"]).ToReoColor(),
            Style = BorderLineStyle.Solid
        };

        [ObservableProperty]
        private PL_Exp_Dsgn_Inject _solutionInject = new()
        {
            Concentration = 1.0,
            Molecular_Weight = 100,
            UnitType = UnitTypeEnum.Mole,
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
            InitSerial(Serial, Sheet);

            _toolTip.PlacementTarget = this.ReoGrid;
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

            //Sheet.CellMouseMove += Sheet_CellMouseEnter;
            Sheet.CellMouseEnter += Sheet_CellMouseEnter;
            Sheet.CellMouseLeave += Sheet_CellMouseLeave;
            //Sheet.RowsDeleted += Sheet_RowsDeleted;
        }

        [RelayCommand]
        private void Change()
        {
            var xx = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).SelectMany(x => x.ParamValues);
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
        private void DeleteProp(string selectedFilterCol)
        {
            _operStatus = OperEnum.DeleteRow;
            if (selectedFilterCol is { Length: > 0 })
            {
                var allSPs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
                foreach (var ch in Serial.SolutionChannels)
                {
                    var sp = ch.SolutionParamList.FirstOrDefault(x => x.ParamName == selectedFilterCol);
                    ch.SolutionParamList.Remove(sp);
                    allSPs.Remove(sp);
                    // 删除完sp后，讲prop置空即可
                    if (ch.SolutionParamList.Count == 0)
                    {
                        var cell = Sheet.GetCell(sp.RowStart, sp.ColStart);
                        cell.Data = null;
                    }
                    else
                    {

                        var row = sp.RowStart;
                        // 更新sp
                        var lowRows = allSPs.Where(x => x.RowStart >= row).ToList();
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
                        var lowChs = Serial.SolutionChannels.Except([ch]).Where(x => x.RowStart >= ch.RowEnd).ToList();
                        foreach (var lch in lowChs)
                        {
                            lch.RowStart--;
                            lch.RowEnd--;
                        }

                        try
                        {
                            Sheet.DeleteRows(sp.RowStart, 1);

                            SetMaxRows();
                            // 更新channel range
                            UpdateChRange(ch);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            _operStatus = OperEnum.None;
        }

        /// <summary>
        /// 新增某个属性
        /// </summary>
        [RelayCommand]
        private void AddProp(string selectedFilterCol)
        {
            _operStatus = OperEnum.InsertRow;
            if (selectedFilterCol is { Length: > 0 })
            {
                Sheet.DisableSettings(WorksheetSettings.Edit_Readonly);
                var allSPs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
                foreach (var ch in Serial.SolutionChannels)
                {
                    var sp = ReoSoluParamValueHelper.CreateSP(selectedFilterCol); // 必须重新生成一个
                    // sp为空时
                    if (ch.SolutionParamList.Count == 0)
                    {
                        sp.RowStart = ch.RowStart;
                        sp.ColStart = ch.ColStart + 1;

                        sp.RowEnd = sp.RowStart;
                        sp.ColEnd = sp.ColStart;

                        ch.SolutionParamList.Add(sp);
                    }
                    else
                    {
                        var existeSP = ch.SolutionParamList.FirstOrDefault(x => x.ParamName == selectedFilterCol);
                        if (existeSP is null)
                        {
                            var row = ch.RowEnd + 1;
                            sp.RowStart = row;
                            sp.RowEnd = row;

                            sp.ColStart = ch.ColStart + 1;
                            sp.ColEnd = sp.ColStart;

                            ch.SolutionParamList.Add(sp);

                            Sheet.InsertRows(row, 1);


                            //var propCell = Sheet.CreateAndGetCell(sp.RowStart, sp.ColStart);
                            //propCell.Data = sp.ParamAlias;
                            //propCell.IsReadOnly = true;

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
                            var lowChs = Serial.SolutionChannels.Except([ch]).Where(x => x.RowStart >= row).ToList();
                            foreach (var lch in lowChs)
                            {
                                lch.RowStart++;
                                lch.RowEnd++;
                            }
                        }
                    }
                    try
                    {
                        var propCell = Sheet.CreateAndGetCell(sp.RowStart, sp.ColStart);
                        propCell.Data = sp.ParamAlias;
                        propCell.IsReadOnly = true;
                    }
                    catch (Exception ex)
                    {

                    }


                    // 更新channel range
                    UpdateChRange(ch);
                }
                // 3. 调整sheet的最大行数
                //var rows = Serial.SolutionChannels.Max(x => x.RowEnd) + 1;
                // Sheet.SetRows(rows);
                SetMaxRows();
            }
            _operStatus = OperEnum.None;
        }


        [RelayCommand]
        private void NameProp(bool isProp)
        {
            if (isProp)
            {
                AddProp(nameof(PL_Exp_Dsgn_Inject.Solution_Name));
            }
            else
            {
                DeleteProp(nameof(PL_Exp_Dsgn_Inject.Solution_Name));
            }
        }


        [RelayCommand]
        private void ConcProp(bool isProp)
        {
            if (isProp)
            {
                AddProp(nameof(PL_Exp_Dsgn_Inject.Concentration));
            }
            else
            {
                DeleteProp(nameof(PL_Exp_Dsgn_Inject.Concentration));
            }
        }

        [RelayCommand]
        private void MWProp(bool isProp)
        {
            if (isProp)
            {
                AddProp(nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));
            }
            else
            {
                DeleteProp(nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));
            }
        }

        [RelayCommand]
        private void InfoProp(bool isProp)
        {
            if (isProp)
            {
                AddProp(nameof(PL_Exp_Dsgn_Inject.Info));
            }
            else
            {
                DeleteProp(nameof(PL_Exp_Dsgn_Inject.Info));
            }
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
                    var spv = Serial.SolutionChannels.FindSoluParamValue(row, col);

                    Serial.SolutionChannels.RemoveSoluParamValue(row, col);
                    RestoreBorder(cell.Row, cell.Column);
                    RestoreDataFormat(cell.Row, cell.Column);

                    AutoCalcuate(row, col, spv.ParamName);
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

                        spv = CreateSPV(row, col, sp.ParamName, cell.Data);
                        //new SolutionParamValue
                        //{
                        //    RowStart = row,
                        //    RowEnd = row,
                        //    ColStart = col,
                        //    ColEnd = col,
                        //    ParamValue = $"{cell.Data}",
                        //    ParamName = sp.ParamName,
                        //};
                        sp.ParamValues.Add(spv);
                        Validate(spv);

                        AutoCalcuate(row, col, spv.ParamName);
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
                //new SolutionParamValue
                //{
                //    RowStart = row,
                //    RowEnd = row,
                //    ColStart = col,
                //    ColEnd = col,
                //    ParamValue = $"{cell.Data}",
                //};
                var sp = Serial.SolutionChannels.FindSoluParam(row, col);
                var oldSpv = sp.FindSoluParamValue(row, col);
                sp?.ParamValues?.Remove(oldSpv); // 目标位置已经有值，则先删除
                RestoreBorder(cell.Row, cell.Column);
                RestoreDataFormat(cell.Row, cell.Column);

                if (sp is not null)
                {
                    var spv = CreateSPV(row, col, sp.ParamName, cell.Data);
                    //spv.ParamName = sp.ParamName;
                    sp.ParamValues.Add(spv);

                    Validate(spv);
                    AutoCalcuate(row, col, spv.ParamName);
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
                        var spv = Serial.SolutionChannels.FindSoluParamValue(row, col);

                        Serial.SolutionChannels.RemoveSoluParamValue(row, col);
                        RestoreBorder(cell.Row, cell.Column);
                        RestoreDataFormat(cell.Row, cell.Column);

                        AutoCalcuate(row, col, spv.ParamName);
                        return true;
                    });
                    break;

                case OperEnum.Delete:
                    Sheet.IterateCells(e.Range, (row, col, cell) =>
                    {
                        var spv = Serial.SolutionChannels.FindSoluParamValue(row, col);

                        Serial.SolutionChannels.RemoveSoluParamValue(row, col);
                        RestoreBorder(cell.Row, cell.Column);
                        RestoreDataFormat(cell.Row, cell.Column);
                        AutoCalcuate(row, col, spv.ParamName);

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

                        spv = CreateSPV(row, col, sp.ParamName, data);
                        //    new SolutionParamValue()
                        //{
                        //    //NameKey = $"{pRow.NameKey}@{col - pRow.ColStart}",
                        //    RowStart = sp.RowStart,
                        //    RowEnd = sp.RowStart,
                        //    ColStart = col,
                        //    ColEnd = col,
                        //    ParamName = sp.ParamName,
                        //    ParamValue = $"{data}",
                        //};
                        sp.ParamValues.Add(spv);
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

                    Validate(spv);
                    AutoCalcuate(row, col, spv.ParamName);
                }
            }
        }

        #endregion Operation

        #region Style
        //public void SetDangerStyle(Cell cell)
        //{
        //    if (cell is not null)
        //    {
        //        cell.Border.All = new RangeBorderStyle()
        //        {
        //            Color = ((Brush)Application.Current.Resources["PL_DangerBrush"]).ToReoColor(),
        //            Style = BorderLineStyle.Solid,
        //        };
        //    }
        //}
        //public void RestoreStyle(Cell cell)
        //{
        //    if (cell is not null)
        //    {
        //        cell.Border.All = RangeBorderStyle.Empty;
        //    }
        //}






        #endregion Style

        #region Validation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="spv"></param>
        private void Validate(SolutionParamValue spv, Cell cell = null)
        {
            if (spv is not null)
            {
                var rule = ValidateHelper.SoluRuleDict[spv.ParamName];
                var validationResult = rule.Validate(spv.ParamValue, CultureInfo.CurrentCulture);
                spv.HasError = !validationResult.IsValid;

                SetDataFormat(spv);
                if (validationResult.IsValid)
                {
                    RestoreBorder(spv.RowStart, spv.ColStart);
                }
                else
                {
                    SetDangerBorder(spv.RowStart, spv.ColStart);
                }
            }
        }

        public void SetDangerBorder(int row, int col, int rows = 1, int cols = 1)
        {
            Sheet.SetRangeBorders(row, col, rows, cols, BorderPositions.Outside, _dangerRangeBdStyle);
        }

        public void RestoreBorder(int row, int col, int rows = 1, int cols = 1)
        {
            Sheet.RemoveRangeBorders(new RangePosition(row, col, rows, cols), BorderPositions.Outside);
        }



        #region CellEvents
        private void Sheet_CellMouseLeave(object sender, unvell.ReoGrid.Events.CellMouseEventArgs e)
        {
            if (_toolTip.IsOpen)
            {
                _toolTip.IsOpen = false;
            }
        }

        private void Sheet_CellMouseEnter(object sender, unvell.ReoGrid.Events.CellMouseEventArgs e)
        {
            var cell = Sheet.GetCell(e.CellPosition);

            if (cell is not null)
            {
                var row = cell.Row;
                var col = cell.Column;
                var sp = Serial.SolutionChannels.FindSoluParam(row, col);
                var spv = sp.FindSoluParamValue(row, col);
                if (spv != null)
                {
                    if (spv.HasError)
                    {
                        var rule = ValidateHelper.SoluRuleDict[spv.ParamName];
                        var validationResult = rule.Validate(spv.ParamValue, CultureInfo.CurrentCulture);

                        //_toolTip.Foreground = (Brush)Application.Current.Resources["PL_DangerBrush"];
                        _toolTip.VerticalOffset = -20; // 调整位置
                        _toolTip.Content = validationResult.ErrorContent;
                        _toolTip.IsOpen = true;
                    }
                }
            }
            //}
        }
        #endregion CellEvents

        #endregion Validation


        #region AutoCalculate
        private void AutoCalcuate(int row, int col, string paramName)
        {
            if (paramName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight))
            {
                var ch = Serial.SolutionChannels.FindSolutionChannel(row, col);
                var sps = ch.SolutionParamList;
                var autoSP = sps.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));


                var colSPVs = sps.SelectMany(x => x.ParamValues).Where(x => x.ColStart == col).ToList();

                var autoSpv = colSPVs.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
                if (autoSpv is null)
                {
                    autoSpv = CreateSPV(autoSP.RowStart, col, autoSP.ParamName, null);
                    Sheet.CreateAndGetCell(autoSpv.RowStart, autoSpv.ColStart);
                }

                var concSpv = colSPVs.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration));
                var mwSpv = colSPVs.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));



                if (autoSpv is not null)
                {
                    var conc = SolutionInject.Concentration;
                    var mw = SolutionInject.Molecular_Weight;

                    var concStr = concSpv?.ParamValue;
                    if (concStr is not null && double.TryParse(concStr, out double paramConc))
                    {
                        conc = paramConc;
                    }

                    var mwStr = mwSpv?.ParamValue;
                    if (mwStr is not null && int.TryParse(mwStr, out int paramMW))
                    {
                        mw = paramMW;
                    }

                    if ((concSpv is not null && string.IsNullOrWhiteSpace(concStr)) ||
                        (mwSpv is not null && string.IsNullOrWhiteSpace(mwStr)))
                    {

                        autoSpv.ParamValue = null;

                    }
                    else
                    {
                        autoSpv.ParamValue = $"{MassMoleUnitHelper.GetMoleMassConc(conc, mw, SolutionInject?.UnitType)}";
                    }

                    Sheet.SetCellData(autoSpv.RowStart, autoSpv.ColStart, autoSpv.ParamValue);
                }

            }
        }


        #endregion AutoCalculate


        private void SetDataFormat(SolutionParamValue spv)
        {
            var places = ReoSoluParamValueHelper.GetDecimalPlaces(spv.ParamName);
            if (places > -1)
            {
                // Sheet.SetRangeDataFormat(spv.RowStart, spv.ColStart, spv.Rows, spv.Cols, CellDataFormatFlag.Custom, new DoubleDataFormt());
            }
        }

        private void RestoreDataFormat(int row, int col, int rows = 1, int cols = 1)
        {
            Sheet.DeleteRangeDataFormat(new RangePosition(row, col, rows, cols));
        }



        private SolutionParamValue CreateSPV(int row, int col, string paramName, object data)
        {
            SolutionParamValue spv = null;
            if (row > -1 && col > -1 && paramName is { Length: > 0 })
            {
                spv = new SolutionParamValue
                {
                    RowStart = row,
                    RowEnd = row,
                    ColStart = col,
                    ColEnd = col,
                    ParamValue = $"{data}",
                    ParamName = paramName,
                };
            }
            return spv;
        }


        private void UpdateChRange(SerialSolutionChannel ch)
        {
            // 更新channel range
            Sheet.UndefineNamedRange(ch.NameKey);
            var newRange = Sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
            newRange.Merge();
            newRange.Style.HorizontalAlign = ReoGridHorAlign.Center;
            newRange.Style.VerticalAlign = ReoGridVerAlign.Middle;
            newRange.Data = new[] { $"channel{ch.ChannelInfo.Channel_No + 1}" };
            newRange.IsReadonly = true;
        }


        /// <summary>
        /// 设置最大行
        /// </summary>
        private void SetMaxRows()
        {
            // 3. 调整sheet的最大行数
            Sheet.UndefineNamedRange("Blank");
            var rows = Serial.SolutionChannels.Max(x => x.RowEnd) + 1;
            Sheet.SetRows(rows + 1);
            var blankRange = Sheet.DefineNamedRange("Blank", rows, 0, 1, Sheet.ColumnCount);
            blankRange.Merge();
            blankRange.IsReadonly = true;
        }

        #region Init Reogrid
        private void InitSetting(ReoGridControl reoGrid)
        {
            reoGrid.SheetTabNewButtonVisible = false;
            reoGrid.SheetTabVisible = false;
            //reoGrid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowSheetTabControl, false);

            var sheet = reoGrid.CurrentWorksheet;
            sheet.Name = "Serial";

            //DataFormatterManager.Instance.DataFormatters.Add(CellDataFormatFlag.Custom, new DoubleDataFormt());
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

        public void InitSerial(SerialRange serial, Worksheet sheet)
        {
            // 1. 设置列宽（前两列需要加宽）
            sheet.SetColumnsWidth(0, 1, 120);
            sheet.SetColumnsWidth(1, 1, 100);

            var sps = serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
            if (sps.Count > 0)
            {
                sheet.DisableSettings(WorksheetSettings.Edit_Readonly);
            }
            else
            {
                sheet.EnableSettings(WorksheetSettings.Edit_Readonly);
            }

            // 2. 填充部分数据
            for (int i = 0; i < serial.SolutionChannels.Count; i++)
            {
                // 1-1 channel的范围1格
                //var chRange = serial.ChNamedRanges[i];
                var chD = serial.SolutionChannels[i];

                Sheet.UndefineNamedRange(chD.NameKey);
                var newRange = Sheet.DefineNamedRange(chD.NameKey, chD.RowStart, chD.ColStart, chD.Rows, chD.Cols);
                newRange.Merge();
                newRange.Style.HorizontalAlign = ReoGridHorAlign.Center;
                newRange.Style.VerticalAlign = ReoGridVerAlign.Middle;
                newRange.IsReadonly = true;
                // 合并成1列
                //sheet.MergeRange(chRange);
                newRange.Data = new[] { $"channel{serial.SolutionChannels[i].ChannelInfo.Channel_No + 1}" };

                for (int j = 0; j < chD.SolutionParamList.Count; j++)
                {
                    // 1-2. Prop的范围1格
                    var propD = chD.SolutionParamList[j];

                    var propCell = sheet.CreateAndGetCell(propD.RowStart, propD.ColStart);
                    propCell.Data = propD.ParamAlias;
                    propCell.IsReadOnly = true;
                    // 1-3. val的范围1格
                    for (int k = 0; k < propD.ParamValues.Count; k++)
                    {
                        var valD = propD.ParamValues[k];
                        var valCell = sheet.CreateAndGetCell(valD.RowStart, valD.ColStart);
                        valCell.Data = valD.ParamValue;
                    }
                }
            }

            // 3. 调整sheet的最大行数
            //var rows = serial.SolutionChannels.Max(x => x.RowEnd) + 1;
            // sheet.SetRows(rows);
            SetMaxRows();
        }
        #endregion Init Reogrid
    }
}
