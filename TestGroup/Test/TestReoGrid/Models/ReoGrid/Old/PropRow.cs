using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models.ReoGrid.Old
{
    public partial class PropRow //: SolutionParam
    {
        public string NameKey { get; set; }
        public int RowStart { get; set; }
        public int ColStart { get; set; }
        public int ColEnd { get; set; }
        public int Rows => 1;
        public int Cols => ColEnd - ColStart + 1;
        public List<ValueCell> Cells { get; set; } = [];
    }
}
