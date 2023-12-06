/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string GUID => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Path));

        /// <summary>
        /// 收集器路径
        /// </summary>
        public string CollectPath => AssetDatabase.GetAssetPath(Path);

        /// <summary>
        /// 自定义数据
        /// </summary>
        public string UserData;

        /// <summary>
        /// 收集器名称
        /// </summary>
        public string Name => System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Path));

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
                   x.Name == y.Name &&
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
                hashCode = (hashCode * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Address.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.RuleCollect != null ? obj.RuleCollect.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.RuleFilter != null ? obj.RuleFilter.GetHashCode() : 0);
                return hashCode;
            }
        }

        public PageDictionary<string, AssetDataInfo> AssetDataInfos = new PageDictionary<string, AssetDataInfo>();

        public List<IAssetRuleFilter> RuleCollects = new List<IAssetRuleFilter>();

        public List<IAssetRuleFilter> RuleFilters = new List<IAssetRuleFilter>();

        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool Folded = true;

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
            var data = new AssetRuleData
            {
                Tags = Tags,
                UserData = UserData,
                PackageName = package,
                GroupName = group,
                CollectPath = CollectPath,
            };

            UpdateCollect();
            UpdateFilter();

            if (AssetDatabase.IsValidFolder(CollectPath)) // 判断Path是否为文件夹
            {
                // 获取文件夹下所有文件
                var files = EHelper.IO.GetFilesRelativeAssetNoMeta(CollectPath, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fixedPath = file.Replace("\\", "/");
                    data.Extension = System.IO.Path.GetExtension(fixedPath).Replace(".", "").ToLower();
                    data.AssetPath = fixedPath.Substring(0, fixedPath.Length - data.Extension.Length - 1);
                    if (!IsCollectAsset(data)) continue;
                    AssetDataInfos[fixedPath] = new AssetDataInfo
                    {
                        Address = GetAssetAddress(data),
                        AssetPath = fixedPath,
                        Extension = data.Extension,
                        Name = System.IO.Path.GetFileNameWithoutExtension(fixedPath),
                        Size = AHelper.IO.GetFileLength(fixedPath),
                        CollectPath = CollectPath,
                    };
                }
            }
            else
            {
                data.Extension = System.IO.Path.GetExtension(CollectPath).Replace(".", "");
                data.AssetPath = CollectPath.Substring(0, CollectPath.Length - data.Extension.Length - 1);
                if (IsCollectAsset(data))
                {
                    AssetDataInfos[CollectPath] = new AssetDataInfo
                    {
                        Address = GetAssetAddress(data),
                        AssetPath = CollectPath, 
                        Extension = data.Extension,
                        Name = System.IO.Path.GetFileNameWithoutExtension(CollectPath),
                        Size = AHelper.IO.GetFileLength(CollectPath),
                    };
                }
            }

            AssetDataInfos.PageSize = 20;
            AssetDataInfos.PageIndex = 0;
        }
    }
}