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

        //private void CreateSerialDatas()
        //{
        //    var serialChs = DataGenerateHelper.GenerateSerialDatas();
        //    var rows = 0;
        //    foreach (var ch in serialChs)
        //    {
        //        if (ch.ChannelInfo.Is_Selected)
        //        {
        //            var rowCount = ch.SolutionParamList.Count;
        //            rows += rowCount;


        //            //var chRange = Sheet.DefineNamedRange(ch.ChannelInfo.Channel_Name,);
        //        }
        //        //else
        //        //{

        //        //}
        //    }
        //    var cell = Sheet.GetCell("A1");
        //}

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
                    ParamType = typeof(string),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var ConcSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Concentration),
                    ParamType = typeof(double),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var MWSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Molecular_Weight),
                    ParamType = typeof(int),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var massConcSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Mass_concentration),
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


        private static List<ChannelRange> CreateSerialDatas(int rowCount, int colCount)
        {
            var result = new List<ChannelRange>();
            List<SerialSolutionChannel> rawDatas = CreateRawDatas();
            var newDatas = CreateSerialDatas(rawDatas);

            var rows = 1;
            var cols = 1;
            var rowMax = rowCount;
            var colMax = colCount;
            for (int r = 0; r < newDatas.Count; r++)
            {
                var preRows = 0;
                if (r > 0)
                {
                    preRows = newDatas[r - 1].RowEnd;
                }
                var ch = newDatas[r];
                ch.RowStart = preRows + 1;
                ch.RowEnd = ch.RowStart + ch.Props.Count - 1;
                ch.ColStart = 1;
                ch.ColEnd = colMax;

                ch.NameKey = $"{ch.ChannelInfo.Channel_No}";

                for (int i = 0; i < ch.Props.Count; i++)
                {
                    var prop = ch.Props[i];
                    prop.RowStart = ch.RowStart + i;
                    prop.ColStart = ch.ColStart + 1;
                    prop.ColEnd = ch.ColEnd;

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


        private static List<NamedRange> CreateNamedRanges(List<ChannelRange> channelRanges, Worksheet sheet)
        {
            List<NamedRange> namedRanges = [];
            if (channelRanges?.Count > 0)
            {
                foreach (var ch in channelRanges)
                {
                    var chRange = sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
                    namedRanges.Add(chRange);
                    foreach (var p in ch.Props)
                    {
                        var pRange = sheet.DefineNamedRange(p.NameKey, p.RowStart, p.ColStart, p.Rows, p.Cols);
                        namedRanges.Add(pRange);
                        foreach (var v in p.Cells)
                        {
                            var vRange = sheet.DefineNamedRange(v.NameKey, v.RowStart, v.ColStart, v.Rows, v.Cols);
                            namedRanges.Add(vRange);
                        }
                    }
                }
            }
            return namedRanges;
        }


        public static List<NamedRange> CreateNamedRanges(Worksheet sheet)
        {
            var channelRanges = CreateSerialDatas(sheet.RowCount, sheet.ColumnCount);
            return CreateNamedRanges(channelRanges, sheet);
        }
        #endregion Serial
    }
}
