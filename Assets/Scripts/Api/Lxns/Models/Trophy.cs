using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 称号收藏品
    /// </summary>
    [Serializable]
    public class Trophy
    {
        /// <summary>
        /// 收藏品 ID
        /// </summary>
        public int id;
        /// <summary>
        /// 收藏品名称
        /// </summary>
        public string name;
        /// <summary>
        /// 称号颜色
        /// </summary>
        public string color;
    }
}
