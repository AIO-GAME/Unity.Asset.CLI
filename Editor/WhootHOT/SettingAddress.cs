#if SUPPORT_WHOOTHOT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using Debug = UnityEngine.Debug;

namespace Rol.Game
{
    public static class SettingAddressEditor
    {
        private static bool CancelingTrasvalSetting = false;
        private static Dictionary<string, string> addressPathMap = new Dictionary<string, string>();
        private static DateTime settingTime;

        [MenuItem("Tools/设置资源地址")]
        public static void SettingAllAddress()
        {
            var sw = new Stopwatch();
            sw.Start();
            settingTime = DateTime.Now;
            Prepare();
            CancelingTrasvalSetting = false;
            TranslationalSetting("Assets", null);
            EditorUtility.ClearProgressBar();
            sw.Stop();
            Debug.Log($"设置资源地址耗时{sw.ElapsedMilliseconds}毫秒");
        }

        public static void TranslationalSetting(string path, PathAddressConfig.ConfigEntry parentConfig)
        {
            if (CancelingTrasvalSetting) return;

            path = path.Replace('\\', '/');
            PathAddressConfig.ConfigEntry config = SettingAddress.PathConfig.GetPathConfigEntry(path) ?? parentConfig;
            if (null != config)
            {
                CancelingTrasvalSetting = EditorUtility.DisplayCancelableProgressBar("Setting Address", path, 1f);
                if (CancelingTrasvalSetting)
                {
                    return;
                }

                var subFiles = Directory.EnumerateFiles(path);
                PathUtils.ParseFileFilter(config.fileFilter, out List<string> containList, out List<string> blockList);
                GetAddressableGroup(config.groupFormat, config, out AddressableAssetSettings settings,
                    out AddressableAssetGroup group);
                foreach (var subFile in subFiles)
                {
                    var file = config.GetRelativePath(subFile);
                    if (PathUtils.IsPathPassFilter(file, containList, blockList))
                    {
                        string address = PathUtils.FormatPath(config.addressFormat, file);
                        if (string.IsNullOrEmpty(address))
                            address = Convert.ToBase64String(Encoding.UTF8.GetBytes(DateTime.Now.ToString() + subFile));

                        SetAddressableInfo(subFile, address, settings, group, config);
                    }
                }
            }
            else if (!SettingAddress.PathConfig.HasSubConfig(path))
            {
                return;
            }

            var subPaths = Directory.EnumerateDirectories(path);
            foreach (var subPath in subPaths)
            {
                TranslationalSetting(subPath, config);
            }
        }

        public static void Prepare()
        {
            SettingAddress.PathConfig.SortConfig();
            addressPathMap.Clear();
        }


        public static void GetAddressableGroup(string groupName, PathAddressConfig.ConfigEntry addressConfig,
            out AddressableAssetSettings settings, out AddressableAssetGroup group)
        {
            settings = AddressableAssetSettingsDefaultObject.Settings;
            group = null;
            if (!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                Debug.LogFormat(
                    "AddressableAssetSettingsDefaultObject.GetSettings 为Null，调用AddressableAssetSettingsDefaultObject.GetSettings(true)后：{0}",
                    settings == null);
            }

            if (null == settings)
            {
                Debug.LogError("AddressableAssetSettingsDefaultObject.Settings 为null");
                return;
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                group = settings.DefaultGroup;
            }
            else
            {
                group = settings.groups.Find(x => x.Name == groupName);
                if (null == group)
                {
                    var schemas = settings.DefaultGroup.Schemas;
                    var types = new Type[schemas.Count];
                    for (int i = 0, max = schemas.Count; i < max; ++i)
                    {
                        types[i] = schemas[i].GetType();
                    }

                    group = settings.CreateGroup(groupName, false, false, false, schemas, types);
                }

                foreach (var groupSchema in group.Schemas)
                {
                    if (groupSchema is BundledAssetGroupSchema bundleGroupSchema)
                    {
                        if (bundleGroupSchema == null) continue;
                        bundleGroupSchema.BuildPath.SetVariableByName(settings,
                            addressConfig.IsLocalGroup
                                ? AddressableAssetSettings.kLocalBuildPath
                                : AddressableAssetSettings.kRemoteBuildPath);
                        bundleGroupSchema.LoadPath.SetVariableByName(settings,
                            addressConfig.IsLocalGroup
                                ? AddressableAssetSettings.kLocalLoadPath
                                : AddressableAssetSettings.kRemoteLoadPath);
                        bundleGroupSchema.UseAssetBundleCrc = false;
                        bundleGroupSchema.UseAssetBundleCrcForCachedBundles = false;
                        bundleGroupSchema.BundleNaming = BundledAssetGroupSchema.BundleNamingStyle.NoHash;
                    }
                    else if (groupSchema is ContentUpdateGroupSchema contentUpdateSchema)
                    {
                        if (contentUpdateSchema == null) continue;
                        contentUpdateSchema.StaticContent = addressConfig.StaticContent;
                    }
                }
            }
        }

        public static void SetAddressableInfo(string path, string address, AddressableAssetSettings settings,
            AddressableAssetGroup group, PathAddressConfig.ConfigEntry addressConfig)
        {
            // Debug.Log($"PATH : {path}\nADDRESS : {address}\nGROUP : {groupName}\nTAG : {tag}");
            if (addressPathMap.TryGetValue(address, out var oldPath))
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error", "资源地址重复，相见控制台", "OK");
                throw new Exception($"资源地址重复:{oldPath}\n{path}");
            }

            addressPathMap.Add(address, path);
            var guid = AssetDatabase.AssetPathToGUID(path);

            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            if (entry == null)
            {
                Debug.LogError($"[Setting Address] Can't Create Entry {path}");
                return;
            }

            entry.address = address;
            entry.labels.Clear();
            if (!string.IsNullOrWhiteSpace(addressConfig.labelFormat))
            {
                entry.SetLabel(addressConfig.labelFormat, true, true, false);
            }
        }

        [MenuItem("Assets/设置资源地址")]
        public static void SetAddress()
        {
            var sw = new Stopwatch();
            sw.Start();
            Prepare();
            if (null != Selection.assetGUIDs && Selection.assetGUIDs.Length > 0)
            {
                CancelingTrasvalSetting = false;
                SettingAddress.PathConfig.SortConfig();
                foreach (var guid in Selection.assetGUIDs)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var isPath = Directory.Exists(path);
                    if (!isPath) continue;
                    var parentConfig = SettingAddress.PathConfig.GetPathParentConfigEntry(path);
                    TranslationalSetting(path, parentConfig);
                }

                EditorUtility.ClearProgressBar();
            }

            sw.Stop();
            Debug.Log($"设置资源地址耗时{sw.ElapsedMilliseconds}毫秒");
        }

        [MenuItem("Assets/程序工具/打印资源地址")]
        public static void PrintAssetAddress()
        {
            if (null == Selection.assetGUIDs || Selection.assetGUIDs.Length <= 0) return;

            Prepare();
            foreach (var guid in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var isFile = File.Exists(path);
                if (!isFile) continue;
                SettingAddress.GetPathAddress(path, out var address, out var labels, out var group,
                    out var cacheLevel);
                Debug.Log(
                    $"File : {path}\nAddress : {address}\nLabels : {labels}\nGroup : {group}\nCache Level : {cacheLevel}");
            }
        }
    }
}
#endif