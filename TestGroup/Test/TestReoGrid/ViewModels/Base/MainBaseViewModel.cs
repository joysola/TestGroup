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
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Events;
using HandyControl.Controls;
using CommunityToolkit.Mvvm.Input;

namespace TestReoGrid.ViewModels
{
    public abstract partial class MainBaseViewModel : ObservableRecipient
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



        #region Properties
        [ObservableProperty]
        private OperEnum _operStatus;

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

        #region Commands
        /// <summary>
        /// Load窗口
        /// </summary>
        /// <param name="reoGrid"></param>
        [RelayCommand]
        private void Load(ReoGridControl reoGrid)
        {
            LoadReoGrid(reoGrid);
        }

        [RelayCommand]
        private void Dilute(double dilutionRatio)
        {
            OperStatus = OperEnum.Dilute;
            Dilute(Sheet.SelectionRange, dilutionRatio);
            OperStatus = OperEnum.None;
        }

        #endregion Commands
        /// <summary>
        /// 加载ReoGrid
        /// </summary>
        /// <param name="reoGrid"></param>
        protected virtual void LoadReoGrid(ReoGridControl reoGrid)
        {
            _dangerColor = (Color)Application.Current.Resources["PL_DangerColor"];
            _mainTextColor = (Color)Application.Current.Resources["PL_MainTextColor"];
            _lightDangerColor = (Color)Application.Current.Resources["PL_LightDangerColor"];

            ReoGrid = reoGrid;
            Sheet = reoGrid.CurrentWorksheet;

            InitGridSheet(ReoGrid);
            Freeze(Sheet);
            RegisterEvents(Sheet);
        }
        /// <summary>
        /// 稀释
        /// </summary>
        /// <param name="range"></param>
        /// <param name="dilutionRatio"></param>
        protected abstract void Dilute(RangePosition range, double dilutionRatio);

        #region Init
        /// <summary>
        /// 初始化reoGrid和sheet
        /// </summary>
        /// <param name="reoGrid"></param>
        protected virtual void InitGridSheet(ReoGridControl reoGrid)
        {
            if (reoGrid is not null)
            {
                reoGrid.SheetTabNewButtonVisible = false;
                reoGrid.SheetTabVisible = false;
                //reoGrid.SetSettings(unvell.ReoGrid.WorkbookSettings.View_ShowSheetTabControl, false);

                var sheet = reoGrid.CurrentWorksheet;
                sheet.Name = "PLSheet";
                //DataFormatterManager.Instance.DataFormatters.Add(CellDataFormatFlag.Custom, new DoubleDataFormt());
            }
        }

        /// <summary>
        /// 冻结
        /// </summary>
        protected virtual void Freeze(Worksheet sheet)
        {
            // 冻结到序号1列
            //Sheet.FreezeToCell(0, 1, FreezeArea.Left);
            //Sheet.Unfreeze();
        }

        #endregion Init

        #region Dispose
        protected virtual void ClearGridSheet()
        {
            UnregisterEvents(Sheet);
            Sheet?.Dispose();
            ReoGrid?.Dispose();
            if (_toolTip is not null)
            {
                _toolTip.PlacementTarget = null;
                _toolTip = null;
            }
        }
        #endregion Dispose

        #region Reogrid Events

        protected virtual void RegisterEvents(Worksheet sheet)
        {
            if (sheet is not null)
            {
                sheet.CellDataChanged += Sheet_CellDataChanged;

                sheet.BeforeRangeMove += Sheet_BeforeRangeMove;
                sheet.AfterRangeMove += Sheet_AfterRangeMove;

                sheet.BeforeCopy += Sheet_BeforeCopy;
                //sheet.AfterCopy += Sheet_AfterCopy;

                sheet.BeforePaste += Sheet_BeforePaste;
                sheet.AfterPaste += Sheet_AfterPaste;


                sheet.RangeDataChanged += Sheet_RangeDataChanged;
                sheet.BeforeDeleteCellContent += Sheet_BeforeDeleteCellContent;

                //Sheet.CellMouseMove += Sheet_CellMouseMove;
                sheet.CellMouseEnter += Sheet_CellMouseEnter;
                sheet.CellMouseLeave += Sheet_CellMouseLeave;

                sheet.BeforeCellKeyDown += Sheet_BeforeCellKeyDown;
            }
        }
        protected virtual void UnregisterEvents(Worksheet sheet)
        {
            if (sheet is not null)
            {
                sheet.CellDataChanged -= Sheet_CellDataChanged;

                sheet.BeforeRangeMove -= Sheet_BeforeRangeMove;
                sheet.AfterRangeMove -= Sheet_AfterRangeMove;

                sheet.BeforeCopy -= Sheet_BeforeCopy;
                //sheet.AfterCopy -= Sheet_AfterCopy;

                sheet.BeforePaste -= Sheet_BeforePaste;
                sheet.AfterPaste -= Sheet_AfterPaste;


                sheet.RangeDataChanged -= Sheet_RangeDataChanged;
                sheet.BeforeDeleteCellContent -= Sheet_BeforeDeleteCellContent;

                //Sheet.CellMouseMove += Sheet_CellMouseMove;
                sheet.CellMouseEnter -= Sheet_CellMouseEnter;
                sheet.CellMouseLeave -= Sheet_CellMouseLeave;

                sheet.BeforeCellKeyDown -= Sheet_BeforeCellKeyDown;
            }
        }
        #region Events

        #region Changed
        protected virtual void Sheet_CellDataChanged(object sender, unvell.ReoGrid.Events.CellEventArgs e)
        {
            if (e.Cell is not null && OperStatus is OperEnum.None)
            {
                CellDataChanged(e.Cell);
            }
        }

