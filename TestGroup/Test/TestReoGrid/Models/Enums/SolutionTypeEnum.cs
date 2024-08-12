using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models.Enums
{
    public enum SolutionTypeEnum
    {
        /// <summary>
        /// 空
        /// </summary>
        None = 0,
        /// <summary>
        /// 配体
        /// </summary>
        Ligand = 5,
        /// <summary>
        /// 再生液
        /// </summary>
        Regeneration = 10,
        /// <summary>
        /// 分析物
        /// </summary>
        Analyte = 15,
        /// <summary>
        /// 活化剂
        /// </summary>
        Activator = 20,
        /// <summary>
        /// 封闭剂/钝化
        /// </summary>
        Deactivator = 25,
        /// <summary>
        /// 捕获
        /// </summary>
        Capture = 30,



        /// <summary>
        /// 阳参
        /// </summary>
        PositiveReference = 35,
        /// <summary>
        /// 阴参
        /// </summary>
        NegativeReference = 40,



        #region 校准
        /// <summary>
        /// 溶剂校准
        /// </summary>
        SolventCorrection = 45,
        /// <summary>
        /// 系统校准
        /// </summary>
        Calibration = 50,
        /// <summary>
        /// 浓度校准
        /// </summary>
        ConcCalibration = 55,
        #endregion 校准



        #region 操作
        /// <summary>
        /// 冲洗
        /// </summary>
        Wash = 60,
        /// <summary>
        /// 等待
        /// </summary>
        Wait = 65,
        #endregion 操作

        /// <summary>
        /// Desorb
        /// </summary>
        Desorb = 70,
        /// <summary>
        /// Normalize
        /// </summary>
        Normalize = 75,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 90,
        /// <summary>
        /// 缓冲液
        /// </summary>
        Buffer = 99,
    }
}
