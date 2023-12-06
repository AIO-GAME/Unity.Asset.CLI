/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-06
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public enum EBuildPipeline
    {
        /// <summary>
        /// 内置打包管线
        /// </summary>
        [InspectorName("内置打包管线")] BuiltinBuildPipeline,

        /// <summary>
        /// 自定义打包管线
        /// </summary>
        [InspectorName("自定义打包管线(需安装)")] ScriptableBuildPipeline,
    }

    /// <summary>
    /// 资源包流水线的构建模式
    /// </summary>
    public enum EBuildMode
    {
        /// <summary>
        /// 强制重建模式
        /// </summary>
        [InspectorName("强制重建模式")] ForceRebuild,

        /// <summary>
        /// 增量构建模式
        /// </summary>
        [InspectorName("增量构建模式")] IncrementalBuild,

        /// <summary>
        /// 演练构建模式
        /// </summary>
        [InspectorName("演练构建模式")] DryRunBuild,

        /// <summary>
        /// 模拟构建模式
        /// </summary>
        [InspectorName("模拟构建模式")] SimulateBuild,
    }

    public class ASBuildConfig : ScriptableObject
    {
        /// <summary>
        /// 首次打包
        /// </summary>
        public bool FirstPack;

        /// <summary>
        /// 打包管线
        /// </summary>
        public EBuildPipeline BuildPipeline;

        /// <summary>
        /// 构建模式
        /// </summary>
        public EBuildMode BuildMode;

        /// <summary>
        /// 构建版本号
        /// </summary>
        public string BuildVersion;

        public string PackageName;

        /// <summary>
        /// 加密模式
        /// </summary>
        public string EncyptionClassName;

        /// <summary>
        /// 压缩模式
        /// </summary>
        public string CompressedModeName;

        /// <summary>
        /// FTP 服务器地址
        /// </summary>
        public string FTPServerIP;

        /// <summary>
        /// FTP 服务器端口
        /// </summary>
        public int FTPServerPort = 21;

        /// <summary>
        /// FTP 用户名
        /// </summary>
        public string FTPUser;

        /// <summary>
        /// FTP 密码
        /// </summary>
        public string FTPPassword;

        /// <summary>
        /// FTP 远程路径
        /// </summary>
        public string FTPRemotePath;
        
        /// <summary>
        /// FTP 本地需要上传的资源包地址
        /// </summary>
        public string FTPLocalPath;

        /// <summary>
        /// 首包标签集合
        /// </summary>
        public string FirstPackTag;

        /// <summary>
        /// 验证构建结果
        /// </summary>
        public bool ValidateBuild;

        /// <summary>
        /// 构建结果输出路径
        /// </summary>
        public string BuildOutputPath;

        /// <summary>
        /// 构建平台
        /// </summary>
        public BuildTarget BuildTarget;
        
        /// <summary>
        /// 获取本地资源包地址
        /// </summary>
        public static ASBuildConfig GetOrCreate()
        {
            var objects = EHelper.IO.GetScriptableObjects<ASBuildConfig>();
            if (objects != null && objects.Length > 0)
            {
                foreach (var asset in objects)
                {
                    if (asset is null) continue;

                    return asset;
                }
            }

            var collect = CreateInstance<ASBuildConfig>();
            AssetDatabase.CreateAsset(collect, "Assets/Editor/ASBuildConfig.asset");
            AssetDatabase.SaveAssets();
            return collect;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
    }
}