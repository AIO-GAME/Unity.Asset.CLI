/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIO.UEngine
{
    [DebuggerNonUserCode]
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
        public bool AutoSaveVersion;

        /// <summary>
        /// URL请求附加时间搓
        /// </summary>
        public bool AppendTimeTicks;

        /// <summary>
        /// 加载路径转小写
        /// </summary>
        public bool LoadPathToLower;

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
        public int DownloadFailedTryAgain;

        /// <summary>
        /// 资源加载的最大数量
        /// </summary>
        public int LoadingMaxTimeSlice;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout;

        /// <summary>
        /// 资源包配置
        /// </summary>
        public AssetsPackageConfig[] Packages;

        /// <summary>
        /// 运行时内置文件根目录
        /// </summary>
        public string RuntimeRootDirectory = "BuiltinFiles";

        public ASConfig(string url)
        {
            URL = url;
            ASMode = EASMode.Remote;
            AutoSaveVersion = true;
            AppendTimeTicks = true;
            LoadPathToLower = true;
            EnableSequenceRecord = true;
            OutputLog = true;
        }

        public ASConfig()
        {
            URL = string.Empty;
            ASMode = EASMode.Local;
            AutoSaveVersion = false;
            AppendTimeTicks = false;
            EnableSequenceRecord = false;
            LoadPathToLower = false;
            OutputLog = true;
        }

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

        public IEnumerator UpdatePackageRemote()
        {
            if (ASMode != EASMode.Remote) yield break;
            {
                if (string.IsNullOrEmpty(URL)) throw new ArgumentNullException(nameof(URL));
                var remote = Path.Combine(URL, "Version",
                    string.Concat(AssetSystem.PlatformNameStr, ".json?t=", DateTime.Now.Ticks));

                yield return AssetSystem.NetLoadStringCO(remote,
                    data => { Packages = AHelper.Json.Deserialize<AssetsPackageConfig[]>(data); });
            }
        }

        public void Check()
        {
            switch (instance.ASMode)
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
                    if (string.IsNullOrEmpty(RuntimeRootDirectory))
                        throw new Exception("Please set the runtime root directory");
                    break;
                case EASMode.Local:
                    if (string.IsNullOrEmpty(RuntimeRootDirectory))
                        throw new Exception("Please set the runtime root directory");
                    break;
                default:
                    break;
            }
        }

        public void UpdatePackage()
        {
            if (ASMode == EASMode.Remote) return;
            switch (ASMode)
            {
                default:
#if UNITY_EDITOR && SUPPORT_YOOASSET
                    var assembly = Assembly.Load("YooAsset.Editor");
                    var type = assembly.GetType("YooAsset.Editor.AssetBundleCollectorSettingData", true);
                    var setting = type.GetProperty("Setting",
                            BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty)
                        ?.GetValue(null, null);

                    var type1 = assembly.GetType("YooAsset.Editor.AssetBundleCollectorSetting", true);
                    if (type1.GetField("Packages", BindingFlags.Instance | BindingFlags.Public)
                            ?.GetValue(setting) is IList packages)
                    {
                        var type2 = assembly.GetType("YooAsset.Editor.AssetBundleCollectorPackage", true);
                        var PackageName = type2.GetField("PackageName", BindingFlags.Instance | BindingFlags.Public);
                        if (PackageName != null)
                        {
                            Packages = (from object package in packages
                                    where !(package is null)
                                    select new AssetsPackageConfig
                                    {
                                        Name = PackageName.GetValue(package) as string,
                                        Version = "-.-.-",
                                        IsDefault = false
                                    })
                                .ToArray();
                            if (Packages.Length > 0) Packages[0].IsDefault = true;
                        }
                    }
#endif
                    break;
            }
        }

        private static ASConfig instance;

        private static ASConfig GetResource()
        {
            ASConfig config = null;
            foreach (var item in Resources.LoadAll<ASConfig>("ASConfig"))
            {
                if (item is null) continue;
                config = item;
                break;
            }

            return config;
        }

        /// <summary>
        /// 获取本地资源包地址
        /// </summary>
        public static ASConfig GetOrCreate()
        {
            if (instance != null) return instance;
            instance = GetResource();
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
                    instance.URL = "";
                    instance.LoadPathToLower = false;
                    instance.AutoSaveVersion = false;
                    instance.AppendTimeTicks = false;
                    instance.OutputLog = false;
                    instance.EnableSequenceRecord = false;
                    instance.Packages = Array.Empty<AssetsPackageConfig>();
                    if (!Directory.Exists(Path.Combine(Application.dataPath, "Resources")))
                        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources"));
                    AssetDatabase.CreateAsset(instance, "Assets/Resources/ASConfig.asset");
                }
            }
#endif
            if (instance is null) throw new Exception("Not found ASConfig.asset ! Please create it !");
            return instance;
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
            if (instance != null) return instance;
            instance = GetResource();
            if (instance is null)
            {
                instance = CreateInstance<ASConfig>();
                instance.URL = url;
                instance.AppendTimeTicks = appendTimeTicks;
                instance.AutoSaveVersion = autoSaveVersion;
                instance.ASMode = EASMode.Remote;
                instance.LoadPathToLower = loadPathToLower;
                instance.OutputLog = outputLog;
                instance.EnableSequenceRecord = autoSequenceRecord;
            }

            return instance;
        }

        /// <summary>
        /// 获取本地资源包配置
        /// </summary>
        /// <param name="loadPathToLower">路径小写</param>
        /// <param name="outputLog">日志输出</param>
        /// <returns>资源配置</returns>
        public static ASConfig GetLocal(bool loadPathToLower = false, bool outputLog = false)
        {
            if (instance != null) return instance;
            instance = GetResource();
            if (instance is null)
            {
                instance = CreateInstance<ASConfig>();
                instance.ASMode = EASMode.Local;
                instance.LoadPathToLower = loadPathToLower;
                instance.OutputLog = outputLog;
            }

            return instance;
        }

#if UNITY_EDITOR

        /// <summary>
        /// 获取编辑器资源包配置
        /// </summary>
        /// <param name="loadPathToLower">路径小写</param>
        /// <param name="outputLog">日志输出</param>
        /// <returns>资源配置</returns>
        public static ASConfig GetEditor(bool loadPathToLower = false, bool outputLog = false)
        {
            if (instance != null) return instance;
            instance = GetResource();
            if (instance is null)
            {
                instance = CreateInstance<ASConfig>();
                instance.ASMode = EASMode.Editor;
                instance.LoadPathToLower = loadPathToLower;
                instance.OutputLog = outputLog;
            }

            return instance;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}