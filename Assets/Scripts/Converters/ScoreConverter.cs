using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Assets.Scripts.Constants;
using Assets.Scripts.Models;

namespace Assets.Scripts.Converters
{
    public static class ScoreConverter
    {
        
        /// <summary>
        /// 将 DiveFish API 的 Score 转换为统一的 Score 类
        /// </summary>
        /// <param name="diveFishScore">DiveFish 的 Score 对象</param>
        /// <returns>统一的 Score 对象</returns>
        public static Score ConvertScoreAsync(Api.DiveFish.Models.Score diveFishScore, int _rank)
        {
            var unifiedScore = new Score
            {
                achievements = diveFishScore.achievements.ToString("F4"),
                dx_score = diveFishScore.dxScore,
                song_id = diveFishScore.song_id,
                level_index = ParseLevelIndex(diveFishScore.level_index),
                song_name = diveFishScore.title,
                type = ParseSongType(diveFishScore.song_id, diveFishScore.type),
                rank = _rank
            };

            unifiedScore.dx_rating = diveFishScore.ra;
            unifiedScore.rate = ParseRateType(diveFishScore.rate);
            unifiedScore.fc = ParseFCType(diveFishScore.fc);
            unifiedScore.fs = ParseFSType(diveFishScore.fs);
            unifiedScore.level_details = diveFishScore.ds.ToString();
            unifiedScore.cover_path = DownloadSongJacketAsync(unifiedScore.song_id);
            return unifiedScore;

        }

        /// <summary>
        /// 将 Lxns API 的 Score 转换为统一的 Score 类
        /// </summary>
        /// <param name="lxnsScore">Lxns 的 Score 对象</param>
        /// <returns>统一的 Score 对象</returns>
        public static Score ConvertScoreAsync(Api.Lxns.Models.Score lxnsScore, int _rank)
        {
            if (lxnsScore == null)
            {
                throw new ArgumentNullException(nameof(lxnsScore));
            }

            var unifiedScore = new Score
            {
                achievements = lxnsScore.achievements.ToString("F4"),
                dx_score = lxnsScore.dx_score,
                song_id = lxnsScore.id,
                level_index = ParseLevelIndex(lxnsScore.level_index),
                song_name = lxnsScore.song_name,
                type = ParseSongType(lxnsScore.id, lxnsScore.type),
                rank = _rank,
                dx_rating = (int)Math.Floor(lxnsScore.dx_rating)
            };

            unifiedScore.rate = ParseRateType(lxnsScore.rate);
            unifiedScore.fc = ParseFCType(lxnsScore.fc);
            unifiedScore.fs = ParseFSType(lxnsScore.fs);

            unifiedScore.level_details = lxnsScore.level;

            unifiedScore.cover_path  = DownloadSongJacketAsync(unifiedScore.song_id);

            return unifiedScore;
            

        }
        #region Helper Methods
        public static string DownloadSongJacketAsync(int songId)
        {
            string url = $"https://assets2.lxns.net/maimai/jacket/{songId}.png";

            string relativePath = Path.Combine("textures", $"{songId}.png");
            string savePath = Path.Combine(ApplicationConstants.baseMenu, relativePath);

            if (File.Exists(savePath))
            {
                return relativePath;
            }

            try
            {
                HttpClient client = new();
                byte[] imageData = client.GetByteArrayAsync(url).Result;
                string directory = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllBytes(savePath, imageData);

                return relativePath;
            }
            catch (HttpRequestException e)
            {
                Logger.Info($"Failed to download cover: {e.Message}");
            }
            catch (IOException e)
            {
                Logger.Info($"Failed to save cover: {e.Message}, use default cover.");
            }

            return Path.Combine("textures", "default.png");
        }
        private static LevelIndex ParseLevelIndex(int levelIndex)
        {
            return levelIndex switch
            {
                0 => LevelIndex.Basic,
                1 => LevelIndex.Advanced,
                2 => LevelIndex.Expert,
                3 => LevelIndex.Master,
                4 => LevelIndex.ReMaster,
                _ => LevelIndex.Basic
            };
        }

        private static SongType ParseSongType(int songId, string type)
        {
            if (songId > 100000)
                return SongType.utage;

            return type.ToLower() switch
            {
                "standard" => SongType.standard,
                "dx" => SongType.dx,
                _ => SongType.standard
            };
        }

        private static RateType ParseRateType(string rate)
        {
            if (Enum.TryParse<RateType>(rate.ToLower(), true, out var result))
                return result;

            return rate.ToLower() switch
            {
                "sssp" => RateType.sssp,
                "ssp" => RateType.ssp,
                "sss+" => RateType.sssp,
                "sss" => RateType.sss,
                "ss+" => RateType.ssp,
                "ss" => RateType.ss,
                "s+" => RateType.sp,
                "s" => RateType.s,
                "aaa" => RateType.aaa,
                "aa" => RateType.aa,
                "a" => RateType.a,
                "bbb" => RateType.bbb,
                "bb" => RateType.bb,
                "b" => RateType.b,
                "c" => RateType.c,
                "d" => RateType.d,
                _ => RateType.d
            };
        }

        private static FCType ParseFCType(string fc)
        {
            if (string.IsNullOrEmpty(fc))
                return FCType.none; 

            return fc.ToLower() switch
            {
                "app" => FCType.app,
                "ap" => FCType.ap,
                "fcp" => FCType.fcp,
                "fc" => FCType.fc,
                _ => FCType.none
            };
        }

        private static FSType ParseFSType(string fs)
        {
            if (string.IsNullOrEmpty(fs))
                return FSType.none;

            return fs.ToLower() switch
            {
                "fsdp" => FSType.fsdp,
                "fsd" => FSType.fsd,
                "fsp" => FSType.fsp,
                "fs" => FSType.fs,
                "sync" => FSType.sync,
                _ => FSType.none
            };
        }

        #endregion
    }
}
