#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO.UEditor.CLI
{
    internal partial class AssetProxyEditor_Yoo_157
    {
        private static async Task<bool> UploadFtpAsync(IEnumerable<AsUploadFtpParameter> parameters)
        {
            foreach (var parameter in parameters)
                if (!await UploadFtpAsync(parameter))
                    return false;

            return true;
        }

        /// <summary>
        ///     上传到Ftp
        /// </summary>
        private static async Task<bool> UploadFtpAsync(AsUploadFtpParameter parameter)
        {
            var localFull = parameter.RootPath;
            var handle = AHandle.FTP.Create(parameter.Server, parameter.Port, parameter.User, parameter.Pass,
                                            parameter.RemoteRelative);
            await handle.InitAsync();
            EHelper.DisplayProgressBar("上传进度", $"开始上传资源 {handle.Absolute}", 0.1f);

            bool succeed;
            // 在判断目标文件夹是否有清单文件 如果有则对比清单文件的MD5值 如果一致则不上传
            if (await handle.CheckFileAsync("Manifest.json"))
            {
                EHelper.DisplayProgressBar("上传", $"[FTP] 远端版本清单存在 : {handle.Absolute}/Manifest.json 开始进行清单对比", 0.2f);
                var remoteMD5 = await handle.GetMD5Async("Manifest.json");
                var manifestPath = Path.Combine(localFull, "Manifest.json");
                var localMD5 = await AHelper.IO.GetFileMD5Async(manifestPath);

                // 如果不一致 则拉取清单文件中的文件进行对比 记录需要上传的文件
                if (localMD5 == remoteMD5)
                {
                    EHelper.DisplayDialog("消息", "当前远端版本清单对比一致 无需上传!", "确定");
                    return true;
                }

                var current = await AHelper.IO.ReadJsonUTF8Async<Dictionary<string, string>>(manifestPath);
                var manifest = await handle.GetTextAsync("Manifest.json");
                var remote = AHelper.Json.Deserialize<Dictionary<string, string>>(manifest);
                var tuple = YooAssetBuild.ComparisonManifest(current, remote);

                foreach (var pair in tuple.Item1) // 添加
                {
                    remote[pair.Key] = pair.Value;
                    var source = Path.Combine(localFull, pair.Key);
                    if (File.Exists(source))
                    {
                        EHelper.DisplayProgressBar("新增任务", $"新增远端文件 : {pair.Key}", 0.4f);
                        await handle.UploadFileAsync(source, pair.Key);
                    }
                    else
                    {
                        EHelper.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                        return false;
                    }
                }

                foreach (var pair in tuple.Item2) // 删除
                {
                    remote.Remove(pair.Key);
                    if (!await handle.CheckFileAsync(pair.Key)) continue;
                    EHelper.DisplayProgressBar("删除任务", $"删除远端文件 : {pair.Key}", 0.6f);
                    await handle.DeleteFileAsync(pair.Key);
                }

                // 修改
                foreach (var pair in tuple.Item3)
                {
                    remote[pair.Key] = pair.Value;
                    var source = Path.Combine(localFull, pair.Key);
                    if (File.Exists(source))
                    {
                        EHelper.DisplayProgressBar("新增任务", $"修改远端文件 : {pair.Key}", 0.8f);
                        await handle.UploadFileAsync(source, pair.Key);
                    }
                    else
                    {
                        EHelper.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                        return false;
                    }
                }

                // 然后再将需要新增 删除 更新的文件上传到Ftp 上传完成后更新清单文件
                var content = AHelper.Json.Serialize(remote.Sort());
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                succeed = await handle.UploadFileAsync(stream, "Manifest.json");
            }
            else
            {
                // 在判断目标文件夹是否有清单文件 如果没有则先删除目标文件夹 再上传
                var progress = new AProgressEvent
                {
                    // OnComplete = report => { EHelper.DisplayDialog("结束", report.ToString(), "确定"); },
                    OnError = error => { EHelper.DisplayDialog("Error", $"上传失败 : {error}", "确定"); },
                    OnProgress = current =>
                    {
                        EHelper.DisplayProgressBar("上传进度", current.ToString(), current.Progress / 100f);
                    }
                };
                EHelper.DisplayProgressBar("上传", $"[FTP] 远端版本清单不存在 : 准备开始上传资源 : {localFull} -> {handle.Absolute}",
                                           0.2f);
                succeed = await handle.UploadDirAsync(localFull, progress);
            }

            if (succeed) // 上传成功后更新版本清单
            {
                var versionPath = $"{handle.URI}/Version/{parameter.BuildTarget.ToString()}.json";
                EHelper.DisplayProgressBar("[FTP] 上传进度", $"更新版本 {versionPath}", 0.9f);
                string versionContent;
                if (await handle.CheckFileAsync(versionPath))
                {
                    List<AssetsPackageConfig> data;
                    try
                    {
                        data = AHelper.Json.Deserialize<List<AssetsPackageConfig>>(
                         await handle.GetTextAsync(versionPath));
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
                succeed = await handle.UploadFileAsync(versionPath, versionTemp);
                AHelper.IO.DeleteFile(versionTemp);
            }

            return succeed;
        }
    }
}
#endif