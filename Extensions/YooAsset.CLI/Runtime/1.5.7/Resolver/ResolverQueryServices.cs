#if SUPPORT_YOOASSET
using System.IO;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    ///     内置文件查询服务类
    /// </summary>
    public class ResolverQueryServices : IBuildinQueryServices
    {
        #region IBuildinQueryServices Members

        public bool QueryStreamingAssets(string packageName, string fileName)
        {
#if UNITY_WEBGL
            var path = Path.Combine(AssetSystem.SandboxRootDirectory, packageName, fileName);
#else
            var path = Path.Combine(AssetSystem.BuildInRootDirectory, packageName, fileName);
#endif
            return File.Exists(path);
        }

        #endregion
    }
}
#endif