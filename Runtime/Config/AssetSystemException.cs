/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-22
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System.ComponentModel;

namespace AIO
{
    /// <summary>
    /// 资源异常枚举
    /// </summary>
    public enum AssetSystemException
    {
        /// <summary>
        /// 无异常
        /// </summary>
        None = 0,

        [Description(@"
[EN:Unknown Exception]
[CN:未知异常]")]
        Unknown,

        [Description(@"
[EN:AssetProxy is null! Verify implementation or enter an instance!]
[CN:AssetProxy 为空！请检查实现或输入实例！]")]
        AssetProxyIsNull,

        [Description(@"
[EN:
ASConfig is null! Verify implementation or enter an instance!
If you are using the default configuration, 
please check whether the configuration file exists in the Resources folder!]
[CN:
ASConfig 为空！请检查实现或输入实例！
如果您使用的是默认配置，
请检查资源文件夹中是否存在配置文件！]")]
        ASConfigIsNull,

        [Description(@"
[EN:AssetProxy is not initialized!]
[CN:AssetProxy 未初始化！]")]
        AssetProxyNotInitialized,

        [Description(@"
[EN:No support for resource mode!]
[CN:不支持的资源模式！]")]
        NoSupportEASMode,


        [Description(@"
[EN:No found asset collect root!]
[CN:未找到资源收集根节点！]")]
        NoFoundAssetCollectRoot,

        [Description(@"
[EN:No found asset collect root!]
[CN:资源包列表为空]")]
        ASConfigPackagesIsNull,

        [Description(@"
[EN:The remote path of resource configuration is empty]
[CN:资源配置远端路径为空]")]
        ASConfigRemoteUrlIsNull,

        [Description(@"
[EN:Description Failed to configure the remote path version for resource configuration]
[CN:资源配置远端路径版本配置请求失败]")]
        ASConfigRemoteUrlRemoteVersionRequestFailure,

        [Description(@"
[EN:Description Failed to parse the Json file of the remote path version]
[CN:资源配置远端路径版本配置解析Json失败]")]
        ASConfigRemoteUrlRemoteVersionParsingJsonFailure,

        [Description(@"
[EN:Abnormal configuration check parameters]
[CN:配置检查参数异常]")]
        ASConfigCheckError,

        [Description(@"
[EN:Configuration initialization failed]
[CN:配置初始化失败]")]
        SettingConfigInitializeFailure,
    }
}