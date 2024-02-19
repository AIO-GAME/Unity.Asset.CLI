/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-19
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        /// 绘制 资源设置模式 导航栏
        /// </summary>
        private void OnDrawHeaderConfigMode()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button(GC_Select_ASConfig, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Selection.activeObject = Config;
            }

            if (GUILayout.Button(GC_SAVE, GEStyle.TEtoolbarbutton, GP_Width_30, GP_Height_20))
            {
                GUI.FocusControl(null);
                Config.Save();
#if UNITY_2021_1_OR_NEWER
                AssetDatabase.SaveAssetIfDirty(Config);
#else
                AssetDatabase.SaveAssets();
#endif
                if (EditorUtility.DisplayDialog("保存", "保存成功", "确定"))
                {
                    AssetDatabase.Refresh();
                }
            }
        }

        /// <summary>
        /// 更新数据 资源设置模式
        /// </summary>
        private void UpdateDataConfigMode()
        {
            AssetProxyEditor.ConvertConfig(Data);
            Config.Packages = new AssetsPackageConfig[Data.Packages.Length];
            for (var index = 0; index < Data.Packages.Length; index++)
            {
                Config.Packages[index] = new AssetsPackageConfig
                {
                    Name = Data.Packages[index].Name,
                    Version = "-.-.-",
                };
            }

            if (Config.Packages.Length == 0) return;
            Config.Packages[0].IsDefault = true;
        }

        /// <summary>
        /// 绘制 资源设置模式
        /// </summary>
        partial void OnDrawConfigMode()
        {
            DoConfigDrawRect.x = 5;
            DoConfigDrawRect.height = CurrentHeight - DrawHeaderHeight;
            DoConfigDrawRect.width = ViewConfig.width;
            ViewConfig.Draw(DoConfigDrawRect, () =>
            {
                ViewConfig.MaxWidth = CurrentWidth - ViewSetting.MinWidth;
                OnDrawConfigScroll = GELayout.VScrollView(OnDrawASConfig, OnDrawConfigScroll);
            }, GEStyle.INThumbnailShadow);

            DoConfigDrawRect.x += ViewConfig.width;
            DoConfigDrawRect.width = CurrentWidth - ViewConfig.width;
            ViewSetting.width = DoConfigDrawRect.width;
            ViewSetting.Draw(DoConfigDrawRect,
                () => { OnDrawSettingScroll = GELayout.VScrollView(OnDrawSetting, OnDrawSettingScroll); },
                GEStyle.INThumbnailShadow);
        }

        /// <summary>
        /// 绘制资源设置
        /// </summary>
        partial void OnDrawSetting()
        {
            var min = GUILayout.MinWidth(ViewSetting.MinWidth - 25);
            EditorGUILayout.LabelField("Setting", GEStyle.HeaderLabel, min);

            using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
            {
                Data.EnableAddressable = EditorGUILayout.ToggleLeft("开启地址化", Data.EnableAddressable,
                    min);
                Data.IncludeAssetGUID = EditorGUILayout.ToggleLeft("包含资源GUID", Data.IncludeAssetGUID,
                    min);
                Data.UniqueBundleName = EditorGUILayout.ToggleLeft("唯一Bundle名称", Data.UniqueBundleName,
                    min);
            }

            EditorGUILayout.Separator();
            using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
            {
                Config.OutputLog = EditorGUILayout.ToggleLeft("开启日志输出", Config.OutputLog,
                    min);
                Config.LoadPathToLower = EditorGUILayout.ToggleLeft("定位地址小写", Config.LoadPathToLower,
                    min);
                if (Config.ASMode == EASMode.Remote)
                {
                    Config.AutoSaveVersion = EditorGUILayout.ToggleLeft("自动激活清单", Config.AutoSaveVersion,
                        min);
                    Config.AppendTimeTicks = EditorGUILayout.ToggleLeft("请求附加时间磋", Config.AppendTimeTicks,
                        min);
                }
            }
        }

        /// <summary>
        /// 绘制资源配置
        /// </summary>
        partial void OnDrawASConfig()
        {
            EditorGUILayout.LabelField("Config", GEStyle.HeaderLabel);
            using (new EditorGUILayout.HorizontalScope(GEStyle.Badge))
            {
                GELayout.Label("加载模式", GP_Width_150);
                Config.ASMode = GELayout.Popup(Config.ASMode, GEStyle.PreDropDown);
                if (GELayout.Button("Clean Cache", GEStyle.toolbarbuttonRight, GP_Width_100))
                {
                    var sandbox = Path.Combine(EHelper.Path.Project, Config.RuntimeRootDirectory);
                    if (Directory.Exists(sandbox))
                        AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                }

                if (GELayout.Button("Clean Bundles", GEStyle.toolbarbuttonRight, GP_Width_100))
                {
                    var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
                    if (Directory.Exists(sandbox))
                        AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                }
            }

            EditorGUILayout.Separator();

            using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GELayout.Label("运行时根目录(文件夹名)", GP_Width_150);
                    Config.RuntimeRootDirectory = GELayout.Field(Config.RuntimeRootDirectory);
                }

