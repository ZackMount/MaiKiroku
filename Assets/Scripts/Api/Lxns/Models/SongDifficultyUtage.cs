using System;

namespace Assets.Scripts.Api.Lxns.Models
{
    /// <summary>
    /// 宴会场曲目谱面难度
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SongDifficultyUtage<T>
    {
        /// <summary>
        /// 谱面属性
        /// </summary>
        public string kanji;
        /// <summary>
        /// 谱面描述
        /// </summary>
        public string description;
        /// <summary>
        /// 是否为 BUDDY 谱面
        /// </summary>
        public bool is_buddy;
        /// <summary>
        /// 谱面物量，is_buddy 为 true 时，notes 为 BuddyNotes。
        /// </summary>
        public T notes;
    }
}
