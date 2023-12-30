/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    public enum EAssetCollectItemType
    {
        [InspectorName("动态资源")] MainAssetCollector,
        [InspectorName("依赖资源")] DependAssetCollector,
        [InspectorName("静态资源")] StaticAssetCollector,
    }

    public enum AssetLocationFormat
    {
        [InspectorName("默认")] None,
        [InspectorName("小写")] ToLower,
        [InspectorName("大写")] ToUpper,
    }

    public enum AssetLoadType
    {
        [InspectorName("始终")] Always,
        [InspectorName("运行时")] Runtime,
        [InspectorName("编辑时")] Editor,
    }

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class AssetCollectItem : IEqualityComparer<AssetCollectItem>
    {
        /// <summary>
        /// 收集器类型
        /// </summary>
        public EAssetCollectItemType Type;

        /// <summary>
        /// 资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        /// 收集器路径
        /// </summary>
        public Object Path;

        /// <summary>
        /// 加载类型
        /// </summary>
        public AssetLoadType LoadType = AssetLoadType.Always;

        [NonSerialized] private string _GUID;

        [NonSerialized] private string _CollectPath;

        [NonSerialized] private string _FileName;

        /// <summary>
        /// 自定义数据
        /// </summary>
        public string UserData;

        /// <summary>
        /// 定位规则
        /// </summary>
        public int Address;

        #region Collect

        /// <summary>
        /// 收集规则(获取收集规则下标)
        /// </summary>
        public int RuleCollectIndex;

        /// <summary>
        /// 收集规则
        /// </summary>
        public bool RuleUseCollectCustom;

        /// <summary>
        /// 收集规则 自定义
        /// </summary>
        public string RuleCollect;

        #endregion

        #region Filter

        /// <summary>
        /// 打包规则
        /// </summary>
        public int RulePackIndex;

        /// <summary>
        /// 过滤规则(获取收集规则下标)
        /// </summary>
        public int RuleFilterIndex;

        /// <summary>
        /// 过滤规则
        /// </summary>
        public bool RuleUseFilterCustom;

        /// <summary>
        /// 过滤规则
        /// </summary>
        public string RuleFilter;

        #endregion

        /// <summary>
        /// 定位格式
        /// </summary>
        public AssetLocationFormat LocationFormat;

        /// <summary>
        /// 是否有后缀
        /// </summary>
        public bool HasExtension;

        public bool Equals(AssetCollectItem x, AssetCollectItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Type == y.Type &&
                   x.Tags == y.Tags &&
                   Equals(x.Path, y.Path) &&
                   x.UserData == y.UserData &&
                   x.FileName == y.FileName &&
                   x.Address == y.Address &&
                   x.RuleCollect == y.RuleCollect &&
                   x.RuleFilter == y.RuleFilter;
        }

        public int GetHashCode(AssetCollectItem obj)
        {
            if (obj.Equals(null)) return 0;
            unchecked
            {
                var hashCode = (int)obj.Type;
                hashCode = (hashCode * 397) ^ (obj.Tags != null ? obj.Tags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Path != null ? obj.Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.UserData != null ? obj.UserData.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.FileName != null ? obj.FileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Address.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.RuleCollect != null ? obj.RuleCollect.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.RuleFilter != null ? obj.RuleFilter.GetHashCode() : 0);
                return hashCode;
            }
        }

        public Dictionary<string, AssetDataInfo> AssetDataInfos = new Dictionary<string, AssetDataInfo>();

        public List<IAssetRuleFilter> RuleCollects = new List<IAssetRuleFilter>();

        public List<IAssetRuleFilter> RuleFilters = new List<IAssetRuleFilter>();

        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool Folded;

        /// <summary>
        /// 自定义过滤器 (分隔符标准: /)
        /// </summary>
        public string[] RuleCustomFilter { get; private set; }

        /// <summary>
        /// 自定义收集器 (分隔符标准: /)
        /// </summary>
        public string[] RuleCustomCollect { get; private set; }

        /// <summary>
        /// 判断是否符合收集规则
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>Ture:忽略 False:需要过滤</returns>
        public bool IsCollectAsset(AssetRuleData data)
        {
            // 判断收集规则是否符合条件 如果不符合则跳过
            if (RuleCollects.Count != 0 &&
                !RuleCollects.Any(filter => filter.IsCollectAsset(data))
               ) return false;

            // 判断自定义收集规则等
            if (RuleUseCollectCustom && RuleCustomCollect?.Length > 0 &&
                !UEditor.RuleCollect.IsCollectAssetCustom(RuleCustomCollect, data.Extension)
               ) return false;

            // 判断过滤规则是否符合条件 如果符合则跳过
            if (RuleFilters.Count != 0 &&
                RuleFilters.Any(filter => filter.IsCollectAsset(data))
               ) return false;

            // 判断自定义过滤规则等
            if (RuleUseFilterCustom && RuleCustomFilter?.Length > 0 &&
                UEditor.RuleCollect.IsCollectAssetCustom(RuleCustomFilter, data.Extension)
               ) return false;

            return true;
        }

        public AssetRulePackResult GetPackRule(AssetRuleData data)
        {
            return AssetCollectSetting.MapPacks.GetValue(RulePackIndex).GetPackRuleResult(data);
        }

        public string GetAssetAddress(AssetRuleData data)
        {
            var rule = AssetCollectSetting.MapAddress.GetValue(Address);
            var address = rule.GetAssetAddress(data);

            if (HasExtension) address = string.Concat(address, ".", data.Extension);
            if (ASConfig.GetOrCreate().LoadPathToLower) return address.ToLower();
            switch (LocationFormat)
            {
                case AssetLocationFormat.ToLower:
                    return address.ToLower();
                case AssetLocationFormat.ToUpper:
                    return address.ToUpper();
                case AssetLocationFormat.None:
                default: return address;
            }
        }

        public void SetAddress<T>() where T : IAssetRuleAddress
        {
            Address = AssetCollectSetting.MapAddress.Values.FindIndex(address => address is T);
        }

        public void UpdateFilter()
        {
            RuleFilters.Clear();
            if (RuleUseFilterCustom)
            {
                if (!string.IsNullOrEmpty(RuleFilter))
                    RuleCustomFilter = RuleFilter?.Split(';');
            }
            else if (RuleFilterIndex < 0)
            {
                RuleFilters.AddRange(AssetCollectSetting.MapFilter.Values);
            }
            else if (RuleFilterIndex > 0)
            {
                var status = 1;
                foreach (var item in AssetCollectSetting.MapFilter.Values)
                {
                    if ((RuleFilterIndex & status) == status) RuleFilters.Add(item);
                    status *= 2;
                }
            }
        }

        public void UpdateCollect()
        {
            RuleCollects.Clear();
            if (RuleUseCollectCustom)
            {
                if (!string.IsNullOrEmpty(RuleCollect))
                    RuleCustomCollect = RuleCollect?.Split(';');
            }
            else if (RuleCollectIndex < 0)
            {
                RuleCollects.AddRange(AssetCollectSetting.MapCollect.Values);
            }
            else if (RuleCollectIndex > 0)
            {
                var status = 1;
                foreach (var item in AssetCollectSetting.MapCollect.Values)
                {
                    if ((RuleCollectIndex & status) == status) RuleCollects.Add(item);
                    status *= 2;
                }
            }
        }

        public string PackageName { get; private set; }

        public string GroupName { get; private set; }

        public void CollectAsset(string package, string group)
        {
            AssetDataInfos.Clear();
            RuleFilters.Clear();
            RuleCollects.Clear();
            PackageName = package;
            GroupName = group;
            if (Path is null || string.IsNullOrEmpty(CollectPath)) return;
            if (Type != EAssetCollectItemType.MainAssetCollector) return;

            UpdateCollect();
            UpdateFilter();

            var data = new AssetRuleData
            {
                Tags = Tags,
                UserData = UserData,
                PackageName = package,
                GroupName = group,
                CollectPath = CollectPath,
            };
            var tags = AssetCollectRoot.GetOrCreate().GetTags(PackageName, GroupName, CollectPath);
            var info = new AssetDataInfo
            {
                CollectPath = data.CollectPath,
                Tags = tags.Length == 0 ? string.Empty : string.Join(";", tags),
            };

            if (AssetDatabase.IsValidFolder(CollectPath)) // 判断Path是否为文件夹
            {
                // 获取文件夹下所有文件
                foreach (var file in
                         EHelper.IO.GetFilesRelativeAssetNoMeta(CollectPath, SearchOption.AllDirectories))
                {
                    var fixedPath = file.Replace("\\", "/");
                    var temp = System.IO.Path.GetFileName(fixedPath);
                    var index = temp.LastIndexOf('.');
                    if (index >= 0)
                    {
                        data.Extension = temp.Substring(index).Replace(".", "").ToLower();
                    }

                    data.AssetPath = fixedPath.Substring(0, fixedPath.Length - data.Extension.Length - 1);
                    if (!IsCollectAsset(data)) continue;
                    info.Address = GetAssetAddress(data);
                    info.AssetPath = fixedPath;
                    info.Extension = data.Extension;
                    AssetDataInfos[fixedPath] = info;
                }
            }
            else
            {
                data.Extension = System.IO.Path.GetExtension(data.CollectPath).Replace(".", "");
                data.AssetPath = data.CollectPath.Substring(0, data.CollectPath.Length - data.Extension.Length - 1);
                if (!IsCollectAsset(data)) return;
                info.Address = GetAssetAddress(data);
                info.AssetPath = data.CollectPath;
                info.Extension = data.Extension;
                AssetDataInfos[data.CollectPath] = info;
            }
        }

        public async Task CollectAssetTask(string package, string group)
        {
            AssetDataInfos.Clear();
            RuleFilters.Clear();
            RuleCollects.Clear();
            PackageName = package;
            GroupName = group;
            if (Path is null || string.IsNullOrEmpty(CollectPath)) return;
            if (Type != EAssetCollectItemType.MainAssetCollector) return;

            UpdateCollect();
            UpdateFilter();

            var data = new AssetRuleData
            {
                Tags = Tags,
                UserData = UserData,
                PackageName = package,
                GroupName = group,
                CollectPath = CollectPath,
            };
            var tags = AssetCollectRoot.GetOrCreate().GetTags(PackageName, GroupName, CollectPath);
            var info = new AssetDataInfo
            {
                CollectPath = data.CollectPath,
                Tags = tags.Length == 0 ? string.Empty : string.Join(";", tags),
            };

            if (AssetDatabase.IsValidFolder(CollectPath)) // 判断Path是否为文件夹
            {
                // 获取文件夹下所有文件
                foreach (var file in await Task.Factory.StartNew(() =>
                             EHelper.IO.GetFilesRelativeAssetNoMeta(CollectPath, SearchOption.AllDirectories)))
                {
                    var fixedPath = file.Replace("\\", "/");
                    var temp = System.IO.Path.GetFileName(fixedPath);
                    var index = temp.LastIndexOf('.');
                    if (index >= 0)
                    {
                        data.Extension = temp.Substring(index).Replace(".", "").ToLower();
                    }

                    data.AssetPath = fixedPath.Substring(0, fixedPath.Length - data.Extension.Length - 1);
                    if (!IsCollectAsset(data)) continue;
                    info.Address = GetAssetAddress(data);
                    info.AssetPath = fixedPath;
                    info.Extension = data.Extension;
                    AssetDataInfos[fixedPath] = info;
                }
            }
            else
            {
                data.Extension = System.IO.Path.GetExtension(data.CollectPath).Replace(".", "");
                data.AssetPath = data.CollectPath.Substring(0, data.CollectPath.Length - data.Extension.Length - 1);
                if (!IsCollectAsset(data)) return;
                info.Address = GetAssetAddress(data);
                info.AssetPath = data.CollectPath;
                info.Extension = data.Extension;
                AssetDataInfos[data.CollectPath] = info;
            }
        }


        /// <summary>
        /// 收集器名称
        /// </summary>
        public string FileName
        {
            get
            {
                if (Path is null) return string.Empty;
                if (!string.IsNullOrEmpty(_FileName)) return _FileName;
                _FileName = System.IO.Path.GetFileName(CollectPath);
                return _FileName;
            }
        }

        /// <summary>
        /// 资源GUID
        /// </summary>
        public string GUID
        {
            get
            {
                if (string.IsNullOrEmpty(_GUID)) _GUID = AssetDatabase.AssetPathToGUID(CollectPath);
                return _GUID;
            }
        }

        /// <summary>
        /// 收集器路径
        /// </summary>
        public string CollectPath
        {
            get
            {
                if (Path is null) return string.Empty;
                if (string.IsNullOrEmpty(_CollectPath)) _CollectPath = AssetDatabase.GetAssetPath(Path);
                return _CollectPath;
            }
        }
    }
}