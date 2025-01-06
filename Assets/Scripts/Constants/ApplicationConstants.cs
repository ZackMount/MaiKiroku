using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Constants
{
    public static class ApplicationConstants
    {
#if !UNITY_EDITOR
        public static string BasePath = Path.Combine(Environment.CurrentDirectory, "assets");
#else
        static string relativePath =  Path.Combine(Environment.CurrentDirectory, "build");
        public static string BasePath = Path.Combine(relativePath, "assets");
        public static string TexturesPath = Path.Combine(BasePath, "textures");
        public static string RecordsPath = Path.Combine(BasePath, "records");
        public static string JacketPath = Path.Combine(TexturesPath, "jacket");
#endif
        public const string Token = "KTg-C8gZvm0UlUnRtF7WAo4epwKXn_2E3iCvT6VECEI=";
        public const string Version = "1.0.0";
        public static Dictionary<string, Sprite> jacketSpriteCache = new Dictionary<string, Sprite>();
    }
}
