using System;
using System.IO;
using UnityEditor;

namespace AIO.UEditor
{
    public class ASUploadParameter
    {
        /// <summary>
        ///     上传平台
        /// </summary>
        public BuildTarget BuildTarget = EditorUserBuildSettings.activeBuildTarget;

        /// <summary>
        ///     是否上传Latest
        /// </summary>
        /// [Ture:上传Latest]
        /// [False:则上传指定版本 但是不会将指定版本再次与Latest进行同步]
        public bool IsUploadLatest;

        /// <summary>
        ///     上传目录 不包含平台 包名 版本
        /// </summary>
        public string LocalFullPath = Path.Combine(EHelper.Path.Project, "Bundles");

        /// <summary>
        ///     上传包名
        /// </summary>
        public string PackageName;

        /// <summary>
        ///     FTP 远程路径
        /// </summary>
        public string RemotePath;

        /// <summary>
        ///     上传版本
        /// </summary>
        public string Version;

        /// <summary>
        ///     远端相对目录
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
    }
}