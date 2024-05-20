using System;
using System.IO;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetWindow
    {
        [LnkTools(Tooltip = "AIO 资源管理工具", IconResource = "Editor/Icon/Asset", ShowMode = ELnkShowMode.Toolbar)]
        public static void OpenWindow() => EditorApplication.ExecuteMenuItem(MENU_WINDOW);

        public const string MENU_ROOT   = "AIO/Asset/";
        public const string MENU_WINDOW = MENU_ROOT + "Window";
        public const string MENU_CONFIG = MENU_ROOT + "Config";

        [MenuItem(MENU_ROOT + "清空运行时缓存")]
        public static void ClearRuntimeCache()
        {
            var sandbox = Path.Combine(EHelper.Path.Project, ASConfig.GetOrCreate().RuntimeRootDirectory);
            if (Directory.Exists(sandbox))
                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
        }

        [MenuItem(MENU_ROOT + "清空构建时缓存")]
        public static void ClearBuildCache()
        {
            var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
            if (Directory.Exists(sandbox))
                AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
        }

        /// <summary>
        ///     上传首包 FTP
        /// </summary>
        public static async void UpdateUploadFirstPack(AssetBuildConfig.FTPConfig config)
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
        public static async void UpdateUploadFirstPack(AssetBuildConfig.GCloudConfig config)
        {
            await config.UploadFirstPack(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged -= EditorQuit;
            EditorApplication.playModeStateChanged += EditorQuit;
        }

        private static void EditorQuit(PlayModeStateChange value)
        {
            if (value != PlayModeStateChange.EnteredPlayMode) return;
            if (!Application.isPlaying) return;

            var config = ASConfig.GetOrCreate();
            if (config.ASMode != EASMode.Editor) return;

            var root = AssetCollectRoot.GetOrCreate();
            if (root is null) throw new Exception($"Not found {nameof(AssetCollectRoot)}.asset ! Please create it !");
            AssetProxyEditor.ConvertConfig(root, false);
            EditorApplication.playModeStateChanged -= EditorQuit;
        }
    }
}