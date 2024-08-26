using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;

namespace TestReoGrid.Helpers
{
    public class AutoCalculateHelper
    {
        public static void AutoCalculateParamValue(int maxLength, IList<SolutionParam> solutionParams, PL_Exp_Dsgn_Inject solutionInject)
        {
            if (maxLength > -1 && solutionParams is not null && solutionInject is not null)
            {
                foreach (var sp in solutionParams)
                {
                    if (sp.ParamName == nameof(PL_Exp_Dsgn_Inject.Flow_Rate) || sp.ParamName == nameof(PL_Exp_Dsgn_Inject.Contact_Time))
                    {
                        var autoParam = solutionParams.FirstOrDefault(x => x.ParamName == nameof(PL_Exp_Dsgn_Inject.Volume));
                        var rateParam = solutionParams.FirstOrDefault(x => x.ParamName == nameof(PL_Exp_Dsgn_Inject.Flow_Rate));
                        var ctParam = solutionParams.FirstOrDefault(x => x.ParamName == nameof(PL_Exp_Dsgn_Inject.Contact_Time));
                        if (autoParam is not null)
                        {
                            for (int i = 0; i < maxLength; i++)
                            {
                                var rate = solutionInject.Flow_Rate; // 流速
                                var ct = solutionInject.Contact_Time; // 接触时间

                                var paramRateStr = rateParam?.ParamValues[i]?.ParamValue;
                                if (paramRateStr is not null && int.TryParse(paramRateStr, out int paramRate))
                                {
                                    rate = paramRate;
                                }

                                var paramCTStr = ctParam?.ParamValues[i]?.ParamValue;
                                if (paramCTStr is not null && int.TryParse(paramCTStr, out int paramCT))
                                {
                                    ct = paramCT;
                                }
                                if ((rateParam is not null && string.IsNullOrWhiteSpace(paramRateStr)) ||
                                    (ctParam is not null && string.IsNullOrWhiteSpace(paramCTStr)))
                                {
                                    autoParam.ParamValues[i].ParamValue = null;
                                }
                                else
                                {
                                    autoParam.ParamValues[i].ParamValue = $"{Math.Round((double)ct / 60 * rate, 0, MidpointRounding.AwayFromZero)}"; // 四舍五入
                                }
                            }
                        }
                    }
                    else if (sp.ParamName == nameof(PL_Exp_Dsgn_Inject.Concentration) || sp.ParamName == nameof(PL_Exp_Dsgn_Inject.Molecular_Weight))
                    {
                        var autoParam = solutionParams.FirstOrDefault(x => x.ParamName == nameof(PL_Exp_Dsgn_Inject.Mass_concentration));
                        var concParam = solutionParams.FirstOrDefault(x => x.ParamName == nameof(PL_Exp_Dsgn_Inject.Concentration));
                        var mwParam = solutionParams.FirstOrDefault(x => x.ParamName == nameof(PL_Exp_Dsgn_Inject.Molecular_Weight));
                        if (autoParam is not null)
                        {
                            for (int i = 0; i < maxLength; i++)
                            {
                                var conc = solutionInject.Concentration;
                                var mw = solutionInject.Molecular_Weight;

                                var paramConcStr = concParam?.ParamValues[i]?.ParamValue;
                                if (paramConcStr is not null && double.TryParse(paramConcStr, out double paramConc))
                                {
                                    conc = paramConc;
                                }

                                var paramMWStr = mwParam?.ParamValues[i]?.ParamValue;
                                if (paramMWStr is not null && int.TryParse(paramMWStr, out int paramMW))
                                {
                                    mw = paramMW;
                                }
                                if ((concParam is not null && string.IsNullOrWhiteSpace(paramConcStr)) ||
                                    (mwParam is not null && string.IsNullOrWhiteSpace(paramMWStr)))
                                {

                                    autoParam.ParamValues[i].ParamValue = null;
                                }
                                else
                                {
                                    //autoParam.ParamValues[i].ParamValue = $"{mw * conc / 1000}";
                                    autoParam.ParamValues[i].ParamValue = $"{MassMoleUnitHelper.GetMoleMassConc(conc, mw, solutionInject?.UnitType)}";
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
