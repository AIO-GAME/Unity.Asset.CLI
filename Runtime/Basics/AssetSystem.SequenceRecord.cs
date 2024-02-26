/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-26
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

#if UNITY_EDITOR
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

namespace AIO
{
    public partial class AssetSystem
    {
#if UNITY_EDITOR
        /// <summary>
        /// 获取序列记录
        /// </summary>
        /// <param name="record">记录</param>
        [DebuggerNonUserCode, DebuggerHidden, Conditional("UNITY_EDITOR")]
        public static void AddSequenceRecord(SequenceRecord record)
        {
            Parameter.SequenceRecord.Add(record);
            WhiteListLocal.Add(record.Location);
        }
#endif

        public class SequenceRecordQueue : IDisposable, ICollection<SequenceRecord>
        {
            private const string FILE_NAME = "ASSETRECORD.json";

            private List<SequenceRecord> Records;

            /// <summary>
            /// 自动激活序列记录
            /// </summary>
            public bool Enable { get; }

            /// <summary>
            /// 序列记录大小
            /// </summary>
            public long Size => Records?.Sum(record => record.Bytes) ?? 0;

            public SequenceRecord this[int index] => Records[index];

            public SequenceRecord this[string guid] => Records.Find(record => record.GUID == guid);

            public SequenceRecordQueue(bool enable = false)
            {
                Enable = enable;
                Records = new List<SequenceRecord>();
            }

            /// <summary>
            /// 更新本地序列记录
            /// </summary>
            public void UpdateLocal()
            {
                Records = new List<SequenceRecord>();
                if (!File.Exists(LOCAL_PATH)) return;
                var temp = AHelper.IO.ReadJsonUTF8<List<SequenceRecord>>(LOCAL_PATH);
                if (temp == null) return;
                if (temp.Count <= 0) return;
                var dic = new Dictionary<string, SequenceRecord>();
                foreach (var item in temp)
                {
                    if (string.IsNullOrEmpty(item.GUID))
                    {
                        if (string.IsNullOrEmpty(item.AssetPath)) continue;
                        item.GUID = AssetDatabase.AssetPathToGUID(item.AssetPath);
                    }

                    if (string.IsNullOrEmpty(item.GUID)) continue;
                    dic[item.GUID] = item;
                }

                Records.AddRange(dic.Values);
            }

            /// <summary>
            /// 是否存在本地序列记录
            /// </summary>
            /// <returns>Ture:存在</returns>
            public bool ExistsLocal()
            {
                return File.Exists(LOCAL_PATH);
            }

            /// <summary>
            /// 下载序列记录
            /// </summary>
            public Task DownloadTask(string URL)
            {
                var handle = AHelper.HTTP.Download(GET_REMOTE_PATH(URL), LOCAL_PATH, true);
                return handle.WaitAsync();
            }

            /// <summary>
            /// 下载序列记录
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
            /// 保存序列记录
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

            public
#if UNITY_EDITOR
                async
#endif
                void Dispose()
            {
                if (Records is null) return;
#if UNITY_EDITOR
                if (Enable) await AHelper.IO.WriteJsonUTF8Async(LOCAL_PATH, Records);
#endif
                Records.Clear();
                Records = null;
            }

            public void Add(SequenceRecord record)
            {
                if (!Enable) return;
                if (record is null) return;
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

            public void CopyTo(SequenceRecord[] array, int arrayIndex)
            {
                Records.CopyTo(array, arrayIndex);
            }

            public bool RemoveGUID(string guid)
            {
                return Records.RemoveAll(record => record.GUID == guid) > 0;
            }

            public bool RemoveAssetPath(string assetPath)
            {
                return Records.RemoveAll(record => record.AssetPath == assetPath) > 0;
            }

            public bool Remove(SequenceRecord item)
            {
                return Records.Remove(item);
            }

            public int Count => Records?.Count ?? 0;
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
            /// 序列记录路径
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

        /// <summary>
        /// 资源包记录序列
        /// </summary>
        public class SequenceRecord
        {
            /// <summary>
            /// 资源GUID Key
            /// </summary>
            public string GUID;

            /// <summary>
            /// 资源包名
            /// </summary>
            public string PackageName;

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

            public override string ToString()
            {
                return
                    $"[{Time}] {PackageName} - {Location} - {AssetPath} - {Bytes.ToConverseStringFileSize()} - {Count}";
            }
        }
    }
}
#endif