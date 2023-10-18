using System.Collections.Generic;

namespace AIO.UEngine
{
    /// <summary>
    /// 包列表清单
    /// </summary>
    public class AssetPakList
    {
        public List<AssetsPackageConfig> Packages { get; }

        public AssetPakList()
        {
            Packages = new List<AssetsPackageConfig>();
        }
    }
}