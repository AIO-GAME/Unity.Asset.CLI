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
    public enum AssetCollectItemType
    {
        Main,
        Dependence,
        Static,
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
        public AssetCollectItemType Type;

        /// <summary>
        /// 资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        /// 收集器路径
        /// </summary>
        public Object Path;

        /// <summary>
        /// 收集器路径
        /// </summary>
        public string CollectorPath => AssetDatabase.GetAssetPath(Path);

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
            unchecked
            {
                var hashCode = (int)obj.Type;
                hashCode = (hashCode * 397) ^ (obj.Tags != null ? obj.Tags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Path != null ? obj.Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.UserData != null ? obj.UserData.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Address != null ? obj.Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.RuleCollect != null ? obj.RuleCollect.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.RuleFilter != null ? obj.RuleFilter.GetHashCode() : 0);
                return hashCode;
            }
        }

        public PageDictionary<string, AssetDataInfo> AssetDataInfos = new PageDictionary<string, AssetDataInfo>();

        public List<IAssetRuleFilter> RuleCollects = new List<IAssetRuleFilter>();

        public List<IAssetRuleFilter> RuleFilters = new List<IAssetRuleFilter>();

        public void CollectAsset(string package, string group)
        {
            if (Path is null || string.IsNullOrEmpty(CollectorPath)) return;
            switch (Type)
            {
                case AssetCollectItemType.Main:
                    break;
                case AssetCollectItemType.Dependence:
                    return;
                case AssetCollectItemType.Static:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            AssetDataInfos.Clear();
            var data = new AssetInfoData
            {
                Tags = Tags,
                UserData = UserData,
                PackageName = package,
                GroupName = group,
                CollectPath = CollectorPath,
            };

            RuleCollects.Clear();
            if (RuleUseCollectCustom && !string.IsNullOrEmpty(RuleCollect))
            {
                data.RuleCustomCollect = RuleCollect.Split(';');
            }
            else
            {
                if (RuleCollectIndex < 0) RuleCollects.AddRange(AssetCollectSetting.MapCollect.Values);
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

            RuleFilters.Clear();
            if (RuleUseFilterCustom && !string.IsNullOrEmpty(RuleFilter))
            {
                data.RuleCustomFilter = RuleFilter.Split(';');
            }
            else
            {
                if (RuleFilterIndex < 0) RuleFilters.AddRange(AssetCollectSetting.MapFilter.Values);
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

            // 判断Path是否为文件夹
            if (AssetDatabase.IsValidFolder(CollectorPath))
            {
                // 获取文件夹下所有文件
                var files = EHelper.IO.GetFilesRelativeAssetNoMeta(CollectorPath, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fixedPath = file.Replace("\\", "/");
                    var address = AssetCollectSetting.MapAddress.GetValue(Address);
                    data.Extension = System.IO.Path.GetExtension(fixedPath).Replace(".", "").ToLower();
                    data.AssetPath = fixedPath.Substring(0, fixedPath.Length - data.Extension.Length - 1);

                    // 判断收集规则是否符合条件 如果不符合则跳过
                    if (RuleCollects.Count != 0 &&
                        !RuleCollects.Any(filter => filter.IsCollectAsset(data))
                       ) continue;

                    // 判断过滤规则是否符合条件 如果符合则跳过
                    if (RuleFilters.Count != 0 &&
                        RuleFilters.Any(filter => filter.IsCollectAsset(data))
                       ) continue;

                    // 判断自定义收集规则等
                    if (RuleUseCollectCustom && data.RuleCustomCollect?.Length > 0 &&
                        !UEditor.RuleCollect.IsCollectAssetCustom(data.RuleCustomCollect, data.Extension))
                        continue;

                    // 判断自定义过滤规则等
                    if (RuleUseFilterCustom && data.RuleCustomFilter?.Length > 0 &&
                        UEditor.RuleCollect.IsCollectAssetCustom(data.RuleCustomFilter, data.Extension))
                        continue;

                    var assetDataInfo = new AssetDataInfo
                    {
                        Address = address.GetAssetAddress(data),
                        AssetPath = fixedPath,
                    };

                    if (HasExtension)
                    {
                        assetDataInfo.Address = assetDataInfo.Address + "." + assetDataInfo.Extension;
                    }

                    if (LocationFormat == AssetLocationFormat.ToLower)
                    {
                        assetDataInfo.Address = assetDataInfo.Address.ToLower();
                    }
                    else if (LocationFormat == AssetLocationFormat.ToUpper)
                    {
                        assetDataInfo.Address = assetDataInfo.Address.ToUpper();
                    }

                    AssetDataInfos[assetDataInfo.AssetPath] = assetDataInfo;
                }
            }

            else
            {
                var address = AssetCollectSetting.MapAddress.GetValue(Address);
                data.Extension = System.IO.Path.GetExtension(CollectorPath).Replace(".", "");
                data.AssetPath = CollectorPath.Substring(0, CollectorPath.Length - data.Extension.Length - 1);
                var assetDataInfo = new AssetDataInfo
                {
                    Address = address.GetAssetAddress(data),
                    AssetPath = CollectorPath,
                };
                AssetDataInfos[assetDataInfo.AssetPath] = assetDataInfo;
            }

            AssetDataInfos.PageIndex = 1;
        }
    }
}