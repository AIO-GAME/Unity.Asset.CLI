namespace AIO
{
    /// <summary>
    ///     资源异常枚举
    /// </summary>
    public enum ASException
    {
        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 无异常 </code>
        /// <c>English</c>
        /// <code> No Exception </code>
        None = 0,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 未知异常 </code>
        /// <c>English</c>
        /// <code> Unknown Exception </code>
        Unknown,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> AssetProxy 为空！请检查实现或输入实例！ </code>
        /// <c>English</c>
        /// <code> AssetProxy is null! Verify implementation or enter an instance! </code>
        AssetProxyIsNull,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code>
        /// ASConfig 为空！请检查实现或输入实例！
        /// 如果您使用的是默认配置，
        /// 请检查资源文件夹中是否存在配置文件
        /// </code>
        /// <c>English</c>
        /// <code>
        /// ASConfig is null! Verify implementation or enter an instance!
        /// If you are using the default configuration, 
        /// please check whether the configuration file exists in the Resources folder!
        /// </code>
        ASConfigIsNull,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> AssetProxy 未初始化！ </code>
        /// <c>English</c>
        /// <code> AssetProxy Not Initialized! </code>
        AssetProxyNotInitialized,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 不支持的资源模式！ </code>
        /// <c>English</c>
        /// <code> No support for resource mode! </code>
        NoSupportEASMode,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 未找到资源收集根节点！ </code>
        /// <c>English</c>
        /// <code> No found asset collect root! </code>
        NoFoundAssetCollectRoot,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 资源包列表为空 </code>
        /// <c>English</c>
        /// <code> No found asset collect root! </code>
        ASConfigPackagesIsNull,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 资源配置远端路径为空 </code>
        /// <c>English</c>
        /// <code> The remote path of resource configuration is empty! </code>
        ASConfigRemoteUrlIsNull,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 资源配置远端路径版本为空 </code>
        /// <c>English</c>
        /// <code> Description Failed to configure the remote path version for resource configuration! </code>
        ASConfigRemoteUrlRemoteVersionRequestFailure,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 资源配置远端路径版本解析Json失败 </code>
        /// <c>English</c>
        /// <code> Description Failed to parse the Json file of the remote path version! </code>
        ASConfigRemoteUrlRemoteVersionParsingJsonFailure,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 配置检查参数异常 </code>
        /// <c>English</c>
        /// <code> Abnormal configuration check parameters! </code>
        ASConfigCheckError,

        /// <summary></summary>
        /// <c>中文</c>
        /// <code> 配置初始化失败 </code>
        /// <c>English</c>
        /// <code> Configuration initialization failed! </code>
        SettingConfigInitializeFailure,
    }
}