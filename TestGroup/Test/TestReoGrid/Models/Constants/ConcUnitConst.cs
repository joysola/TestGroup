using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models.Enums;

namespace TestReoGrid.Models.Constants
{
    public class ConcUnitConst
    {
        /// <summary>
        /// 摩尔浓度
        /// </summary>
        public const string ConcUnit = "ConcUnit";
        /// <summary>
        /// 质量浓度
        /// </summary>
        public const string MassConcUnit = "MassConcUnit";
        /// <summary>
        /// unitType 2 dictkey
        /// </summary>
        public static Dictionary<UnitTypeEnum, string> UnitTypeDictKeyDict { get; } = new()
        {
            [UnitTypeEnum.Mole] = ConcUnit,
            [UnitTypeEnum.Mass] = MassConcUnit,
        };
        #region Conc
        /// <summary>
        /// 纳摩尔
        /// </summary>
        public const string NanoMole = "0001";
        /// <summary>
        /// 微摩尔
        /// </summary>
        public const string MicroMole = "0002";
        /// <summary>
        /// 毫摩尔
        /// </summary>
        public const string MilliMole = "0003";
        /// <summary>
        /// 摩尔
        /// </summary>
        public const string Mole = "0004";
        #endregion Conc

        #region MassConc
        /// <summary>
        /// ng/ml
        /// </summary>
        public const string NanoGram = "0005";

        /// <summary>
        /// μg/ml
        /// </summary>
        public const string MicroGram = "0006";

        /// <summary>
        /// mg/ml
        /// </summary>
        public const string MilliGram = "0007";

        /// <summary>
        /// g/l
        /// </summary>
        public const string GramLiter = "0008";
        #endregion MassConc

        /// <summary>
        /// 摩尔浓度字典
        /// </summary>
        public static Dictionary<string, double> MoleConcUnitRateDict { get; } = new()
        {
            { ConcUnitConst.NanoMole, Math.Pow(10,-9) },
            { ConcUnitConst.MicroMole, Math.Pow(10,-6)},
            { ConcUnitConst.MilliMole, Math.Pow(10,-3) },
            { ConcUnitConst.Mole,  1},
        };
        /// <summary>
        /// 质量浓度字典
        /// </summary>
        public static Dictionary<string, double> MassConcUnitRateDict { get; } = new()
        {
            { ConcUnitConst.NanoGram, Math.Pow(10,-6) },
            { ConcUnitConst.MicroGram, Math.Pow(10,-3)},
            { ConcUnitConst.MilliGram, 1 },
            { ConcUnitConst.GramLiter,  1},
        };

        /// <summary>
        /// 浓度单位类型字典
        /// </summary>
        public static Dictionary<string, UnitTypeEnum> MoleMassUnitTypeDict { get; } = new()
        {
            { ConcUnitConst.NanoMole, UnitTypeEnum.Mole },
            { ConcUnitConst.MicroMole, UnitTypeEnum.Mole},
            { ConcUnitConst.MilliMole, UnitTypeEnum.Mole },
            { ConcUnitConst.Mole, UnitTypeEnum.Mole },

            { ConcUnitConst.NanoGram, UnitTypeEnum.Mass },
            { ConcUnitConst.MicroGram, UnitTypeEnum.Mass},
            { ConcUnitConst.MilliGram, UnitTypeEnum.Mass },
            { ConcUnitConst.GramLiter, UnitTypeEnum.Mass },
        };


        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string> UnitTransferDict { get; } = new()
        {
            { ConcUnitConst.NanoMole, ConcUnitConst.NanoGram },
            { ConcUnitConst.MicroMole, ConcUnitConst.MicroGram},
            { ConcUnitConst.MilliMole, ConcUnitConst.MilliGram },
            { ConcUnitConst.Mole, null }, // ????

            { ConcUnitConst.NanoGram, ConcUnitConst.NanoMole },
            { ConcUnitConst.MicroGram, ConcUnitConst.MicroMole},
            { ConcUnitConst.MilliGram, ConcUnitConst.MilliMole },
            { ConcUnitConst.GramLiter, ConcUnitConst.MilliMole },
        };

        /// <summary>
        /// 根据unitKey获取对应的字典key
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetUnitDictKey(string unit)
        {
            string key = string.Empty;
            if (MoleMassUnitTypeDict.ContainsKey(unit))
            {
                var unitType = MoleMassUnitTypeDict[unit];
                if (UnitTypeDictKeyDict.ContainsKey(unitType))
                {
                    key = UnitTypeDictKeyDict[unitType];
                }
            }
            return key;
        }
    }
}
