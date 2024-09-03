using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;

namespace TestReoGrid.Helpers
{
    public static class ReoSoluParamValueExtensions
    {
        public static SolutionParam FindSerialSoluParam(this IList<SerialSolutionChannel> solutionChannels, int row, int col)
        {
            SolutionParam sp = null;
            if (solutionChannels?.Count > 0 && row > -1 && col > -1)
            {
                var soluCh = solutionChannels.FirstOrDefault(x => row >= x.RowStart && row <= x.RowEnd);
                if (soluCh is not null)
                {
                    sp = soluCh.SolutionParamList.FirstOrDefault(x => x.RowStart == row);
                    //var spv = sp.ParamValues.FirstOrDefault(x => x.RowStart == row && x.ColStart == col);
                }
            }
            return sp;
        }
        public static SolutionParam FindParallelSoluParam(this IList<SolutionParam> sps, int row, int col)
        {
            var sp = sps?.FirstOrDefault(x => x.ColStart == col);
            return sp;
        }
        /// <summary>
        /// 从sp寻找spv
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static SolutionParamValue FindSoluParamValue(this SolutionParam sp, int row, int col)
        {
            SolutionParamValue spv = null;
            if (sp is not null && row > -1 && col > -1)
            {
                spv = sp.ParamValues.FirstOrDefault(x => x.RowStart == row && x.ColStart == col);
            }
            return spv;
        }
        /// <summary>
        /// (Serial)
        /// </summary>
        /// <param name="solutionChannels"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static SolutionParamValue FindSoluParamValue(this IList<SerialSolutionChannel> solutionChannels, int row, int col)
        {
            SolutionParamValue spv = null;
            var sp = FindSerialSoluParam(solutionChannels, row, col);
            if (sp is not null && row > -1 && col > -1)
            {
                spv = sp.ParamValues.FirstOrDefault(x => x.RowStart == row && x.ColStart == col);
            }
            return spv;
        }

        /// <summary>
        /// (Parallel)
        /// </summary>
        /// <param name="sps"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static SolutionParamValue FindSoluParamValue(this IList<SolutionParam> sps, int row, int col)
        {
            SolutionParamValue spv = null;
            if (sps?.Count > 0)
            {
                var sp = sps.FindParallelSoluParam(row, col);
                if (sp is not null)
                {
                    spv = sp.FindSoluParamValue(row, col);
                }
            }
            return spv;
        }

        public static SerialSolutionChannel FindSolutionChannel(this IList<SerialSolutionChannel> solutionChannels, int row, int col)
        {
            SerialSolutionChannel soluCh = null;
            soluCh = solutionChannels.FirstOrDefault(x => row >= x.RowStart && row <= x.RowEnd);
            return soluCh;
        }

        /// <summary>
        /// Serial
        /// </summary>
        /// <param name="solutionChannels"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool RemoveSerialSoluParamValue(this IList<SerialSolutionChannel> solutionChannels, int row, int col)
        {
            var result = false;
            var sp = solutionChannels.FindSerialSoluParam(row, col);
            var spv = sp.FindSoluParamValue(row, col);
            if (sp is not null && spv is not null)
            {
                result = sp.ParamValues.Remove(spv);
            }
            return result;
        }
        /// <summary>
        /// Parallel
        /// </summary>
        /// <param name="sps"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool RemoveParallelSoluParamValue(this IList<SolutionParam> sps, int row, int col)
        {
            var result = false;
            var sp = sps.FindParallelSoluParam(row, col);
            var spv = sp?.FindSoluParamValue(row, col);
            if (sp is not null && spv is not null)
            {
                result = sp.ParamValues.Remove(spv);
            }
            return result;
        }

    }
}
