#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal partial class AssetProxyEditor_Yoo_157 : IAssetProxyEditor
    {
        #region IAssetProxyEditor Members

        public string Version => Ghost.YooAsset.Version;
        public string Scopes  => Ghost.YooAsset.Scopes;
        public string Name    => Ghost.YooAsset.Name;

        public bool CreateConfig(string bundlesDir, bool mergeToLatest) { return CreateConfig157(bundlesDir, mergeToLatest); }

        public bool ConvertConfig(AssetCollectRoot config) { return ConvertYooAsset.Convert(config); }

        public bool BuildArtList(IEnumerable<string> packageNames, AssetBuildCommand command)
        {
            var enumerable = packageNames.ToArray();
            foreach (var packageName in enumerable)
            {
                command.BuildPackage = packageName;
                var result = YooAssetBuild.ArtBuild(command);
                if (result.Success) continue;
                Debug.LogError($"构建 {command.BuildPackage} 失败 : {result.ErrorInfo}");
                return false;
            }

            return true;
        }

        public bool BuildArt(AssetBuildCommand command)
        {
            var result = YooAssetBuild.ArtBuild(command);
            if (result.Success)
            {
                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) Debug.Log("构建资源成功");
                else if (!EHelper.IsCMD()) EditorUtility.RevealInFinder(result.OutputPackageDirectory);
            }
            else
            {
                Debug.LogError($"构建失败 {result.ErrorInfo}");
            }

            return result.Success;
        }

        /// <summary>
        ///     上传到GCloud
        ///     生成一份清单文件 记录当前文件夹下的所有文件的MD5值
        ///     在上传的时候会对比清单文件的MD5值 如果一致则不上传
        ///     如果不一致 则拉取清单文件中的文件进行对比 记录需要上传的文件
        ///     然后再将需要上传的文件上传到GCloud 上传完成后更新清单文件
        ///     Tips:
        ///     需要本地保留一份原始清单 否则会覆盖远端最新的清单文件 导致无法对比
        /// </summary>
        public Task<bool> UploadGCloud(ICollection<AssetUploadGCloudParameter> parameters) { return UploadGCloudAsync(parameters); }

        // [LnkTools(IconResource = "Editor/Icon/App/Microsoft")]
        // public static async void Test()
        // {
        //     // var config = new ASUploadGCloudConfig();
        //     // config.RemotePath = "rol-files/AIO";
        //     // config.PackageName = "Default Package";
        //     // config.MetaData = "--cache-control=no-cache";
        //     // config.IsUploadLatest = true;
        //     // await UploadGCloudAsync(config);
        // }

        /// <summary>
        ///     上传到Ftp
        /// </summary>
        public async Task<bool> UploadFtp(ICollection<AssetUploadFtpParameter> parameters) { return await UploadFtpAsync(parameters); }

        #endregion

        [MenuItem("YooAsset/Create Config")]
        public static void CreateConfig() { CreateConfig157(Path.Combine(EHelper.Path.Project, "Bundles"), AssetBuildConfig.GetOrCreate().MergeToLatest); }

        /// <summary>
        ///     创建配置
        /// </summary>
        /// <param name="BundlesDir">构建目录</param>
        /// <param name="MergeToLatest">开启合并Latest</param>
        private static bool CreateConfig157(string BundlesDir, bool MergeToLatest)
        {
            if (!Directory.Exists(BundlesDir))
            {
                Debug.LogWarningFormat("Bundles 目录不存在 : 无需创建配置文件");
                return false;
            }

            var BundlesConfigDir = Path.Combine(BundlesDir, "Version");
            if (!Directory.Exists(BundlesConfigDir)) Directory.CreateDirectory(BundlesConfigDir);

            var BundlesInfo = new DirectoryInfo(BundlesDir);
            var Versions    = new List<DirectoryInfo>();
            var TableDic    = new Dictionary<BuildTarget, Dictionary<string, AssetsPackageConfig>>();

            foreach (var PlatformInfo in BundlesInfo.GetDirectories("*", SearchOption.TopDirectoryOnly)
                                                    .Where(PlatformInfo => !PlatformInfo.Name.StartsWith("Version")))
            {
                switch (PlatformInfo.Name)
                {
                    case nameof(BuildTarget.Android):
                    case nameof(BuildTarget.WebGL):
                    case nameof(BuildTarget.iOS):
                    case nameof(BuildTarget.StandaloneWindows):
                    case nameof(BuildTarget.StandaloneWindows64):
                    case nameof(BuildTarget.StandaloneOSX):
                        break;
                    default: continue;
                }


                foreach (var PackageInfo in new DirectoryInfo(PlatformInfo.FullName).GetDirectories("*",
                                                                                                    SearchOption.TopDirectoryOnly))
                {
                    Versions.Clear();
                    Versions.AddRange(PackageInfo.GetDirectories("*", SearchOption.TopDirectoryOnly)
                                                 .Where(VersionInfo => !VersionInfo.Name.EndsWith("OutputCache"))
                                                 .Where(VersionInfo => !VersionInfo.Name.EndsWith("Latest"))
                                                 .Where(VersionInfo => !VersionInfo.Name.EndsWith("Simulate")));

                    if (Versions.Count <= 0) continue;
                    if (Enum.TryParse<BuildTarget>(PlatformInfo.Name, out var enums))
                    {
                        if (!TableDic.ContainsKey(enums))
                            TableDic[enums] = new Dictionary<string, AssetsPackageConfig>();
                        if (!TableDic[enums].ContainsKey(PackageInfo.Name))
                            TableDic[enums].Add(PackageInfo.Name, new AssetsPackageConfig());

                        var last = AHelper.IO.GetLastWriteTimeUtc(Versions);
                        TableDic[enums][PackageInfo.Name].Version = MergeToLatest ? "Latest" : last.Name;
                        TableDic[enums][PackageInfo.Name].Name    = PackageInfo.Name;
                        var table = AHelper.IO.ReadJson<Hashtable>(Path.Combine(last.FullName,
                                                                                $"BuildReport_{PackageInfo.Name}_{last.Name}.json"));
                        if (table is null) continue;

                        switch (table.Cast<DictionaryEntry>()
                                     .FirstOrDefault(entry => entry.Key.ToString() == "Summary")
                                     .Value?
                                     .To<JObject>()
                                     ?
                                     .Value<int>("CompressOption"))
                        {
                            case (int)ECompressOption.LZMA:
                                TableDic[enums][PackageInfo.Name].CompressMode = ECompressMode.LZMA;
                                break;
                            case (int)ECompressOption.LZ4:
                                TableDic[enums][PackageInfo.Name].CompressMode = ECompressMode.LZ4;
                                break;
                            default:
                                TableDic[enums][PackageInfo.Name].CompressMode = ECompressMode.None;
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("未知平台 : {0}", PackageInfo.Name);
                    }
                }
            }

            foreach (var pair in TableDic.Where(hashtable => hashtable.Value.Count > 0))
                AHelper.IO.WriteJsonUTF8(
                                         Path.Combine(BundlesConfigDir, string.Concat(pair.Key.ToString(), ".json")),
                                         pair.Value.Values.ToArray()
                                        );

            return true;
        }
    }
}

#endif