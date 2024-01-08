/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-08
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

namespace AIO.UEditor
{
    public interface IAssetProxyEditor
    {
        /// <summary>
        /// 版本号
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 作用域
        /// </summary>
        string Scopes { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 转换配置
        /// </summary>
        /// <param name="config">收集器配置</param>
        void ConvertConfig(AssetCollectRoot config);

        /// <summary>
        /// 创建配置
        /// </summary>
        /// <param name="bundlesDir">导出目标文件夹</param>
        void CreateConfig(string bundlesDir);

        /// <summary>
        /// 构建资源
        /// </summary>
        /// <param name="command">构建命令</param>
        void BuildArt(AssetBuildCommand command);

        /// <summary>
        /// 构建资源
        /// </summary>
        /// <param name="config">构建配置</param>
        void BuildArt(ASBuildConfig config);
    }
}