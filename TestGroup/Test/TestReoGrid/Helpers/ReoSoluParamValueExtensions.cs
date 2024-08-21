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
        public static SolutionParam FindSoluParam(this IList<SerialSolutionChannel> solutionChannels, int row, int col)
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


        public static SolutionParamValue FindSoluParamValue(this SolutionParam sp, int row, int col)
        {
            SolutionParamValue spv = null;
            if (sp is not null && row > -1 && col > -1)
            {
                spv = sp.ParamValues.FirstOrDefault(x => x.RowStart == row && x.ColStart == col);
            }
            return spv;
        }

        public static bool RemoveSoluParamValue(this IList<SerialSolutionChannel> solutionChannels, int row, int col)
        {
            var result = false;
            var sp = solutionChannels.FindSoluParam(row, col);
            var spv = sp.FindSoluParamValue(row, col);
            if (sp is not null && spv is not null)
            {
                result = sp.ParamValues.Remove(spv);
            }
            return result;
        }

    }
}
