using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        [AInit(mode: EInitAttrMode.Both, int.MaxValue)]
        public static void Initialize()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.GetName().Name.Contains("Editor")) continue;
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

        public static async Task<bool> UploadGCloud(ICollection<AsUploadGCloudParameter> parameters,
            bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return false;
            }

            var sw = Stopwatch.StartNew();
            foreach (var parameter in parameters)
            {
                if (!AHelper.IO.ExistsDir(parameter.LocalFullPath))
                    throw new Exception($"本地文件夹不存在 -> {parameter.LocalFullPath}");

                if (!string.IsNullOrEmpty(parameter.GCLOUD_PATH)) PrGCloud.Gcloud = parameter.GCLOUD_PATH;
                if (!string.IsNullOrEmpty(parameter.GSUTIL_PATH)) PrGCloud.Gsutil = parameter.GSUTIL_PATH;
            }

            var succeed = await Editor.UploadGCloud(parameters);
            var info = $"{(succeed ? "资源上传完成" : "资源上传失败")} 一共耗时 : {sw.Elapsed.TotalSeconds:F2} 秒";
            EHelper.DisplayDialog("消息", info, "确定");
            return succeed;
        }

        /// <summary>
        /// 上传到GCloud
        /// </summary>
        public static async Task<bool> UploadGCloud(AsUploadGCloudParameter parameter, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return false;
            }

            if (!AHelper.IO.ExistsDir(parameter.LocalFullPath))
                throw new Exception($"本地文件夹不存在 -> {parameter.LocalFullPath}");

            if (!string.IsNullOrEmpty(parameter.GCLOUD_PATH)) PrGCloud.Gcloud = parameter.GCLOUD_PATH;
            if (!string.IsNullOrEmpty(parameter.GSUTIL_PATH)) PrGCloud.Gsutil = parameter.GSUTIL_PATH;

            var sw = Stopwatch.StartNew();
            var succeed = await Editor.UploadGCloud(new[] { parameter });
            var info = $"{(succeed ? "资源上传完成" : "资源上传失败")} 一共耗时 : {sw.Elapsed.TotalSeconds:F2} 秒";
            EHelper.DisplayDialog("消息", info, "确定");
            return succeed;
        }


        /// <summary>
        /// 上传到Ftp
        /// </summary>
        public static async Task<bool> UploadFtp(AsUploadFtpParameter parameter, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return false;
            }

            var sw = Stopwatch.StartNew();
            var succeed = await Editor.UploadFtp(new[] { parameter });
            var info = $"{(succeed ? "资源上传完成" : "资源上传失败")} 一共耗时 : {sw.Elapsed.TotalSeconds:F2} 秒";
            EHelper.DisplayDialog("消息", info, "确定");
            return succeed;
        }

        public static bool BuildArtAll(ASBuildConfig config, bool isTips = false)
        {
            var command = new AssetBuildCommand
            {
                PackageVersion = config.BuildVersion,
                BuildPackage = config.PackageName,
                CompressOption = config.CompressedMode,
                ActiveTarget = config.BuildTarget,
                BuildPipeline = config.BuildPipeline,
                OutputRoot = config.BuildOutputPath,
                BuildMode = config.BuildMode,
                CopyBuildInFileTags = config.FirstPackTag,
                MergeToLatest = config.MergeToLatest,
            };
            var array = AssetCollectRoot.GetOrCreate().GetPackageNames();
            if (config.BuildFirstPackage) array = array.Add(AssetSystem.TagsRecord);
            return BuildArtList(array, command, isTips);
        }

        /// <summary>
        /// 构建资源
        /// </summary>
        /// <param name="config"></param>
        /// <param name="isTips"></param>
        public static bool BuildArt(ASBuildConfig config, bool isTips = false)
        {
            var command = new AssetBuildCommand
            {
                PackageVersion = config.BuildVersion,
                BuildPackage = config.PackageName,
                CompressOption = config.CompressedMode,
                ActiveTarget = config.BuildTarget,
                BuildPipeline = config.BuildPipeline,
                OutputRoot = config.BuildOutputPath,
                BuildMode = config.BuildMode,
                CopyBuildInFileTags = config.FirstPackTag,
                MergeToLatest = config.MergeToLatest,
            };
            if (config.BuildFirstPackage) command.BuildPackage = AssetSystem.TagsRecord;
            return BuildArt(command, isTips);
        }

        /// <summary>
        /// 构建所有资源
        /// </summary>
        public static bool BuildArtList(IEnumerable<string> packageNames, AssetBuildCommand command,
            bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return false;
            }

            SaveScene();
            var sw = Stopwatch.StartNew();
            var enumerable = packageNames as string[] ?? packageNames.ToArray();
            var succeed = Editor.BuildArtList(enumerable, command);
            var info =
                $"构建 {string.Join(",", enumerable)} {(succeed ? "成功" : "失败")} 一共耗时 : {sw.Elapsed.TotalSeconds:F2} 秒";
            EHelper.DisplayDialog("构建成功", info, "确定");
            return succeed;
        }

        public static bool BuildArt(AssetBuildCommand command, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return false;
            }

            SaveScene();
            var sw = Stopwatch.StartNew();
            var succeed = Editor.BuildArt(command);
            var info = $"构建 {command.BuildPackage} {(succeed ? "成功" : "失败")} 一共耗时 : {sw.Elapsed.TotalSeconds:F2} 秒";
            EHelper.DisplayDialog("构建成功", info, "确定");
            return succeed;
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
                EditorGUILayout.Separator();
                using (new EditorGUILayout.HorizontalScope())
                {
                    type = (Types)EditorGUILayout.EnumPopup(type);
                    isCN = EditorGUILayout.ToggleLeft("国区", isCN, GUILayout.Width(45));
                }

                EditorGUILayout.Separator();
                if (GUILayout.Button("确定"))
                {
                    switch (type)
                    {
                        case Types.YooAsset:
                            EHelper.Ghost.InstallOpenUpm("com.tuyoogame.yooasset", "1.5.7", isCN);
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