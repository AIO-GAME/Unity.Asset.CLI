using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEngine;
#if SUPPORT_YOOASSET
using YooAsset;
#endif

namespace AIO
{
    public partial class AssetSystem
    {
        private static AssetProxy Proxy;

        /// <summary>
        /// 白名单 - 定位指定白名单 - 允许同步加载
        /// </summary>
        public static List<string> WhiteListLocal { get; private set; } = new List<string>();

        /// <summary>
        /// 资源包配置
        /// </summary>
        public static ICollection<AssetsPackageConfig> PackageConfigs => Parameter.Packages;

        /// <summary>
        /// 资源热更新配置
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static ASConfig Parameter { get; private set; }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// 序列记录队列
        /// </summary>
        public static SequenceRecordQueue SequenceRecords { get; private set; }
#if UNITY_EDITOR
            = new SequenceRecordQueue();
#endif

        /// <summary>
        /// 资源包记录序列
        /// </summary>
        public struct SequenceRecord
        {
            /// <summary>
            /// 资源包名
            /// </summary>
            public string Name;

            /// <summary>
            /// 资源包寻址路径
            /// </summary>
            public string Location;

            /// <summary>
            /// 资源路径
            /// </summary>
            public string AssetPath;

            /// <summary>
            /// 记录时间
            /// </summary>
            public DateTime Time;

            /// <summary>
            /// 记录大小
            /// </summary>
            public long Bytes;

            /// <summary>
            /// 记录数量
            /// </summary>
            public int Count;

#if SUPPORT_YOOASSET
            public static implicit operator AssetInfo(SequenceRecord record)
            {
                return YooAssets.GetPackage(record.Name).GetAssetInfo(record.Location);
            }
#endif
        }

        public class SequenceRecordQueue : IDisposable, ICollection<SequenceRecord>
        {
            internal const string FILE_NAME = "record.json";

            /// <summary>
            /// 序列记录路径
            /// </summary>
            internal static readonly string REMOTE_PATH =
#if UNITY_EDITOR
                Path.Combine(string.IsNullOrEmpty(Parameter?.URL) ? string.Empty : Parameter.URL, "Version", FILE_NAME);
#else
                Path.Combine(Parameter.URL, "Version", FILE_NAME);
#endif

#if UNITY_EDITOR
            internal static string GET_REMOTE_PATH(ASConfig config)
            {
                return string.IsNullOrEmpty(config.URL) ? string.Empty : Path.Combine(config.URL, "Version", FILE_NAME);
            }
#endif
            /// <summary>
            /// 序列记录路径
            /// </summary>
            internal static readonly string LOCAL_PATH =
#if UNITY_EDITOR
                Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Bundles", "Version", FILE_NAME);
#else
                Path.Combine(Application.persistentDataPath, "BuildinFiles", FILE_NAME);
#endif

            internal Queue<SequenceRecord> Records;

            public SequenceRecordQueue()
            {
                Records = new Queue<SequenceRecord>();
            }

            public async Task LoadAsync()
            {
                if (Parameter.AutoSequenceRecord) return;
#if !UNITY_EDITOR
                if (File.Exists(LOCAL_PATH)) // 如果在编辑器下存在本地记录则加载
                    Records = await AHelper.IO.ReadJsonUTF8Async<Queue<SequenceRecord>>(LOCAL_PATH);
#else
                if (File.Exists(LOCAL_PATH)) Records = new Queue<SequenceRecord>(); // 如果存在本地记录则不加载
                else if (Parameter.ASMode == EASMode.Remote)
                {
                    Records = AHelper.Net.HTTP.GetJson<Queue<SequenceRecord>>(REMOTE_PATH);
                    if (Records is null) Records = new Queue<SequenceRecord>();
                    await AHelper.IO.WriteJsonUTF8Async(LOCAL_PATH, Records);
                }
#endif
                if (Records is null) Records = new Queue<SequenceRecord>();
            }

            public async void Dispose()
            {
#if UNITY_EDITOR
                if (Parameter.AutoSequenceRecord)
                    await AHelper.IO.WriteJsonUTF8Async(LOCAL_PATH, Records);
#endif
                Records.Clear();
                Records = null;
            }

            public void Add(SequenceRecord record)
            {
                if (Parameter.AutoSequenceRecord)
                    Records.Enqueue(record);
            }

            public void Clear()
            {
                Records.Clear();
            }

            public bool Contains(SequenceRecord item)
            {
                return Records.Contains(item);
            }

            public void CopyTo(SequenceRecord[] array, int arrayIndex)
            {
                Records.CopyTo(array, arrayIndex);
            }

            public bool Remove(SequenceRecord item)
            {
                return false;
            }

            public int Count => Records?.Count ?? 0;
            public bool IsReadOnly => false;

#if SUPPORT_YOOASSET
            public static implicit operator Dictionary<string, List<AssetInfo>>(SequenceRecordQueue recordQueue)
            {
                var list = new Dictionary<string, List<AssetInfo>>();
                if (recordQueue?.Records is null) return list;
                foreach (var record in recordQueue.Records)
                {
                    var info = YooAssets.GetPackage(record.Name).GetAssetInfo(record.Location);
                    if (info is null) continue;
                    if (!list.ContainsKey(record.Name)) list.Add(record.Name, new List<AssetInfo>());
                    if (list[record.Name].Contains(info)) continue;
                    list[record.Name].Add(info);
                }

                return list;
            }
#endif

            public IEnumerator<SequenceRecord> GetEnumerator()
            {
                if (Records is null) Records = new Queue<SequenceRecord>();
                return Records.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}