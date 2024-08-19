using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;
using TestReoGrid.Models.ReoGrid.Old;
using unvell.ReoGrid;

namespace TestReoGrid.Helpers
{
    public class DataGenerateHelper
    {

        private static readonly IMapper _mapper = new Mapper();
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

        #region Old
        //private static List<ChannelRange> CreateSerialDatas(IList<SerialSolutionChannel> rawDatas)
        //{
        //    var result = new List<ChannelRange>();
        //    var map = new Mapper();
        //    foreach (var oldCh in rawDatas)
        //    {
        //        var ch = map.Map<ChannelRange>(oldCh);
        //        foreach (var oldProp in oldCh.SolutionParamList)
        //        {
        //            var prop = map.Map<PropRow>(oldProp);
        //            ch.Props.Add(prop);
        //            foreach (var oldVal in oldProp.ParamValues)
        //            {
        //                var val = map.Map<ValueCell>(oldVal);
        //                prop.Cells.Add(val);
        //            }
        //        }
        //        result.Add(ch);
        //    }
        //    return result;
        //}

        /// <summary>
        /// !!!
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        //private static List<ChannelRange> CreateSerialDatas(int rowCount, int colCount)
        //{
        //    var result = new List<ChannelRange>();
        //    List<SerialSolutionChannel> rawDatas = CreateRawDatas();
        //    var newDatas = CreateSerialDatas(rawDatas);

        //    var rowMax = rowCount;
        //    var colMax = colCount;
        //    for (int r = 0; r < newDatas.Count; r++)
        //    {
        //        var preRows = 0;
        //        if (r > 0)
        //        {
        //            preRows = newDatas[r - 1].RowEnd + 1;
        //        }
        //        var ch = newDatas[r];
        //        ch.RowStart = preRows/* + 1*/; // 从0开始算
        //        ch.RowEnd = ch.RowStart + ch.Props.Count - 1;
        //        ch.ColStart = 0/*1*/;
        //        ch.ColEnd = 0;

        //        ch.NameKey = $"{ch.ChannelInfo.Channel_No}";

        //        for (int i = 0; i < ch.Props.Count; i++)
        //        {
        //            var prop = ch.Props[i];
        //            prop.RowStart = ch.RowStart + i;
        //            prop.ColStart = ch.ColStart + 1;
        //            prop.ColEnd = prop.ColStart;//ch.ColEnd;

        //            prop.NameKey = $"{ch.NameKey}@{prop.ParamName}";
        //            for (int j = 0; j < prop.Cells.Count; j++)
        //            {
        //                var cell = prop.Cells[j];
        //                cell.NameKey = $"{prop.NameKey}@{cell.ColStart}"; // ???????
        //                cell.RowStart = prop.RowStart;
        //                cell.ColStart = prop.ColStart + 1 + j;
        //            }
        //        }
        //        result.Add(ch);
        //    }
        //    return result;
        //}


        //private static SerialRange CreateSerialNamedRanges(List<ChannelRange> channelRanges, Worksheet sheet)
        //{
        //    SerialRange serial = new();
        //    if (channelRanges?.Count > 0)
        //    {
        //        foreach (var ch in channelRanges)
        //        {
        //            serial.ChRanges.Add(ch);
        //            var chRange = sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
        //            serial.ChNamedRanges.Add(chRange);

        //            foreach (var p in ch.Props)
        //            {
        //                serial.PropRows.Add(p);
        //                var pRange = sheet.DefineNamedRange(p.NameKey, p.RowStart, p.ColStart, p.Rows, p.Cols);
        //                serial.PropNamedRanges.Add(pRange);
        //                foreach (var v in p.Cells)
        //                {
        //                    serial.ValCells.Add(v);
        //                    var vRange = sheet.DefineNamedRange(v.NameKey, v.RowStart, v.ColStart, v.Rows, v.Cols);
        //                    serial.ValNamedRanges.Add(vRange);
        //                }
        //            }
        //        }
        //    }
        //    return serial;
        //}


        //public static void InitSerial(SerialRange serial, Worksheet sheet)
        //{
        //    // 1. 设置列宽（前两列需要加宽）
        //    sheet.SetColumnsWidth(0, 1, 120);
        //    sheet.SetColumnsWidth(1, 1, 100);

        //    // 2. 填充部分数据
        //    for (int i = 0; i < serial.ChNamedRanges.Count; i++)
        //    {
        //        // 1-1 channel的范围1格
        //        var chR = serial.ChNamedRanges[i];
        //        var chD = serial.ChRanges[i];

        //        chR.Style.HorizontalAlign = ReoGridHorAlign.Center;
        //        chR.Style.VerticalAlign = ReoGridVerAlign.Middle;
        //        chR.IsReadonly = true;
        //        // 合并成1列
        //        sheet.MergeRange(chR);
        //        chR.Data = new[] { $"channel{serial.ChRanges[i].ChannelInfo.Channel_No + 1}" };

        //        for (int j = 0; j < serial.PropNamedRanges.Count; j++)
        //        {
        //            // 1-2. Prop的范围1格
        //            var propR = serial.PropNamedRanges[j];
        //            propR.Data = $"{serial.PropRows[j].ParamAlias}";
        //            propR.IsReadonly = true;

