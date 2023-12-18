/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-05
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AssetCollectWindow
    {
        private bool FoldoutAutoRecord;
        private List<AssetsPackageConfig> _packages;

        partial void OnDrawASConfig()
        {
            var width = GTOption.Width(ViewConfig.width - 20);
            using (GELayout.Vertical(GEStyle.GridList, width))
            {
                using (GELayout.VHorizontal())
                {
                    GELayout.Label("⇘", GEStyle.TEtoolbarbutton, GP_Width_20);
                    if (GELayout.Button("Config", GEStyle.PreToolbar))
                    {
                        ViewConfig.IsShow = false;
                        GUI.FocusControl(null);
                    }
                }

                using (GELayout.VHorizontal())
                {
                    GELayout.Label("加载模式", GP_Width_150);
                    Config.ASMode = GELayout.Popup(Config.ASMode, GEStyle.PreDropDown);
                    if (GELayout.Button("Clean Sandbox", GEStyle.toolbarbutton, GP_Width_100))
                    {
                        var sandbox = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Sandbox");
                        if (Directory.Exists(sandbox))
                            AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                    }

                    if (GELayout.Button("Clean Bundles", GEStyle.toolbarbutton, GP_Width_100))
                    {
                        var sandbox = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Bundles");
                        if (Directory.Exists(sandbox))
                            AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                    }
                }


                switch (Config.ASMode)
                {
                    case EASMode.Remote:
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

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GELayout.Label("远端资源地址", GP_Width_150);
                            if (!string.IsNullOrEmpty(Config.URL))
                            {
                                GELayout.Separator();
                                if (GELayout.Button("Open", GP_Width_50))
                                    Application.OpenURL(Config.URL);
                            }
                        }

                        Config.URL = GELayout.AreaText(Config.URL, GUILayout.Height(50));
                        break;
                    default:
                        GELayout.List("资源包配置", _packages,
                            config =>
                            {
                                config.Name = GELayout.Field(config.Name);
                                config.IsDefault = GELayout.Toggle(config.IsDefault, GUILayout.Width(20));
                                if (!config.IsDefault) return;
                                foreach (var package in _packages.Where(package => config.Name != package.Name))
                                    package.IsDefault = false;
                            },
                            () => new AssetsPackageConfig());
                        GELayout.Button("Update", UpdateConfig);
                        break;
                }


                Config.OutputLog = GELayout.ToggleLeft("开启日志输出", Config.OutputLog);
                Config.LoadPathToLower = GELayout.ToggleLeft("定位地址小写", Config.LoadPathToLower);
                if (Config.ASMode == EASMode.Remote)
                {
                    Config.AutoSaveVersion = GELayout.ToggleLeft("自动激活清单", Config.AutoSaveVersion);
                    Config.AppendTimeTicks = GELayout.ToggleLeft("请求附加时间磋", Config.AppendTimeTicks);
                }
            }
        }

        private void UpdateConfig()
        {
            Config.UpdatePackage();
            _packages = Config.Packages is null
                ? _packages
                : Config.Packages.ToList();
        }
    }
}