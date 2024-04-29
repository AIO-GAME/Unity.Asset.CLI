using System;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    partial class AssetCollectItem
    {
        public bool IsValidate
        {
            get
            {
                if (string.IsNullOrEmpty(CollectPath)) return false;
                if (CollectPath.EndsWith("/Editor/") || CollectPath.EndsWith("Editor")) return false;
                if (CollectPath.Contains("/Resources/") || CollectPath.EndsWith("Resources"))
                {
                    AssetSystem.LogWarningFormat("Resources 目录下的资源不允许打包 !!! 已自动过滤 !!! -> {0}", CollectPath);
                    return false;
                }

                return AHelper.IO.Exists(FullPath);
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
        public bool AllowThread => AssetCollectSetting.MapAddress?.GetValue(AddressIndex)?.AllowThread ?? false;

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
}