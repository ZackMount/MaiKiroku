using System;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// 谱面类型。仅宴会场曲目（曲目 ID 大于 100000）为 utage 类型。
    /// </summary>
    [Serializable]
    public enum SongType
    {
        /// <summary>
        /// 标准谱面
        /// </summary>
        standard,
        /// <summary>
        /// DX谱面
        /// </summary>
        dx,
        /// <summary>
        /// 宴会场谱面
        /// </summary>
        utage
    }
}
