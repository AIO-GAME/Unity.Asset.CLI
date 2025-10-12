#if UNITY_EDITOR

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
#if UNITY_2022_1_OR_NEWER
using Unity.Profiling;
#endif

#endregion

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 获取序列记录
        /// </summary>
        /// <param name="record">记录</param>
        [DebuggerNonUserCode, DebuggerHidden, Conditional("UNITY_EDITOR")]
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
#endif
        public static void AddSequenceRecord(SequenceRecord record)
        {
            Parameter.SequenceRecord.Add(record);
            WhiteListLocal.Add(record.Location);
        }

        #region Nested type: SequenceRecord

        /// <summary>
        /// 资源包记录序列
        /// </summary>
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
#endif
        [Preserve]
        public struct SequenceRecord
        {
            /// <summary>
            /// 资源GUID Key
            /// </summary>
            [Preserve]
            public string GUID
            {
                get
                {
                    if (string.IsNullOrEmpty(_GUID)) _GUID = AssetDatabase.AssetPathToGUID(AssetPath);

                    return _GUID;
                }
            }

            [Preserve]
            private string _GUID;

            /// <summary>
            /// 资源包名
            /// </summary>
            [Preserve]
            public string PackageName;

            /// <summary>
            /// 设置资源包名
            /// </summary>
            /// <param name="packageName">资源包名</param>
            [Preserve]
            public void SetPackageName(string packageName) { PackageName = packageName; }

            /// <summary>
            /// 资源包寻址路径
            /// </summary>
            /// <param name="assetPath">资源路径</param>
            [Preserve]
            public void SetAssetPath(string assetPath)
            {
                AssetPath = assetPath;
                _GUID     = string.Empty;
            }

            /// <summary>
            /// 设置寻址路径
            /// </summary>
            /// <param name="guid">资源GUID</param>
            [Preserve]
            public void SetGUID(string guid) { _GUID = guid; }

            /// <summary>
            /// 资源包寻址路径
            /// </summary>
            [Preserve]
            public string Location;

            /// <summary>
            /// 资源路径
            /// </summary>
            [Preserve]
            public string AssetPath;

            /// <summary>
            /// 记录时间
            /// </summary>
            [Preserve]
            public DateTime Time;

            /// <summary>
            /// 记录大小
            /// </summary>
            [Preserve]
            public long Bytes;

            /// <summary>
            /// 记录数量
            /// </summary>
            [Preserve]
            public int Count;

            /// <summary>
            /// 是否为空
            /// </summary>
            [Preserve]
            public bool IsNull =>
                string.IsNullOrEmpty(AssetPath) ||
                string.IsNullOrEmpty(GUID);

            public override string ToString()
            {
                return
                    $"[{Time}] {PackageName} - {Location} - {AssetPath} - {Bytes.ToConverseStringFileSize()} - {Count}";
            }
        }

        #endregion

        #region Nested type: SequenceRecordQueue

        [Preserve]
        public class SequenceRecordQueue : IDisposable, ICollection<SequenceRecord>
        {
            private const string FILE_NAME = "ASSETRECORD.json";

            [Preserve]
            private List<SequenceRecord> Records;

            [Preserve]
            private Dictionary<string, SequenceRecord> RecordCacheGuid;

            public SequenceRecordQueue(bool enable = false)
            {
                Enable          = enable;
                Records         = new List<SequenceRecord>(8);
                RecordCacheGuid = new Dictionary<string, SequenceRecord>(8);
            }

            /// <summary>
            /// 自动激活序列记录
            /// </summary>
            [Preserve]
            public bool Enable { get; }

            /// <summary>
            /// 序列记录大小
            /// </summary>
            [Preserve]
            public long Size => Records?.Sum(record => record.Bytes) ?? 0;

            [Preserve]
            public SequenceRecord this[int index] => Records[index];

            [Preserve]
            public SequenceRecord this[string guid] => Records.Find(record => record.GUID == guid);

            #region ICollection<SequenceRecord> Members

            public void Add(SequenceRecord record)
            {
                if (!Enable
                 || record.IsNull
                 || ContainsGUID(record.GUID)
                   ) return;
                RecordCacheGuid[record.GUID] = record;
                Records.Add(record);
            }

            public void Clear()
            {
                Records.Clear();
                RecordCacheGuid.Clear();
            }

            public bool Contains(SequenceRecord item) => RecordCacheGuid.ContainsKey(item.GUID);

            public void CopyTo(SequenceRecord[] array, int arrayIndex) { Records.CopyTo(array, arrayIndex); }

            public bool Remove(SequenceRecord item) => RecordCacheGuid.Remove(item.GUID) && Records.Remove(item);

            public int  Count      => Records?.Count ?? 0;
            public bool IsReadOnly => false;

            public IEnumerator<SequenceRecord> GetEnumerator()
            {
                if (Records is null) Records = new List<SequenceRecord>();
                return Records.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

            #endregion

            #region IDisposable Members

            public async void Dispose()
            {
                if (Records is null) return;
                if (Enable) await AHelper.IO.WriteJsonUTF8Async(LOCAL_PATH, Records);
                Records.Clear();
                Records = null;
            }

            #endregion

            /// <summary>
            /// 更新本地序列记录
            /// </summary>
            [Preserve]
            public void UpdateLocal()
            {
                Records.Clear();
                RecordCacheGuid.Clear();
                if (!File.Exists(LOCAL_PATH)) return;
                var records = AHelper.IO.ReadJsonUTF8<List<SequenceRecord>>(LOCAL_PATH);
                if (records == null || records.Count <= 0) return;
                foreach (var record in
                         from record in records
                         where !string.IsNullOrEmpty(record.GUID)
                         where !RecordCacheGuid.ContainsKey(record.GUID)
                         select record)
                    RecordCacheGuid[record.GUID] = record;
                Records.AddRange(RecordCacheGuid.Values);
            }

            /// <summary>
            /// 是否存在本地序列记录
            /// </summary>
            /// <returns>Ture:存在</returns>
            [Preserve]
            public bool ExistsLocal() { return File.Exists(LOCAL_PATH); }

            /// <summary>
            /// 下载序列记录
            /// </summary>
            [Preserve]
            public Task DownloadTask(string URL) { return AHelper.Http.DownloadAsync(GET_REMOTE_PATH(URL), LOCAL_PATH, true); }

            /// <summary>
            /// 下载序列记录
            /// </summary>
            [Preserve]
            public IEnumerator DownloadCo(string URL)
            {
                yield return NetLoadStringCO(GET_REMOTE_PATH(URL), data =>
                {
                    Records = string.IsNullOrEmpty(data)
                        ? new List<SequenceRecord>()
                        : AHelper.IO.ReadJsonUTF8<List<SequenceRecord>>(data);
                    AHelper.IO.WriteJsonUTF8(LOCAL_PATH, Records);
                });
            }

            /// <summary>
            /// 保存序列记录
            /// </summary>
            [Preserve]
            public void Save()
            {
                if (Records is null) return;
                var temp                                                                                  = new Dictionary<string, SequenceRecord>();
                foreach (var item in Records.Where(item => !temp.ContainsKey(item.GUID))) temp[item.GUID] = item;
                Records.Clear();
                Records.AddRange(temp.Values);
                Records.Sort((a, b) => b.Time.CompareTo(a.Time));
                AHelper.IO.WriteJsonUTF8(LOCAL_PATH, Records);
            }

            [Preserve]
            public bool ContainsGUID(string guid) { return RecordCacheGuid.ContainsKey(guid); }

            [Preserve]
            public bool ContainsAssetPath(string assetPath) { return Records.Exists(record => record.AssetPath == assetPath); }

            [Preserve]
            public bool ContainsAssetPath(string assetPath, string packageName) { return Records.Exists(record => record.AssetPath == assetPath && record.PackageName == packageName); }

            [Preserve]
            public bool RemoveGUID(string guid)
            {
                Records.RemoveAll(record => record.GUID == guid);
                return RecordCacheGuid.Remove(guid);
            }

            [Preserve]
            public bool RemoveAssetPath(string assetPath) { return Records.RemoveAll(record => record.AssetPath == assetPath) > 0; }

            #region static

            [Preserve]
            public static string GET_REMOTE_PATH(ASConfig config) { return GET_REMOTE_PATH(config.URL); }

            [Preserve]
            public static string GET_REMOTE_PATH(string URL)
            {
                return string.IsNullOrEmpty(URL)
                    ? string.Empty
                    : Path.Combine(URL, "Version", FILE_NAME).Replace('\\', '/');
            }

            /// <summary>
            /// 序列记录路径
            /// </summary>
            [Preserve]
            public static string LOCAL_PATH
            {
                get
                {
                    var root =
#if UNITY_EDITOR
                        Path.Combine(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')),
                                     "Bundles", "Version");
#else
                        Path.Combine(Application.persistentDataPath, Parameter.RuntimeRootDirectory, "Version");
#endif
                    if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                    return Path.Combine(root, FILE_NAME).Replace('\\', '/');
                }
            }

            #endregion
        }

        #endregion
    }
}
#endif