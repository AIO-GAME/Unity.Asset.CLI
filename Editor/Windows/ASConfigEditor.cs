/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-11-21
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    [CustomEditor(typeof(ASConfig))]
    public class ASConfigEditor : NILInspector<ASConfig>
    {
        [MenuItem("AIO/Gen/Asset System Config")]
        public static void Create()
        {
            Selection.activeObject = ASConfig.GetOrCreate();
        }

        private List<AssetsPackageConfig> _packages;
        private bool FoldoutAutoRecord = true;
        private string RecordQueueSizeStr = "0 bytes";

        protected override void OnActivation()
        {
            if (_packages is null)
                _packages = Target.Packages is null
                    ? new List<AssetsPackageConfig>()
                    : Target.Packages.ToList();

            UpdateRecordQueue();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnGUI()
        {
            using (GELayout.VHorizontal())
            {
                if (GELayout.Button("Clean Sandbox"))
                {
                    var sandbox = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Sandbox");
                    if (Directory.Exists(sandbox)) AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                }

                if (GELayout.Button("Clean Bundles"))
                {
                    var sandbox = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Bundles");
                    if (Directory.Exists(sandbox)) AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                }
            }

            Target.ASMode = GELayout.Popup("加载模式", Target.ASMode);
            Target.OutputLog = GELayout.ToggleLeft("开启日志输出", Target.OutputLog);
            Target.LoadPathToLower = GELayout.ToggleLeft("定位地址小写", Target.LoadPathToLower);

            switch (Target.ASMode)
            {
                case EASMode.Remote:
                    Target.AutoSaveVersion = GELayout.ToggleLeft("自动激活清单", Target.AutoSaveVersion);
                    Target.AppendTimeTicks = GELayout.ToggleLeft("请求附加时间磋", Target.AppendTimeTicks);
                    Target.EnableSequenceRecord = GELayout.ToggleLeft("自动序列记录", Target.EnableSequenceRecord);
                    Target.DownloadFailedTryAgain = GELayout.Slider("下载失败尝试次数", Target.DownloadFailedTryAgain, 1, 100);
                    Target.LoadingMaxTimeSlice = GELayout.Slider("资源加载的最大数量", Target.LoadingMaxTimeSlice, 144, 8192);
                    Target.Timeout = GELayout.Slider("请求超时时间", Target.Timeout, 3, 180);
                    using (new EditorGUILayout.HorizontalScope(GEStyle.DropzoneStyle))
                    {
                        GELayout.Label("远端资源地址");
                        if (!string.IsNullOrEmpty(Target.URL))
                        {
                            if (GELayout.Button("Open")) Application.OpenURL(Target.URL);
                        }
                    }


                    Target.URL = GELayout.AreaText(Target.URL, GUILayout.Height(50));

                    if (Target.EnableSequenceRecord)
                    {
                        if (EditorApplication.isPlaying)
                        {
                            using (new EditorGUILayout.HorizontalScope(GEStyle.DropzoneStyle))
                            {
                                GELayout.LabelPrefix("序列记录");
                                if (!string.IsNullOrEmpty(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Target)))
                                {
                                    if (GELayout.Button("Open"))
                                        Application.OpenURL(AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Target));

                                    if (GELayout.Button("Upload FTP"))
                                    {
                                        AHandle.FTP.Create("", "", "").UploadFile(
                                            AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                                    }
                                }
                            }
                        }

                        FoldoutAutoRecord = GELayout.VFoldout($"序列记录 Size {RecordQueueSizeStr}", FoldoutAutoRecord);
                        if (FoldoutAutoRecord)
                        {
                            if (AssetSystem.SequenceRecords != null)
                            {
                                using (GELayout.Vertical())
                                {
                                    var index = 0;
                                    foreach (var record in AssetSystem.SequenceRecords)
                                    {
                                        GELayout.Label(
                                            $"{++index} : {record.PackageName} -> {record.Location} : {record.AssetPath} ");
                                        GELayout.HelpBox(
                                            $"{record.Time:yyyy-MM-dd HH:mm:ss} [Num : {record.Count}] [Size : {record.Bytes.ToConverseStringFileSize()}] ");
                                    }
                                }
                            }

                            using (GELayout.VHorizontal())
                            {
                                GELayout.Button("Update", UpdateRecordQueue);

                                if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH))
                                {
                                    if (GELayout.Button("Open Local"))
                                    {
                                        Application.OpenURL(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                                    }

                                    if (GELayout.Button("Delete Local"))
                                    {
                                        AHelper.IO.DeleteFile(AssetSystem.SequenceRecordQueue.LOCAL_PATH);
                                    }
                                }
                            }
                        }
                    }


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
                    GELayout.Button("Update", Update);
                    break;
            }
        }

        private void UpdateRecordQueue()
        {
            if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH)) // 如果在编辑器下存在本地记录则加载
                AssetSystem.SequenceRecords.UpdateLocal();
            RecordQueueSizeStr = AssetSystem.SequenceRecords.Size.ToConverseStringFileSize();
        }

        private void Update()
        {
            Target.UpdatePackage();
            _packages = Target.Packages is null
                ? _packages
                : Target.Packages.ToList();
        }

        protected override void OnChange()
        {
            Target.Packages = _packages.ToArray();
        }
    }
}