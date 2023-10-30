/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 离线模式
    /// </summary>
    internal class YAParametersWebGLMode : YAssetParameters
    {
        /// <summary>
        /// 内置资源查询服务接口
        /// </summary>
        public IQueryServices QueryServices { get; set; } = null;

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        public IRemoteServices RemoteServices { get; set; } = null;

        public YAParametersWebGLMode() : base(EPlayMode.WebPlayMode)
        {
            Parameters = GetParameters();
        }

        protected sealed override InitializeParameters GetParameters()
        {
            var initParameters = new WebPlayModeParameters();
            initParameters.DecryptionServices = DecryptionServices;
            initParameters.LoadingMaxTimeSlice = LoadingMaxTimeSlice;
            initParameters.QueryServices = QueryServices;
            initParameters.RemoteServices = RemoteServices;
            return initParameters;
        }
    }
}
#endif
