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
        ///     绘制 资源设置模式 导航栏
        /// </summary>
        partial void OnDrawHeaderConfigMode(Rect rect)
        {
            rect.x     = rect.width - 30;
            rect.width = 30;
            if (GUI.Button(rect, GC_SAVE, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Config.SequenceRecord.Save();
                Config.Save();
#if UNITY_2021_1_OR_NEWER
                AssetDatabase.SaveAssetIfDirty(Config);
#else
                AssetDatabase.SaveAssets();
#endif
                if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
            }

            rect.x     -= rect.width;
            rect.width =  30;
            if (GUI.Button(rect, GC_Select_ASConfig, GEStyle.TEtoolbarbutton))
            {
                GUI.FocusControl(null);
                Selection.activeObject = Config;
            }
        }

        /// <summary>
        ///     更新数据 资源设置模式
        /// </summary>
        private void UpdateDataConfigMode()
        {
            AssetProxyEditor.ConvertConfig(Data);
            Config.Packages = new AssetsPackageConfig[Data.Packages.Length];
            for (var index = 0; index < Data.Packages.Length; index++)
                Config.Packages[index] = new AssetsPackageConfig
                {
                    Name    = Data.Packages[index].Name,
                    Version = "-.-.-"
                };

            if (Config.Packages.Length == 0) return;
            Config.Packages[0].IsDefault = true;
        }

        /// <summary>
        ///     绘制 资源设置模式
        /// </summary>
        partial void OnDrawConfigMode(Rect rect)
        {
            ViewConfig.x        = 5;
            ViewConfig.y        = rect.y;
            ViewConfig.height   = rect.height - 5;
            ViewConfig.width    = rect.width - ViewSetting.MaxWidth;
            ViewConfig.MaxWidth = rect.width - ViewSetting.MinWidth;
            ViewConfig.Draw(OnDrawASConfig, GEStyle.INThumbnailShadow);

            ViewSetting.x      = ViewConfig.width + ViewConfig.x + 5;
            ViewSetting.width  = rect.width - ViewSetting.x - 5;
            ViewSetting.height = ViewConfig.height;
            ViewSetting.y      = rect.y;
            ViewSetting.Draw(OnDrawSetting, GEStyle.INThumbnailShadow);
        }

        /// <summary>
        ///     绘制资源设置
        /// </summary>
        partial void OnDrawSetting(Rect rect)
        {
            rect.x      += 5;
            rect.y      += 5;
            rect.width  -= 10;
            rect.height -= 10;
            var cell = new Rect(rect.x + 10, rect.y, 100, 20);
            {
                EditorGUI.LabelField(cell, "资源可寻址", GEStyle.HeaderLabel);
                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, Data.EnableAddressable ? "已开启" : "已关闭", GEStyle.toolbarbuttonRight)) Data.EnableAddressable = !Data.EnableAddressable;
            }

            {
                cell.y     += 21;
                cell.width =  100;
                cell.x     =  rect.x + 10;
                EditorGUI.LabelField(cell, "包含资源GUID", GEStyle.HeaderLabel);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, Data.IncludeAssetGUID ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Data.IncludeAssetGUID = !Data.IncludeAssetGUID;
            }

            {
                cell.y     += 21;
                cell.width =  100;
                cell.x     =  rect.x + 10;
                EditorGUI.LabelField(cell, "唯一Bundle名称", GEStyle.HeaderLabel);
                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, Data.UniqueBundleName ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Data.UniqueBundleName = !Data.UniqueBundleName;
            }

            {
                cell.y     += 21;
                cell.width =  100;
                cell.x     =  rect.x + 10;
                EditorGUI.LabelField(cell, "日志输出", GEStyle.HeaderLabel);
                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, Config.OutputLog ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Config.OutputLog = !Config.OutputLog;
            }

            {
                cell.y     += 21;
                cell.width =  100;
                cell.x     =  rect.x + 10;
                EditorGUI.LabelField(cell, "定位地址小写", GEStyle.HeaderLabel);
                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, Config.LoadPathToLower ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Config.LoadPathToLower = !Config.LoadPathToLower;
            }

            {
                cell.y     += 21;
                cell.width =  100;
                cell.x     =  rect.x + 10;
                EditorGUI.LabelField(cell, "可寻址包含扩展名", GEStyle.HeaderLabel);
                cell.x     += cell.width;
                cell.width =  rect.width - cell.x;
                if (GUI.Button(cell, Config.HasExtension ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Config.HasExtension = !Config.HasExtension;
            }

            if (Config.ASMode == EASMode.Remote)
            {
                {
                    cell.y     += 21;
                    cell.width =  100;
                    cell.x     =  rect.x + 10;
                    EditorGUI.LabelField(cell, "自动激活清单", GEStyle.HeaderLabel);
                    cell.x     += cell.width;
                    cell.width =  rect.width - cell.x;
                    if (GUI.Button(cell, Config.AutoSaveVersion ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Config.AutoSaveVersion = !Config.AutoSaveVersion;
                }

                {
                    cell.y     += 21;
                    cell.width =  100;
                    cell.x     =  rect.x + 10;
                    EditorGUI.LabelField(cell, "请求附加时间磋", GEStyle.HeaderLabel);
                    cell.x     += cell.width;
                    cell.width =  rect.width - cell.x;
                    if (GUI.Button(cell, Config.AppendTimeTicks ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight)) Config.AppendTimeTicks = !Config.AppendTimeTicks;
                }
            }

            {
                cell.y     += 21;
                cell.width =  100;
                cell.x     =  rect.x + 10;
                EditorGUI.LabelField(cell, "首包打包规则", GEStyle.HeaderLabel);
                cell.x                      += cell.width;
                cell.width                  =  rect.width - cell.x;
                Data.SequenceRecordPackRule =  (AssetCollectRoot.PackRule)EditorGUI.EnumPopup(cell, Data.SequenceRecordPackRule, GEStyle.toolbarbuttonRight);
            }
        }

        /// <summary>
        ///     绘制资源配置
        /// </summary>
        partial void OnDrawASConfig(Rect rect)
        {
            rect.x      += 5;
            rect.y      += 5;
            rect.width  -= 10;
            rect.height -= 10;
            var cell = new Rect(rect.x + 10, rect.y, rect.width - rect.x - 10, 20);
            {
                cell.width = 150;
                EditorGUI.LabelField(cell, "加载模式", GEStyle.HeaderLabel);

                cell.x        += cell.width;
                cell.width    =  rect.width - cell.x - 200;
                Config.ASMode =  (EASMode)EditorGUI.EnumPopup(cell, Config.ASMode, GEStyle.toolbarbuttonRight);

                cell.x     += cell.width;
                cell.width =  100;
                if (GUI.Button(cell, "清空运行缓存", GEStyle.toolbarbuttonRight))
                {
                    var sandbox = Path.Combine(EHelper.Path.Project, Config.RuntimeRootDirectory);
                    if (Directory.Exists(sandbox))
                        AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                }

                cell.x     += cell.width;
                cell.width =  100;
                if (GUI.Button(cell, "清空构建缓存", GEStyle.toolbarbuttonRight))
                {
                    var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
                    if (Directory.Exists(sandbox))
                        AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                }
            }


            {
                cell.y     += cell.height;
                cell.width =  150;
                cell.x     =  rect.x + 10;
                GUI.Label(cell, "运行时根目录(文件夹名)", GEStyle.HeaderLabel);

                cell.x                      += cell.width;
                cell.width                  =  rect.width - cell.x;
                Config.RuntimeRootDirectory =  GUI.TextField(cell, Config.RuntimeRootDirectory);
            }

            {
                cell.y     += cell.height;
                cell.x     =  rect.x + 10;
                cell.width =  rect.width - cell.x;
                GUI.Label(cell,
#if UNITY_ANDROID
                          $"编辑器 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}"
#elif UNITY_STANDALONE_WIN
                          $"编辑器 : {EHelper.Path.Project}/{Config.RuntimeRootDirectory}"
#elif UNITY_IPHONE || UNITY_IOS
                          $"编辑器 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}"
#elif UNITY_WEBGL
                          $"编辑器 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}"
#endif
                        , GEStyle.HeaderLabel);
            }

            {
                cell.y     += cell.height;
                cell.x     =  rect.x + 10;
                cell.width =  rect.width - cell.x;
                GUI.Label(cell,
#if UNITY_ANDROID
                          $"运行时 : Application.persistentDataPath/{Config.RuntimeRootDirectory}"
#elif UNITY_STANDALONE_WIN
                          $"运行时 : Application.streamingAssetsPath/{Config.RuntimeRootDirectory}"
#elif UNITY_IPHONE || UNITY_IOS
                          $"运行时 : Application.persistentDataPath/{Config.RuntimeRootDirectory}"
#elif UNITY_WEBGL
                          $"运行时 : Application.persistentDataPath/{Config.RuntimeRootDirectory}"
#endif
                        , GEStyle.HeaderLabel);
            }

            if (string.IsNullOrEmpty(Config.RuntimeRootDirectory)) GUI.enabled = false;

            switch (Config.ASMode)
            {
                case EASMode.Remote:
                {
                    {
                        cell.y     += cell.height;
                        cell.x     =  rect.x + 10;
                        cell.width =  150;
                        GUI.Label(cell, "远端资源地址", GEStyle.HeaderLabel);
                    }
                    using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(Config.URL)))
                    {
                        cell.width = 100;
                        cell.x     = rect.width - cell.width;
                        if (GUI.Button(cell, "跳转首包清单", GEStyle.toolbarbutton))
                        {
                            WindowMode = Mode.LookFirstPackage;
                            UpdateData();
                            GUI.FocusControl(null);
                        }

                        cell.width =  100;
                        cell.x     -= cell.width;
                        if (GUI.Button(cell, "打开远端网页", GEStyle.toolbarbutton))
                        {
                            Application.OpenURL(Config.URL);
                            GUI.FocusControl(null);
                        }
                    }

                    {
                        cell.y      += cell.height;
                        cell.x      =  rect.x + 10;
                        cell.width  =  rect.width - cell.x;
                        cell.height =  50;
                        Config.URL  =  GUI.TextArea(cell, Config.URL);
                    }

                    {
                        cell.y      += cell.height;
                        cell.x      =  rect.x + 10;
                        cell.width  =  150;
                        cell.height =  20;
                        GUI.Label(cell, "下载失败尝试次数", GEStyle.HeaderLabel);
                        cell.x                        += cell.width;
                        cell.width                    =  rect.width - cell.x;
                        Config.DownloadFailedTryAgain =  EditorGUI.IntSlider(cell, Config.DownloadFailedTryAgain, 3, 36);
                    }

                    {
                        cell.y     += cell.height;
                        cell.x     =  rect.x + 10;
                        cell.width =  150;
                        GUI.Label(cell, "资源加载的最大数量", GEStyle.HeaderLabel);
                        cell.x                     += cell.width;
                        cell.width                 =  rect.width - cell.x;
                        Config.LoadingMaxTimeSlice =  EditorGUI.IntSlider(cell, Config.LoadingMaxTimeSlice, 144, 8192);
                    }

                    {
                        cell.y     += cell.height;
                        cell.x     =  rect.x + 10;
                        cell.width =  150;
                        GUI.Label(cell, "请求超时时间", GEStyle.HeaderLabel);
                        cell.x         += cell.width;
                        cell.width     =  rect.width - cell.x;
                        Config.Timeout =  EditorGUI.IntSlider(cell, Config.Timeout, 3, 180);
                    }

                    if (string.IsNullOrEmpty(Config.URL)) GUI.enabled = true;
                    break;
                }
                default:
                    using (new EditorGUI.DisabledGroupScope(false))
                    {
                        cell.y     += cell.height;
                        cell.x     =  rect.x + 10;
                        cell.width =  150;
                        GUI.Label(cell, "资源包配置", GEStyle.HeaderLabel);
                        foreach (var config in Config.Packages)
                        {
                            cell.y     += cell.height;
                            cell.x     =  rect.x + 10;
                            cell.width =  rect.width - 20 - cell.x;
                            GUI.Label(cell, config.Name, GEStyle.HeaderLabel);

                            cell.x           += cell.width;
                            cell.width       =  20;
                            config.IsDefault =  GUI.Toggle(cell, config.IsDefault, "");
                        }
                    }

                    break;
            }

            if (string.IsNullOrEmpty(Config.RuntimeRootDirectory)) GUI.enabled = true;
        }
    }
}