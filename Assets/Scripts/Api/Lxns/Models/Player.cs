using System;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 玩家
    /// </summary>
    [Serializable]
    public class Player
    {
        /// <summary>
        /// 游戏内名称
        /// </summary>
        public string name;
        /// <summary>
        /// 玩家 DX Rating
        /// </summary>
        public int rating;
        /// <summary>
        /// 好友码
        /// </summary>
        public ulong friend_code;
        /// <summary>
        /// 称号
        /// </summary>
        public Trophy trophy;
        /// <summary>
        /// 段位 ID
        /// </summary>
        public int course_rank;
        /// <summary>
        /// 阶级 ID
        /// </summary>
        public int class_rank;
        /// <summary>
        /// 搭档觉醒数
        /// </summary>
        public int star;
        /// <summary>
        /// 头像
        /// </summary>
        public CollectionGenre icon;
        /// <summary>
        /// 姓名框
        /// </summary>
        public CollectionGenre name_plate;
        /// <summary>
        /// 背景
        /// </summary>
        public CollectionGenre frame;
        /// <summary>
        /// 玩家被同步时的 UTC 时间
        /// </summary>
        public string upload_time;
    }
}
