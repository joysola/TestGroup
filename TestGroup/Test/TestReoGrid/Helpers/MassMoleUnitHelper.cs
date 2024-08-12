using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models.Constants;
using TestReoGrid.Models.Enums;

namespace TestReoGrid.Helpers
{
    public class MassMoleUnitHelper
    {
        /// <summary>
        /// 进行mole mass的转换
        /// </summary>
        /// <param name="conc"></param>
        /// <param name="molecularWeight"></param>
        /// <param name="unitType"></param>
        /// <returns></returns>
        public static double? GetMoleMassConc(double? conc, int? molecularWeight, UnitTypeEnum? unitType)
        {
            double? result = null;
            if (conc.HasValue && molecularWeight.HasValue && unitType.HasValue)
            {
                switch (unitType)
                {
                    case UnitTypeEnum.Mole: // mole -> mass
                        result = conc.Value * molecularWeight.Value / 1000;
                        break;
                    case UnitTypeEnum.Mass: // mass -> mole
                        result = conc * 1000 / molecularWeight.Value;
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取mole浓度(nM)
        /// </summary>
        /// <param name="conc"></param>
        /// <param name="molecularWeight"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double? GetMoleConc(double? conc, int? molecularWeight, string unit)
        {
            double? result = null;
            if (conc.HasValue && ConcUnitConst.MoleMassUnitTypeDict.ContainsKey(unit))
            {
                var unitType = ConcUnitConst.MoleMassUnitTypeDict[unit];
                decimal? moleConc = null;
                string moleUnit = null;
                switch (unitType)
                {
                    case UnitTypeEnum.Mole:
                        if (ConcUnitConst.MoleConcUnitRateDict.ContainsKey(unit))
                        {
                            //result = conc.Value * Math.Pow(10, 9) * ConcUnitConst.MoleConcUnitRateDict[unit];
                            moleConc = (decimal)conc.Value;
                            moleUnit = unit;
                        }
                        break;
                    case UnitTypeEnum.Mass:
                        if (molecularWeight.HasValue && ConcUnitConst.UnitTransferDict.ContainsKey(unit))
                        {
                            moleUnit = ConcUnitConst.UnitTransferDict[unit];
                            if (ConcUnitConst.MoleConcUnitRateDict.ContainsKey(moleUnit))
                            {
                                moleConc = (decimal)conc.Value * 1000 / molecularWeight.Value;
                            }
                        }
                        break;
                }
                if (moleConc.HasValue && moleUnit is { Length: > 0 })
                {
                    var rate = (decimal)ConcUnitConst.MoleConcUnitRateDict[moleUnit] * (decimal)Math.Pow(10, 9);
                    result = (double)(moleConc * rate);
                }
            }
            return result;
        }
    }
}
