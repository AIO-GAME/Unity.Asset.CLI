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
        public static DisplayList<IAssetRuleAddress> MapAddress
        {
            get
            {
                if (_MapAddress is null) Initialize();
                return _MapAddress;
            }
        }

        public static DisplayList<IAssetRuleFilter> MapCollect
        {
            get
            {
                if (_MapCollect is null) Initialize();
                return _MapCollect;
            }
        }

        public static DisplayList<IAssetRuleFilter> MapFilter
        {
            get
            {
                if (_MapFilter is null) Initialize();
                return _MapFilter;
            }
        }


        public static DisplayList<IAssetRulePack> MapPacks
        {
            get
            {
                if (_MapPacks is null) Initialize();
                return _MapPacks;
            }
        }

        private static DisplayList<IAssetRuleAddress> _MapAddress;
        private static DisplayList<IAssetRuleFilter> _MapCollect;
        private static DisplayList<IAssetRuleFilter> _MapFilter;
        private static DisplayList<IAssetRulePack> _MapPacks;

        private static void Initialize()
        {
            var assetAddress = typeof(IAssetRuleAddress).FullName;
            var assetFilter = typeof(IAssetRuleFilter).FullName;
            var assetPack = typeof(IAssetRulePack).FullName;

            if (_MapAddress is null) _MapAddress = new DisplayList<IAssetRuleAddress>();
            else _MapAddress.Clear();

            if (_MapCollect is null) _MapCollect = new DisplayList<IAssetRuleFilter>();
            else _MapCollect.Clear();

            if (_MapFilter is null) _MapFilter = new DisplayList<IAssetRuleFilter>();
            else _MapFilter.Clear();

            if (_MapPacks is null) _MapPacks = new DisplayList<IAssetRulePack>();
            else _MapPacks.Clear();

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
                        _MapAddress.Add(key, InstanceAddress.DisplayAddressName, InstanceAddress);
                    }

                    if (iAssetPack != null && instance is IAssetRulePack InstancePack)
                    {
                        _MapPacks.Add(key, InstancePack.DisplayPackName, InstancePack);
                    }

                    if (iAssetFilter != null && instance is IAssetRuleFilter InstanceFilter)
                    {
                        var keyFilter = InstanceFilter.DisplayFilterName;
                        _MapCollect.Add(key, keyFilter, InstanceFilter);
                        _MapFilter.Add(key, keyFilter, InstanceFilter);
                    }
                }
            }
        }
    }
}