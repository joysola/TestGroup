using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models.ReoGrid.Old
{
    public class ValueCell //: SolutionParamValue
    {
        public string NameKey { get; set; }
        public int RowStart { get; set; }
        public int ColStart { get; set; }
        public int Rows => 1;
        public int Cols => 1;
    }
}
