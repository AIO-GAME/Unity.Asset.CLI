/*|============|*|
|*|Author:     |*| xinan                
|*|Date:       |*| 2024-01-03               
|*|E-Mail:     |*| 1398581458@qq.com     
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
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
        private static string[] GetCollectorDisPlayNames(ICollection<AssetCollectItem> collectors)
        {
            var page = collectors.Count > 15;
            return (from item in collectors
                    where item.Type == EAssetCollectItemType.MainAssetCollector
                    where !string.IsNullOrEmpty(item.CollectPath)
                    select page
                        ? string.Concat(char.ToUpper(item.CollectPath[0]), '/',
                            item.CollectPath.Replace('/', '\\').TrimEnd('\\'))
                        : item.CollectPath.Replace('/', '\\').TrimEnd('\\'))
                .ToArray();
        }

        /// <summary>
        /// 获取收集器显示名称
        /// </summary>
        private static string[] GetCollectorDisPlayNames(ICollection<AssetCollectItem> collectors, Func<AssetCollectItem, bool> condition)
        {
            var page = collectors.Count > 15;
            return (from item in collectors
                    where item.Type == EAssetCollectItemType.MainAssetCollector
                    where !string.IsNullOrEmpty(item.CollectPath)
                    where condition(item)
                    select page
                        ? string.Concat(char.ToUpper(item.CollectPath[0]), '/',
                            item.CollectPath.Replace('/', '\\').TrimEnd('\\'))
                        : item.CollectPath.Replace('/', '\\').TrimEnd('\\'))
                .ToArray();
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
            var handle = await config.Validate();
            EditorUtility.DisplayDialog("提示", handle ? "连接成功" : "连接失败", "确定");
        }

        private static void SaveScene()
        {
            var currentScene = SceneManager.GetSceneAt(0);
            if (!string.IsNullOrEmpty(currentScene.path))
            {
                var scene = SceneManager.GetSceneByPath(currentScene.path);
                if (scene.isDirty) // 获取当前场景的修改状态
                {
                    if (EditorUtility.DisplayDialog("提示", "当前场景未保存,是否保存?", "保存", "取消"))
                    {
                        EditorSceneManager.SaveScene(scene);
                    }
                }
            }
        }
    }
}