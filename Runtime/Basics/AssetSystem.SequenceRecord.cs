/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-26
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <summary>
        /// 序列记录队列
        /// </summary>
        internal static SequenceRecordQueue SequenceRecords { get; private set; }
#if UNITY_EDITOR
            = new SequenceRecordQueue();
#endif

        public class SequenceRecordQueue : IDisposable, ICollection<SequenceRecord>
        {
            private const string FILE_NAME = "ASSETRECORD";

            private Queue<SequenceRecord> Records;

            /// <summary>
            /// 自动激活序列记录
            /// </summary>
            public bool Enable { get; }

            /// <summary>
            /// 更新本地序列记录
            /// </summary>
            public void UpdateLocal()
            {
                Records = File.Exists(LOCAL_PATH)
                    ? AHelper.IO.ReadJsonUTF8<Queue<SequenceRecord>>(LOCAL_PATH)
                    : new Queue<SequenceRecord>();
            }

            /// <summary>
            /// 序列记录大小
            /// </summary>
            public long Size => Records?.Sum(record => record.Bytes) ?? 0;

            public Task DownloadTask(string URL)
            {
                var handle = AHelper.HTTP.Download(GET_REMOTE_PATH(URL), LOCAL_PATH, true);
                return handle.WaitAsync();
            }

            public IEnumerator DownloadCo(string URL)
            {
                yield return NetLoadStringCO(GET_REMOTE_PATH(URL), data =>
                {
                    Records = string.IsNullOrEmpty(data)
                        ? new Queue<SequenceRecord>()
                        : AHelper.IO.ReadJsonUTF8<Queue<SequenceRecord>>(data);
                    AHelper.IO.WriteJsonUTF8(LOCAL_PATH, Records);
                });
            }

            public static string GET_REMOTE_PATH(ASConfig config)
            {
                return GET_REMOTE_PATH(config.URL);
            }

            public static string GET_REMOTE_PATH(string URL)
            {
                return string.IsNullOrEmpty(URL)
                    ? string.Empty
                    : Path.Combine(URL, "Version", FILE_NAME).Replace("\\", "/");
            }

            public bool ExistsLocal()
            {
                return File.Exists(LOCAL_PATH);
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
                        Path.Combine(Application.dataPath.Substring(
                                0, Application.dataPath.LastIndexOf('/')),
                            "Bundles", "Version");
#else
                        Path.Combine(Application.persistentDataPath, Parameter.RuntimeRootDirectory, "Version");
#endif
                    if (!Directory.Exists(root)) Directory.CreateDirectory(root);
                    return Path.Combine(root, FILE_NAME);
                }
            }

            public SequenceRecordQueue(bool enable = false)
            {
                Enable = enable;
                Records = new Queue<SequenceRecord>();
            }

            public IEnumerator LoadCo()
            {
                if (!Enable) yield break;
                if (Parameter.ASMode == EASMode.Remote) yield return DownloadCo(Parameter.URL);
                UpdateLocal();
            }

            public async Task LoadAsync()
            {
                if (!Enable) return;
                if (Parameter.ASMode == EASMode.Remote)
                    await DownloadTask(Parameter.URL);
                UpdateLocal();
            }

            public async void Dispose()
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
                if (Enable) Records.Enqueue(record);
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
            public Dictionary<string, List<AssetInfo>> ToYoo()
            {
                var list = new Dictionary<string, List<AssetInfo>>();
                if (Records is null) return list;
                foreach (var record in Records)
                {
                    var info = YooAssets.GetPackage(record.PackageName).GetAssetInfo(record.Location);
                    if (info is null) continue;
                    if (!YooAssets.GetPackage(record.PackageName).IsNeedDownloadFromRemote(info)) continue;
                    if (!list.ContainsKey(record.PackageName)) list.Add(record.PackageName, new List<AssetInfo>());
                    if (list[record.PackageName].Contains(info)) continue;
                    list[record.PackageName].Add(info);
                }

                return list;
            }
#endif

            public void Save()
            {
                if (Records is null) return;
                AHelper.IO.WriteJsonUTF8(LOCAL_PATH, Records);
            }

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

        /// <summary>
        /// 资源包记录序列
        /// </summary>
        public struct SequenceRecord
        {
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

#if SUPPORT_YOOASSET
            public static implicit operator AssetInfo(SequenceRecord record)
            {
                return YooAssets.GetPackage(record.PackageName).GetAssetInfo(record.Location);
            }
#endif

            public override string ToString()
            {
                return
                    $"[{Time}] {PackageName} - {Location} - {AssetPath} - {Bytes.ToConverseStringFileSize()} - {Count}";
            }
        }
    }
}