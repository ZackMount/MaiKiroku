using Assets.Scripts.Models;
using System;


namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 谱面难度
    /// </summary>
    [Serializable]
    public class SongDifficulty
    {
        /// <summary>
        /// 谱面类型
        /// </summary>
        public SongType type;
        /// <summary>
        /// 难度
        /// </summary>
        public LevelIndex difficulty;
        /// <summary>
        /// 难度标级
        /// </summary>
        public string level;
        /// <summary>
        /// 谱面定数
        /// </summary>
        public float level_value;
        /// <summary>
        /// 谱师
        /// </summary>
        public string note_designer;
        /// <summary>
        /// 谱面首次出现版本
        /// </summary>
        public int version;
        /// <summary>
        /// 谱面物量
        /// </summary>
        public Notes notes;
    }
}
