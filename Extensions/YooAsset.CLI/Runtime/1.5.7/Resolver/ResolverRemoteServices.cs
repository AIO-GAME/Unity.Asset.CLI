#if SUPPORT_YOOASSET

using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    ///     内置文件远程服务类
    /// </summary>
    public class ResolverRemoteServices : IRemoteServices
    {
        public ResolverRemoteServices(AssetsPackageConfig config)
        {
            Config = config;
        }

        private AssetsPackageConfig Config { get; }

        #region IRemoteServices Members

        public string GetRemoteMainURL(string fileName)
        {
            return AssetSystem.Parameter.GetRemoteURL(fileName, Config.Name, Config.CurrentVersion);
        }

        public string GetRemoteFallbackURL(string fileName)
        {
            return AssetSystem.Parameter.GetRemoteURL(fileName, Config.Name, Config.CurrentVersion);
        }

        #endregion
    }
}
#endif