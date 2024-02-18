/*|============|*|
|*|Author:     |*| xinan
|*|Date:       |*| 2024-01-07
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源 代理 编辑器
    /// </summary>
    public static class AssetProxyEditor
    {
        private static IAssetProxyEditor Editor;

        static AssetProxyEditor()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    if (!typeof(IAssetProxyEditor).IsAssignableFrom(type)) continue;
                    Editor = (IAssetProxyEditor)Activator.CreateInstance(type);
                    break;
                }
            }
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        /// <param name="BundlesDir">资源构建目录</param>
        /// <param name="MergeToLatest">是否合并为latest版本</param>
        /// <param name="isTips">提示</param>
        public static void CreateConfig(string BundlesDir, bool MergeToLatest = false, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            Editor.CreateConfig(BundlesDir, MergeToLatest);
        }

        /// <summary>
        /// 上传到GCloud
        /// </summary>
        public static async Task UploadGCloud(ASUploadGCloudConfig config, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            PrGCloud.Gcloud = string.IsNullOrEmpty(config.GCLOUD_PATH) ? "gcloud" : config.GCLOUD_PATH;
            PrGCloud.Gsutil = string.IsNullOrEmpty(config.GSUTIL_PATH) ? "gsutil" : config.GSUTIL_PATH;
            await Editor.UploadGCloud(config);
        }


        /// <summary>
        /// 上传到Ftp
        /// </summary>
        public static async Task UploadFtp(ASUploadFTPConfig config, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            await Editor.UploadFtp(config);
        }

        /// <summary>
        /// 构建资源
        /// </summary>
        /// <param name="config"></param>
        /// <param name="isTips"></param>
        public static void BuildArt(ASBuildConfig config, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            SaveScene();

            var command = new AssetBuildCommand
            {
                PackageVersion = config.BuildVersion,
                BuildPackage = config.PackageName,
                CompressOption = config.CompressedMode,
                ActiveTarget = config.BuildTarget,
                BuildPipeline = config.BuildPipeline,
                OutputRoot = config.BuildOutputPath,
                BuildMode = config.BuildMode,
                CopyBuildinFileTags = config.FirstPackTag,
                MergeToLatest = config.MergeToLatest,
            };
            if (config.BuildFirstPackage) command.BuildPackage = AssetSystem.TagsRecord;
            Editor.BuildArt(command);
        }

        public static void BuildArt(AssetBuildCommand command, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            SaveScene();

            Editor.BuildArt(command);
        }

        /// <summary>
        /// 转换配置
        /// </summary>
        /// <param name="config">配饰文件</param>
        /// <param name="ignoreTips">忽略提示</param>
        /// <exception cref="Exception">异常</exception>
        public static void ConvertConfig(AssetCollectRoot config, bool ignoreTips = true)
        {
            if (Editor is null)
            {
                if (ignoreTips) return;
                TipsInstall();
                throw new Exception("未安装 第三方 插件");
            }

            Editor.ConvertConfig(config);
        }

        private static void SaveScene()
        {
            var currentScene = SceneManager.GetSceneAt(0);
            if (string.IsNullOrEmpty(currentScene.path)) return;
            var scene = SceneManager.GetSceneByPath(currentScene.path);
            if (!scene.isDirty) return; // 获取当前场景的修改状态
            if (EHelper.IsCMD()) EditorSceneManager.SaveScene(scene);
            else if (EditorUtility.DisplayDialog("提示", "当前场景未保存,是否保存?", "保存", "取消"))
                EditorSceneManager.SaveScene(scene);
        }

        private class InstallPopup : EditorWindow
        {
            private enum Types
            {
                [InspectorName("YooAsset [Latest]")] YooAsset,
            }

            private bool isCN;
            private Types type;

            private void Awake()
            {
                // 位置显示在屏幕中间
                var temp = Screen.currentResolution;
                position = new Rect(
                    temp.width / 2f,
                    temp.height / 2f,
                    300,
                    50);
            }

            private void OnGUI()
            {
                GELayout.Separator();
                using (GELayout.VHorizontal())
                {
                    type = GELayout.Popup(type);
                    isCN = GELayout.ToggleLeft("国区", isCN, GUILayout.Width(45));
                }

                GELayout.Separator();
                if (GELayout.Button("确定"))
                {
                    switch (type)
                    {
                        case Types.YooAsset:
                            EHelper.Ghost.OpenupmInstall("com.tuyoogame.yooasset", "1.5.7", isCN);
                            break;
                    }

                    Close();
                }
            }
        }

        private static void TipsInstall()
        {
            var window = ScriptableObject.CreateInstance<InstallPopup>();
            window.titleContent = new GUIContent("提示");
            window.ShowUtility();
            window.Focus();
        }
    }
}