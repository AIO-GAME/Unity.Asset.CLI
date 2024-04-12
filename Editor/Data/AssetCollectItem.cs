using System;
using System.Collections.Generic;
using System.IO;
using AIO.UEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    partial class AssetCollectItem
    {
        /// <summary>
        ///     是否折叠
        /// </summary>
        public bool Folded;

        /// <summary>
        ///     收集器类型
        /// </summary>
        public EAssetCollectItemType Type;

        /// <summary>
        ///     资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        ///     加载类型
        /// </summary>
        public EAssetLoadType LoadType = EAssetLoadType.Always;

        /// <summary>
        ///     收集器名称
        /// </summary>
        public string FileName;

        /// <summary>
        ///     资源GUID
        /// </summary>
        public string GUID;

        /// <summary>
        ///     收集器路径
        /// </summary>
        public string CollectPath;

        /// <summary>
        ///     自定义数据
        /// </summary>
        public string UserData;

        /// <summary>
        ///     定位规则
        /// </summary>
        public int Address;

        /// <summary>
        ///     定位格式
        /// </summary>
        public EAssetLocationFormat LocationFormat;

        /// <summary>
        ///     是否有后缀
        /// </summary>
        public bool HasExtension;

        /// <summary>
        ///     打包规则
        /// </summary>
        public int RulePackIndex = 1;

        /// <summary>
        ///     资源包名称
        /// </summary>
        public string PackageName { get; internal set; }

        /// <summary>
        ///     资源组名称
        /// </summary>
        public string GroupName { get; internal set; }

        #region Collect

        /// <summary>
        ///     收集规则(获取收集规则下标)
        /// </summary>
        public int RuleCollectIndex;

        /// <summary>
        ///     开启自定义收集规则
        /// </summary>
        public bool RuleUseCollectCustom;

        /// <summary>
        ///     收集规则 自定义
        /// </summary>
        public string RuleCollect;

        #endregion

        #region Filter

        /// <summary>
        ///     过滤规则(获取收集规则下标)
        /// </summary>
        public int RuleFilterIndex;

        /// <summary>
        ///     开启自定义过滤规则
        /// </summary>
        public bool RuleUseFilterCustom;

        /// <summary>
        ///     过滤规则 自定义
        /// </summary>
        public string RuleFilter;

        #endregion
    }

    partial class AssetCollectItem
    {
        public bool IsValidate
        {
            get
            {
                if (string.IsNullOrEmpty(CollectPath)) return false;
                if (CollectPath.Contains("/Resources/") || CollectPath.EndsWith("Resources"))
                {
                    AssetSystem.LogWarningFormat("Resources 目录下的资源不允许打包 !!! 已自动过滤 !!! -> {0}", CollectPath);
                    return false;
                }

                if (!AHelper.IO.Exists(FullPath)) return false;
                return true;
            }
        }

        /// <summary>
        ///     收集器全路径
        /// </summary>
        public string FullPath => System.IO.Path.Combine(EHelper.Path.Project, CollectPath);

        /// <summary>
        ///     收集器路径
        /// </summary>
        public Object Path
        {
            get => _Path;
            set
            {
                _Path = value;
                UpdateData();
            }
        }

        /// <summary>
        ///     是否允许多线程收集
        /// </summary>
        public bool AllowThread => AssetCollectSetting.MapAddress?.GetValue(Address)?.AllowThread ?? false;

        /// <summary>
        ///     获取收集器标签
        /// </summary>
        public string[] AllTags
        {
            get
            {
                if (Type != EAssetCollectItemType.MainAssetCollector) return Array.Empty<string>();
                if (string.IsNullOrEmpty(Tags)) return Array.Empty<string>();
                if (!AHelper.IO.Exists(CollectPath)) return Array.Empty<string>();
                return Tags.Split(';', ' ', ',');
            }
        }
    }

    /// <summary>
    ///     收集器列表
    /// </summary>
    [Serializable]
    public sealed partial class AssetCollectItem : IDisposable, IEqualityComparer<AssetCollectItem>
    {
        /// <summary>
        ///     收集器路径
        /// </summary>
        private Object _Path { get; set; }

        /// <summary>
        ///     自定义过滤器 (分隔符标准: /)
        /// </summary>
        private string[] RuleCustomFilter { get; set; }

        /// <summary>
        ///     自定义收集器 (分隔符标准: /)
        /// </summary>
        private string[] RuleCustomCollect { get; set; }

        /// <summary>
        ///     资源数据信息
        /// </summary>
        private Dictionary<string, AssetDataInfo> AssetDataInfos { get; set; } =
            new Dictionary<string, AssetDataInfo>();

        /// <summary>
        ///     获取收集规则
        /// </summary>
        private List<IAssetRuleFilter> RuleCollects { get; set; } =
            new List<IAssetRuleFilter>();

        /// <summary>
        ///     获取收集规则
        /// </summary>
        private List<IAssetRuleFilter> RuleFilters { get; set; } =
            new List<IAssetRuleFilter>();

        public void Dispose()
        {
            AssetDataInfos?.Clear();
            RuleFilters?.Clear();
            RuleCollects?.Clear();
            UpdateData();
            if (_Path == null) AssetSystem.LogWarningFormat("收集器路径为空 !!! -> {0}", CollectPath);
        }

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

        /// <summary>
        ///     获取收集规则
        /// </summary>
        public IAssetRulePack GetPackRule()
        {
            return AssetCollectSetting.MapPacks.GetValue(RulePackIndex);
        }

        /// <summary>
        ///     设置可寻址规则
        /// </summary>
        public void SetAddress<T>()
        where T : IAssetRuleAddress => Address = AssetCollectSetting.MapAddress.Values.FindIndex(address => address is T);

        /// <summary>
        ///     判断是否符合收集规则
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>Ture:忽略 False:需要过滤</returns>
        public bool IsCollectAsset(AssetRuleData data)
        {
            if (string.IsNullOrEmpty(data.AssetPath)) return false;

            var rootPath = data.AssetPath.Substring(0, data.AssetPath.LastIndexOf('/'));
            if (rootPath.EndsWith("Editor"))
            {
                AssetSystem.LogWarningFormat("Editor 目录下的资源不允许打包 已自动过滤 -> {0}", data.AssetPath);
                return false;
            }

            if (rootPath.Contains("Resources/") || rootPath.EndsWith("Resources")) // 判断是否为Resources目录下的资源
            {
                AssetSystem.LogWarningFormat("Resources 目录下的资源不允许打包 已自动过滤 -> {0}", data.AssetPath);
                return false;
            }

            if (RuleUseCollectCustom) // 判断是否使用自定义收集规则
            {
                if (RuleCustomCollect?.Length > 0 && // 判断自定义收集规则等
                    !UEditor.RuleCollect.IsCollectAssetCustom(RuleCustomCollect, data.Extension)
                   ) return false;
            }
            else
            {
                if (RuleCollects.Count != 0 && // 判断收集规则是否符合条件 如果不符合则跳过
                    !RuleCollects.Exists(filter => filter.IsCollectAsset(data))
                   ) return false;
            }

            if (RuleUseFilterCustom) // 判断是否使用自定义过滤规则
            {
                if (RuleCustomFilter?.Length > 0 && // 判断自定义过滤规则等
                    UEditor.RuleCollect.IsCollectAssetCustom(RuleCustomFilter, data.Extension)
                   ) return false;
            }
            else
            {
                if (RuleFilters.Count != 0 && // 判断过滤规则是否符合条件 如果符合则跳过
                    RuleFilters.Exists(filter => filter.IsCollectAsset(data))
                   ) return false;
            }

            return true;
        }

        public string GetAddressByGUID(string guid)
        {
            return GetAddress(AssetDatabase.GUIDToAssetPath(guid));
        }

        public string GetAddress(Object asset)
        {
            return GetAddress(AssetDatabase.GetAssetPath(asset));
        }

        /// <summary>
        ///     获取资源可寻地址
        /// </summary>
        /// <param name="assetPath">资源相对路径</param>
        /// <returns>可寻址路径</returns>
        public string GetAddress(string assetPath)
        {
            UpdateCollect();
            UpdateFilter();
            var data = new AssetRuleData
            {
                Tags        = Tags,
                UserData    = UserData,
                PackageName = PackageName,
                GroupName   = GroupName,
                CollectPath = CollectPath,
                Extension   = System.IO.Path.GetExtension(assetPath).Replace(".", "").ToLower()
            };
            data.AssetPath = assetPath.Substring(0, assetPath.Length - data.Extension.Length - 1);
            var config = ASConfig.GetOrCreate();
            return IsCollectAsset(data)
                ? GetAssetAddress(data, config.LoadPathToLower, config.HasExtension)
                : string.Empty;
        }

        public bool AssetExist(Object asset)
        {
            return AssetExist(AssetDatabase.GetAssetPath(asset));
        }

        public bool AssetExistByGUID(string guid)
        {
            return AssetExist(AssetDatabase.GUIDToAssetPath(guid));
        }

        /// <summary>
        ///     是否存在指定资源
        /// </summary>
        /// <param name="assetPath">资源相对路径</param>
        /// <returns>Ture:存在 False:不存在</returns>
        public bool AssetExist(string assetPath)
        {
            UpdateFilter();
            var data = new AssetRuleData
            {
                Tags        = Tags,
                UserData    = UserData,
                PackageName = PackageName,
                GroupName   = GroupName,
                CollectPath = CollectPath,
                Extension   = System.IO.Path.GetExtension(assetPath).Replace(".", "").ToLower()
            };
            data.AssetPath = assetPath.Substring(0, assetPath.Length - data.Extension.Length - 1);
            return IsCollectAsset(data);
        }

        public string GetAssetAddress(AssetRuleData data, bool pathToLower, bool hasExtension)
        {
            var rule = AssetCollectSetting.MapAddress.GetValue(Address);
            var address = rule.GetAssetAddress(data);
            if (string.IsNullOrEmpty(address)) return string.Empty;

            if (HasExtension || hasExtension) address = string.Concat(address, ".", data.Extension);
            if (pathToLower) return address.ToLower();
            switch (LocationFormat)
            {
                case EAssetLocationFormat.ToLower:
                    return address.ToLower();
                case EAssetLocationFormat.ToUpper:
                    return address.ToUpper();
                case EAssetLocationFormat.None:
                default: return address;
            }
        }

        /// <summary>
        ///     更新过滤规则
        /// </summary>
        public void UpdateFilter()
        {
            if (RuleUseFilterCustom)
            {
                if (!string.IsNullOrEmpty(RuleFilter))
                    RuleCustomFilter = RuleFilter.Split(';', ' ', ',', '|');
            }
            else if (RuleFilterIndex < 0)
            {
                RuleFilters.Clear();
                RuleFilters.AddRange(AssetCollectSetting.MapFilter.Values);
            }
            else if (RuleFilterIndex > 0)
            {
                RuleFilters.Clear();
                var status = 1;
                foreach (var item in AssetCollectSetting.MapFilter.Values)
                {
                    if ((RuleFilterIndex & status) == status) RuleFilters.Add(item);
                    status *= 2;
                }
            }
        }

        /// <summary>
        ///     更新收集规则
        /// </summary>
        public void UpdateCollect()
        {
            if (RuleUseCollectCustom)
            {
                if (!string.IsNullOrEmpty(RuleCollect))
                    RuleCustomCollect = RuleCollect.Split(';', ' ', ',', '|');
            }
            else if (RuleCollectIndex < 0) // 判断是否为全部收集
            {
                RuleCollects.Clear();
                RuleCollects.AddRange(AssetCollectSetting.MapCollect.Values);
            }
            else if (RuleCollectIndex > 0) // 判断是否为选择收集
            {
                RuleCollects.Clear();
                var status = 1;
                foreach (var item in AssetCollectSetting.MapCollect.Values)
                {
                    if ((RuleCollectIndex & status) == status) RuleCollects.Add(item);
                    status *= 2;
                }
            }
        }

        /// <summary>
        ///     收集资源
        /// </summary>
        /// <param name="tags">标签列表</param>
        /// <param name="toLower">路径是否小写</param>
        /// <param name="hasExtension">路径是否包含后缀</param>
        private void CollectAsset(string[] tags, bool toLower, bool hasExtension)
        {
            var data = new AssetRuleData
            {
                Tags        = tags.Length == 0 ? string.Empty : string.Join(";", tags),
                UserData    = UserData,
                PackageName = PackageName,
                GroupName   = GroupName,
                CollectPath = CollectPath
            };

            var info = new AssetDataInfo
            {
                CollectPath = data.CollectPath,
                Package     = data.PackageName,
                Group       = data.GroupName,
                Tags        = data.Tags
            };

            if (Directory.Exists(CollectPath)) // 判断Path是否为文件夹
            {
                var len = EHelper.Path.Project.Length + 1;
                foreach (var fileInfo in AHelper.IO.GetFilesInfo(CollectPath, "*", SearchOption.AllDirectories))
                {
                    if (fileInfo.FullName.EndsWith(".meta")) continue;
                    data.Extension = fileInfo.Extension.TrimStart('.', ' ').ToLower();
                    data.AssetPath = fileInfo.FullName.Substring(
                        len,
                        fileInfo.FullName.Length - len - data.Extension.Length - 1
                    ).Replace("\\", "/");
                    if (!IsCollectAsset(data)) continue;
                    info.AssetPath = string.Concat(data.AssetPath, '.', data.Extension);
                    info.Extension = data.Extension;
                    info.Address   = GetAssetAddress(data, toLower, hasExtension);
                    if (string.IsNullOrEmpty(info.Address)) continue;

                    if (AssetDataInfos.ContainsKey(info.Address))
                    {
                        AssetSystem.LogWarningFormat("[资源已存在 请检查 收集器不允许一个资源多个可寻址路径] !!! -> {0}", data.AssetPath);
                    }
                    else
                    {
                        AssetDataInfos[info.Address] = info;
                    }
                }
            }
            else if (File.Exists(CollectPath)) // 判断Path是否为文件
            {
                data.Extension = System.IO.Path.GetExtension(data.CollectPath).Replace(".", "").ToLower();
                data.AssetPath = data.CollectPath.Substring(0, data.CollectPath.Length - data.Extension.Length - 1);
                if (!IsCollectAsset(data)) return;
                info.AssetPath = data.CollectPath;
                info.Extension = data.Extension;
                info.Address   = GetAssetAddress(data, toLower, hasExtension);
                if (string.IsNullOrEmpty(info.Address)) return;

                if (AssetDataInfos.ContainsKey(info.Address))
                {
                    AssetSystem.LogWarningFormat("[资源已存在 请检查 收集器不允许一个资源多个可寻址路径] !!! -> {0}", data.CollectPath);
                }
                else
                {
                    AssetDataInfos[info.Address] = info;
                }
            }
        }

        public void CollectAssetAsync(
            string                                    package,
            string                                    group,
            Action<Dictionary<string, AssetDataInfo>> cb = null)
        {
            AssetDataInfos.Clear();
            if (!IsValidate) return;
            if (Type != EAssetCollectItemType.MainAssetCollector) return;

            PackageName = package;
            GroupName   = group;

            var tags = AssetCollectRoot.GetOrCreate().GetTags(PackageName, GroupName, CollectPath);
            var toLower = ASConfig.GetOrCreate().LoadPathToLower;
            var hasExtension = ASConfig.GetOrCreate().HasExtension;
            UpdateCollect();
            UpdateFilter();

            if (AllowThread) Runner.StartCoroutine(Action);
            else Action();

            return;

            void Action()
            {
                CollectAsset(tags, toLower, hasExtension);
                cb?.Invoke(AssetDataInfos);
            }
        }

        public void UpdateData()
        {
            if (_Path == null)
            {
                Folded = false;
            }
            else
            {
                var temp = AssetDatabase.GetAssetPath(_Path).Replace("\\", "/");
                if (!AHelper.IO.Exists(temp)) return;
                CollectPath = temp;
                GUID        = AssetDatabase.AssetPathToGUID(CollectPath);
                FileName    = System.IO.Path.GetFileName(CollectPath);
            }
        }

        public override bool Equals(object obj) => obj is AssetCollectItem item && Equals(this, item);
        public override int  GetHashCode()      => GetHashCode(this);
    }
}