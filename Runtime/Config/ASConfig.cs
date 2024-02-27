/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ - - - - - |*/

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIO.UEngine
{
    [Description("资源系统配置")]
    [Serializable]
    [HelpURL(
        "https://github.com/AIO-GAME/Unity.Asset.CLI/blob/main/.github/API_USAGE/Config.md#-aiouengineasconfig---%E8%B5%84%E6%BA%90%E7%B3%BB%E7%BB%9F%E9%85%8D%E7%BD%AE-")]
    public class ASConfig : ScriptableObject
    {
        /// <summary>
        /// 资源加载模式
        /// </summary>
        public EASMode ASMode;

        /// <summary>
        /// 热更新资源包服务器地址
        /// </summary>
        public string URL;

        /// <summary>
        /// 自动激活清单
        /// </summary>
        public bool AutoSaveVersion = true;

        /// <summary>
        /// URL请求附加时间搓
        /// </summary>
        public bool AppendTimeTicks = true;

        /// <summary>
        /// 加载路径转小写
        /// </summary>
        public bool LoadPathToLower = true;

        /// <summary>
        /// 自动序列记录
        /// </summary>
        public bool EnableSequenceRecord;

        /// <summary>
        /// 输出日志
        /// </summary>
        public bool OutputLog;

        /// <summary>
        /// 下载失败尝试次数
        /// 注意：默认值为MaxValue
        /// </summary>
        public int DownloadFailedTryAgain = 1;

        /// <summary>
        /// 资源加载的最大数量
        /// </summary>
        public int LoadingMaxTimeSlice = 144;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout = 60;

        /// <summary>
        /// 异步系统 每帧执行消耗的最大时间切片
        /// </summary>
        public int AsyncMaxTimeSlice = 30;

        /// <summary>
        /// 资源包配置
        /// </summary>
        public AssetsPackageConfig[] Packages;

        /// <summary>
        /// 运行时内置文件根目录
        /// </summary>
        public string RuntimeRootDirectory = "BuiltinFiles";

#if UNITY_EDITOR
        private AssetSystem.SequenceRecordQueue _SequenceRecord;

        public AssetSystem.SequenceRecordQueue SequenceRecord
        {
            get
            {
                if (_SequenceRecord is null)
                {
                    _SequenceRecord = new AssetSystem.SequenceRecordQueue(true);
                    _SequenceRecord.UpdateLocal();
                }

                return _SequenceRecord;
            }
        }

#endif

        /// <summary>
        /// 获取远程资源包地址
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="package">包名</param>
        /// <param name="version">版本</param>
        public string GetRemoteURL(string fileName, string package, string version)
        {
            return Path.Combine(URL, AssetSystem.PlatformNameStr, package, version, fileName);
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        /// <exception cref="Exception">配置异常</exception>
        public void Check()
        {
            switch (ASMode)
            {
                case EASMode.Editor:
                    if (Packages == null || Packages.Length == 0)
                        throw new Exception("Please set the package configuration");
                    if (Packages.Any(value => string.IsNullOrEmpty(value.Name)))
                        throw new Exception("Please set the package name");
                    if (Packages.Any(value => string.IsNullOrEmpty(value.Version)))
                        throw new Exception("Please set the package version");
                    break;
                case EASMode.Remote:
                    if (string.IsNullOrEmpty(URL))
                        throw new Exception("Please set the remote URL");
                    if (Packages.Any(value => string.IsNullOrEmpty(value.Name)))
                        throw new Exception("Please set the package name");
                    if (Packages.Any(value => string.IsNullOrEmpty(value.Version)))
                        throw new Exception("Please set the package version");
                    if (string.IsNullOrEmpty(RuntimeRootDirectory))
                        throw new Exception("Please set the runtime root directory");
                    break;
                case EASMode.Local:
                    if (string.IsNullOrEmpty(RuntimeRootDirectory))
                        throw new Exception("Please set the runtime root directory");
                    break;
            }
        }

        #region static

#if UNITY_EDITOR
        private static ASConfig instance;
#endif

        private static ASConfig GetResource()
        {
            return Resources.LoadAll<ASConfig>(nameof(ASConfig)).FirstOrDefault(item => !(item is null));
        }

        /// <summary>
        /// 获取本地资源包地址
        /// </summary>
        public static ASConfig GetOrCreate()
        {
#if UNITY_EDITOR
            if (instance is null)
            {
                foreach (var item in AssetDatabase.FindAssets("t:ASConfig", new string[] { "Assets" })
                             .Select(AssetDatabase.GUIDToAssetPath)
                             .Select(AssetDatabase.LoadAssetAtPath<ASConfig>)
                             .Where(value => value != null))
                {
                    if (item.Equals(null)) continue;
                    instance = item;
                    break;
                }

                if (instance is null)
                {
                    instance = CreateInstance<ASConfig>();
                    instance.ASMode = EASMode.Editor;
                    instance.Packages = Array.Empty<AssetsPackageConfig>();
                    if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
                        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
                    AssetDatabase.CreateAsset(instance, "Assets/Resources/ASConfig.asset");
                    AssetDatabase.SaveAssets();
                }

                if (Application.isPlaying && instance.ASMode == EASMode.Editor)
                {
                    var type = Type.GetType("AIO.UEditor.AssetCollectRoot, AIO.Asset.Editor", true);
                    var temp = type.GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.Public)
                        ?.Invoke(null, new object[] { });
                    if (temp != null)
                    {
                        Type.GetType("AIO.UEditor.AssetProxyEditor, AIO.Asset.Editor", true)
                            .GetMethod("ConvertConfig", BindingFlags.Static | BindingFlags.Public)
                            ?.Invoke(null, new object[] { temp, false });
                    }
                }
            }

            if (instance is null) throw new Exception("Not found ASConfig.asset ! Please create it !");
            return instance;
#else
            return GetResource();
#endif
        }

        /// <summary>
        /// 获取本地资源包配置
        /// </summary>
        /// <param name="autoSaveVersion">自动激活资源清单</param>
        /// <param name="loadPathToLower">路径小写</param>
        /// <param name="autoSequenceRecord">自动序列记录</param>
        /// <param name="outputLog">日志输出</param>
        /// <param name="url">远端路径</param>
        /// <param name="appendTimeTicks">附加时间磋</param>
        /// <returns>资源配置</returns>
        public static ASConfig GetRemote(string url,
            bool appendTimeTicks = true,
            bool autoSaveVersion = true,
            bool loadPathToLower = false,
            bool autoSequenceRecord = true,
            bool outputLog = false)
        {
            var config = CreateInstance<ASConfig>();
            config.AppendTimeTicks = appendTimeTicks;
            config.AutoSaveVersion = autoSaveVersion;
            config.LoadPathToLower = loadPathToLower;
            config.OutputLog = outputLog;
            config.EnableSequenceRecord = autoSequenceRecord;
            config.ASMode = EASMode.Remote;
            config.URL = url;
            return config;
        }

        /// <summary>
        /// 获取本地资源包配置
        /// </summary>
        /// <param name="list">包列表</param>
        /// <param name="loadPathToLower">路径小写</param>
        /// <returns>资源配置</returns>
        public static ASConfig GetLocal(AssetsPackageConfig[] list, bool loadPathToLower = false)
        {
            var config = CreateInstance<ASConfig>();
            config.LoadPathToLower = loadPathToLower;
            config.ASMode = EASMode.Local;
            config.Packages = list;
            return config;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 获取编辑器资源包配置
        /// </summary>
        /// <param name="loadPathToLower">路径小写</param>
        /// <returns>资源配置</returns>
        public static ASConfig GetEditor(bool loadPathToLower = false)
        {
            var config = CreateInstance<ASConfig>();
            config.ASMode = EASMode.Editor;
            config.LoadPathToLower = loadPathToLower;
            return config;
        }

        public void Save()
        {
            if (Equals(null)) return;
            if (SequenceRecord != null) SequenceRecord.Save();
            EditorUtility.SetDirty(this);
        }

#endif

        #endregion
    }
}