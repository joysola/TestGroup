using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestReoGrid.Models
{
    public class PLBase : ObservableObject
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Create_User { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Create_Time { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string Update_User { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? Update_Time { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }
}
