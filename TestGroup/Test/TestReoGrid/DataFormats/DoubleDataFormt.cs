using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.ReoGrid;
using unvell.ReoGrid.DataFormat;

namespace TestReoGrid.DataFormats
{
    public class DoubleDataFormt : IDataFormatter
    {
        public FormatCellResult FormatCell(Cell cell)
        {
            double val = cell.GetData<double>();
            return new FormatCellResult("g", val);
        }

        public bool PerformTestFormat()
        {
            return true;
        }
    }
}
