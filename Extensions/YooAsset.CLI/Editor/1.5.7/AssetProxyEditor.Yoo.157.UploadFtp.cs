/*|============|*|
|*|Author:     |*| USER
|*|Date:       |*| 2024-01-11
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor.CLI
{
    internal partial class AssetProxyEditor_Yoo_157
    {
        /// <summary>
        /// 上传到Ftp
        /// </summary>
        public static async Task UploadFtpAsync(ASUploadFTPConfig config)
        {
            var localFull = config.RootPath;
            var handle = AHandle.FTP.Create(config.Server, config.Port, config.User, config.Pass,
                config.RemoteRelative);
            await handle.InitAsync();
            Console.WriteLine($"[FTP] 连接成功 : {handle.Absolute}");
            // 在判断目标文件夹是否有清单文件 如果没有则先删除目标文件夹 再上传
            var progress = new AProgressEvent();
            if (EHelper.IsCMD()) progress.OnProgress = Console.WriteLine;
            else
            {
                progress.OnProgress = current =>
                {
                    EditorUtility.DisplayProgressBar("上传进度", current.ToString(), current.Progress / 100f);
                };
                progress.OnComplete = (current) => { EditorUtility.ClearProgressBar(); };
            }

            progress.OnError = (error) =>
            {
                if (EHelper.IsCMD()) Debug.LogError($"上传失败 : {error}");
                else EditorUtility.DisplayDialog("Error", $"上传失败 : {error}", "确定");
            };
            // 在判断目标文件夹是否有清单文件 如果有则对比清单文件的MD5值 如果一致则不上传
            if (await handle.CheckFileAsync("Manifest.json"))
            {
                Console.WriteLine($"[FTP] 远端版本清单存在 : {handle.Absolute}/Manifest.json 开始进行清单对比");
                var remoteMD5 = await handle.GetMD5Async("Manifest.json");
                var manifestPath = Path.Combine(localFull, "Manifest.json");
                var localMD5 = await AHelper.IO.GetFileMD5Async(manifestPath);

                // 如果不一致 则拉取清单文件中的文件进行对比 记录需要上传的文件
                if (localMD5 == remoteMD5)
                {
                    if (EHelper.IsCMD()) Debug.Log("当前远端版本清单对比一致 无需上传!");
                    else EditorUtility.DisplayDialog("消息", "当前远端版本清单对比一致 无需上传!", "确定");
                    return;
                }

                var current = await AHelper.IO.ReadJsonUTF8Async<Dictionary<string, string>>(manifestPath);
                var remote = AHelper.Json.Deserialize<Dictionary<string, string>>(
                    await handle.GetTextAsync("Manifest.json"));
                var tuple = YooAssetBuild.ComparisonManifest(current, remote);

                foreach (var pair in tuple.Item1) // 添加
                {
                    remote[pair.Key] = pair.Value;
                    var source = Path.Combine(localFull, pair.Key);
                    if (File.Exists(source))
                    {
                        Console.WriteLine($"新增文件 : {pair.Key}");
                        await handle.UploadFileAsync(source, pair.Key);
                    }
                    else
                    {
                        if (EHelper.IsCMD()) Debug.LogError($"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源");
                        else EditorUtility.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                        return;
                    }
                }

                foreach (var pair in tuple.Item2) // 删除
                {
                    remote.Remove(pair.Key);
                    if (!await handle.CheckFileAsync(pair.Key)) continue;
                    Console.WriteLine($"删除文件 : {pair.Key}");
                    await handle.DeleteFileAsync(pair.Key);
                }

                // 修改
                foreach (var pair in tuple.Item3)
                {
                    remote[pair.Key] = pair.Value;
                    var source = Path.Combine(localFull, pair.Key);
                    if (File.Exists(source))
                    {
                        Console.WriteLine($"修改文件 : {pair.Key}");
                        await handle.UploadFileAsync(source, pair.Key);
                    }
                    else
                    {
                        if (EHelper.IsCMD()) Debug.LogError($"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源");
                        else EditorUtility.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                        return;
                    }
                }

                // 然后再将需要新增 删除 更新的文件上传到Ftp 上传完成后更新清单文件
                var content = AHelper.Json.Serialize(remote.Sort());
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                await handle.UploadFileAsync(stream, "Manifest.json");
            }
            else
            {
                Console.WriteLine($"[FTP] 远端版本清单不存在 : 准备开始上传资源 : {localFull} -> {handle.Absolute}");
                await handle.UploadDirAsync(localFull, progress);
            }

            if (EHelper.IsCMD()) Debug.Log($"资源上传完成");
            else EditorUtility.DisplayDialog("Info", $"资源上传完成", "确定");
        }
    }
}
#endif