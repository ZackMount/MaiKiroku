using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 收藏品分类
    /// </summary>
    [Serializable]
    public class CollectionGenre
    {
        /// <summary>
        /// 收藏品分类 ID
        /// </summary>
        public int id;
        /// <summary>
        /// 分类标题
        /// </summary>
        public string name;
        /// <summary>
        /// 分类标题（日文）
        /// </summary>
        public string genre;
    }
}
