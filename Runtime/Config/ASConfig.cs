/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// 资源包配置
        /// </summary>
        public AssetsPackageConfig[] Packages;

        /// <summary>
        /// 运行时内置文件根目录
        /// </summary>
        public string RuntimeRootDirectory = "BuiltinFiles";

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
        /// 从远端资源包列表
        /// </summary>
        /// <exception cref="ArgumentNullException">Url路径为空</exception>
        public IEnumerator UpdatePackageRemote()
        {
            if (ASMode != EASMode.Remote) yield break;
            if (string.IsNullOrEmpty(URL)) throw new ArgumentNullException(nameof(URL));
            var remote = Path.Combine(
                URL, "Version", string.Concat(AssetSystem.PlatformNameStr, ".json?t=", DateTime.Now.Ticks));

            yield return AssetSystem.NetLoadStringCO(remote,
                data => { Packages = AHelper.Json.Deserialize<AssetsPackageConfig[]>(data); });
            foreach (var item in Packages)
            {
                item.IsLatest = item.Version == "Latest";
                if (item.IsLatest)
                {
                    remote = Path.Combine(
                        URL,
                        AssetSystem.PlatformNameStr,
                        item.Name,
                        item.Version,
                        $"PackageManifest_{item.Name}.version?t={DateTime.Now.Ticks}");
                    yield return AssetSystem.NetLoadStringCO(remote, data => item.Version = data);
                }
            }
        }

        /// <summary>
        /// 检查配置
        /// </summary>
        /// <exception cref="Exception">配置异常</exception>
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
            }
        }

        public void UpdatePackage()
        {
            if (ASMode == EASMode.Remote) return;
            switch (ASMode)
            {
                case EASMode.Local:
                    Packages = AHelper.IO.ReadJsonUTF8<AssetsPackageConfig[]>(
                        Path.Combine(Application.streamingAssetsPath, $"Version/{AssetSystem.PlatformNameStr}.json"));
                    if (Packages is null)
                        throw new Exception("Not found Version.json or AssetsPackageConfig list is null !");
                    break;
#if UNITY_EDITOR
                default:
                    var assembly = Assembly.Load("AIO.Asset.Editor");
                    var type = assembly.GetType("AIO.UEditor.AssetCollectRoot", true);
                    var getOrCreate = type.GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.Public);
                    var CollectRoot = getOrCreate?.Invoke(null, new object[] { });
                    if (CollectRoot is null) break;
                    var packages = type.GetField("Packages", BindingFlags.Instance | BindingFlags.Public)
                        ?.GetValue(CollectRoot);
                    if (packages is Array array)
                    {
                        var list = new List<AssetsPackageConfig>();
                        var fieldInfo = assembly
                            .GetType("AIO.UEditor.AssetCollectPackage", true)
                            .GetField("Name", BindingFlags.Instance | BindingFlags.Public);
                        foreach (var item in array)
                        {
                            if (item is null) continue;
                            list.Add(new AssetsPackageConfig
                            {
                                Name = fieldInfo?.GetValue(item) as string,
                                Version = "-.-.-",
                                IsDefault = false
                            });
                        }

                        if (list.Count > 0)
                        {
                            Packages = list.ToArray();
                            Packages[0].IsDefault = true;
                        }
                    }

                    break;
#endif
            }
        }

        private static ASConfig instance;

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

                if (Application.isPlaying)
                {
                    if (instance.ASMode == EASMode.Editor)
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if (assembly.GetName().Name != "AIO.Asset.Editor") continue;
                            var temp = assembly.GetType("AIO.UEditor.AssetCollectRoot", true)
                                ?.GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.Public)
                                ?.Invoke(null, new object[] { });
                            if (temp == null) break;
                            assembly.GetType("AIO.UEditor.AssetProxyEditor", true)
                                ?.GetMethod("ConvertConfig", BindingFlags.Static | BindingFlags.Public)
                                ?.Invoke(null, new object[] { temp, false });
                            break;
                        }
                    }
                }
            }

#else
            instance = GetResource();
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
            config.UpdatePackage();
            return config;
        }

        public void Save()
        {
            if (Equals(null)) return;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}