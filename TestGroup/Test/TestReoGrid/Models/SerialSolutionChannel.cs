using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public partial class SerialSolutionChannel : ObservableObject
    {
        /// <summary>
        /// 记录所在通道
        /// </summary>
        [ObservableProperty]
        private PL_Exp_Channel _channelInfo;

        /// <summary>
        /// serial的参数布置
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SolutionParam> _solutionParamList = new();
    }
}
