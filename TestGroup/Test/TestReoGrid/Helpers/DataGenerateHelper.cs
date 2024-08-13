using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;
using unvell.ReoGrid;

namespace TestReoGrid.Helpers
{
    public class DataGenerateHelper
    {
        public static List<PL_Exp_Channel> Channels =>
        [
            new PL_Exp_Channel { Channel_No = 0, Channel_Name = "Channel1", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 1, Channel_Name = "Channel2", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 2, Channel_Name = "Channel3", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 3, Channel_Name = "Channel4", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 4, Channel_Name = "Channel5", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 5, Channel_Name = "Channel6", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 6, Channel_Name = "Channel7", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 7, Channel_Name = "Channel8", Is_Selected = true },
        ];


        #region Serial
        /// <summary>
        /// serial 数据生成
        /// </summary>
        /// <returns></returns>
        private static List<SerialSolutionChannel> CreateRawDatas()
        {
            List<SerialSolutionChannel> result = [];
            var channels = Channels;
            foreach (var ch in channels)
            {
                SerialSolutionChannel serialSoluCh = new()
                {
                    ChannelInfo = ch,
                };

                var nameSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Solution_Name),
                    ParamAlias = "Name",
                    ParamType = typeof(string),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var ConcSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Concentration),
                    ParamAlias = "Conc.",
                    ParamType = typeof(double),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var MWSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Molecular_Weight),
                    ParamAlias = "MW",
                    ParamType = typeof(int),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var massConcSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Mass_concentration),
                    ParamAlias = "Mass Conc.",
                    ParamType = typeof(double),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                serialSoluCh.SolutionParamList = [nameSP, ConcSP, MWSP, massConcSP];

                result.Add(serialSoluCh);
            }
            return result;
        }

        private static List<ChannelRange> CreateSerialDatas(IList<SerialSolutionChannel> rawDatas)
        {
            var result = new List<ChannelRange>();
            var map = new Mapper();
            foreach (var oldCh in rawDatas)
            {
                var ch = map.Map<ChannelRange>(oldCh);
                foreach (var oldProp in oldCh.SolutionParamList)
                {
                    var prop = map.Map<PropRow>(oldProp);
                    ch.Props.Add(prop);
                    foreach (var oldVal in oldProp.ParamValues)
                    {
                        var val = map.Map<ValueCell>(oldVal);
                        prop.Cells.Add(val);
                    }
                }
                result.Add(ch);
            }
            return result;
        }

        /// <summary>
        /// !!!
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        private static List<ChannelRange> CreateSerialDatas(int rowCount, int colCount)
        {
            var result = new List<ChannelRange>();
            List<SerialSolutionChannel> rawDatas = CreateRawDatas();
            var newDatas = CreateSerialDatas(rawDatas);

            var rowMax = rowCount;
            var colMax = colCount;
            for (int r = 0; r < newDatas.Count; r++)
            {
                var preRows = 0;
                if (r > 0)
                {
                    preRows = newDatas[r - 1].RowEnd + 1;
                }
                var ch = newDatas[r];
                ch.RowStart = preRows/* + 1*/; // 从0开始算
                ch.RowEnd = ch.RowStart + ch.Props.Count - 1;
                ch.ColStart = 0/*1*/;
                ch.ColEnd = 0;

                ch.NameKey = $"{ch.ChannelInfo.Channel_No}";

                for (int i = 0; i < ch.Props.Count; i++)
                {
                    var prop = ch.Props[i];
                    prop.RowStart = ch.RowStart + i;
                    prop.ColStart = ch.ColStart + 1;
                    prop.ColEnd = prop.ColStart;//ch.ColEnd;

                    prop.NameKey = $"{ch.NameKey}@{prop.ParamName}";
                    for (int j = 0; j < prop.Cells.Count; j++)
                    {
                        var cell = prop.Cells[j];
                        cell.NameKey = $"{prop.NameKey}@{cell.ColStart}"; // ???????
                        cell.RowStart = prop.RowStart;
                        cell.ColStart = prop.ColStart + 1 + j;
                    }
                }
                result.Add(ch);
            }
            return result;
        }


        private static SerialRange CreateSerialNamedRanges(List<ChannelRange> channelRanges, Worksheet sheet)
        {
            SerialRange serial = new();
            if (channelRanges?.Count > 0)
            {
                foreach (var ch in channelRanges)
                {
                    serial.ChRanges.Add(ch);
                    var chRange = sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
                    serial.ChNamedRanges.Add(chRange);

                    foreach (var p in ch.Props)
                    {
                        serial.PropRows.Add(p);
                        var pRange = sheet.DefineNamedRange(p.NameKey, p.RowStart, p.ColStart, p.Rows, p.Cols);
                        serial.PropNamedRanges.Add(pRange);
                        foreach (var v in p.Cells)
                        {
                            serial.ValCells.Add(v);
                            var vRange = sheet.DefineNamedRange(v.NameKey, v.RowStart, v.ColStart, v.Rows, v.Cols);
                            serial.ValNamedRanges.Add(vRange);
                        }
                    }
                }
            }
            return serial;
        }


        public static SerialRange CreateNamedRanges(Worksheet sheet)
        {
            var channelRanges = CreateSerialDatas(sheet.RowCount, sheet.ColumnCount);
            return CreateSerialNamedRanges(channelRanges, sheet);
        }


        public static void InitSerial(SerialRange serial, Worksheet sheet)
        {
            // 设置列宽（前两列需要加宽）
            sheet.SetColumnsWidth(0, 1, 120);
            sheet.SetColumnsWidth(1, 1, 100);

            // 填充部分数据
            for (int i = 0; i < serial.ChNamedRanges.Count; i++)
            {
                // 1-1 channel的范围1格
                var chR = serial.ChNamedRanges[i];
                var chD = serial.ChRanges[i];

                chR.Style.HorizontalAlign = ReoGridHorAlign.Center;
                chR.Style.VerticalAlign = ReoGridVerAlign.Middle;
                chR.IsReadonly = true;
                // 合并成1列
                sheet.MergeRange(chR);
                chR.Data = new[] { $"channel{serial.ChRanges[i].ChannelInfo.Channel_No + 1}" };

                for (int j = 0; j < serial.PropNamedRanges.Count; j++)
                {
                    // 1-2. Prop的范围1格
                    var propR = serial.PropNamedRanges[j];
                    propR.Data = $"{serial.PropRows[j].ParamAlias}";
                    propR.IsReadonly = true;

                    // 1-3. val的范围1格
                    for (int k = 0; k < serial.ValNamedRanges.Count; k++)
                    {
                        var valR = serial.ValNamedRanges[k];
                        var valD = serial.ValCells[k];

                        valR.Data = new[] { valD.ParamValue };
                    }
                }
            }
        }

        #endregion Serial
    }
}
