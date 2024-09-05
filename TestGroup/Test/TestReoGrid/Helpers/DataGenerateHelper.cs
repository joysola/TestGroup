using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;
using TestReoGrid.Models.ReoGrid;
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

                //var nameSP = ReoSoluParamValueHelper.CreateSP(nameof(PL_Exp_Dsgn_Inject.Solution_Name));


                // var ConcSP = ReoSoluParamValueHelper.CreateSP(nameof(PL_Exp_Dsgn_Inject.Concentration));



                // var MWSP = ReoSoluParamValueHelper.CreateSP(nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));


                //var massConcSP = ReoSoluParamValueHelper.CreateSP(nameof(PL_Exp_Dsgn_Inject.Mass_concentration));


                //serialSoluCh.SolutionParamList = [nameSP, ConcSP, MWSP, massConcSP];

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
                    preRows = rawDatas[r - 1].ChannelInfo.RowEnd + 1;
                }
                var ch = rawDatas[r];
                ch.ChannelInfo.RowStart = preRows/* + 1*/; // 从0开始算
                var spCount = ch.SolutionParamList.Count;
                if (spCount == 0)
                {
                    spCount = 1;
                }
                ch.ChannelInfo.RowEnd = ch.ChannelInfo.RowStart + spCount - 1;
                ch.ChannelInfo.ColStart = 0/*1*/;
                ch.ChannelInfo.ColEnd = 0;

                ch.NameKey = $"{ch.ChannelInfo.Channel_No}";

                for (int i = 0; i < ch.SolutionParamList.Count; i++)
                {
                    var prop = ch.SolutionParamList[i];
                    prop.RowStart = ch.ChannelInfo.RowStart + i;
                    prop.RowEnd = prop.RowStart;

                    prop.ColStart = ch.ChannelInfo.ColStart + 1;
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
                    //if (ch.SolutionParamList?.Count > 0)
                    //{
                    //    var chRange = sheet.DefineNamedRange(ch.NameKey, ch.RowStart, ch.ColStart, ch.Rows, ch.Cols);
                    //    serial.ChNamedRanges.Add(chRange);
                    //}
                    ////foreach (var p in ch.SolutionParamList)
                    ////{
                    ////    //serial.PropRows.Add(p);
                    ////    var pRange = sheet.DefineNamedRange(p.NameKey, p.RowStart, p.ColStart, p.Rows, p.Cols);
                    ////    serial.PropNamedRanges.Add(pRange);
                    ////    foreach (var v in p.ParamValues)
                    ////    {
                    ////        //serial.ValCells.Add(v);
                    ////        var vRange = sheet.DefineNamedRange(v.NameKey, v.RowStart, v.ColStart, v.Rows, v.Cols);
                    ////        serial.ValNamedRanges.Add(vRange);
                    ////    }
                    ////}
                }
            }
            return serial;
        }

        public static SerialRange CreateNamedRanges(Worksheet sheet)
        {
            var channelRanges = CreateSerialDatas(sheet.RowCount, sheet.ColumnCount);
            return CreateSerialNamedRanges(channelRanges, sheet);
        }



        public static ParallelRange CreateParallelRange(Worksheet sheet)
        {
            var range = new ParallelRange();
            range.Channels = [.. Channels.DeepMap()];

            var channels = range.Channels;
            var rowMax = sheet.RowCount;
            var colMax = sheet.ColumnCount;
            for (int r = 0; r < channels.Count; r++)
            {
                var ch = channels[r];
                ch.RowStart = r + 1; // 从1开始算,第一行空开
                ch.RowEnd = ch.RowStart;
                ch.ColStart = 0;
                ch.ColEnd = 0;

                ch.NameKey = $"{ch.Channel_No}";

                //result.Add(ch);
            }

            for (int i = 0; i < range.ParallelSolutionParams.Count; i++)
            {
                var prop = range.ParallelSolutionParams[i];
                prop.RowStart = 0;
                prop.RowEnd = prop.RowStart;

                prop.ColStart = i + 1;
                prop.ColEnd = prop.ColStart;

                //prop.NameKey = $"{ch.NameKey}@{prop.ParamName}";
                if (channels.Count == prop.ParamValues.Count)
                {
                    for (int j = 0; j < prop.ParamValues.Count; j++)
                    {
                        var cell = prop.ParamValues[j];
                        //cell.NameKey = $"{prop.NameKey}@{cell.ColStart}"; // ???????
                        cell.RowStart = channels[j].RowStart;
                        cell.RowEnd = channels[j].RowEnd;

                        cell.ColStart = prop.ColStart;
                    }
                }
            }

            return range;
        }







        #endregion Serial
    }
}
