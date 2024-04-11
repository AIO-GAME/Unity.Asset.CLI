using System.IO;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
using MessageType = UnityEditor.MessageType;

namespace AIO.UEditor
{
    [CustomEditor(typeof(ASConfig))]
    public class ASConfigEditor : AFInspector<ASConfig>
    {
        private bool FoldoutAutoRecord = true;

        private GUILayoutOption OptionLabelWidth_100;
        private GUIStyle        GSValue      => GEStyle.toolbarbutton;
        private GUIStyle        GSBackground => GEStyle.TEToolbar;

        [MenuItem("AIO/Gen/Asset System Config")]
        public static void Create()
        {
            Selection.activeObject = ASConfig.GetOrCreate();
        }

        protected override void OnHeaderGUI()
        {
            var rect = GUILayoutUtility.GetRect(0, 0, 20, 20, GUILayout.ExpandWidth(true));

            var content = new GUIContent(AssetPreview.GetMiniThumbnail(this));
            GUI.Box(rect, GUIContent.none, GEStyle.TEBoxBackground);

            rect.x = rect.width / 2;
            GUI.Label(rect, "Asset System Config", GEStyle.HeaderLabel);

            rect.y     = 3;
            rect.x     = 20;
            rect.width = 30;
            if (GUI.Button(rect, content, GEStyle.IconButton)) EditorApplication.ExecuteMenuItem("AIO/Window/Asset");
        }

        protected override void OnActivation()
        {
            OptionLabelWidth_100 = GUILayout.Width(100);
            UpdateRecordQueue();
        }

        protected override void OnGUI()
        {
            using (GELayout.Vertical(GEStyle.TEBoxBackground))
            {
                using (GELayout.VHorizontal(GSBackground))
                {
                    GELayout.Label("加载模式", GTOption.Width(100));
                    Target.ASMode = GELayout.Popup(Target.ASMode, GEStyle.PreDropDown);
                }

                if (Target.ASMode == EASMode.Remote)
                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.Label("清空运行缓存", OptionLabelWidth_100);
                        if (GELayout.Button("Execute", GSValue))
                        {
                            var sandbox = Path.Combine(EHelper.Path.Project, Target.RuntimeRootDirectory);
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
                    if (GELayout.Button(Target.OutputLog ? "已启用" : "已禁用", GSValue))
                        Target.OutputLog = !Target.OutputLog;
                }

                using (GELayout.VHorizontal(GSBackground))
                {
                    GELayout.Label("定位地址小写", OptionLabelWidth_100);
                    if (GELayout.Button(Target.LoadPathToLower ? "已启用" : "已禁用", GSValue))
                        Target.LoadPathToLower = !Target.LoadPathToLower;
                }

                using (new EditorGUILayout.HorizontalScope(GEStyle.Toolbar))
                {
                    EditorGUILayout.LabelField("可寻址包含扩展名", GUILayout.Width(100));
                    if (GUILayout.Button(Target.HasExtension ? "已启用" : "已禁用", GEStyle.toolbarbuttonRight))
                        Target.HasExtension = !Target.HasExtension;
                }

                using (GELayout.VHorizontal(GSBackground))
                {
                    GELayout.Label(new GUIContent("运行时根目录", "请输入文件夹名"), OptionLabelWidth_100);
                    Target.RuntimeRootDirectory = GELayout.FieldDelayed(Target.RuntimeRootDirectory, GSValue);
                }

                if (Target.RuntimeRootDirectory.Contains("\\") || Target.RuntimeRootDirectory.Contains("/"))
                    GELayout.HelpBox("运行时根目录不能包含路径符号", MessageType.Error);
            }

            GELayout.Space();
            using (GELayout.Vertical(GEStyle.TEBoxBackground))
            {
                switch (Target.ASMode)
                {
                    case EASMode.Remote:
                        OnGUIRemote();
                        break;
                    default:
                        GUI.enabled = false;
                        GELayout.List("资源包配置",
                                      Target.Packages,
                                      config => { config.Name = GELayout.Field(config.Name); },
                                      null);
                        GUI.enabled = true;
                        GELayout.Button("Update", Update);
                        break;
                }
            }
        }

