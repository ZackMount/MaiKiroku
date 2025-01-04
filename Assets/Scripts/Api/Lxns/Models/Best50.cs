using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 获取玩家缓存的 Best 50
    /// </summary>
    [Serializable]
    public class Best50
    {
        /// <summary>
        /// 旧版本谱面 Best 35 总分
        /// </summary>
        public int standard_total;
        /// <summary>
        /// 现版本谱面 Best 15 总分
        /// </summary>
        public int dx_total;
        /// <summary>
        /// 旧版本谱面 Best 35 列表
        /// </summary>
        public Score[] standard;
        /// <summary>
        /// 现版本谱面 Best 15 列表
        /// </summary>
        public Score[] dx;
    }
}
