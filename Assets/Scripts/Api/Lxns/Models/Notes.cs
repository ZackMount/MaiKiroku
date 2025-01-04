using System;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 谱面物量
    /// </summary>
    [Serializable]
    public class Notes
    {
        /// <summary>
        /// 总物量
        /// </summary>
        public int total;
        /// <summary>
        /// TAP 物量
        /// </summary>
        public int tap;
        /// <summary>
        /// 	HOLD 物量
        /// </summary>
        public int hold;
        /// <summary>
        /// 	SLIDE 物量
        /// </summary>
        public int slide;
        /// <summary>
        /// TOUCH 物量
        /// </summary>
        public int touch;
        /// <summary>
        /// BREAK 物量
        /// </summary>
        public int _break;
    }
}
