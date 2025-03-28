using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Assets.Scripts.Constants;
using Assets.Scripts.Models;

namespace Assets.Scripts.Converters
{
    public static class PlayerConverter
    {
        
        /// <summary>
        /// 将 Lxns API 的 Score 转换为统一的 Score 类
        /// </summary>
        /// <param name="lxnsScore">Lxns 的 Score 对象</param>
        /// <returns>统一的 Score 对象</returns>
        public static Player ConvertPlayerAsync(Api.Lxns.Models.Player lxnsPlayer)
        {
            if (lxnsPlayer == null)
            {
                throw new ArgumentNullException(nameof(lxnsPlayer));
            }

            var unifiedPlayer = new Player();
            {
                unifiedPlayer.name = lxnsPlayer.name;
                unifiedPlayer.trophy = lxnsPlayer.trophy.name;
                unifiedPlayer.rating = lxnsPlayer.rating;
                unifiedPlayer.class_rank = lxnsPlayer.class_rank;
                unifiedPlayer.course_rank = lxnsPlayer.course_rank;
                unifiedPlayer.trophy_color = lxnsPlayer.trophy.color;
                unifiedPlayer.icon_path = DownloadPlayerIconAsync(lxnsPlayer.icon.id);
            };



            return unifiedPlayer;

        }
        #region Helper Methods
        public static string DownloadPlayerIconAsync(int iconId)
        {
            string url = $"https://assets2.lxns.net/maimai/icon/{iconId}.png";
            string savePath = Path.Combine(ApplicationConstants.IconPath, $"{iconId}.png");

            if (File.Exists(savePath))
            {
                return savePath;
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
                Logger.Info($"Cover {iconId} not exists and downloaded successfully.");
                return savePath;
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
       

        #endregion
    }
}
