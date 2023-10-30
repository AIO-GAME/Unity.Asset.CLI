#if SUPPORT_WHOOTHOT
using System.Collections.Generic;

namespace Rol.Game
{
    public class SettingAddress
    {
        internal static PathAddressConfig PathConfig => PathAddressConfig.Instance;

        public static void GetPathAddress(string file, out string address, out string labels, out string group,
            out CacheLevel cacheLevel)
        {
            address = null;
            labels = null;
            group = null;
            cacheLevel = CacheLevel.None;

            file = file.Replace('\\', '/');
            var path = file.Substring(0, file.LastIndexOf('/'));
            var needParent = true;
            var config = PathConfig.GetPathConfigEntry(path, ref needParent);
            if (config is null) return;
            PathUtils.ParseFileFilter(config.fileFilter, out List<string> containList, out List<string> blockList);
            file = config.GetRelativePath(file);
            if (!PathUtils.IsPathPassFilter(file, containList, blockList)) return;
            address = PathUtils.FormatPath(config.addressFormat, file);
            ;
            group = config.groupFormat;
            labels = config.labelFormat;
            cacheLevel = config.cacheLevel;
        }
    }
}
#endif