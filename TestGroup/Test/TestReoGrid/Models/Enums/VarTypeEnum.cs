using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public enum VarTypeEnum
    {
        None = 0,
        /// <summary>
        /// 溶液自身属性变量
        /// </summary>
        Solution = 5,
        /// <summary>
        /// 机器变量
        /// </summary>
        Machine = 10,
    }
}
