﻿/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-11-15
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

#if SUPPORT_YOOASSET

using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 内置文件远程服务类
    /// </summary>
    public class ResolverRemoteServices : IRemoteServices
    {
        private AssetsPackageConfig Config { get; set; }

        public ResolverRemoteServices(AssetsPackageConfig config)
        {
            Config = config;
        }

        public string GetRemoteMainURL(string fileName)
        {
            return AssetSystem.Parameter.GetRemoteURL(fileName, Config.Name, Config.Version);
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return AssetSystem.Parameter.GetRemoteURL(fileName, Config.Name, Config.Version);
        }
    }
}
#endif