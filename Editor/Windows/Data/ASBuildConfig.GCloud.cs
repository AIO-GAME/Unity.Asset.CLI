/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-26
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源 上传 GCloud 配置
    /// </summary>
    public class ASUploadGCloudConfig
    {
        /// <summary>
        /// Gcloud 路径
        /// </summary>
        public string GCLOUD_PATH = "gcloud";

        /// <summary>
        /// Gsutil 路径
        /// </summary>
        public string GSUTIL_PATH = "gsutil";

        /// <summary>
        /// 是否上传Latest
        /// </summary>
        /// [Ture:上传Latest]
        /// [False:则上传指定版本 但是不会将指定版本再次与Latest进行同步]
        public bool IsUploadLatest;

        /// <summary>
        /// 上传目录 不包含平台 包名 版本
        /// </summary>
        public string LocalFullPath = Path.Combine(EHelper.Path.Project, "Bundles");

        /// <summary>
        /// 上传包名
        /// </summary>
        public string PackageName;

        /// <summary>
        /// 上传平台
        /// </summary>
        public BuildTarget BuildTarget = EditorUserBuildSettings.activeBuildTarget;

        /// <summary>
        /// 上传版本
        /// </summary>
        public string Version;

        /// <summary>
        /// FTP 远程路径
        /// </summary>
        public string RemotePath;

        /// <summary>
        /// 元数据
        /// </summary>
        /// <remarks>
        /// --content-type=image/png
        /// --cache-control=public,max-age=3600
        /// --content-language=unset
        /// --content-encoding=unset
        /// --content-disposition=disposition
        /// </remarks>
        public string MetaData;

        /// <summary>
        /// 远端相对目录
        /// </summary>
        public string RemoteRelative
        {
            get
            {
                if (string.IsNullOrEmpty(PackageName))
                    throw new ArgumentNullException($"{PackageName} is null or empty");

                var version = IsUploadLatest ? "Latest" : Version;
                if (string.IsNullOrEmpty(version))
                    throw new ArgumentNullException($"{version} is null or empty");

                return (string.IsNullOrEmpty(RemotePath)
                        ? Path.Combine(BuildTarget.ToString(), PackageName, version)
                        : Path.Combine(RemotePath, BuildTarget.ToString(), PackageName, version)
                    ).Replace("\\", "/");
            }
        }


        /// <summary>
        /// 更目录
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
                return path.Replace("\\", "/");
            }
        }
    }


    /// <summary>
    /// nameof(ASBuildConfig_GCloud)
    /// </summary>
    public partial class ASBuildConfig
    {
        public void AddOrNewGCloud()
        {
            var temp = new GCloudConfig
            {
                DirTreeFiled = new DirTreeFiled(BuildOutputPath.Replace('\\', '/'), 3)
                {
                    OptionShowDepth = false,
                    OptionSearchPatternFolder = "(?i)^(?!.*\b(Version|OutputCache|Simulate)\b).*$",
                }
            };
            GCloudConfigs = GCloudConfigs is null
                ? new[] { temp }
                : GCloudConfigs.Add(temp);
        }

        [Serializable]
        public class GCloudConfig
        {
            /// <summary>
            /// Gcloud 路径
            /// </summary>
            public string GCLOUD_PATH = "gcloud";

            /// <summary>
            /// Gsutil 路径
            /// </summary>
            public string GSUTIL_PATH = "gsutil";

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
            public async Task UploadFirstPack(string location)
            {
                PrGCloud.Gcloud = GCLOUD_PATH;
                PrGCloud.Gsutil = GSUTIL_PATH;
                var result = await PrGCloud.UploadFileAsync(
                    AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(BUCKET_NAME),
                    location,
                    MetaData);
                EditorUtility.DisplayDialog("提示", result
                    ? "上传成功 "
                    : "上传失败", "确定");
            }

            public override string ToString()
            {
                return string.IsNullOrEmpty(Name) ? BUCKET_NAME : Name;
            }

            public async Task Upload()
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
                var config = new ASUploadGCloudConfig();
                config.RemotePath = BUCKET_NAME;
                config.PackageName = two;
                config.Version = three;
                config.BuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), one, false);
                config.MetaData = MetaData;
                config.LocalFullPath = DirTreeFiled.DirPath.Replace("\\", "/");
                await AssetProxyEditor.UploadGCloud(config);
                isUploading = false;
            }
        }
    }
}