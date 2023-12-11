/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-05
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections;
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
        private string RecordQueueSizeStr;
        private List<AssetsPackageConfig> _packages;

        partial void OnDrawASConfig()
        {
            using (GELayout.Vertical(GEStyle.INThumbnailShadow))
            {
                using (GELayout.VHorizontal())
                {
                    Config.ASMode = GELayout.Popup("加载模式", Config.ASMode);
                    if (GELayout.Button("Clean Sandbox", 100))
                    {
                        var sandbox = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Sandbox");
                        if (Directory.Exists(sandbox))
                            AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                    }

                    if (GELayout.Button("Clean Bundles", 100))
                    {
                        var sandbox = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Bundles");
                        if (Directory.Exists(sandbox))
                            AHelper.IO.DeleteFolder(sandbox, SearchOption.AllDirectories, true);
                    }
                }


                Config.OutputLog = GELayout.ToggleLeft("开启日志输出", Config.OutputLog);
                Config.LoadPathToLower = GELayout.ToggleLeft("定位地址小写", Config.LoadPathToLower);

                switch (Config.ASMode)
                {
                    case EASMode.Remote:
                        Config.AutoSaveVersion = GELayout.ToggleLeft("自动激活清单", Config.AutoSaveVersion);
                        Config.AppendTimeTicks = GELayout.ToggleLeft("请求附加时间磋", Config.AppendTimeTicks);
                        Config.AutoSequenceRecord = GELayout.ToggleLeft("自动序列记录", Config.AutoSequenceRecord);
                        Config.DownloadFailedTryAgain =
                            GELayout.Slider("下载失败尝试次数", Config.DownloadFailedTryAgain, 1, 100);
                        Config.LoadingMaxTimeSlice =
                            GELayout.Slider("资源加载的最大数量", Config.LoadingMaxTimeSlice, 144, 8192);
                        Config.Timeout = GELayout.Slider("请求超时时间", Config.Timeout, 3, 180);
                        using (new EditorGUILayout.HorizontalScope(GEStyle.DropzoneStyle))
                        {
                            GELayout.Label("远端资源地址");
                            if (!string.IsNullOrEmpty(Config.URL))
                            {
                                if (GELayout.Button("Open")) Application.OpenURL(Config.URL);
                            }
                        }


                        Config.URL = GELayout.AreaText(Config.URL, GUILayout.Height(50));

                        if (Config.AutoSequenceRecord)
                        {
                            if (EditorApplication.isPlaying)
                            {
                                using (new EditorGUILayout.HorizontalScope(GEStyle.DropzoneStyle))
                                {
                                    GELayout.LabelPrefix("序列记录");
                                    if (!string.IsNullOrEmpty(
                                            AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Config)))
                                    {
                                        if (GELayout.Button("Open"))
                                            Application.OpenURL(
                                                AssetSystem.SequenceRecordQueue.GET_REMOTE_PATH(Config));

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
                                using (GELayout.VHorizontal())
                                {
                                    GELayout.Button("Update", UpdateDataRecordQueue);

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

                                if (AssetSystem.SequenceRecords != null)
                                {
                                    using (GELayout.Vertical())
                                    {
                                        var index = 0;
                                        foreach (var record in AssetSystem.SequenceRecords)
                                        {
                                            GELayout.Label(
                                                $"{++index} : {record.Name} -> {record.Location} : {record.AssetPath} ");
                                            GELayout.HelpBox(
                                                $"{record.Time:yyyy-MM-dd HH:mm:ss} [Num : {record.Count}] [Size : {record.Bytes.ToConverseStringFileSize()}] ");
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
                        GELayout.Button("Update", UpdateConfig);
                        break;
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

        private void UpdateDataRecordQueue()
        {
            if (File.Exists(AssetSystem.SequenceRecordQueue.LOCAL_PATH)) // 如果在编辑器下存在本地记录则加载
                AssetSystem.SequenceRecords.Records = AHelper.IO.ReadJsonUTF8<Queue<AssetSystem.SequenceRecord>>(
                    AssetSystem.SequenceRecordQueue.LOCAL_PATH);
            RecordQueueSizeStr = AssetSystem.SequenceRecords.Sum(record => record.Bytes).ToConverseStringFileSize();
        }
    }
}