/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-22
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Threading.Tasks;
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
            /// 是否上传首包配置
            /// </summary>
            public bool UploadFirstPack;

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

                    var handler = AHelper.FTP.UploadDir(handle.URI, handle.User, handle.Pass,
                        DirTreeFiled.GetFullPath());
                    var iEvent = new AProgressEvent
                    {
                        OnProgress = progress =>
                        {
                            if (EditorUtility.DisplayCancelableProgressBar("Upload FTP", progress.ToString(),
                                    progress.Progress / 100f))
                            {
                                handler.Cancel();
                            }
                        },
                        OnError = error =>
                        {
                            Debug.LogException(error);
                            EditorUtility.ClearProgressBar();
                        },
                        OnComplete = e =>
                        {
                            EditorUtility.ClearProgressBar();
                            EditorUtility.DisplayDialog("Upload FTP",
                                e.State == EProgressState.Finish
                                    ? "Upload FTP Complete"
                                    : "Upload FTP Failure", "OK");
                        }
                    };
                    handler.Event = iEvent;
                    handler.Begin();
                    await handler.WaitAsync();
                }
            }
        }
    }
}