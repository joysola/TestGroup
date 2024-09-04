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
using TestReoGrid.Models.ReoGrid;
using TestReoGrid.Models.ReoGrid.Old;
using unvell.ReoGrid;
using unvell.ReoGrid.Actions;
using unvell.ReoGrid.Data;
using unvell.ReoGrid.DataFormat;
using unvell.ReoGrid.Formula;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Interaction;

namespace TestReoGrid
{
    public partial class MainWindow_SerialViewModel : ObservableRecipient
    {
        private const string DilutionFormula = "Dilute";
        /// <summary>
        /// 小数位数
        /// </summary>
        private const int Digital = 3;

        private Color _mainTextColor;
        private Color _dangerColor;
        private Color _lightDangerColor;
        private Color _greyColor;

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

        /// <summary>
        /// Serial 命名Range集合
        /// </summary>
        public SerialRange Serial { get; set; }


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

        [ObservableProperty]
        private AutoColumnFilter _propFilter;


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
        //[ObservableProperty]
        //private NamedRange _channelsRange;


        //[ObservableProperty]
        //private NamedRange _propRange;
        #endregion Properties


        #region Commands
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="reoGrid"></param>
        [RelayCommand]
        private void LoadReoGrid(ReoGridControl reoGrid)
        {
            _dangerColor = (Color)Application.Current.Resources["PL_DangerColor"];
            _mainTextColor = (Color)Application.Current.Resources["PL_MainTextColor"];
            _lightDangerColor = (Color)Application.Current.Resources["PL_LightDangerColor"];
            _greyColor = (Color)Application.Current.Resources["PL_GreyColor"];

            ReoGrid = reoGrid;
            Sheet = reoGrid.CurrentWorksheet;
            InitSetting(reoGrid);
            //RowCol();
            Freeze();
            // CreateRanges();
            //Data();
            //InitFormula();

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
            Sheet.BeforeCellKeyDown += Sheet_BeforeCellKeyDown;
        }

        /// <summary>
        /// 禁用撤回和恢复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sheet_BeforeCellKeyDown(object sender, unvell.ReoGrid.Events.BeforeCellKeyDownEventArgs e)
        {
            if (e.KeyCode is (KeyCode.Z | KeyCode.Control) or (KeyCode.Y | KeyCode.Control))
            {
                e.IsCancelled = true;
            }
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
        private void GetData()
        {
            // 结束编辑
            Sheet.EndEdit(EndEditReason.NormalFinish);
            Check(); // 检查数据
            var xx = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
            var yy = xx.SelectMany(x => x.ParamValues).ToList();
            if (yy.Exists(x => x.HasError))
            {
                MessageBox.Show("Error！");
            }
            else
            {
                MessageBox.Show("Success！");
            }
            //DataGenerateHelper.GetSerialData(Serial, Sheet);
        }


        [RelayCommand]
        private void Dilute(double dilutionRatio)
        {
            _operStatus = OperEnum.Dilute;
            var selRange = Sheet.SelectionRange;
            // serial时的处理, 需要第三列开始计算
            if (selRange.Cols > 1 && selRange.Rows == 1 && selRange.Col > 1)
            {
                var row = selRange.Row;
                var col_End = selRange.EndCol;
                var col_start = selRange.Col;

                var dataCell = Sheet.GetCell(row, col_End); // 找最后一个
                var sp = Serial.SolutionChannels.FindSerialSoluParam(row, col_End);
                var spv = sp.FindSoluParamValue(row, col_End);
                if (dataCell is not null &&
                    spv is not null &&
                    spv.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration) &&
                    !spv.HasError &&
                    double.TryParse(spv.ParamValue, out var conc))
                {

                    var currentConc = conc;
                    for (int i = col_End - 1; i >= col_start; i--)
                    {
                        currentConc = Math.Round(currentConc / dilutionRatio, Digital, MidpointRounding.AwayFromZero);

                        var newSPV = sp.FindSoluParamValue(row, i);
                        if (newSPV is null)
                        {
                            newSPV = CreateSPV(row, i, sp.ParamName, currentConc);
                            sp.ParamValues.Add(newSPV); // 新增则加入集合
                        }
                        else
                        {
                            newSPV.ParamValue = $"{currentConc}";
                        }

                        // 稀释后进行自动计算
                        AutoCalcuate(newSPV.RowStart, newSPV.ColStart, newSPV.ParamName);
                        var cell = Sheet.CreateAndGetCell(row, i);
                        cell.Data = currentConc;
                        Validate(newSPV);
                        //ReoGrid.DoAction(new SetCellDataAction(row, i, currentConc));
                        //Sheet.SetCellData(row, i, currentConc);
                    }
                }
            }
            _operStatus = OperEnum.None;
        }
        // 稀释
        //var cell = Sheet.CreateAndGetCell(selectedRange.Row, selectedRange.Col);
        //cell.Formula = $"{DilutionFormula}({DilutionRatio})";

