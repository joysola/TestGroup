using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models.ReoGrid
{
    public class ParallelRange
    {
        public List<PL_Exp_Channel> Channels { get; set; } = [];
        public ObservableCollection<SolutionParam> ParallelSolutionParams { get; set; } = [];
    }
}