        protected virtual void Sheet_RangeDataChanged(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            switch (OperStatus)
            {
                case OperEnum.Copy:
                case OperEnum.Delete:
                    Sheet?.IterateCells(e.Range, (row, col, cell) =>
                    {
                        RangeDataChanged(row, col, cell);
                        return true;
                    });

                    if (OperStatus is OperEnum.Delete)
                    {
                        OperStatus = OperEnum.None;
                    }
                    break;
            }
        }

        protected virtual void Sheet_BeforeDeleteCellContent(object sender, unvell.ReoGrid.Events.DeleteCellContentEventArgs e)
        {
            if (OperStatus is OperEnum.None && !e.Cell.IsReadOnly)
            {
                OperStatus = OperEnum.Delete;
            }
        }
        #endregion Changed

        #region Move
        protected virtual void Sheet_BeforeRangeMove(object sender, unvell.ReoGrid.Events.BeforeCopyOrMoveRangeEventArgs e)
        {
            if (OperStatus is OperEnum.None)
            {
                OperStatus = OperEnum.Move;
            }
        }

        protected virtual void Sheet_AfterRangeMove(object sender, unvell.ReoGrid.Events.CopyOrMoveRangeEventArgs e)
        {
            if (OperStatus is OperEnum.Move)
            {
                // from
                Sheet?.IterateCells(e.FromRange, (row, col, cell) =>
                {
                    MoveFromRange(row, col, cell);
                    return true;
                });
                // to
                Sheet?.IterateCells(e.ToRange, (row, col, cell) =>
                {
                    MoveToRange(row, col, cell);
                    return true;
                });

                OperStatus = OperEnum.None;
            }
        }
        #endregion Move

        #region Copy
        protected virtual void Sheet_BeforeCopy(object sender, unvell.ReoGrid.Events.BeforeRangeOperationEventArgs e)
        {
            // 不可copy 范围中含有readonly的格子
            Sheet?.IterateCells(e.Range, (row, col, cell) =>
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
                OperStatus = OperEnum.Copy;
            }
        }

        //protected virtual void Sheet_AfterCopy(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        //{
        //    switch (OperStatus)
        //    {
        //        case OperEnum.Copy:
        //        case OperEnum.Cut:
        //            break;
        //    }
        //}
        #endregion Copy

        #region Paste
        protected virtual void Sheet_BeforePaste(object sender, unvell.ReoGrid.Events.BeforeRangeOperationEventArgs e)
        {
            // 重点，否则after就没有这个区域了
            Sheet?.SelectRange(e.Range);
            // 可以拿到 目标位置的range
            switch (OperStatus)
            {
                case OperEnum.Copy:
                    break;
                case OperEnum.Cut:
                    break;
            }
        }

        protected virtual void Sheet_AfterPaste(object sender, unvell.ReoGrid.Events.RangeEventArgs e)
        {
            Sheet?.IterateCells(e.Range, (row, col, cell) =>
            {
                AfterPasteRange(row, col, cell);
                return true;
            });

            // 没啥用
            OperStatus = OperEnum.None;
        }
        #endregion Paste

        #region Mouse
        protected virtual void Sheet_CellMouseEnter(object sender, unvell.ReoGrid.Events.CellMouseEventArgs e)
        {
            if (_toolTip is not null)
            {
                var cell = Sheet?.GetCell(e.CellPosition);
                if (cell is not null)
                {
                    var (isValid, errorContent) = VaildateCell(cell);
                    if (!isValid)
                    {
                        _toolTip.VerticalOffset = -20; // 调整位置
                        _toolTip.Content = errorContent;
                        _toolTip.IsOpen = true;
                    }
                }
            }
        }

        protected virtual void Sheet_CellMouseLeave(object sender, unvell.ReoGrid.Events.CellMouseEventArgs e)
        {
            if (_toolTip is not null && _toolTip.IsOpen)
            {
                _toolTip.IsOpen = false;
            }
        }
        #endregion Mouse

        #region Key
        /// <summary>
        /// BeforeCellKeyDown方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Sheet_BeforeCellKeyDown(object sender, unvell.ReoGrid.Events.BeforeCellKeyDownEventArgs e)
        {
            ForbidKeys(e);
        }
        #endregion Key

        #endregion Events

        #region Event Methods
        /// <summary>
        /// 单元格变化（edit）
        /// </summary>
        /// <param name="cell"></param>
        protected abstract void CellDataChanged(Cell cell);
        /// <summary>
        /// Range改变（copy/delete）
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="cell"></param>
        protected abstract void RangeDataChanged(int row, int col, Cell cell);

        /// <summary>
        /// 移动源头
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="cell"></param>
        protected abstract void MoveFromRange(int row, int col, Cell cell);
        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="cell"></param>
        protected abstract void MoveToRange(int row, int col, Cell cell);

        /// <summary>
        /// 粘贴range
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="cell"></param>
        protected abstract void AfterPasteRange(int row, int col, Cell cell);

        /// <summary>
        /// 校验单元格
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected abstract (bool isVaild, string errorContent) VaildateCell(Cell cell);


        /// <summary>
        /// 禁止用键
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ForbidKeys(BeforeCellKeyDownEventArgs e)
        {
            if (e is not null)
            {
                switch (e.KeyCode)
                {
                    case KeyCode.Z | KeyCode.Control:
                    case KeyCode.Y | KeyCode.Control:
                        e.IsCancelled = true;
                        break;
                }
            }
            //if (e.KeyCode is (KeyCode.Z | KeyCode.Control) or (KeyCode.Y | KeyCode.Control))
            //{
            //e.IsCancelled = true;
            //}
        }
        #endregion Event Methods

        #endregion Reogrid Events

    }
}
