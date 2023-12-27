/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-22
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections;
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
    public partial class ASBuildConfig
    {
        /// <summary>
        /// FTP上传配置
        /// </summary>
        public FTPConfig[] FTPConfigs;

        public void AddOrNewFTP()
        {
            FTPConfigs = FTPConfigs is null
                ? new FTPConfig[] { new FTPConfig() }
                : FTPConfigs.Add(new FTPConfig());
        }

        [Serializable]
        public class FTPConfig
        {
            /// <summary>
            /// 是否显示
            /// </summary>
            public bool Folded;

            /// <summary>
            /// FTP 服务器名称
            /// </summary>
            public string Name;

            /// <summary>
            /// FTP 服务器描述
            /// </summary>
            public string Description;

            /// <summary>
            /// FTP 服务器地址
            /// </summary>
            public string Server;

            /// <summary>
            /// FTP 服务器端口
            /// </summary>
            public int Port = 21;

            /// <summary>
            /// FTP 用户名
            /// </summary>
            public string User;

            /// <summary>
            /// FTP 密码
            /// </summary>
            public string Pass;

            /// <summary>
            /// FTP 远程路径
            /// </summary>
            public string RemotePath;

            /// <summary>
            /// 上传首包配置
            /// </summary>
            public async Task UploadFirstPack(string target)
            {
                var versionDir = Path.Combine(Server, RemotePath, "Version");
                if (!await AHelper.FTP.CheckDirAsync(versionDir, User, Pass))
                    await AHelper.FTP.CreateDirAsync(versionDir, User, Pass);

                var remote = AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Path.Combine(Server, RemotePath));
                var op = AHelper.FTP.UploadFile(remote, User, Pass, target);
                op.Begin();
                await op.WaitAsync();
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

                var versionDir = Path.Combine(Server, RemotePath, "Version");
                if (!await AHelper.FTP.CheckDirAsync(versionDir, User, Pass))
                    await AHelper.FTP.CreateDirAsync(versionDir, User, Pass);

                var version = Path.Combine(Server, RemotePath, "Version", string.Concat(one, ".json"));
                string content;
                if (await AHelper.FTP.CheckFileAsync(version, User, Pass))
                {
                    var text = await AHelper.FTP.GetTextAsync(version, User, Pass);
                    var data = AHelper.Json.Deserialize<List<AssetsPackageConfig>>(text);
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

                var bytes = Encoding.UTF8.GetBytes(content);
                var op = AHelper.FTP.UploadFile(version, User, Pass, bytes);
                op.Begin();
                await op.WaitAsync();
            }

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

            public Task<bool> Validate()
            {
                if (string.IsNullOrEmpty(Server)) return Task.FromResult(false);
                var handle = AHandle.FTP.Create(Server, Port, User, Pass, RemotePath);
                return handle.InitAsync();
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(Name) ? Server : Name;
            }

            public IProgressOperation UploadOperation { get; private set; }
            public IProgressInfo UploadProgress { get; private set; }

            public async void Upload()
            {
                if (string.IsNullOrEmpty(Server))
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

                using (var handle = AHandle.FTP.Create(Server, Port, User, Pass,
                           string.Concat(RemotePath, '/', one, '/', two, '/', three)))
                {
                    var result = await handle.InitAsync();
                    if (!result)
                    {
                        EditorUtility.DisplayDialog("Upload FTP", $"FTP 服务器 {handle.URI} 连接失败", "OK");
                        return;
                    }

                    UploadOperation = AHelper.FTP.UploadDir(handle.URI, handle.User, handle.Pass,
                        DirTreeFiled.GetFullPath());
                    isUploading = true;
                    var iEvent = new AProgressEvent
                    {
                        OnProgress = progress => { UploadProgress = progress; },
                        OnError = error =>
                        {
                            isUploading = false;
                            Debug.LogException(error);
                        },
                        OnComplete = e =>
                        {
                            isUploading = false;
                            EditorUtility.DisplayDialog("Upload FTP", e.State == EProgressState.Finish
                                ? $"上传完成 \n{UploadOperation.Report}"
                                : $"上传失败 \n{UploadOperation.Report}", "OK");
                        }
                    };
                    UploadOperation.Event = iEvent;
                    UploadOperation.Begin();
                    await UploadOperation.WaitAsync();
                }
            }
        }
    }
}