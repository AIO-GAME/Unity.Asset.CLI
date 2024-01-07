using System.IO;
using Newtonsoft.Json.Linq;

#if UNITY_EDITOR
namespace AIO
{
    internal static class Setting
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "AIO.Asset";

        /// <summary>
        /// 版本
        /// </summary>
        public static string Version { get; private set; }

        [UnityEditor.InitializeOnLoadMethod]
        public static void Initialize()
        {
            var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(Setting).Assembly);
            var packageJson = AHelper.IO.ReadJsonUTF8<JObject>(package.resolvedPath + "/package.json");
            Version = packageJson["version"].ToString();
        }
    }
}

#endif