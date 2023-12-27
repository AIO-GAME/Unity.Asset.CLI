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
using Console = System.Console;

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
                var result = await PrGCloud.Storage.UploadFile(
                    AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(BUCKET_NAME),
                    target,
                    MetaData);
                EditorUtility.DisplayDialog("提示", result.ExitCode == 0
                    ? $"上传成功 {result.StdOut}"
                    : $"上传失败 {result.StdError}", "确定");
            }

            /// <summary>
            /// 更新配置版本
            /// </summary>
            public async Task UploadVersion()
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
                var saveToLocation = $"{EHelper.Path.Temp}/{one}.json";
                var BUCKET_PATH = string.Concat(BUCKET_NAME, '/', "Version/", one, ".json");
                await PrGCloud.Storage.DownloadFile(
                    BUCKET_PATH,
                    saveToLocation);

                if (File.Exists(saveToLocation))
                {
                    var data = AHelper.IO.ReadJson<List<AssetsPackageConfig>>(saveToLocation);
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

                AHelper.IO.WriteText(saveToLocation, content, Encoding.UTF8);
                await PrGCloud.Storage.UploadFile(
                    BUCKET_PATH,
                    saveToLocation, MetaData
                );
                AHelper.IO.DeleteFile(saveToLocation);
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(Name) ? BUCKET_NAME : Name;
            }

            public async void Upload()
            {
                if (string.IsNullOrEmpty(BUCKET_NAME))
                {
                    EditorUtility.DisplayDialog("Upload Google Cloud", "FTP 服务器地址不能为空", "OK");
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
                    EditorUtility.DisplayDialog("Upload Google Cloud", "FTP 上传目标路径无效", "OK");
                    return;
                }

                isUploading = true;
                await PrGCloud.Storage.UploadDir(
                    string.Concat(BUCKET_NAME, '/', one, '/', two, '/', three),
                    DirTreeFiled.GetFullPath(),
                    MetaData
                );

                await UploadVersion();
                EditorUtility.DisplayDialog("提示", "上传成功", "确定");
                isUploading = false;
            }
        }
    }
}