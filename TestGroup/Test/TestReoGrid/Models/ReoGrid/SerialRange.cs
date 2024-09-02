using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.ReoGrid;

namespace TestReoGrid.Models.ReoGrid
{
    public class SerialRange
    {
        public List<NamedRange> ChNamedRanges { get; set; } = [];
        //public List<ChannelRange> ChRanges { get; set; } = [];
        //public List<NamedRange> PropNamedRanges { get; set; } = [];
        //public List<NamedRange> ValNamedRanges { get; set; } = [];


        public List<SerialSolutionChannel> SolutionChannels { get; set; } = [];


        //public List<PropRow> PropRows { get; set; } = [];



        //public List<ValueCell> ValCells { get; set; } = [];
    }
}
