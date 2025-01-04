using System;
namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 曲目
    /// </summary>
    [Serializable]
    public class Song
    {
        /// <summary>
        /// 曲目 ID
        /// </summary>
        public int id;
        /// <summary>
        /// 曲名
        /// </summary>
        public string title;
        /// <summary>
        /// 艺术家
        /// </summary>
        public string artist;
        /// <summary>
        /// 曲目分类
        /// </summary>
        public string genre;
        /// <summary>
        /// 曲目 BPM
        /// </summary>
        public int bpm;
        /// <summary>
        /// 曲目所属区域
        /// </summary>
        public string map;
        /// <summary>
        /// 曲目首次出现版本
        /// </summary>
        public int version;
        /// <summary>
        /// 曲目版权信息
        /// </summary>
        public string rights;
        /// <summary>
        /// 是否被禁用，默认值为 false
        /// </summary>
        public bool disabled = false;
        /// <summary>
        /// 难度
        /// </summary>
        public SongDifficulties difficulties;
    }
}
