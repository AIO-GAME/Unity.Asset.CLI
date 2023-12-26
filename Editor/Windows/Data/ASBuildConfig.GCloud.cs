/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-26
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// nameof(ASBuildConfig_GCloud)
    /// </summary>
    public partial class ASBuildConfig
    {
        /// <summary>
        /// FTP上传配置
        /// </summary>
        public GCloudConfig[] GCloudConfigs;

        public void AddOrNewGCloud()
        {
            GCloudConfigs = GCloudConfigs is null
                ? new GCloudConfig[] { new GCloudConfig() }
                : GCloudConfigs.Add(new GCloudConfig());
        }

        [Serializable]
        public class GCloudConfig
        {
            /// <summary>
            /// 是否显示
            /// </summary>
            public bool Folded;

            /// <summary>
            /// 名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 描述
            /// </summary>
            public string Description;

            /// <summary>
            /// 桶名称
            /// </summary>
            public string BUCKET_NAME;

            /// <summary>
            /// 元数据
            /// </summary>
            public string MetaData;

            /// <summary>
            /// 上传状态 : true 正在上传
            /// </summary>
            [NonSerialized] public bool isUploading;

            /// <summary>
            /// 目录
            /// </summary>
            public DirTreeFiled DirTreeFiled = new DirTreeFiled
            {
                OptionShowDepth = false,
                OptionDirDepth = 3,
                OptionSearchPatternFolder = "(?i)^(?!.*\b(Version|OutputCache|Simulate)\b).*$",
            };

            /// <summary>
            /// 上传首包配置
            /// </summary>
            public async Task UploadFirstPack(string target)
            {
                var op = PrGCloud.Storage.UploadFile(
                    AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(BUCKET_NAME),
                    target,
                    MetaData);
                var result = await op.Async();
                if (result.ExitCode == 0)
                {
                    EditorUtility.DisplayDialog("提示", $"上传成功 {result.StdOut}", "确定");
                }
                else EditorUtility.DisplayDialog("提示", $"上传失败 {result.StdError}", "确定");
            }

            /// <summary>
            /// 更新配置版本
            /// </summary>
            public async void UploadVersion()
            {
                var one = DirTreeFiled[0];
                if (string.IsNullOrEmpty(one))
                {
                    EditorUtility.DisplayDialog("提示", "上传目录 平台 不能为空", "确定");
                    return;
                }

                var two = DirTreeFiled[1];
                if (string.IsNullOrEmpty(two))
                {
                    EditorUtility.DisplayDialog("提示", "上传目录 包名 不能为空", "确定");
                    return;
                }

                var three = DirTreeFiled[2];
                if (string.IsNullOrEmpty(three))
                {
                    EditorUtility.DisplayDialog("提示", "上传目录 版本 不能为空", "确定");
                    return;
                }

                string content;
                var saveJson = $"{EHelper.Path.Temp}/{one}.json";
                await PrGCloud.Storage.DownloadFile(
                    BUCKET_NAME,
                    string.Concat("Version/", one, ".json"),
                    saveJson);

                if (File.Exists(saveJson))
                {
                    var data = AHelper.IO.ReadJson<List<AssetsPackageConfig>>(saveJson);
                    Console.WriteLine(AHelper.Json.Serialize(data));
                    var exists = false;
                    foreach (var item in data.Where(item => item.Name == two))
                    {
                        exists = true;
                        item.Version = three;
                        break;
                    }

                    if (!exists)
                    {
                        data.Add(new AssetsPackageConfig
                        {
                            Name = two,
                            Version = three,
                        });
                    }

                    content = AHelper.Json.Serialize(data);
                }
                else
                {
                    content = AHelper.Json.Serialize(new[]
                    {
                        new AssetsPackageConfig
                        {
                            Name = two,
                            Version = three,
                        }
                    });
                }

                Console.WriteLine(saveJson);
                AHelper.IO.WriteText(saveJson, content, Encoding.UTF8);
                await PrGCloud.Storage.UploadFile(
                    string.Concat(BUCKET_NAME, "/Version"),
                    saveJson
                );
                await PrGCloud.Storage.MetadataUpdate(
                    BUCKET_NAME,
                    string.Concat("Version/", one, ".json"),
                    MetaData
                );
                AHelper.IO.DeleteFile(saveJson);
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(Name) ? BUCKET_NAME : Name;
            }

            public async void Upload()
            {
                if (string.IsNullOrEmpty(BUCKET_NAME))
                {
                    EditorUtility.DisplayDialog("Upload FTP", "FTP 服务器地址不能为空", "OK");
                    return;
                }

                var one = DirTreeFiled[0];
                if (string.IsNullOrEmpty(one))
                {
                    EditorUtility.DisplayDialog("提示", "上传目录 平台 不能为空", "确定");
                    return;
                }

                var two = DirTreeFiled[1];
                if (string.IsNullOrEmpty(two))
                {
                    EditorUtility.DisplayDialog("提示", "上传目录 包名 不能为空", "确定");
                    return;
                }

                var three = DirTreeFiled[2];
                if (string.IsNullOrEmpty(three))
                {
                    EditorUtility.DisplayDialog("提示", "上传目录 版本 不能为空", "确定");
                    return;
                }

                if (!DirTreeFiled.IsValidity())
                {
                    EditorUtility.DisplayDialog("Upload FTP", "FTP 上传目标路径无效", "OK");
                    return;
                }

                UploadVersion();

                //
                // using (var handle = AHandle.FTP.Create(BUCKET_NAME, 
                //            string.Concat(RemotePath, '/', one, '/', two, '/', three)))
                // {
                //     var result = await handle.InitAsync();
                //     if (!result)
                //     {
                //         EditorUtility.DisplayDialog("Upload FTP", $"FTP 服务器 {handle.URI} 连接失败", "OK");
                //         return;
                //     }
                //
                //     UploadOperation = AHelper.FTP.UploadDir(handle.URI, handle.User, handle.Pass,
                //         DirTreeFiled.GetFullPath());
                //     isUploading = true;
                //     var iEvent = new AProgressEvent
                //     {
                //         OnProgress = progress => { UploadProgress = progress; },
                //         OnError = error =>
                //         {
                //             isUploading = false;
                //             Debug.LogException(error);
                //         },
                //         OnComplete = e =>
                //         {
                //             isUploading = false;
                //             EditorUtility.DisplayDialog("Upload FTP", e.State == EProgressState.Finish
                //                 ? $"上传完成 \n{UploadOperation.Report}"
                //                 : $"上传失败 \n{UploadOperation.Report}", "OK");
                //         }
                //     };
                //     UploadOperation.Event = iEvent;
                //     UploadOperation.Begin();
                //     await UploadOperation.WaitAsync();
                // }
            }
        }
    }
}