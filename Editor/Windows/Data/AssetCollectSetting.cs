/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;

namespace AIO.UEditor
{
    internal static class AssetCollectSetting
    {
        public static DisplayList<IAssetRuleAddress> MapAddress;
        public static DisplayList<IAssetRuleFilter> MapCollect;
        public static DisplayList<IAssetRuleFilter> MapFilter;

        public static void Initialize()
        {
            var assetAddress = typeof(IAssetRuleAddress).FullName;
            var assetFilter = typeof(IAssetRuleFilter).FullName;

            if (MapAddress is null) MapAddress = new DisplayList<IAssetRuleAddress>();
            else MapAddress.Clear();
            if (MapCollect is null) MapCollect = new DisplayList<IAssetRuleFilter>();
            else MapCollect.Clear();
            if (MapFilter is null) MapFilter = new DisplayList<IAssetRuleFilter>();
            else MapFilter.Clear();

            var status = 1L;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || type.IsEnum) continue;
                    var iAssetAddress = type.GetInterface(assetAddress);
                    var iAssetFilter = type.GetInterface(assetFilter);
                    if (iAssetAddress == null && iAssetFilter == null) continue;

                    var instance = Activator.CreateInstance(type);
                    var key = type.FullName ?? type.Name;
                    if (iAssetAddress != null && instance is IAssetRuleAddress InstanceAddress)
                    {
                        MapAddress.Add(key, InstanceAddress.DisplayAddressName, InstanceAddress);
                    }

                    if (iAssetFilter != null && instance is IAssetRuleFilter InstanceFilter)
                    {
                        var keyFilter = InstanceFilter.DisplayFilterName;
                        MapCollect.Add(key, keyFilter, InstanceFilter);
                        MapFilter.Add(key, keyFilter, InstanceFilter);
                        status *= 2;
                    }
                }
            }
        }
    }
}