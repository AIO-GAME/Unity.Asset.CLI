using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetWindow
    {
        [LnkTools(Tooltip = "AIO 资源管理工具", IconResource = "Editor/Icon/Asset", ShowMode = ELnkShowMode.Toolbar)]
        public static void OpenWindow() => EditorApplication.ExecuteMenuItem("AIO/Window/Asset");

        [MenuItem("AIO/Asset/清空运行时缓存")]
        public static void ClearRuntimeCache()
        {
            var sandbox = Path.Combine(EHelper.Path.Project, ASConfig.GetOrCreate().RuntimeRootDirectory);
            if (Directory.Exists(sandbox))
                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
        }

        [MenuItem("AIO/Asset/清空构建时缓存")]
        public static void ClearBuildCache()
        {
            var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
            if (Directory.Exists(sandbox))
                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
        }

        /// <summary>
        ///     上传首包 FTP
        /// </summary>
        public static async void UpdateUploadFirstPack(ASBuildConfig.FTPConfig config)
        {
            if (await config.IsExistRemoteFirstPack())
            {
                EditorUtility.DisplayCustomMenu(new Rect(Screen.width / 2f, Screen.height / 2f, 200, 40),
                                                new[] { new GUIContent("覆盖首包配置"), new GUIContent("合并首包配置") },
                                                -1, (userData, options, selected) =>
                                                {
                                                    switch (selected)
                                                    {
                                                        case 0:
                                                            OAction();
                                                            break;
                                                        case 1:
                                                            MAction();
                                                            break;
                                                    }
                                                },
                                                null);
                return;

                async void MAction()
                {
                    EditorUtility.DisplayProgressBar("合并首包配置", "正在上传首包配置", 0.5f);
                    if (await config.MergeFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                    {
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("提示", "合并成功", "确定");
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("提示", "合并失败", "确定");
                    }
                }
            }

            OAction();
            return;

            async void OAction()
            {
                EditorUtility.DisplayProgressBar("覆盖首包配置", "正在上传首包配置", 0.5f);
                if (await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("提示", "覆盖成功", "确定");
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("提示", "覆盖失败", "确定");
                }
            }
        }

        /// <summary>
        ///     上传首包 Google Cloud
        /// </summary>
        public static async void UpdateUploadFirstPack(ASBuildConfig.GCloudConfig config)
        {
            await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
        }
    }
}