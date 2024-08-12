using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestReoGrid.Models;

namespace TestReoGrid.Helpers
{
    public class DataGenerateHelper
    {
        public static List<PL_Exp_Channel> Channels =>
        [
            new PL_Exp_Channel { Channel_No = 0, Channel_Name = "Channel1", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 1, Channel_Name = "Channel2", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 2, Channel_Name = "Channel3", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 3, Channel_Name = "Channel4", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 4, Channel_Name = "Channel5", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 5, Channel_Name = "Channel6", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 6, Channel_Name = "Channel7", Is_Selected = true },
            new PL_Exp_Channel { Channel_No = 7, Channel_Name = "Channel8", Is_Selected = true },
        ];

        /// <summary>
        /// serial 数据生成
        /// </summary>
        /// <returns></returns>
        public static List<SerialSolutionChannel> GenerateSerialDatas()
        {
            List<SerialSolutionChannel> result = [];
            var channels = Channels;
            foreach (var ch in channels)
            {
                SerialSolutionChannel serialSoluCh = new()
                {
                    ChannelInfo = ch,
                };

                var nameSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Solution_Name),
                    ParamType = typeof(string),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var ConcSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Concentration),
                    ParamType = typeof(double),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var MWSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Molecular_Weight),
                    ParamType = typeof(int),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                var massConcSP = new SolutionParam()
                {
                    ParamName = nameof(PL_Exp_Dsgn_Inject.Mass_concentration),
                    ParamType = typeof(double),
                    VarType = VarTypeEnum.Solution,
                    //ValidationKey = , // 验证校验
                };

                serialSoluCh.SolutionParamList = [nameSP, ConcSP, MWSP, massConcSP];

                result.Add(serialSoluCh);
            }
            return result;
        }


    }
}
