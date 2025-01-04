using System;


namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 谱面难度
    /// </summary>
    [Serializable]
    public class SongDifficulties
    {
        /// <summary>
        /// 曲目标准谱面难度列表
        /// </summary>
        public SongDifficulty[] standard;
        /// <summary>
        /// 曲目 DX 谱面难度列表
        /// </summary>
        public SongDifficulty[] dx;
    }
}
