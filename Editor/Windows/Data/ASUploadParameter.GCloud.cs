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
    public class AsUploadGCloudParameter : ASUploadParameter
    {
        /// <summary>
        /// Gcloud 路径
        /// </summary>
        public string GCLOUD_PATH;

        /// <summary>
        /// Gsutil 路径
        /// </summary>
        public string GSUTIL_PATH;

        /// <summary>
        /// 元数据 键
        /// </summary>
        /// <remarks>
        /// --content-type=image/png
        /// --cache-control=public,max-age=3600
        /// --content-language=unset
        /// --content-encoding=unset
        /// --content-disposition=disposition
        /// </remarks>
        public string MetaDataKey;

        /// <summary>
        /// 元数据 值
        /// </summary>
        public string MetaDataValue;
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
            /// 元数据Key
            /// </summary>
            public string MetaDataKey;

            /// <summary>
            /// 元数据Value
            /// </summary>
            public string MetaDataValue;

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
                    location, MetaDataKey, MetaDataValue);
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
                var config = new AsUploadGCloudParameter();
                config.RemotePath = BUCKET_NAME;
                config.PackageName = two;
                config.Version = three;
                config.BuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), one, false);
                config.MetaDataKey = MetaDataKey;
                config.MetaDataValue = MetaDataValue;
                config.LocalFullPath = DirTreeFiled.DirPath.Replace("\\", "/");
                config.GCLOUD_PATH = GCLOUD_PATH;
                config.GSUTIL_PATH = GSUTIL_PATH;
                await AssetProxyEditor.UploadGCloud(config);
                isUploading = false;
            }
        }
    }
}