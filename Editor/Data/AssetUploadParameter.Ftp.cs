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
    ///     资源 上传 FTP 配置
    /// </summary>
    public class AssetUploadFtpParameter : AssetUploadParameter
    {
        /// <summary>
        ///     FTP 密码
        /// </summary>
        public string Pass;

        /// <summary>
        ///     FTP 服务器端口
        /// </summary>
        public int Port = 21;

        /// <summary>
        ///     FTP 服务器地址
        /// </summary>
        public string Server;

        /// <summary>
        ///     FTP 用户名
        /// </summary>
        public string User;

        /// <summary>
        ///     更目录
        /// </summary>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        public string RootPath
        {
            get
            {
                if (string.IsNullOrEmpty(LocalFullPath))
                    throw new ArgumentNullException($"{LocalFullPath} is null or empty");

                if (string.IsNullOrEmpty(PackageName))
                    throw new ArgumentNullException($"{PackageName} is null or empty");

                var version = IsUploadLatest ? "Latest" : Version;
                if (string.IsNullOrEmpty(version))
                    throw new ArgumentNullException($"{version} is null or empty");

                var path = Path.Combine(LocalFullPath, BuildTarget.ToString(), PackageName, version);
                if (!Directory.Exists(path)) throw new DirectoryNotFoundException($"{path} is not found");
                return path;
            }
        }
    }

    public partial class AssetBuildConfig
    {
        public void AddOrNewFTP()
        {
            var temp = new FTPConfig
            {
                DirTreeFiled = new DirTreeFiled(BuildOutputPath.Replace('\\', '/'), 3)
                {
                    OptionShowDepth           = false,
                    OptionSearchPatternFolder = "(?i)^(?!.*\b(Version|OutputCache|Simulate)\b).*$"
                }
            };
            FTPConfigs = FTPConfigs is null
                ? new[] { temp }
                : FTPConfigs.Add(temp);
        }

        [Serializable]
        public class FTPConfig
        {
            /// <summary>
            ///     是否显示
            /// </summary>
            public bool Folded;

            /// <summary>
            ///     FTP 服务器名称
            /// </summary>
            public string Name;

            /// <summary>
            ///     FTP 服务器描述
            /// </summary>
            public string Description;

            /// <summary>
            ///     FTP 服务器地址
            /// </summary>
            public string Server;

            /// <summary>
            ///     FTP 服务器端口
            /// </summary>
            public int Port = 21;

            /// <summary>
            ///     FTP 用户名
            /// </summary>
            public string User;

            /// <summary>
            ///     FTP 密码
            /// </summary>
            public string Pass;

            /// <summary>
            ///     FTP 远程路径
            /// </summary>
            public string RemotePath;

            /// <summary>
            ///     目录
            /// </summary>
            public DirTreeFiled DirTreeFiled = new DirTreeFiled()
            {
                OptionShowDepth           = false,
                OptionDirDepth            = 3,
                OptionSearchPatternFolder = "(?i)^(?!.*\b(Version|OutputCache|Simulate)\b).*$"
            };

            /// <summary>
            ///     上传状态 : true 正在上传
            /// </summary>
            [NonSerialized] public bool isUploading;

            public string LocalFullPath => DirTreeFiled.GetFullPath();

            public IProgressOperation UploadOperation { get; private set; }
            public IProgressInfo      UploadProgress  { get; private set; }

            /// <summary>
            ///     是否存在远端首包配置
            /// </summary>
            public Task<bool> IsExistRemoteFirstPack()
            {
                var remote = AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Path.Combine(Server, RemotePath));
                return AHelper.FTP.CheckFileAsync(remote, User, Pass);
            }

            /// <summary>
            ///     上传首包配置
            /// </summary>
            public async Task<bool> UploadFirstPack(string target)
            {
                try
                {
                    var versionDir = Path.Combine(Server, RemotePath, "Version");
                    if (!await AHelper.FTP.CheckDirAsync(versionDir, User, Pass))
                        await AHelper.FTP.CreateDirAsync(versionDir, User, Pass);

                    var remote = AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Path.Combine(Server, RemotePath));
                    var op     = AHelper.FTP.UploadFile(remote, User, Pass, target);
                    op.Begin();
                    await op.WaitAsync();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }

                return true;
            }

            /// <summary>
            ///     合并首包配置
            /// </summary>
            public async Task<bool> MergeFirstPack(string target)
            {
                var remote = AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Path.Combine(Server, RemotePath));
                var status = false;
                try
                {
                    var dic         = new Dictionary<string, AssetSystem.SequenceRecord>();
                    var op          = await AHelper.FTP.GetTextAsync(remote, User, Pass);
                    var remote_data = AHelper.Json.Deserialize<AssetSystem.SequenceRecord[]>(op);
                    if (remote_data != null)
                        foreach (var item in remote_data)
                        {
                            if (item.IsNull) continue;
                            dic[item.GUID] = item;
                        }

                    var local_data = AHelper.IO.ReadJsonUTF8<AssetSystem.SequenceRecord[]>(target);
                    if (local_data != null)
                        foreach (var item in local_data)
                        {
                            if (item.IsNull) continue;
                            dic[item.GUID] = item;
                        }


                    var data = dic.Values.ToList();
                    data.Sort((a, b) => b.Time.CompareTo(a.Time));
                    var content = AHelper.Json.Serialize(data);
                    using (var op2 = AHelper.FTP.UploadFile(remote, User, Pass, Encoding.UTF8.GetBytes(content)))
                    {
                        op2.Begin();
                        await op2.WaitAsync();
                    }

                    status = true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                return status;
            }

            /// <summary>
            ///     更新配置版本
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

                var    version = Path.Combine(Server, RemotePath, "Version", string.Concat(one, ".json"));
                string content;
                if (await AHelper.FTP.CheckFileAsync(version, User, Pass))
                {
                    var text   = await AHelper.FTP.GetTextAsync(version, User, Pass);
                    var data   = AHelper.Json.Deserialize<List<AssetsPackageConfig>>(text);
                    var exists = false;
                    if (data.Exists(item => item.Name == two))
                    {
                        exists                                       = true;
                        data.First(item => item.Name == two).Version = three;
                    }

                    if (!exists)
                        data.Add(new AssetsPackageConfig
                        {
                            Name    = two,
                            Version = three
                        });

                    content = AHelper.Json.Serialize(data);
                }
                else
                {
                    content = AHelper.Json.Serialize(new[]
                    {
                        new AssetsPackageConfig
                        {
                            Name    = two,
                            Version = three
                        }
                    });
                }

                var bytes = Encoding.UTF8.GetBytes(content);
                var op    = AHelper.FTP.UploadFile(version, User, Pass, bytes);
                op.Begin();
                await op.WaitAsync();
            }

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