        private void OnGUIRemote()
        {
            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("自动激活清单", OptionLabelWidth_100);
                if (GELayout.Button(Target.AutoSaveVersion ? "已开启" : "已关闭", GSValue))
                    Target.AutoSaveVersion = !Target.AutoSaveVersion;
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("请求附加时间磋", OptionLabelWidth_100);
                if (GELayout.Button(Target.AppendTimeTicks ? "已开启" : "已关闭", GSValue))
                    Target.AppendTimeTicks = !Target.AppendTimeTicks;
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("自动序列记录", OptionLabelWidth_100);
                if (GELayout.Button(Target.EnableSequenceRecord ? "已开启" : "已关闭", GSValue))
                    Target.EnableSequenceRecord = !Target.EnableSequenceRecord;
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("下载失败尝试次数", OptionLabelWidth_100);
                using (GELayout.VHorizontal(GSValue))
                {
                    Target.DownloadFailedTryAgain = GELayout.Slider(Target.DownloadFailedTryAgain, 1, 100);
                }
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("资源加载最大数量", OptionLabelWidth_100);
                using (GELayout.VHorizontal(GSValue))
                {
                    Target.LoadingMaxTimeSlice =
                        GELayout.Slider(Target.LoadingMaxTimeSlice, 72, 4096);
                }
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("请求超时时间", OptionLabelWidth_100);
                using (GELayout.VHorizontal(GSValue))
                {
                    Target.Timeout = GELayout.Slider(Target.Timeout, 3, 180);
                }
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                GELayout.Label("远端资源地址", OptionLabelWidth_100);

                if (!string.IsNullOrEmpty(Target.URL))
                    if (GELayout.Button("打开", GSValue))
                        Application.OpenURL(Target.URL);
            }

            using (GELayout.VHorizontal(GSBackground))
            {
                Target.URL = GELayout.AreaText(Target.URL, GEStyle.ToolbarTextField, GTOption.WidthExpand(true));
            }

            if (Target.EnableSequenceRecord)
            {
                if (EditorApplication.isPlaying)
                    using (GELayout.VHorizontal(GSBackground))
                    {
                        GELayout.LabelPrefix("序列记录");
                        if (!string.IsNullOrEmpty(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Target)))
                        {
                            if (GELayout.Button("Open", GSValue))
                                Application.OpenURL(
                                                    AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Target));

                            if (GELayout.Button("Upload FTP", GSValue))
                                AHandle.FTP.Create("", "", "").UploadFile(
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

                if (FoldoutAutoRecord && Target.SequenceRecord != null)
                    using (GELayout.Vertical(GEStyle.PreBackground))
                    {
                        GELayout.Label($"一共记录 {Target.SequenceRecord.Count} 个文件", GEStyle.HeaderLabel);

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
            foreach (var record in Target.SequenceRecord)
                using (GELayout.Vertical())
                {
                    using (GELayout.VHorizontal())
                    {
                        GELayout.Label($"{++index:000} : {record.PackageName}",
                                       GEStyle.HeaderLabel,
                                       GTOption.WidthMin(10),
                                       GTOption.WidthMax(100));

                        GELayout.Label(record.Location,
                                       GTOption.WidthMin(50));

                        if (GELayout.Button("寻址路径",
                                            GSValue,
                                            GTOption.WidthMin(20),
                                            GTOption.WidthMax(75)))
                            EditorGUIUtility.systemCopyBuffer = record.Location;

                        if (GELayout.Button("资源路径",
                                            GSValue,
                                            GTOption.WidthMin(20),
                                            GTOption.WidthMax(75)))
                            EditorGUIUtility.systemCopyBuffer = record.AssetPath;

                        if (GELayout.Button("定位",
                                            GSValue,
                                            GTOption.WidthMin(20),
                                            GTOption.WidthMax(50)))
                        {
                            var path = record.AssetPath;
                            if (File.Exists(path))
                            {
                                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                                if (obj != null) Selection.activeObject = obj;
                            }
                        }
                    }
                }
        }

        private void UpdateRecordQueue()
        {
            if (Target.Equals(null)) return;
            if (Target.SequenceRecord is null) return;
            // 如果在编辑器下存在本地记录则加载
            if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH)) Target.SequenceRecord.UpdateLocal();
        }

        private void Update()
        {
            var Data = AssetCollectRoot.GetOrCreate().GetNames();
            if (Data.Length == 0) return;
            Target.Packages = new AssetsPackageConfig[Data.Length];
            for (var index = 0; index < Data.Length; index++)
                Target.Packages[index] = new AssetsPackageConfig
                {
                    Name    = Data[index],
                    Version = "-.-.-"
                };

            Target.Packages[0].IsDefault = true;
        }
    }
}