        #region Prop Commands

        [RelayCommand]
        private void Change(string propName)
        {
            if (propName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight))
            {
                var spvs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).SelectMany(x => x.ParamValues)
                     .Where(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight)).ToList();
                foreach (var spv in spvs)
                {
                    AutoCalcuate(spv.RowStart, spv.ColStart, spv.ParamName);
                }
            }
        }
        /// <summary>
        /// 删除某个属性
        /// </summary>
        [RelayCommand]
        private void DeleteProp(string selectedFilterCol)
        {
            _operStatus = OperEnum.DeleteRow;
            UndefineAutoRange(); // 解除定义，防止删除丢失范围
            DeletePropCore(selectedFilterCol);
            DefineAutoRange(); // 恢复定义
            _operStatus = OperEnum.None;
        }

        /// <summary>
        /// 新增某个属性
        /// </summary>
        [RelayCommand]
        private void AddProp(string selectedFilterCol)
        {
            _operStatus = OperEnum.InsertRow;
            var ap = ContainsAutoSP();
            if (ap)
            {
                DeleteAutoSP();
            }
            AddPropCore(selectedFilterCol);
            if (ap)
            {
                AddAutoSP();
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
            AutoSP(nameof(PL_Exp_Dsgn_Inject.Concentration), isProp);
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
            AutoSP(nameof(PL_Exp_Dsgn_Inject.Molecular_Weight), isProp);

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
        #endregion Prop Commands

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

                    Serial.SolutionChannels.RemoveSerialSoluParamValue(row, col);
                    //RestoreBorder(cell.Row, cell.Column);
                    RestoreCell(cell.Row, cell.Column);
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
                        var sp = Serial.SolutionChannels.FindSerialSoluParam(row, col);
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
                var sp = Serial.SolutionChannels.FindSerialSoluParam(row, col);
                var oldSpv = sp.FindSoluParamValue(row, col);
                sp?.ParamValues?.Remove(oldSpv); // 目标位置已经有值，则先删除
                                                 //RestoreBorder(cell.Row, cell.Column);
                RestoreCell(cell.Row, cell.Column);

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

                        Serial.SolutionChannels.RemoveSerialSoluParamValue(row, col);
                        //RestoreBorder(cell.Row, cell.Column);
                        RestoreCell(cell.Row, cell.Column);

                        RestoreDataFormat(cell.Row, cell.Column);

                        return true;
                    });
                    break;

                case OperEnum.Delete:
                    Sheet.IterateCells(e.Range, (row, col, cell) =>
                    {
                        var spv = Serial.SolutionChannels.FindSoluParamValue(row, col);

                        if (spv is not null)
                        {
                            Serial.SolutionChannels.RemoveSerialSoluParamValue(row, col);
                            //RestoreBorder(cell.Row, cell.Column);
                            RestoreCell(cell.Row, cell.Column);

                            RestoreDataFormat(cell.Row, cell.Column);
                            AutoCalcuate(row, col, spv.ParamName);
                        }

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


                var sp = Serial.SolutionChannels.FindSerialSoluParam(row, col);
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

                //SetDataFormat(spv);
                if (validationResult.IsValid)
                {
                    //RestoreBorder(spv.RowStart, spv.ColStart);
                    RestoreCell(spv.RowStart, spv.ColStart);
                }
                else
                {
                    //SetDangerBorder(spv.RowStart, spv.ColStart);
                    SetDangerCell(spv.RowStart, spv.ColStart);
                }
            }
        }

        //public void SetDangerBorder(int row, int col, int rows = 1, int cols = 1)
        //{
        //    Sheet.SetRangeBorders(row, col, rows, cols, BorderPositions.Outside, _dangerRangeBdStyle);
        //}

        //public void RestoreBorder(int row, int col, int rows = 1, int cols = 1)
        //{
        //    Sheet.RemoveRangeBorders(new RangePosition(row, col, rows, cols), BorderPositions.Outside);
        //}

        public void SetDangerCell(int row, int col, int rows = 1, int cols = 1)
        {
            var cell = Sheet.CreateAndGetCell(row, col);
            if (cell is not null)
            {
                cell.Style.TextColor = _dangerColor.ToReoColor();
                if (cell.Data is null)
                {
                    SetNullDanger(row, col);
                }
            }
        }

        public void RestoreCell(int row, int col, int rows = 1, int cols = 1)
        {
            var cell = Sheet.GetCell(row, col);
            if (cell is not null)
            {
                if (IsAutoSPV(row, col))
                {
                    cell.Style.TextColor = _mainTextColor.ToReoColor();
                }
                else
                {
                    cell.Style.TextColor = SolidColor.Black;
                }
                cell.Style.BackColor = SolidColor.Transparent;
            }
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
                var sp = Serial.SolutionChannels.FindSerialSoluParam(row, col);
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
        /// <summary>
        /// 处理自动sp
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="isAdd"></param>
        private void AutoSP(string paramName, bool isAdd)
        {
            _operStatus = OperEnum.Auto;
            if (paramName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight))
            {
                var sps = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList);
                var autoSPs = sps.Where(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration)).ToList();
                var pairSPs = sps.Where(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight)).ToList();

                if (isAdd && autoSPs.Count == 0)
                {
                    AddAutoSP();
                }
                else
                {
                    if (pairSPs.Count == 0)
                    {
                        DeleteAutoSP();
                    }
                    else // 存在conc或mw时
                    {
                        // 自动计算一下
                        var spvs = pairSPs.SelectMany(x => x.ParamValues);
                        foreach (var spv in spvs)
                        {
                            AutoCalcuate(spv.RowStart, spv.ColStart, spv.ParamName);
                        }
                    }
                }
            }
            _operStatus = OperEnum.None;
        }


        private void AutoCalcuate(int row, int col, string paramName)
        {
            if (paramName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight))
            {
                AutoCalcuateCore(row, col);
            }
        }

        /// <summary>
        /// 自动计算核心
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AutoCalcuateCore(int row, int col)
        {
            var ch = Serial.SolutionChannels.FindSolutionChannel(row, col);
            var sps = ch.SolutionParamList;
            var autoSP = sps.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
            var concSP = sps.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration));
            var mwSP = sps.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));

            var colSPVs = sps.SelectMany(x => x.ParamValues).Where(x => x.ColStart == col).ToList();

            var autoSpv = colSPVs.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
            if (autoSpv is null)
            {
                autoSpv = CreateSPV(autoSP.RowStart, col, autoSP.ParamName, null);
                autoSP.ParamValues.Add(autoSpv);
                //var autoCell = Sheet.CreateAndGetCell(autoSpv.RowStart, autoSpv.ColStart);
                //autoCell.IsReadOnly = true;
            }

            var concSpv = colSPVs.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration));
            var mwSpv = colSPVs.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));

            if (autoSpv is not null)
            {
                double? conc = null;
                int? mw = null;

                if (!(concSP is not null && mwSP is not null)) // 只存在一个conc或mw时，需要继承部分数据
                {
                    conc = SolutionInject.Concentration;
                    mw = SolutionInject.Molecular_Weight;
                }

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


                if (concSpv is null && mwSpv is null) // 防止都不存在时，依然计算
                {
                    autoSP.ParamValues.Remove(autoSpv);
                    autoSpv.ParamValue = null;
                }
                else if ((concSpv is not null && string.IsNullOrWhiteSpace(concStr)) || (mwSpv is not null && string.IsNullOrWhiteSpace(mwStr))) // 存在，但是没值时
                {
                    autoSpv.ParamValue = null;
                }
                else
                {
                    var value = MassMoleUnitHelper.GetMoleMassConc(conc, mw, SolutionInject?.UnitType);
                    autoSpv.ParamValue = value.HasValue ? $"{Math.Round(value.Value, Digital, MidpointRounding.AwayFromZero)}" : null;
                }

                var cell = Sheet.CreateAndGetCell(autoSpv.RowStart, autoSpv.ColStart);
                cell.Style.TextColor = _mainTextColor.ToReoColor();
                cell.Data = autoSpv.ParamValue;
                //ReoGrid.DoAction(new SetCellDataAction(cell.Row, cell.Column, autoSpv.ParamValue));
            }
        }

        /// <summary>
        /// 是否包含自动计算的sp
        /// </summary>
        /// <returns></returns>
        private bool ContainsAutoSP()
        {
            var autoSPs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).Where(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration)).ToList();
            return autoSPs.Count > 0;
        }

        private bool IsAutoSPV(int row, int column)
        {
            var result = false;
            var spv = Serial.SolutionChannels.FindSoluParamValue(row, column);
            if (spv is not null)
            {
                result = spv.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration);
            }
            return result;
        }

        private bool IsAuto(string propName) => propName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration);


        /// <summary>
        /// 删除自动计算的sp
        /// </summary>
        /// <returns></returns>
        private void DeleteAutoSP()
        {
            // 1. 
            UndefineAutoRange();
            // 2.
            DeletePropCore(nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
        }
        /// <summary>
        /// 解除定义autoRange
        /// </summary>
        private void UndefineAutoRange()
        {
            foreach (var ch in Serial.SolutionChannels)
            {
                var autoSP = ch.SolutionParamList.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));

                if (autoSP is not null)
                {
                    var rangeName = $"{ch.ChannelInfo.Channel_No}@{autoSP.ParamName}";
                    var range = Sheet.GetNamedRange(rangeName);
                    if (range is not null)
                    {
                        range.Style.TextColor = SolidColor.Black;
                        range.IsReadonly = false;
                    }
                    var xx = Sheet.UndefineNamedRange(rangeName);
                }
            }
        }

        /// <summary>
        /// 定义autorange
        /// </summary>
        private void DefineAutoRange()
        {
            foreach (var ch in Serial.SolutionChannels)
            {
                var autoSP = ch.SolutionParamList.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));

                if (autoSP is not null)
                {
                    var newRange = Sheet.DefineNamedRange($"{ch.ChannelInfo.Channel_No}@{autoSP.ParamName}", autoSP.RowStart, autoSP.ColStart, 1, Sheet.ColumnCount);
                    if (newRange is not null)
                    {
                        newRange.Style.TextColor = _mainTextColor.ToReoColor();
                        newRange.IsReadonly = true;
                    }
                }
            }
        }


        /// <summary>
        /// 新增自动计算sp
        /// </summary>
        private void AddAutoSP()
        {
            // 1. 
            AddPropCore(nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
            foreach (var ch in Serial.SolutionChannels)
            {
                var autoSP = ch.SolutionParamList.FirstOrDefault(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
                if (autoSP is not null)
                {

                    var cell = Sheet.GetCell(autoSP.RowStart, autoSP.ColStart);
                    if (cell != null)
                        cell.Style.TextColor = _mainTextColor.ToReoColor();

                    //var newRange = Sheet.DefineNamedRange($"{ch.ChannelInfo.Channel_No}@{autoSP.ParamName}", autoSP.RowStart, autoSP.ColStart, 1, Sheet.ColumnCount);
                    //if (newRange is not null)
                    //{
                    //    newRange.IsReadonly = true;
                    //}
                    // 重新触发自动计算
                    var pairSPs = ch.SolutionParamList.Where(x => x.ParamName is nameof(PL_Exp_Dsgn_Inject.Concentration) or nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));
                    var pairSPVs = pairSPs.SelectMany(x => x.ParamValues);
                    foreach (var spv in pairSPVs)
                    {
                        AutoCalcuate(spv.RowStart, spv.ColStart, spv.ParamName);
                    }
                }
            }
            // 2. 
            DefineAutoRange();
        }


        private void AddPropCore(string selectedFilterCol)
        {
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
                            //ReoGrid.DoAction(new InsertRowsAction(row, 1));


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
                        //ReoGrid.DoAction(new SetCellDataAction(sp.RowStart, sp.ColStart, sp.ParamAlias));
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
        }

        private void DeletePropCore(string selectedFilterCol)
        {
            if (selectedFilterCol is { Length: > 0 })
            {
                var allSPs = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
                foreach (var ch in Serial.SolutionChannels)
                {
                    var sp = ch.SolutionParamList.FirstOrDefault(x => x.ParamName == selectedFilterCol);
                    if (sp != null)
                    {
                        ch.SolutionParamList.Remove(sp);
                        allSPs.Remove(sp);
                        // 删除完sp后，讲prop置空即可
                        if (ch.SolutionParamList.Count == 0)
                        {
                            Sheet.SetCellData(sp.RowStart, sp.ColStart, null);
                            //ReoGrid.DoAction(new SetCellDataAction(sp.RowStart, sp.ColStart, null));

                            // 清空值
                            foreach (var spv in sp.ParamValues)
                            {
                                //spv.ParamValue = null;
                                var cell = Sheet.GetCell(spv.RowStart, spv.ColStart);
                                if (cell is not null)
                                {
                                    //ReoGrid.DoAction(new SetCellDataAction(spv.RowStart, spv.ColStart, null));
                                    cell.Data = null;
                                    //Validate(spv);
                                    RestoreNullDanger(cell.Row, cell.Column); // 恢复
                                }
                            }
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
                                //ReoGrid.DoAction(new RemoveRowsAction(sp.RowStart, 1));
                                // 更新channel range
                                UpdateChRange(ch);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                SetMaxRows();
            }
        }

        #endregion AutoCalculate

        #region Formula

        //private void InitFormula()
        //{
        //    FormulaExtension.CustomFunctions[DilutionFormula] = Dilute;
        //}

        //private object Dilute(Cell cell, object[] args)
        //{
        //    if (args is [double ratio] && cell.Data is double conc)
        //    {
        //        return conc / ratio;
        //    }
        //    return null;
        //}

        #endregion Formula

        #region Check
        /// <summary>
        /// 检查数据
        /// </summary>
        private void Check()
        {
            var sps = Serial.SolutionChannels.SelectMany(x => x.SolutionParamList).ToList();
            var spvs = sps.SelectMany(x => x.ParamValues).ToList();
            //if (spvs.Count > 0)
            {
                var maxCol = spvs.Count > 0 ? spvs.Max(x => x.ColStart) : 2;
                var minCol = 2;
                foreach (var sp in sps)
                {
                    if (!IsAuto(sp.ParamName))
                    {
                        var row = sp.RowStart;
                        for (int i = minCol; i <= maxCol; i++)
                        {
                            var spv = sp.FindSoluParamValue(row, i);
                            if (spv is null)
                            {
                                spv = CreateSPV(row, i, sp.ParamName, null);
                                sp.ParamValues.Add(spv);
                                //break;
                            }

                            Validate(spv);

                            if (spv.HasError)
                            {
                                Sheet.SelectRange(row, i, 1, 1); // 防止不能立即显色
                            }
                        }
                    }
                }
            }
        }

        private void SetNullDanger(int row, int col)
        {
            var cell = Sheet.CreateAndGetCell(row, col);
            cell.Style.BackColor = _lightDangerColor.ToReoColor();
        }

        private void RestoreNullDanger(int row, int col)
        {
            var cell = Sheet.CreateAndGetCell(row, col);
            cell.Style.BackColor = SolidColor.Transparent;
        }

        #endregion Check

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
            //Sheet.SetRows(rows + 1);
            //var blankRange = Sheet.DefineNamedRange("Blank", rows, 0, 1, Sheet.ColumnCount);
            var blankRange = Sheet.DefineNamedRange("Blank", rows, 0, Sheet.RowCount - rows, Sheet.ColumnCount);
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

            var rgcs = ControlAppearanceStyle.CreateDefaultControlStyle();
            rgcs[ControlAppearanceColors.LeadHeadNormal] = _greyColor.ToReoColor();
            rgcs[ControlAppearanceColors.ColHeadNormalStart] = _greyColor.ToReoColor();
            rgcs[ControlAppearanceColors.ColHeadNormalEnd] = _greyColor.ToReoColor();
            rgcs[ControlAppearanceColors.ColHeadText] = _mainTextColor.ToReoColor();
            rgcs[ControlAppearanceColors.RowHeadNormal] = _greyColor.ToReoColor();
            rgcs[ControlAppearanceColors.RowHeadText] = _mainTextColor.ToReoColor();

            reoGrid.ControlStyle = rgcs;
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
