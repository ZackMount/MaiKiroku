using System;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 游玩成绩
    /// </summary>
    [Serializable]
    public class Score
    {
        /// <summary>
        /// 曲目 ID
        /// </summary>
        public int id;
        /// <summary>
        /// 曲名
        /// </summary>
        public string song_name;
        /// <summary>
        /// 难度标级，如 14+
        /// </summary>
        public string level;
        /// <summary>
        /// 难度
        /// </summary>
        public int level_index;
        /// <summary>
        /// 达成率
        /// </summary>
        public float achievements;
        /// <summary>
        /// FULL COMBO 类型
        /// </summary>
        public string fc;
        /// <summary>
        /// FULL SYNC 类型
        /// </summary>
        public string fs;
        /// <summary>
        /// DX 分数
        /// </summary>
        public int dx_score;
        /// <summary>
        /// DX Rating，计算时需要向下取整
        /// </summary>
        public float dx_rating;
        /// <summary>
        /// 评级类型
        /// </summary>
        public string rate;
        /// <summary>
        /// 谱面类型
        /// </summary>
        public string type;
        /// <summary>
        /// 游玩的 UTC 时间，精确到分钟
        /// </summary>
        public string play_time;
        /// <summary>
        /// 成绩被同步时的 UTC 时间
        /// </summary>
        public string upload_time;
    }
}
