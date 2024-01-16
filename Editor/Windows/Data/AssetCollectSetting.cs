/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;

namespace AIO.UEditor
{
    public static class AssetCollectSetting
    {
        public static DisplayList<IAssetRuleAddress> MapAddress { get; private set; }
        public static DisplayList<IAssetRuleFilter> MapCollect { get; private set; }
        public static DisplayList<IAssetRuleFilter> MapFilter { get; private set; }
        public static DisplayList<IAssetRulePack> MapPacks { get; private set; }

        public static void Initialize()
        {
            var assetAddress = typeof(IAssetRuleAddress).FullName;
            var assetFilter = typeof(IAssetRuleFilter).FullName;
            var assetPack = typeof(IAssetRulePack).FullName;

            if (MapAddress is null) MapAddress = new DisplayList<IAssetRuleAddress>();
            else MapAddress.Clear();

            if (MapCollect is null) MapCollect = new DisplayList<IAssetRuleFilter>();
            else MapCollect.Clear();

            if (MapFilter is null) MapFilter = new DisplayList<IAssetRuleFilter>();
            else MapFilter.Clear();

            if (MapPacks is null) MapPacks = new DisplayList<IAssetRulePack>();
            else MapPacks.Clear();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || type.IsEnum) continue;
                    var iAssetAddress = type.GetInterface(assetAddress);
                    var iAssetFilter = type.GetInterface(assetFilter);
                    var iAssetPack = type.GetInterface(assetPack);
                    if (iAssetAddress == null && iAssetFilter == null && iAssetPack == null) continue;

                    var instance = Activator.CreateInstance(type);
                    var key = type.FullName ?? type.Name;
                    if (iAssetAddress != null && instance is IAssetRuleAddress InstanceAddress)
                    {
                        MapAddress.Add(key, InstanceAddress.DisplayAddressName, InstanceAddress);
                    }

                    if (iAssetPack != null && instance is IAssetRulePack InstancePack)
                    {
                        MapPacks.Add(key, InstancePack.DisplayPackName, InstancePack);
                    }

                    if (iAssetFilter != null && instance is IAssetRuleFilter InstanceFilter)
                    {
                        var keyFilter = InstanceFilter.DisplayFilterName;
                        MapCollect.Add(key, keyFilter, InstanceFilter);
                        MapFilter.Add(key, keyFilter, InstanceFilter);
                    }
                }
            }
        }
    }
}