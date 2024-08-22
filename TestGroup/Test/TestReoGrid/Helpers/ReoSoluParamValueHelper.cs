using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;

namespace TestReoGrid.Helpers
{
    public class ReoSoluParamValueHelper
    {
        public static SolutionParam CreateSP(string key)
        {
            SolutionParam sp = null;
            switch (key)
            {
                case nameof(PL_Exp_Dsgn_Inject.Solution_Name):
                    sp = new SolutionParam
                    {
                        ParamName = key,
                        ParamAlias = "Name",
                        ParamType = typeof(string),
                        VarType = VarTypeEnum.Solution,
                    };
                    break;
                case nameof(PL_Exp_Dsgn_Inject.Concentration):
                    sp = new SolutionParam
                    {
                        ParamName = key,
                        ParamAlias = "Conc.",
                        ParamType = typeof(double),
                        VarType = VarTypeEnum.Solution,
                    };
                    break;
                case nameof(PL_Exp_Dsgn_Inject.Molecular_Weight):
                    sp = new SolutionParam
                    {
                        ParamName = key,
                        ParamAlias = "MW",
                        ParamType = typeof(int),
                        VarType = VarTypeEnum.Solution,
                    };
                    break;
                case nameof(PL_Exp_Dsgn_Inject.Mass_concentration):
                    sp = new SolutionParam
                    {
                        ParamName = key,
                        ParamAlias = "Mass Conc.",
                        ParamType = typeof(double),
                        VarType = VarTypeEnum.Solution,
                    };
                    break;
                case nameof(PL_Exp_Dsgn_Inject.Info):
                    sp = new SolutionParam
                    {
                        ParamName = key,
                        ParamAlias = "Info.",
                        ParamType = typeof(string),
                        VarType = VarTypeEnum.Solution,
                    };
                    break;
            }
            return sp;
        }

        public static int GetDecimalPlaces(string key)
        {
            int places = -1;
            switch (key)
            {
                case nameof(PL_Exp_Dsgn_Inject.Concentration):
                    places = 6;
                    break;
                case nameof(PL_Exp_Dsgn_Inject.Molecular_Weight):
                    places = 0;
                    break;
                case nameof(PL_Exp_Dsgn_Inject.Mass_concentration):
                    places = 6;
                    break;
            }
            return places;
        }

    }
}
