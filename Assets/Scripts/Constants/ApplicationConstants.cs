using System;
using System.IO;

namespace Assets.Scripts.Constants
{
    public static class ApplicationConstants
    {
        public static string baseMenu = Path.Combine(Environment.CurrentDirectory, "resources");
        public const string version = "1.0.0";
    }
}
