/*|============|*|
|*|Author:     |*| xinan
|*|Date:       |*| 2024-01-07
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using System;
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
            if (string.IsNullOrEmpty(currentScene.path)) return;
            var scene = SceneManager.GetSceneByPath(currentScene.path);
            if (!scene.isDirty) return; // 获取当前场景的修改状态
            if (EditorUtility.DisplayDialog("提示", "当前场景未保存,是否保存?", "保存", "取消"))
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
                position = new Rect(
                    Screen.currentResolution.width / 2,
                    Screen.currentResolution.height / 2,
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
        }
    }
}