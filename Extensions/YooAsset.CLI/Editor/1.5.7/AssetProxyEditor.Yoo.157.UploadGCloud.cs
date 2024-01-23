/*|============|*|
|*|Author:     |*| USER
|*|Date:       |*| 2024-01-11
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO.UEditor.CLI
{
    internal partial class AssetProxyEditor_Yoo_157
    {
        /// <summary>
        /// 上传到Ftp
        /// </summary>
        public static async Task UploadGCloudAsync(ASUploadGCloudConfig config)
        {
            EHelper.DisplayProgressBar("上传进度", $"开始上传资源 {config.RemoteRelative}", 0.1f);
            var localFull = config.RootPath;
            var remoteManifest = string.Concat(config.RemoteRelative, '/', "Manifest.json");
            var succeed = true;

            // 在判断目标文件夹是否有清单文件 如果有则对比清单文件的MD5值 如果一致则不上传
            if (await PrGCloud.ExistsAsync(remoteManifest))
            {
                EHelper.DisplayProgressBar("上传进度", $"开始进行清单对比 : {remoteManifest}", 0.2f);

                var remoteMD5 = await PrGCloud.GetMD5Async(remoteManifest);
                var manifestPath = string.Concat(localFull, '/', "Manifest.json");
                var localMD5 = await AHelper.IO.GetFileMD5Async(manifestPath);

                // 如果不一致 则拉取清单文件中的文件进行对比 记录需要上传的文件
                if (localMD5 == remoteMD5)
                {
                    EHelper.DisplayDialog("消息", "当前远端版本清单对比一致 无需上传!", "确定");
                    return;
                }

                EHelper.DisplayProgressBar("消息", "正在进行清单对比", 0.3f);

                var current = await AHelper.IO.ReadJsonUTF8Async<Dictionary<string, string>>(manifestPath);
                var remote = AHelper.Json.Deserialize<Dictionary<string, string>>(
                    await PrGCloud.ReadTextAsync(remoteManifest));
                var tuple = YooAssetBuild.ComparisonManifest(current, remote);
                foreach (var pair in tuple.Item1) // 添加
                {
                    remote[pair.Key] = pair.Value;
                    var source = string.Concat(localFull, '/', pair.Key);
                    if (File.Exists(source))
                    {
                        var target = string.Concat(config.RemoteRelative, '/', pair.Key);
                        EHelper.DisplayProgressBar("新增文件", target, 0.4f);
                        await PrGCloud.UploadFileAsync(target, source, config.MetaDataKey, config.MetaDataValue);
                    }
                    else
                    {
                        EHelper.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                        return;
                    }
                }

                EHelper.DisplayProgressBar("删除任务", "等待任务完成", 0.6f);
                await PrGCloud.DeleteFileAsync(tuple.Item2.Values);

                foreach (var pair in tuple.Item3) // 修改
                {
                    remote[pair.Key] = pair.Value;
                    var source = string.Concat(localFull, '/', pair.Key);
                    if (File.Exists(source))
                    {
                        var target = string.Concat(config.RemoteRelative, '/', pair.Key);
                        EHelper.DisplayProgressBar("修改文件", target, 0.8f);
                        await PrGCloud.UploadFileAsync(target, source, config.MetaDataKey, config.MetaDataValue);
                    }
                    else
                    {
                        EHelper.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                        return;
                    }
                }

                // 然后再将需要新增 删除 更新的文件上传到Ftp 上传完成后更新清单文件
                EHelper.DisplayProgressBar("上传进度", "更新远端资源清单配置", 0.9f);
                var content = AHelper.Json.Serialize(remote.Sort());
                var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                await AHelper.IO.WriteUTF8Async(temp, content);
                await PrGCloud.UploadFileAsync(remoteManifest, temp, config.MetaDataKey, config.MetaDataValue);
                AHelper.IO.DeleteFile(temp);
            }
            else
            {
                EHelper.DisplayProgressBar("上传进度", $"准备开始上传资源 {config.RemoteRelative}", 0.2f);
                succeed = await PrGCloud.UploadDirAsync(config.RemoteRelative.PathGetLastFloder(), localFull,
                    config.MetaDataKey, config.MetaDataValue);
            }

            string VersionContent;
            var Version_Path = string.Concat(config.RemotePath, "/Version/", config.BuildTarget.ToString(), ".json");

            EHelper.DisplayProgressBar("上传进度", $"[GCloud] 更新远端平台版本 : : {Version_Path}", 0.95f);

            if (await PrGCloud.ExistsAsync(Version_Path))
            {
                VersionContent = await PrGCloud.ReadTextAsync(Version_Path);
                var data = AHelper.Json.Deserialize<List<AssetsPackageConfig>>(VersionContent);
                var data2 = data.Find(item => item.Name == config.PackageName);
                if (data2 is null)
                {
                    data.Add(new AssetsPackageConfig
                    {
                        Name = config.PackageName,
                        Version = config.IsUploadLatest ? "Latest" : config.Version
                    });
                }
                else data2.Version = config.Version;

                VersionContent = AHelper.Json.Serialize(data);
            }
            else
            {
                VersionContent = AHelper.Json.Serialize(new[]
                {
                    new AssetsPackageConfig
                    {
                        Name = config.PackageName,
                        Version = config.IsUploadLatest ? "Latest" : config.Version
                    }
                });
            }

            var VersionTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            await AHelper.IO.WriteUTF8Async(VersionTemp, VersionContent);
            await PrGCloud.UploadFileAsync(Version_Path, VersionTemp, config.MetaDataKey, config.MetaDataValue);
            AHelper.IO.DeleteFile(VersionTemp);

            EHelper.DisplayDialog("Info", succeed ? "资源上传完成" : "资源上传失败", "确定");
        }
    }
}
#endif