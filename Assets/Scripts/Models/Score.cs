namespace Assets.Scripts.Models
{
    public class Score
    {
        /// <summary>
        /// 达成率
        /// </summary>
        public string achievements;
        /// <summary>
        /// DX Rating，计算时需要向下取整
        /// </summary>
        public int dx_rating;
        /// <summary>
        /// DX 分数
        /// </summary>
        public int dx_score;
        /// <summary>
        /// 当前谱面总物量
        /// </summary>
        public int notes_total;
        /// <summary>
        /// FULL COMBO 类型
        /// </summary>
        public FCType fc;
        /// <summary>
        /// FULL SYNC 类型
        /// </summary>
        public FSType fs;
        /// <summary>
        /// 曲目 ID
        /// </summary>
        public int song_id;
        /// <summary>
        /// 难度具体定数，如 14.5
        /// </summary>
        public string level_details;
        /// <summary>
        /// 难度
        /// </summary>
        public LevelIndex level_index;
        /// <summary>
        /// 评级类型
        /// </summary>
        public RateType rate;
        /// <summary>
        /// 该成绩在该版本中的排名
        /// </summary>
        public int rank;
        /// <summary>
        /// 曲名
        /// </summary>
        public string song_name;
        /// <summary>
        /// 谱面类型
        /// </summary>
        public SongType type;
        /// <summary>
        /// 封面文件路径
        /// </summary>
        public string cover_path;
    }
}
