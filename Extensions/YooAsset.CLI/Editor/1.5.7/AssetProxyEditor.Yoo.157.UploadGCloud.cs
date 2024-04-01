#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;
using Debug = UnityEngine.Debug;

namespace AIO.UEditor.CLI
{
    internal partial class AssetProxyEditor_Yoo_157
    {
        /// <summary>
        ///     上传到GCloud 对比清单文件上传
        /// </summary>
        private static async Task<bool> UploadGCloudExist(AsUploadGCloudParameter parameter)
        {
            var location = parameter.LocalFullPath;
            var remotePath = parameter.RemoteRelative;
            var remoteManifest = string.Concat(remotePath, '/', "Manifest.json");
            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "开始进行清单对比", 0.1f);

            var remoteMD5 = await PrGCloud.GetMD5Async(remoteManifest);
            var manifestPath = string.Concat(location, '/', "Manifest.json");
            var localMD5 = await AHelper.IO.GetFileMD5Async(manifestPath);

            // 如果不一致 则拉取清单文件中的文件进行对比 记录需要上传的文件
            if (localMD5 == remoteMD5)
            {
                EHelper.DisplayDialog("[Google Cloud] 消息", "当前远端版本清单对比一致 无需上传!", "确定");
                return false;
            }

            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "正在进行清单对比", 0.3f);

            var current = await AHelper.IO.ReadJsonUTF8Async<Dictionary<string, string>>(manifestPath);
            var remoteContent = await PrGCloud.ReadTextAsync(remoteManifest);
            var remote = AHelper.Json.Deserialize<Dictionary<string, string>>(remoteContent);
            var tuple = YooAssetBuild.ComparisonManifest(current, remote);
            float total = tuple.Item1.Count;
            var index = 0;
            var tempDir = AHelper.IO.GetTempPath();
            AHelper.IO.CreateDir(tempDir, true);
            foreach (var pair in tuple.Item1) // 添加
            {
                remote[pair.Key] = pair.Value;
                var source = string.Concat(location, '/', pair.Key);
                if (File.Exists(source))
                {
                    File.Copy(source, string.Concat(tempDir, '/', pair.Key), true);
                    EHelper.DisplayProgressBar($"[Google Cloud] 收集新增文件 {++index}/{total}", pair.Key, index / total);
                }
                else
                {
                    Debug.LogError($"目标新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源");
                    return false;
                }
            }

            foreach (var pair in tuple.Item2)
                if (remote.ContainsKey(pair.Key))
                    remote.Remove(pair.Key);

            total = tuple.Item3.Count;
            index = 0;
            foreach (var pair in tuple.Item3) // 修改
            {
                remote[pair.Key] = pair.Value;
                var source = string.Concat(location, '/', pair.Key);
                if (File.Exists(source))
                {
                    File.Copy(source, string.Concat(tempDir, '/', pair.Key), true);
                    tuple.Item2[pair.Key] = pair.Value;
                    EHelper.DisplayProgressBar($"[Google Cloud] 收集修改文件 {++index}/{total}", pair.Key, index / total);
                }
                else
                {
                    Debug.LogError($"目标新增修改不存在 : {source} 目标源结构被篡改 请重新构建资源");
                    return false;
                }
            }

            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "删除远端文件", 0.6f);
            foreach (var pair in tuple.Item2.Keys.ToArray())
                tuple.Item2[pair] = string.Concat(remotePath, '/', pair);
            await PrGCloud.DeleteFileAsync(tuple.Item2.Values); // 删除

            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "上传新增修改文件", 0.7f);
            var succeed = await PrGCloud.UploadDirAsync(remotePath.PathGetLastFloder(), location,
                                                        parameter.MetaDataKey, parameter.MetaDataValue);
            if (!succeed)
            {
                Debug.LogError("上传新增修改文件失败");
                return false;
            }

            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "整理资源清单", 0.82f);
            var content = AHelper.Json.Serialize(remote.Sort());
            var temp = Path.Combine(tempDir, Path.GetRandomFileName());
            succeed = await AHelper.IO.WriteUTF8Async(temp, content);
            if (!succeed)
            {
                Debug.LogError("写入临时文件失败");
                return false;
            }

            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "更新资源清单配置", 0.83f);
            succeed = await PrGCloud.UploadFileAsync(remoteManifest, temp, parameter.MetaDataKey,
                                                     parameter.MetaDataValue);
            if (!succeed)
            {
                Debug.LogError("上传远端资源清单配置失败");
                return false;
            }

            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "清空缓存文件夹", 0.84f);
            await PrPlatform.Folder.Del(tempDir);
            return true;
        }

        private static async Task<bool> UploadGCloudAsync(IEnumerable<AsUploadGCloudParameter> parameters)
        {
            foreach (var parameter in parameters)
                if (!await UploadGCloudAsync(parameter))
                    return false;

            return true;
        }

        /// <summary>
        ///     上传到GCloud
        /// </summary>
        private static async Task<bool> UploadGCloudAsync(AsUploadGCloudParameter parameter)
        {
            var localFull = parameter.LocalFullPath;
            var remotePath = parameter.RemoteRelative;
            var remoteManifest = string.Concat(remotePath, '/', "Manifest.json");
            bool succeed;
            EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "判断是否有清单", 0.1f);

            // 在判断目标文件夹是否有清单文件 如果有则对比清单文件的MD5值 如果一致则不上传
            if (await PrGCloud.ExistsAsync(remoteManifest))
            {
                succeed = await UploadGCloudExist(parameter);
            }
            else
            {
                EHelper.DisplayProgressBar("[Google Cloud] 上传进度", "上传资源", 0.2f);
                succeed = await PrGCloud.UploadDirAsync(remotePath.PathGetLastFloder(), localFull,
                                                        parameter.MetaDataKey, parameter.MetaDataValue);
            }

            if (succeed)
            {
                var versionPath = $"{parameter.RemotePath}/Version/{parameter.BuildTarget.ToString()}.json";
                EHelper.DisplayProgressBar("[Google Cloud] 上传进度", $"更新版本 {versionPath}", 0.9f);

                string versionContent;
                if (await PrGCloud.ExistsAsync(versionPath))
                {
                    List<AssetsPackageConfig> data;
                    try
                    {
                        data = AHelper.Json.Deserialize<List<AssetsPackageConfig>>(
                             await PrGCloud.ReadTextAsync(versionPath));
                    }
                    catch (Exception)
                    {
                        data = new List<AssetsPackageConfig>();
                    }

                    var data2 = data.Find(item => item.Name == parameter.PackageName);
                    if (data2 is null)
                        data.Add(new AssetsPackageConfig
                        {
                            Name    = parameter.PackageName,
                            Version = parameter.IsUploadLatest ? "Latest" : parameter.Version
                        });
                    else data2.Version = parameter.IsUploadLatest ? "Latest" : parameter.Version;

                    versionContent = AHelper.Json.Serialize(data);
                }
                else
                {
                    versionContent = AHelper.Json.Serialize(new[]
                    {
                        new AssetsPackageConfig
                        {
                            Name    = parameter.PackageName,
                            Version = parameter.IsUploadLatest ? "Latest" : parameter.Version
                        }
                    });
                }

                var versionTemp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                await AHelper.IO.WriteUTF8Async(versionTemp, versionContent);
                succeed = await PrGCloud.UploadFileAsync(versionPath, versionTemp,
                                                         parameter.MetaDataKey, parameter.MetaDataValue);
                AHelper.IO.DeleteFile(versionTemp);
            }

            return succeed;
        }
    }
}
#endif