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

#endregion

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        ///     获取序列记录
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
        ///     资源包记录序列
        /// </summary>
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
#endif
        public struct SequenceRecord
        {
            /// <summary>
            ///     资源GUID Key
            /// </summary>
            public string GUID
            {
                get
                {
                    if (string.IsNullOrEmpty(_GUID)) _GUID = AssetDatabase.AssetPathToGUID(AssetPath);

                    return _GUID;
                }
            }

            private string _GUID;

            /// <summary>
            ///     资源包名
            /// </summary>
            public string PackageName;

            /// <summary>
            ///     设置资源包名
            /// </summary>
            /// <param name="packageName">资源包名</param>
            public void SetPackageName(string packageName)
            {
                PackageName = packageName;
            }

            /// <summary>
            ///     资源包寻址路径
            /// </summary>
            /// <param name="assetPath">资源路径</param>
            public void SetAssetPath(string assetPath)
            {
                AssetPath = assetPath;
                _GUID     = string.Empty;
            }

            /// <summary>
            ///     设置寻址路径
            /// </summary>
            /// <param name="guid">资源GUID</param>
            public void SetGUID(string guid)
            {
                _GUID = guid;
            }

            /// <summary>
            ///     资源包寻址路径
            /// </summary>
            public string Location;

            /// <summary>
            ///     资源路径
            /// </summary>
            public string AssetPath;

            /// <summary>
            ///     记录时间
            /// </summary>
            public DateTime Time;

            /// <summary>
            ///     记录大小
            /// </summary>
            public long Bytes;

            /// <summary>
            ///     记录数量
            /// </summary>
            public int Count;

            /// <summary>
            ///     是否为空
            /// </summary>
            public bool IsNull => string.IsNullOrEmpty(AssetPath) ||
                                  string.IsNullOrEmpty(GUID);


            public override string ToString()
            {
                return
                    $"[{Time}] {PackageName} - {Location} - {AssetPath} - {Bytes.ToConverseStringFileSize()} - {Count}";
            }
        }

        #endregion

        #region Nested type: SequenceRecordQueue

        public class SequenceRecordQueue : IDisposable, ICollection<SequenceRecord>
        {
            private const string FILE_NAME = "ASSETRECORD.json";

            private List<SequenceRecord> Records;

            public SequenceRecordQueue(bool enable = false)
            {
                Enable  = enable;
                Records = new List<SequenceRecord>();
            }

            /// <summary>
            ///     自动激活序列记录
            /// </summary>
            public bool Enable { get; }

            /// <summary>
            ///     序列记录大小
            /// </summary>
            public long Size => Records?.Sum(record => record.Bytes) ?? 0;

            public SequenceRecord this[int index] => Records[index];

            public SequenceRecord this[string guid] => Records.Find(record => record.GUID == guid);

            #region ICollection<SequenceRecord> Members

            public void Add(SequenceRecord record)
            {
                if (!Enable) return;
                if (record.IsNull) return;
                if (ContainsGUID(record.GUID)) return;
                Records.Add(record);
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
                return Records.Remove(item);
            }

            public int  Count      => Records?.Count ?? 0;
            public bool IsReadOnly => false;

            public IEnumerator<SequenceRecord> GetEnumerator()
            {
                if (Records is null) Records = new List<SequenceRecord>();
                return Records.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

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
            ///     更新本地序列记录
            /// </summary>
            public void UpdateLocal()
            {
                Records = new List<SequenceRecord>();
                if (!File.Exists(LOCAL_PATH)) return;
                var temp = AHelper.IO.ReadJsonUTF8<List<SequenceRecord>>(LOCAL_PATH);
                if (temp == null) return;
                if (temp.Count <= 0) return;
                var dic = new Dictionary<string, SequenceRecord>();
                foreach (var item in temp.Where(item => !string.IsNullOrEmpty(item.GUID))) dic[item.GUID] = item;

                Records.AddRange(dic.Values);
            }

            /// <summary>
            ///     是否存在本地序列记录
            /// </summary>
            /// <returns>Ture:存在</returns>
            public bool ExistsLocal()
            {
                return File.Exists(LOCAL_PATH);
            }

            /// <summary>
            ///     下载序列记录
            /// </summary>
            public Task DownloadTask(string URL)
            {
                var handle = AHelper.HTTP.Download(GET_REMOTE_PATH(URL), LOCAL_PATH, true);
                return handle.WaitAsync();
            }

            /// <summary>
            ///     下载序列记录
            /// </summary>
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
            ///     保存序列记录
            /// </summary>
            public void Save()
            {
                if (Records is null) return;
                var temp = new Dictionary<string, SequenceRecord>();
                foreach (var item in Records.Where(item => !temp.ContainsKey(item.GUID))) temp[item.GUID] = item;
                Records.Clear();
                Records.AddRange(temp.Values);
                Records.Sort((a, b) => b.Time.CompareTo(a.Time));
                AHelper.IO.WriteJsonUTF8(LOCAL_PATH, Records);
            }

            public bool ContainsGUID(string guid)
            {
                return Records.Exists(record => record.GUID == guid);
            }

            public bool ContainsAssetPath(string assetPath)
            {
                return Records.Exists(record => record.AssetPath == assetPath);
            }

            public bool ContainsAssetPath(string assetPath, string packageName)
            {
                return Records.Exists(record => record.AssetPath == assetPath && record.PackageName == packageName);
            }

            public bool RemoveGUID(string guid)
            {
                return Records.RemoveAll(record => record.GUID == guid) > 0;
            }

            public bool RemoveAssetPath(string assetPath)
            {
                return Records.RemoveAll(record => record.AssetPath == assetPath) > 0;
            }

            #region static

            public static string GET_REMOTE_PATH(ASConfig config)
            {
                return GET_REMOTE_PATH(config.URL);
            }

            public static string GET_REMOTE_PATH(string URL)
            {
                return string.IsNullOrEmpty(URL)
                    ? string.Empty
                    : Path.Combine(URL, "Version", FILE_NAME).Replace('\\', '/');
            }

            /// <summary>
            ///     序列记录路径
            /// </summary>
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