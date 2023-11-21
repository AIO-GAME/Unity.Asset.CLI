/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
        public EASMode ASMode { get; set; }

        /// <summary>
        /// 热更新资源包服务器地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 自动激活清单
        /// </summary>
        public bool AutoSaveVersion { get; set; }

        /// <summary>
        /// URL请求附加时间搓
        /// </summary>
        public bool AppendTimeTicks { get; set; }

        /// <summary>
        /// 加载路径转小写
        /// </summary>
        public bool LoadPathToLower { get; set; }

        /// <summary>
        /// 自动序列记录
        /// </summary>
        public bool AutoSequenceRecord { get; set; }

        /// <summary>
        /// 序列记录远端路径
        /// </summary>
        public string SequenceRecordRemotePath { get; set; }

        /// <summary>
        /// 输出日志
        /// </summary>
        public bool OutputLog { get; set; }

        /// <summary>
        /// 下载失败尝试次数
        /// 注意：默认值为MaxValue
        /// </summary>
        public int DownloadFailedTryAgain { get; set; } = 10;

        /// <summary>
        /// 资源加载的最大数量
        /// </summary>
        public int LoadingMaxTimeSlice { get; set; } = 2048;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; } = 60;

        /// <summary>
        /// 资源包配置
        /// </summary>
        public AssetsPackageConfig[] Packages;

        public ASConfig(string url)
        {
            URL = url;
            ASMode = EASMode.Remote;
            AutoSaveVersion = true;
            AppendTimeTicks = true;
            LoadPathToLower = true;
            AutoSequenceRecord = true;
            OutputLog = true;
        }

        public ASConfig()
        {
            URL = string.Empty;
            ASMode = EASMode.Local;
            AutoSaveVersion = false;
            AppendTimeTicks = false;
            AutoSequenceRecord = false;
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

        public async Task UpdatePackage()
        {
            switch (ASMode)
            {
                case EASMode.Remote:
                    Packages = await AssetSystem.GetRemotePackageList(URL);
                    if (string.IsNullOrEmpty(SequenceRecordRemotePath)) return;
                    await AHelper.Net.HTTP.DownloadAsync(SequenceRecordRemotePath, AssetSystem.SequenceRecordPath);
                    break;
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
                            Packages[0].IsDefault = true;
                        }
                    }
#endif
                    break;
            }
        }

        /// <summary>
        /// 获取本地资源包地址
        /// </summary>
        public static ASConfig GetOrCreate()
        {
            foreach (var item in Resources.LoadAll<ASConfig>(""))
            {
                if (item != null)
                    return item;
            }

#if UNITY_EDITOR
            foreach (var item in AssetDatabase.FindAssets("t:ASConfig", new string[] { "Assets" })
                         .Select(AssetDatabase.GUIDToAssetPath)
                         .Select(AssetDatabase.LoadAssetAtPath<ASConfig>)
                         .Where(value => value != null))
            {
                if (item != null) return item;
            }

            var config = CreateInstance<ASConfig>();
            config.ASMode = EASMode.Editor;
            config.URL = "";
            config.AutoSaveVersion = true;
            config.AppendTimeTicks = true;
            config.LoadPathToLower = true;
            config.OutputLog = false;
            AssetDatabase.CreateAsset(config, "Assets/Resources/ASConfig.asset");
            return config;
#else
            throw new System.Exception("Not found ASConfig.asset ! Please create it !");
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
            config.URL = url;
            config.AppendTimeTicks = appendTimeTicks;
            config.AutoSaveVersion = autoSaveVersion;
            config.ASMode = EASMode.Remote;
            config.LoadPathToLower = loadPathToLower;
            config.OutputLog = outputLog;
            config.AutoSequenceRecord = autoSequenceRecord;
            return config;
        }

        /// <summary>
        /// 获取本地资源包配置
        /// </summary>
        /// <param name="loadPathToLower">路径小写</param>
        /// <param name="outputLog">日志输出</param>
        /// <returns>资源配置</returns>
        public static ASConfig GetLocal(bool loadPathToLower = false, bool outputLog = false)
        {
            var config = CreateInstance<ASConfig>();
            config.ASMode = EASMode.Local;
            config.LoadPathToLower = loadPathToLower;
            config.OutputLog = outputLog;
            return config;
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
            var config = CreateInstance<ASConfig>();
            config.ASMode = EASMode.Editor;
            config.LoadPathToLower = loadPathToLower;
            config.OutputLog = outputLog;
            return config;
        }
#endif
    }
}