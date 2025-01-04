using System;

namespace Assets.Scripts.Api.DiveFish.Models
{
    /// <summary>
    /// 游玩成绩
    /// </summary>
    [Serializable]
    public class Score
    {
        /// <summary>
        /// 达成率
        /// </summary>
        public float achievements;
        /// <summary>
        /// 定数
        /// </summary>
        public float ds;
        /// <summary>
        /// DX 分数
        /// </summary>
        public int dxScore;
        /// <summary>
        /// FULL COMBO 类型
        /// </summary>
        public string fc;
        /// <summary>
        /// FULL SYNC 类型
        /// </summary>
        public string fs;
        /// <summary>
        /// 难度标级，如 14+
        /// </summary>
        public string level;
        /// <summary>
        /// 难度
        /// </summary>
        public int level_index;
        /// <summary>
        /// 难度名称
        /// </summary>
        public string level_label;
        /// <summary>
        /// DX Rating
        /// </summary>
        public int ra;
        /// <summary>
        /// 评级类型
        /// </summary>
        public string rate;
        /// <summary>
        /// 曲目 ID
        /// </summary>
        public int song_id;
        /// <summary>
        /// 曲名
        /// </summary>
        public string title;
        /// <summary>
        /// 谱面类型
        /// </summary>
        public string type;
    }
}
