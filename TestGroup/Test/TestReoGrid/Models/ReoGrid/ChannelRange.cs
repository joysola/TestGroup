using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public class ChannelRange : SerialSolutionChannel
    {
        public string NameKey { get; set; }

        public int RowStart { get; set; }
        public int RowEnd { get; set; }

        public int ColStart { get; set; }
        public int ColEnd { get; set; }

        public int Rows => RowEnd == 0 ? 0 : RowEnd - RowStart + 1;
        public int Cols => ColEnd == 0 ? 0 : ColEnd - ColStart + 1;

        public List<PropRow> Props { get; set; } = [];
    }
}
