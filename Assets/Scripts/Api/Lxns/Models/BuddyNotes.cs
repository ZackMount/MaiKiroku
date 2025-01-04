using System;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 仅宴会场曲目，BUDDY 谱面物量
    /// </summary>
    [Serializable]
    public class BuddyNotes
    {
        /// <summary>
        /// 1P 谱面物量
        /// </summary>
        public Notes left;
        /// <summary>
        /// 2P 谱面物量
        /// </summary>
        public Notes right;
    }
}
