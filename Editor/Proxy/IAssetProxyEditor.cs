/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-08
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="mergeToLatest">合并Latest</param>
        void CreateConfig(string bundlesDir, bool mergeToLatest);

        /// <summary>
        /// 构建资源
        /// </summary>
        /// <param name="command">构建命令</param>
        bool BuildArt(AssetBuildCommand command);

        /// <summary>
        /// 构建资源列表
        /// </summary>
        /// <param name="packageNames">包列表</param>
        /// <param name="command">构建命令</param>
        bool BuildArtList(IEnumerable<string> packageNames, AssetBuildCommand command);
        
        /// <summary>
        /// 上传到GCloud
        /// 生成一份清单文件 记录当前文件夹下的所有文件的MD5值
        /// 在上传的时候会对比清单文件的MD5值 如果一致则不上传
        /// 如果不一致 则拉取清单文件中的文件进行对比 记录需要上传的文件
        /// 然后再将需要上传的文件上传到GCloud 上传完成后更新清单文件
        /// Tips:
        /// 需要本地保留一份原始清单 否则会覆盖远端最新的清单文件 导致无法对比
        /// </summary>
        Task UploadGCloud(ASUploadGCloudConfig config);

        /// <summary>
        /// 上传到Ftp
        /// </summary>
        Task UploadFtp(ASUploadFTPConfig config);
    }
}