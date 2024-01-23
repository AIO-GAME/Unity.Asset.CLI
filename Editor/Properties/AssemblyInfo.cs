using Newtonsoft.Json.Linq;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

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

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            var package = PackageInfo.FindForAssembly(typeof(Setting).Assembly);
            var packageJson = AHelper.IO.ReadJsonUTF8<JObject>(string.Concat(package.resolvedPath, "/package.json"));
            Version = packageJson.Value<string>("version");
        }
    }
}