        //            // 1-3. val的范围1格
        //            for (int k = 0; k < serial.ValNamedRanges.Count; k++)
        //            {
        //                var valR = serial.ValNamedRanges[k];
        //                var valD = serial.ValCells[k];

        //                valR.Data = new[] { valD.ParamValue };
        //            }
        //        }
        //    }

        //    // 3. 调整sheet的最大行数
        //    var rows = serial.ChRanges.Max(x => x.RowEnd) + 1;
        //    sheet.SetRows(rows);
        //}
        #endregion Old

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

       

        private static List<SerialSolutionChannel> CreateSerialDatas(int rowCount, int colCount)
        {

            List<SerialSolutionChannel> rawDatas = CreateRawDatas();
            //var newDatas = CreateSerialDatas(rawDatas);

            var rowMax = rowCount;
            var colMax = colCount;
            for (int r = 0; r < rawDatas.Count; r++)
            {
                var preRows = 0;
                if (r > 0)
                {
                    preRows = rawDatas[r - 1].RowEnd + 1;
                }
                var ch = rawDatas[r];
                ch.RowStart = preRows/* + 1*/; // 从0开始算
                ch.RowEnd = ch.RowStart + ch.SolutionParamList.Count - 1;
                ch.ColStart = 0/*1*/;
                ch.ColEnd = 0;

                ch.NameKey = $"{ch.ChannelInfo.Channel_No}";

                for (int i = 0; i < ch.SolutionParamList.Count; i++)
                {
                    var prop = ch.SolutionParamList[i];
                    prop.RowStart = ch.RowStart + i;
                    prop.ColStart = ch.ColStart + 1;
                    prop.ColEnd = prop.ColStart;//ch.ColEnd;

                    prop.NameKey = $"{ch.NameKey}@{prop.ParamName}";
                    for (int j = 0; j < prop.ParamValues.Count; j++)
                    {
                        var cell = prop.ParamValues[j];
                        cell.NameKey = $"{prop.NameKey}@{cell.ColStart}"; // ???????
                        cell.RowStart = prop.RowStart;
                        cell.ColStart = prop.ColStart + 1 + j;
                    }
                }
                //result.Add(ch);
            }
            return rawDatas;
        }

     

        private static SerialRange CreateSerialNamedRanges(IList<SerialSolutionChannel> solutionChannels, Worksheet sheet)
        {
            SerialRange serial = new();
            if (solutionChannels?.Count > 0)
            {
                foreach (var ch in solutionChannels)
                {
                    serial.SolutionChannels.Add(ch);
                    var chRange = sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
                    serial.ChNamedRanges.Add(chRange);

                    //foreach (var p in ch.SolutionParamList)
                    //{
                    //    //serial.PropRows.Add(p);
                    //    var pRange = sheet.DefineNamedRange(p.NameKey, p.RowStart, p.ColStart, p.Rows, p.Cols);
                    //    serial.PropNamedRanges.Add(pRange);
                    //    foreach (var v in p.ParamValues)
                    //    {
                    //        //serial.ValCells.Add(v);
                    //        var vRange = sheet.DefineNamedRange(v.NameKey, v.RowStart, v.ColStart, v.Rows, v.Cols);
                    //        serial.ValNamedRanges.Add(vRange);
                    //    }
                    //}
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
            // 1. 设置列宽（前两列需要加宽）
            sheet.SetColumnsWidth(0, 1, 120);
            sheet.SetColumnsWidth(1, 1, 100);

            // 2. 填充部分数据
            for (int i = 0; i < serial.ChNamedRanges.Count; i++)
            {
                // 1-1 channel的范围1格
                var chRange = serial.ChNamedRanges[i];
                var chD = serial.SolutionChannels[i];

                chRange.Style.HorizontalAlign = ReoGridHorAlign.Center;
                chRange.Style.VerticalAlign = ReoGridVerAlign.Middle;
                chRange.IsReadonly = true;
                // 合并成1列
                sheet.MergeRange(chRange);
                chRange.Data = new[] { $"channel{serial.SolutionChannels[i].ChannelInfo.Channel_No + 1}" };

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
            var rows = serial.SolutionChannels.Max(x => x.RowEnd) + 1;
            sheet.SetRows(rows);
        }




        public static List<SerialSolutionChannel> GetSerialData(SerialRange serial, Worksheet sheet)
        {
            var result = new List<SerialSolutionChannel>();
            if (serial is not null && sheet is not null)
            {
                foreach (var rCh in serial.SolutionChannels)
                {
                    var newCh = _mapper.Map<SerialSolutionChannel>(rCh);
                    //newCh.SolutionParamList.Clear();
                    //foreach (var rP in rCh.Props)
                    //{
                    //    var sp = _mapper.Map<SolutionParam>(rP);
                    //    foreach (var rC in rP.Cells)
                    //    {
                    //        var spv = _mapper.Map<SolutionParamValue>(rC);
                    //        sp.ParamValues.Add(spv);
                    //    }
                    //    newCh.SolutionParamList.Add(sp);
                    //}
                    result.Add(newCh);
                }
            }
            return result;
        }


        #endregion Serial
    }
}
