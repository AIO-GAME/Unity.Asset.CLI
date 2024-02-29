using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        /// 是否过滤 收集器
        /// </summary>
        private static bool IsFilterCollectors(int index, string collectPath, IList<string> displays)
        {
            if (index < 1) return true;
            if (displays is null) return false;

            if (displays.Count >= 31)
            {
                if (displays.Count <= index) index = displays.Count - 1;
                if (displays[index].EndsWith(collectPath))
                    return true;
            }
            else if (displays.Count > 15)
            {
                var status = 1L;
                collectPath = collectPath.Replace('/', '\\').TrimEnd('\\');
                foreach (var display in displays)
                {
                    if ((index & status) == status && display.EndsWith(collectPath)) return true;
                    status *= 2;
                }
            }
            else
            {
                var status = 1L;
                collectPath = collectPath.Replace('/', '\\').TrimEnd('\\');
                foreach (var display in displays)
                {
                    if ((index & status) == status && display == collectPath) return true;
                    status *= 2;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否过滤 指定类型
        /// </summary>
        private static bool IsFilterTypes(int index, string assetPath, ICollection<string> displays)
        {
            if (index < 1) return true;
            if (displays is null) return false;
            var objectType = AssetDatabase.GetMainAssetTypeAtPath(assetPath)?.FullName;
            if (string.IsNullOrEmpty(objectType)) objectType = "Unknown";
            var status = 1L;
            foreach (var display in displays)
            {
                if ((index & status) == status && objectType == display) return true;
                status *= 2;
            }

            return false;
        }

        /// <summary>
        /// 是否过滤 搜索文本
        /// </summary>
        private static bool IsFilterSearch(string searchText, AssetDataInfo data)
        {
            if (string.IsNullOrEmpty(searchText)) return true;

            if (data.AssetPath.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                data.Address.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
               ) return true;

            if (searchText.Contains(data.AssetPath, StringComparison.CurrentCultureIgnoreCase) ||
                searchText.Contains(data.Address, StringComparison.CurrentCultureIgnoreCase)
               ) return true;

            return false;
        }

        /// <summary>
        /// 是否过滤 指定标签
        /// </summary>
        private static bool IsFilterTags(int index, ICollection<string> tags, ICollection<string> displays)
        {
            if (index < 1) return true;
            if (displays is null) return false;
            if (tags is null) return false;

            var status = 1L;
            foreach (var display in displays)
            {
                if ((index & status) == status && tags.Contains(display)) return true;
                status *= 2;
            }

            return false;
        }

        /// <summary>
        /// 获取组显示名称
        /// </summary>
        private static string[] GetGroupDisPlayNames(ICollection<AssetCollectGroup> groups)
        {
            var page = groups.Count > 15;
            return (from t in groups
                    select t.Name
                    into groupName
                    where !string.IsNullOrEmpty(groupName)
                    select page
                        ? string.Concat(char.ToUpper(groupName[0]), '/', groupName)
                        : groupName)
                .ToArray();
        }

        /// <summary>
        /// 获取收集器显示名称
        /// </summary>
        private static string[] GetCollectorDisPlayNames(IEnumerable<AssetCollectItem> collectors)
        {
            return (from item in collectors
                    where item.Type == EAssetCollectItemType.MainAssetCollector
                    where !string.IsNullOrEmpty(item.CollectPath)
                    select item.CollectPath)
                .ToArray();
        }

        private static string[] GetCollectorDisPlayNames(IList<string> collectors)
        {
            if (collectors.Count >= 31)
            {
                var temp = new string[collectors.Count + 1];
                temp[0] = "All";
                for (var index = 0; index < collectors.Count; index++) temp[index + 1] = collectors[index];
                return temp;
            }

            if (collectors.Count > 15)
            {
                for (var index = 0; index < collectors.Count; index++)
                {
                    collectors[index] = string.Concat(char.ToUpper(collectors[index][0]), '/',
                        collectors[index].Replace('/', '\\').TrimEnd('\\'));
                }
            }
            else
            {
                for (var index = 0; index < collectors.Count; index++)
                {
                    collectors[index] = collectors[index].Replace('/', '\\').TrimEnd('\\');
                }
            }

            return collectors.ToArray();
        }

        /// <summary>
        /// 获取收集器显示名称
        /// </summary>
        private static string[] GetCollectorDisPlayNames(ICollection<AssetCollectItem> collectors,
            Func<AssetCollectItem, bool> condition)
        {
            var page = collectors.Count > 15;
            return (from item in collectors
                    where item.Type == EAssetCollectItemType.MainAssetCollector
                    where !string.IsNullOrEmpty(item.CollectPath)
                    where condition(item)
                    select (page
                        ? string.Concat(char.ToUpper(item.CollectPath[0]), '/', item.CollectPath)
                        : item.CollectPath).Replace('/', '\\').TrimEnd('\\')
                ).ToArray();
        }

        /// <summary>
        /// 上传首包 FTP
        /// </summary>
        private static async void UpdateUploadFirstPack(ASBuildConfig.FTPConfig config)
        {
            await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
            EditorUtility.DisplayDialog("提示", "上传成功", "确定");
        }

        /// <summary>
        /// 上传首包 Google Cloud
        /// </summary>
        private static async void UpdateUploadFirstPack(ASBuildConfig.GCloudConfig config)
        {
            await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
        }

        /// <summary>
        /// 创建 FTP
        /// </summary>
        private static async void CreateFTP(ASBuildConfig.FTPConfig config)
        {
            using (var handle = AHandle.FTP.Create(
                       config.Server,
                       config.Port,
                       config.User,
                       config.Pass,
                       config.RemotePath))
            {
                EditorUtility.DisplayDialog("提示", await handle.InitAsync()
                    ? $"创建成功 {handle.URI}"
                    : $"创建失败 {handle.URI}", "确定");
            }
        }

        /// <summary>
        /// 验证 FTP 是否有效
        /// </summary>
        private static async void ValidateFTP(ASBuildConfig.FTPConfig config)
        {
            EditorUtility.DisplayDialog("提示", await config.Validate() ? "连接成功" : "连接失败", "确定");
        }
    }
}