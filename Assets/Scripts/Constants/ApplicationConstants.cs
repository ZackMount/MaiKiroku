using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Constants
{
    public static class ApplicationConstants
    {
#if UNITY_EDITOR
        private static string _editorPath = Path.Combine(Environment.CurrentDirectory, "build", "assets");
        public static string BasePath = _editorPath;
#else
        public static string BasePath = Path.Combine(Application.persistentDataPath, "assets");
#endif

        public static string TexturesPath = Path.Combine(BasePath, "textures");
        public static string RecordsPath = Path.Combine(BasePath, "records");
        public static string JacketPath = Path.Combine(TexturesPath, "jacket");
        public static string IconPath = Path.Combine(TexturesPath, "icon");
        public static string PlatePath = Path.Combine(TexturesPath, "plate");

        public const string Token = "Fill in your token";
        public const string Version = "1.0.0";

        public static Dictionary<string, Sprite> JacketSpriteCache = new Dictionary<string, Sprite>();
        public static Dictionary<string, Sprite> IconSpriteCache = new Dictionary<string, Sprite>();
        public static Dictionary<string, Sprite> PlateSpriteCache = new Dictionary<string, Sprite>();

        static ApplicationConstants()
        {
            CreateDirectoryIfNeeded(BasePath);
            CreateDirectoryIfNeeded(TexturesPath);
            CreateDirectoryIfNeeded(RecordsPath);
            CreateDirectoryIfNeeded(JacketPath);
            CreateDirectoryIfNeeded(IconPath);
            CreateDirectoryIfNeeded(PlatePath);
        }
        private static void CreateDirectoryIfNeeded(string path)
        {
            if (Directory.Exists(path)) return;

            try
            {
                Directory.CreateDirectory(path);

#if UNITY_ANDROID && !UNITY_EDITOR
                if (Application.platform == RuntimePlatform.Android)
                {
                    using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        var mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection");
                        mediaScanner.CallStatic("scanFile", activity, new[] { path }, null, null);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error($"Directory creation failed: {ex.Message}");
            }
        }
    }
}