#if UNITY_ANDROID
                using (new EditorGUILayout.VerticalScope())
                {
                    GELayout.Label($"编辑器 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}");
                    GELayout.Label($"运行时 : Application.persistentDataPath/{Config.RuntimeRootDirectory}");
                }

#endif

#if UNITY_STANDALONE_WIN
                using (new EditorGUILayout.VerticalScope())
                {
                    GELayout.Label($"编辑器 : {EHelper.Path.Project}/{Config.RuntimeRootDirectory}");
                    GELayout.Label($"运行时 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}");
                }
#endif

#if UNITY_IPHONE || UNITY_IOS
                using (new EditorGUILayout.VerticalScope())
                {
                    GELayout.Label($"编辑器 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}");
                    GELayout.Label($"运行时 : Application.persistentDataPath/{Config.RuntimeRootDirectory}");
                }

#endif
#if UNITY_WEBGL
                using (new EditorGUILayout.VerticalScope())
                {
                    GELayout.Label($"编辑器 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}");
                    GELayout.Label($"运行时 : Application.persistentDataPath/{Config.RuntimeRootDirectory}");
                }
#endif
            }

            if (string.IsNullOrEmpty(Config.RuntimeRootDirectory)) GUI.enabled = false;
            EditorGUILayout.Separator();
            using (new EditorGUILayout.VerticalScope(GEStyle.Badge))
            {
                switch (Config.ASMode)
                {
                    case EASMode.Remote:

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GELayout.Label("远端资源地址", GP_Width_150);
                            if (string.IsNullOrEmpty(Config.URL)) GUI.enabled = false;
                            EditorGUILayout.Separator();
                            if (GELayout.Button("Open", GEStyle.toolbarbutton, GP_Width_50))
                            {
                                Application.OpenURL(Config.URL);
                                GUI.FocusControl(null);
                            }

                            if (GELayout.Button("Switch", GEStyle.toolbarbutton, GP_Width_50))
                            {
                                WindowMode = Mode.LookFirstPackage;
                                UpdateData();
                                GUI.FocusControl(null);
                            }

                            if (string.IsNullOrEmpty(Config.URL)) GUI.enabled = true;
                        }

                        Config.URL = GELayout.AreaText(Config.URL, GUILayout.Height(50));

                        if (string.IsNullOrEmpty(Config.URL)) GUI.enabled = false;
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GELayout.Label("下载失败尝试次数", GP_Width_150);
                            Config.DownloadFailedTryAgain = GELayout.Slider(Config.DownloadFailedTryAgain, 1, 100);
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GELayout.Label("资源加载的最大数量", GP_Width_150);
                            Config.LoadingMaxTimeSlice = GELayout.Slider(Config.LoadingMaxTimeSlice, 144, 8192);
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GELayout.Label("请求超时时间", GP_Width_150);
                            Config.Timeout = GELayout.Slider(Config.Timeout, 3, 180);
                        }

                        if (string.IsNullOrEmpty(Config.URL)) GUI.enabled = true;
                        break;
                    default:
                        GUI.enabled = false;
                        GELayout.List("资源包配置", Config.Packages,
                            config =>
                            {
                                config.Name = GELayout.Field(config.Name);
                                config.IsDefault = GELayout.Toggle(config.IsDefault, GUILayout.Width(20));
                                if (!config.IsDefault) return;
                                foreach (var package in Config.Packages.Where(package => config.Name != package.Name))
                                    package.IsDefault = false;
                            });
                        GUI.enabled = true;
                        break;
                }

                if (string.IsNullOrEmpty(Config.RuntimeRootDirectory)) GUI.enabled = true;
            }
        }
    }
}