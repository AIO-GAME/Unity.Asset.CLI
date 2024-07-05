using System;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    using MessageType = MessageType;
    using Object = Object;

    public class AssetConfigCommonEditor
    {
        private GUILayoutOption OptionLabelWidth_100;

        private GUIStyle   GSValue      => GEStyle.toolbarbutton;
        private GUIStyle   GSBackground => GEStyle.TEToolbar;
        private GUIContent HeaderContent;
        private Texture2D  HeaderIcon;

        public void OnActivation()
        {
            ASConfig = ASConfig.GetOrCreate();
            ABConfig = AssetBuildConfig.GetOrCreate();
            ACConfig = AssetCollectRoot.GetOrCreate();

            OptionLabelWidth_100 = GTOptions.Width(100);
            HeaderContent        = new GUIContent("资源系统 配置管理", "资源系统配置");

            HeaderIcon = AssetPreview.GetMiniThumbnail(ASConfig);
            UpdateRecordQueue();
            PackageNames     = ACConfig.GetNames() ?? Array.Empty<string>();
            PackageNameIndex = PackageNames.ToList().IndexOf(ABConfig.PackageName);
        }

        public void OnGUI()
        {
            GUILayout.Space(10);
            FoldoutASConfig = GELayout.VFoldoutHeader(OnDrawASConfig, "运行配置", FoldoutASConfig);
            GUILayout.Space(10);
            FoldoutABConfig = GELayout.VFoldoutHeader(OnDrawABConfig, "构建配置", FoldoutABConfig);
            GUILayout.Space(10);
            FoldoutACConfig = GELayout.VFoldoutHeader(OnDrawACConfig, "收集器配置", FoldoutACConfig);
        }

        public void OnHeaderGUI()
        {
            var rect = GUILayoutUtility.GetRect(0, 0, 20, 27, GTOptions.WidthExpand(true));
            GUI.Box(rect, GUIContent.none, GEStyle.INThumbnailShadow);

            rect.x = rect.width / 2 - GEStyle.HeaderLabel.CalcSize(HeaderContent).x / 2;
            GUI.Label(rect, HeaderContent, GEStyle.HeaderLabel);

            rect.x     -= 30;
            rect.width =  30;
            if (GUI.Button(rect, HeaderIcon, GEStyle.IconButton)) EditorApplication.ExecuteMenuItem(AssetWindow.MENU_WINDOW);
        }

        #region AssetCollectRoot

        private bool             FoldoutACConfig = true;
        private AssetCollectRoot ACConfig;
        private string[]         PackageNames;

        private void OnDrawACConfig()
        {
            GUILayout.Space(5);
            using (GELayout.Vertical(GEStyle.TEBoxBackground))
                if (ACConfig)
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField(string.Empty, GTOptions.Width(100));
                        if (GUILayout.Button("保存", GEStyle.toolbarbuttonRight))
                        {
                            GUI.FocusControl(null);
                            ACConfig.Save();
#if UNITY_2021_1_OR_NEWER
                            AssetDatabase.SaveAssetIfDirty(ACConfig);
#else
                            AssetDatabase.SaveAssets();
#endif
                            if (EditorUtility.DisplayDialog("保存", "保存成功", "确定")) AssetDatabase.Refresh();
                        }

                        if (GUILayout.Button("打开", GEStyle.toolbarbuttonRight))
                        {
                            AssetWindow.OpenPage<AssetPageEditCollect>();
                            EditorApplication.ExecuteMenuItem(AssetWindow.MENU_WINDOW);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("开启可寻址路径", GTOptions.Width(100));
                        if (GUILayout.Button(ACConfig.EnableAddressable ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight))
                            ACConfig.EnableAddressable = !ACConfig.EnableAddressable;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("包含资源GUID", GTOptions.Width(100));
                        if (GUILayout.Button(ACConfig.IncludeAssetGUID ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight))
                            ACConfig.IncludeAssetGUID = !ACConfig.IncludeAssetGUID;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("唯一资源包名", GTOptions.Width(100));
                        if (GUILayout.Button(ACConfig.UniqueBundleName ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight))
                            ACConfig.UniqueBundleName = !ACConfig.UniqueBundleName;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("首包资源规则", GTOptions.Width(100));
                        ACConfig.SequenceRecordPackRule = GELayout.Popup(ACConfig.SequenceRecordPackRule, GEStyle.toolbarbuttonRight);
                    }
                }
                else
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("创建配置", GTOptions.Width(100));
                        if (GUILayout.Button("Create", GEStyle.toolbarbuttonRight)) Selection.activeObject = AssetCollectRoot.GetOrCreate();
                    }
                }
        }

        #endregion

        #region ASBuildConfigEditor

        private AssetBuildConfig ABConfig;
        private bool             FoldoutABConfig = true;
        private int              PackageNameIndex;

        private void OnDrawABConfig()
        {
            GUILayout.Space(5);
            using (GELayout.Vertical(GEStyle.TEBoxBackground))
            {
                if (ABConfig)
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField(string.Empty, GTOptions.Width(125));
                        if (GUILayout.Button("保存", GEStyle.toolbarbuttonRight))
                        {
                            EditorUtility.SetDirty(ABConfig);
                            AssetDatabase.SaveAssets();
                        }

                        if (GUILayout.Button("打开", GEStyle.toolbarbuttonRight))
                        {
                            AssetWindow.OpenPage<AssetPageEditBuild>();
                            EditorApplication.ExecuteMenuItem(AssetWindow.MENU_WINDOW);
                        }

                        if (GUILayout.Button("构建", GEStyle.toolbarbutton))
                        {
                            AssetProxyEditor.BuildArt(ABConfig);
                            GUIUtility.ExitGUI();
                            return;
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("构建平台", GTOptions.Width(125));
                        ABConfig.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(ABConfig.BuildTarget, GEStyle.TEToolbarDropDown);
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("打包管线", GTOptions.Width(125));
                        ABConfig.BuildPipeline =
                            (EBuildPipeline)EditorGUILayout.EnumPopup(ABConfig.BuildPipeline, GEStyle.TEToolbarDropDown);
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("构建模式", GTOptions.Width(125));
                        ABConfig.BuildMode = (EBuildMode)EditorGUILayout.EnumPopup(ABConfig.BuildMode, GEStyle.TEToolbarDropDown);
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("压缩模式", GTOptions.Width(125));
                        ABConfig.CompressedMode = (ECompressMode)EditorGUILayout.EnumPopup(ABConfig.CompressedMode, GEStyle.TEToolbarDropDown);
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("版本号", GTOptions.Width(125));
                        using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                        {
                            ABConfig.BuildVersion = EditorGUILayout.TextField(ABConfig.BuildVersion);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("构建资源包名称", GTOptions.Width(125));
                        PackageNameIndex = EditorGUILayout.Popup(PackageNameIndex, PackageNames, GEStyle.PreDropDown);
                        using (var scope = new EditorGUI.ChangeCheckScope())
                        {
                            if (scope.changed)
                            {
                                var temp = ACConfig.GetNames();
                                ABConfig.PackageName = temp.Length > PackageNameIndex ? temp[PackageNameIndex] : string.Empty;
                            }
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("加密模式", GTOptions.Width(125));
                        using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                        {
                            ABConfig.EncryptionClassName = EditorGUILayout.TextField(ABConfig.EncryptionClassName);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("首包标签集合", GTOptions.Width(125));
                        using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                        {
                            ABConfig.FirstPackTag = EditorGUILayout.TextField(ABConfig.FirstPackTag);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("构建结果输出路径", GTOptions.Width(125));
                        using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                        {
                            ABConfig.BuildOutputPath = EditorGUILayout.TextField(ABConfig.BuildOutputPath);
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("验证构建结果", GTOptions.Width(125));
                        if (GUILayout.Button(ABConfig.ValidateBuild ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                            ABConfig.ValidateBuild = !ABConfig.ValidateBuild;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("资源包合并至Latest", GTOptions.Width(125));
                        if (GUILayout.Button(ABConfig.MergeToLatest ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                            ABConfig.MergeToLatest = !ABConfig.MergeToLatest;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("导至StreamingAssets", GTOptions.Width(125));
                        if (GUILayout.Button(ABConfig.ExportToStreamingAssets ? "已启用" : "已禁用", GEStyle.toolbarbutton))
                            ABConfig.ExportToStreamingAssets = !ABConfig.ExportToStreamingAssets;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("自动清理缓存数量", GTOptions.Width(125));
                        using (new EditorGUILayout.HorizontalScope(GEStyle.toolbarbutton))
                        {
                            ABConfig.AutoCleanCacheNumber = EditorGUILayout.IntSlider(ABConfig.AutoCleanCacheNumber, 3, 10);
                        }
                    }
                }
                else
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("创建配置", GTOptions.Width(100));
                        if (GUILayout.Button("Create", GEStyle.toolbarbuttonRight)) Selection.activeObject = AssetBuildConfig.GetOrCreate();
                    }
                }
            }
        }

        #endregion

        #region ASConfig

        private ASConfig ASConfig;
        private bool     FoldoutASConfig   = true;
        private bool     FoldoutAutoRecord = true;

        private void OnDrawASConfig()
        {
            GUILayout.Space(5);
            using (GELayout.Vertical(GEStyle.TEBoxBackground))
            {
                if (ASConfig)
                {
                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label("加载模式", GTOptions.Width(100));
                        ASConfig.ASMode = GELayout.Popup(ASConfig.ASMode, GEStyle.PreDropDown);
                    }

                    if (ASConfig.ASMode == EASMode.Remote)
                        using (GELayout.VHorizontal(GSBackground))
                        {
                            GELayout.Label("清空运行缓存", OptionLabelWidth_100);
                            if (GELayout.Button("Execute", GSValue))
                            {
                                var sandbox = Path.Combine(EHelper.Path.Project, ASConfig.RuntimeRootDirectory);
                                if (Directory.Exists(sandbox))
                                    AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                            }
                        }

                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label("清空构建缓存", OptionLabelWidth_100);
                        if (GELayout.Button("Execute", GSValue))
                        {
                            var sandbox = Path.Combine(EHelper.Path.Project, "Bundles");
                            if (Directory.Exists(sandbox)) AHelper.IO.DeleteDir(sandbox, SearchOption.AllDirectories, true);
                        }
                    }

                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label("开启日志输出", OptionLabelWidth_100);
                        if (GELayout.Button(ASConfig.OutputLog ? "已启用" : "已禁用", GSValue))
                            ASConfig.OutputLog = !ASConfig.OutputLog;
                    }

                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label("定位地址小写", OptionLabelWidth_100);
                        if (GELayout.Button(ASConfig.LoadPathToLower ? "已启用" : "已禁用", GSValue))
                            ASConfig.LoadPathToLower = !ASConfig.LoadPathToLower;
                    }

                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("可寻址包含扩展名", GUILayout.Width(100));
                        if (GUILayout.Button(ASConfig.HasExtension ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight))
                            ASConfig.HasExtension = !ASConfig.HasExtension;
                    }

                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label(new GUIContent("运行时根目录", "请输入文件夹名"), OptionLabelWidth_100);
                        ASConfig.RuntimeRootDirectory = GELayout.FieldDelayed(ASConfig.RuntimeRootDirectory, GSValue);
                    }

                    if (ASConfig.RuntimeRootDirectory.Contains("\\") || ASConfig.RuntimeRootDirectory.Contains("/"))
                        GELayout.HelpBox("运行时根目录不能包含路径符号", MessageType.Error);

                    GUILayout.Space(5);
                    using (GELayout.Vertical(GEStyle.TEBoxBackground))
                    {
                        switch (ASConfig.ASMode)
                        {
                            case EASMode.Remote:
                                OnGUIRemote();
                                break;
                            default:
                                using (new EditorGUI.DisabledScope(true))
                                {
                                    GELayout.List("资源包配置", ASConfig.Packages, config => { config.Name = EditorGUILayout.TextField(config.Name); },
                                                  null);
                                }

                                GELayout.Button("Update", Update);
                                break;
                        }
                    }
                }
                else
                {
                    using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                    {
                        EditorGUILayout.LabelField("创建配置", GTOptions.Width(100));
                        if (GUILayout.Button("Create", GEStyle.toolbarbuttonRight)) Selection.activeObject = ASConfig.GetOrCreate();
                    }
                }
            }
        }

        private Vector2 Vector;

        private void OnGUIRemote()
        {
            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("自动激活清单", OptionLabelWidth_100);
                if (GELayout.Button(ASConfig.AutoSaveVersion ? "已开启" : "已关闭", GSValue))
                    ASConfig.AutoSaveVersion = !ASConfig.AutoSaveVersion;
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("请求附加时间磋", OptionLabelWidth_100);
                if (GELayout.Button(ASConfig.AppendTimeTicks ? "已开启" : "已关闭", GSValue))
                    ASConfig.AppendTimeTicks = !ASConfig.AppendTimeTicks;
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("自动序列记录", OptionLabelWidth_100);
                if (GELayout.Button(ASConfig.EnableSequenceRecord ? "已开启" : "已关闭", GSValue))
                    ASConfig.EnableSequenceRecord = !ASConfig.EnableSequenceRecord;
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("下载失败尝试次数", OptionLabelWidth_100);
                using (GELayout.VHorizontal(GSValue))
                {
                    ASConfig.DownloadFailedTryAgain = GELayout.Slider(ASConfig.DownloadFailedTryAgain, 1, 100);
                }
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("资源加载最大数量", OptionLabelWidth_100);
                using (GELayout.VHorizontal(GSValue))
                {
                    ASConfig.LoadingMaxTimeSlice =
                        GELayout.Slider(ASConfig.LoadingMaxTimeSlice, 72, 4096);
                }
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("请求超时时间", OptionLabelWidth_100);
                using (GELayout.VHorizontal(GSValue))
                {
                    ASConfig.Timeout = GELayout.Slider(ASConfig.Timeout, 3, 180);
                }
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("远端资源地址", OptionLabelWidth_100);

                if (!string.IsNullOrEmpty(ASConfig.URL))
                    if (GELayout.Button("打开", GSValue))
                        Application.OpenURL(ASConfig.URL);
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                ASConfig.URL = GELayout.AreaText(ASConfig.URL, GEStyle.ToolbarTextField, GTOptions.WidthExpand(true));
            }

            if (ASConfig.EnableSequenceRecord)
            {
                if (EditorApplication.isPlaying)
                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.LabelPrefix("序列记录");
                        if (!string.IsNullOrEmpty(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(ASConfig)))
                        {
                            if (GELayout.Button("Open", GSValue))
                                Application.OpenURL(
                                                    AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(ASConfig));

                            if (GELayout.Button("Upload FTP", GSValue))
                                AHandle.FTP.Create("", "", "")
                                       .UploadFile(
                                                   AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                        }
                    }

                using (GELayout.VHorizontal(GSBackground))
                {
                    GELayout.Label("序列记录", OptionLabelWidth_100);
                    if (GELayout.Button(FoldoutAutoRecord ? "隐藏" : "显示", GSValue))
                        FoldoutAutoRecord = !FoldoutAutoRecord;

                    if (GELayout.Button("更新", GSValue)) UpdateRecordQueue();
                }

                if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label("本地序列配置", OptionLabelWidth_100);
                        if (GELayout.Button("打开", GSValue))
                            EditorUtility.OpenWithDefaultApp(AssetSystem.SequenceRecordQueue.LOCAL_PATH);

                        if (GELayout.Button("删除", GSValue))
                            AHelper.IO.DeleteFile(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                    }

                if (FoldoutAutoRecord && ASConfig.SequenceRecord != null)
                    using (GELayout.Vertical(GEStyle.PreBackground))
                    {
                        GELayout.Label($"一共记录 {ASConfig.SequenceRecord.Count} 个文件", GEStyle.HeaderLabel);

                        using (var scope = GELayout.VScrollView(Vector))
                        {
                            Vector = scope.scrollPosition;
                            OnGUISequenceRecord();
                        }
                    }
            }
        }

        private void OnGUISequenceRecord()
        {
            var index = 0;
            foreach (var record in ASConfig.SequenceRecord)
                using (GELayout.Vertical())
                {
                    using (GELayout.VHorizontal())
                    {
                        GELayout.Label($"{++index:000} : {record.PackageName}",
                                       GEStyle.HeaderLabel,
                                       GTOptions.WidthMin(10),
                                       GTOptions.WidthMax(100));

                        GELayout.Label(record.Location,
                                       GTOptions.WidthMin(50));

                        if (GELayout.Button("寻址路径",
                                            GSValue,
                                            GTOptions.WidthMin(20),
                                            GTOptions.WidthMax(75)))
                            EditorGUIUtility.systemCopyBuffer = record.Location;

                        if (GELayout.Button("资源路径",
                                            GSValue,
                                            GTOptions.WidthMin(20),
                                            GTOptions.WidthMax(75)))
                            EditorGUIUtility.systemCopyBuffer = record.AssetPath;

                        if (GELayout.Button("定位",
                                            GSValue,
                                            GTOptions.WidthMin(20),
                                            GTOptions.WidthMax(50)))
                        {
                            var path = record.AssetPath;
                            if (File.Exists(path))
                            {
                                var obj                                 = AssetDatabase.LoadAssetAtPath<Object>(path);
                                if (obj != null) Selection.activeObject = obj;
                            }
                        }
                    }
                }
        }

        private void UpdateRecordQueue()
        {
            if (ASConfig.Equals(null)) return;
            if (ASConfig.SequenceRecord is null) return;
            // 如果在编辑器下存在本地记录则加载
            if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH)) ASConfig.SequenceRecord.UpdateLocal();
        }

        private void Update()
        {
            var Data = AssetCollectRoot.GetOrCreate().GetNames();
            if (Data.Length == 0) return;
            ASConfig.Packages = new AssetsPackageConfig[Data.Length];
            for (var index = 0; index < Data.Length; index++)
                ASConfig.Packages[index] = new AssetsPackageConfig
                {
                    Name    = Data[index],
                    Version = "-.-.-"
                };

            ASConfig.Packages[0].IsDefault = true;
        }

        #endregion
    }
}
