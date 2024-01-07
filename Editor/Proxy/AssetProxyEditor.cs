/*|============|*|
|*|Author:     |*| xinan                
|*|Date:       |*| 2024-01-07               
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
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
                    if (typeof(IAssetProxyEditor).IsAssignableFrom(type))
                    {
                        Editor = (IAssetProxyEditor)Activator.CreateInstance(type);
                        break;
                    }
                }
            }
        }

        public static void CreateConfig(string BundlesDir, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            Editor.CreateConfig(BundlesDir);
        }

        public static void BuildArt(ASBuildConfig config, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            SaveScene();

            Editor.BuildArt(config);
        }

        public static void ConvertConfig(AssetCollectRoot config, bool isTips = false)
        {
            if (Editor is null)
            {
                if (isTips) TipsInstall();
                return;
            }

            Editor.ConvertConfig(config);
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

        private static void TipsInstall()
        {
            // switch (EditorUtility.DisplayDialogComplex(
            //             "提示",
            //             "当前没有导入资源实现工具",
            //             $"导入 YooAsset [{Ghost.YooAsset.Version}]",
            //             "取消",
            //             $"导入 YooAsset [{Ghost.YooAsset.Version}][CN]"))
            // {
            //     case 0:
            //         Ghost.YooAsset.Install();
            //         Console.WriteLine("导入 YooAsset");
            //         return;
            //     case 1: // 取消
            //         break;
            //     case 2:
            //         Ghost.YooAsset.InstallCN();
            //         Console.WriteLine("导入 YooAsset[CN]");
            //         break;
            // }
        }
    }

    public interface IAssetProxyEditor
    {
        string Version { get; }
        string Scopes { get; }
        string Name { get; }

        void ConvertConfig(AssetCollectRoot config);
        void CreateConfig(string BundlesDir);

        void BuildArt(ASBuildConfig config);
    }
}