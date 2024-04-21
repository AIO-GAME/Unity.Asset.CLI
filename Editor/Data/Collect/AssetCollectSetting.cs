#region

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    public static class AssetCollectSetting
    {
        private static DisplayList<IAssetRuleAddress> _MapAddress;
        private static DisplayList<IAssetRuleFilter>  _MapCollectFilter;
        private static DisplayList<IAssetRulePack>    _MapPacks;

        private static string[] _CollectFilterDisplays { get; set; }
        private static string[] _PackDisplays          { get; set; }
        private static string[] _AddressDisplays       { get; set; }

        public static string[] CollectFilterDisplays
        {
            get
            {
                if (_CollectFilterDisplays is null) Initialize();
                return _CollectFilterDisplays;
            }
        }

        public static string[] PackDisplays
        {
            get
            {
                if (_PackDisplays is null) Initialize();
                return _PackDisplays;
            }
        }

        public static string[] AddressDisplays
        {
            get
            {
                if (_AddressDisplays is null) Initialize();
                return _AddressDisplays;
            }
        }

        private static GUIContent[] _GT_CollectFilterDisplays { get; set; }
        private static GUIContent[] _GT_PackDisplays          { get; set; }
        private static GUIContent[] _GT_AddressDisplays       { get; set; }

        public static GUIContent[] GT_GTCollectFilterDisplays
        {
            get
            {
                if (_GT_CollectFilterDisplays is null) Initialize();
                return _GT_CollectFilterDisplays;
            }
        }

        public static GUIContent[] GT_PackDisplays
        {
            get
            {
                if (_GT_PackDisplays is null) Initialize();
                return _GT_PackDisplays;
            }
        }

        public static GUIContent[] GT_AddressDisplays
        {
            get
            {
                if (_GT_AddressDisplays is null) Initialize();
                return _GT_AddressDisplays;
            }
        }

        public static IAssetRuleFilter GetAssetRuleFilter<T>()
        where T : IAssetRuleFilter
        {
            if (_MapCollectFilter is null) Initialize();
            return _MapCollectFilter?.GetValue(typeof(T).FullName ?? typeof(T).Name);
        }

        public static IAssetRuleAddress GetAssetRuleAddress<T>()
        where T : IAssetRuleAddress
        {
            if (_MapAddress is null) Initialize();
            return _MapAddress?.GetValue(typeof(T).FullName ?? typeof(T).Name);
        }

        public static IAssetRulePack GetAssetRulePack<T>()
        where T : IAssetRulePack
        {
            if (_MapPacks is null) Initialize();
            return _MapPacks?.GetValue(typeof(T).FullName ?? typeof(T).Name);
        }

        /// <summary>
        ///     资源可寻址类型列表
        /// </summary>
        public static DisplayList<IAssetRuleAddress> MapAddress
        {
            get
            {
                if (_MapAddress is null) Initialize();
                return _MapAddress;
            }
        }

        /// <summary>
        ///     资源收集类型列表
        /// </summary>
        public static DisplayList<IAssetRuleFilter> MapCollect
        {
            get
            {
                if (_MapCollectFilter is null) Initialize();
                return _MapCollectFilter;
            }
        }

        /// <summary>
        ///     资源过滤类型列表
        /// </summary>
        public static DisplayList<IAssetRuleFilter> MapFilter
        {
            get
            {
                if (_MapCollectFilter is null) Initialize();
                return _MapCollectFilter;
            }
        }


        /// <summary>
        ///     资源打包类型列表
        /// </summary>
        public static DisplayList<IAssetRulePack> MapPacks
        {
            get
            {
                if (_MapPacks is null) Initialize();
                return _MapPacks;
            }
        }

        private static void Initialize()
        {
            var assetAddress = typeof(IAssetRuleAddress).FullName;
            var assetFilter = typeof(IAssetRuleFilter).FullName;
            var assetPack = typeof(IAssetRulePack).FullName;

            if (_MapAddress is null) _MapAddress = new DisplayList<IAssetRuleAddress>();
            else _MapAddress.Clear();

            if (_MapCollectFilter is null) _MapCollectFilter = new DisplayList<IAssetRuleFilter>();
            else _MapCollectFilter.Clear();

            if (_MapPacks is null) _MapPacks = new DisplayList<IAssetRulePack>();
            else _MapPacks.Clear();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.FullName.Contains("Editor")) continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || type.IsEnum) continue;

                    var isNullAddress = type.GetInterface(assetAddress) is null;
                    var isNullFilter = type.GetInterface(assetFilter) is null;
                    var isNullPack = type.GetInterface(assetPack) is null;
                    if (isNullAddress && isNullFilter && isNullPack) continue;

                    var instance = Activator.CreateInstance(type);
                    var key = type.FullName ?? type.Name;
                    if (!isNullAddress && instance is IAssetRuleAddress InstanceAddress)
                        _MapAddress.Add(key, InstanceAddress.DisplayAddressName, InstanceAddress);

                    if (!isNullPack && instance is IAssetRulePack InstancePack)
                        _MapPacks.Add(key, InstancePack.DisplayPackName, InstancePack);

                    if (!isNullFilter && instance is IAssetRuleFilter InstanceFilter)
                        _MapCollectFilter.Add(key, InstanceFilter.DisplayFilterName, InstanceFilter);
                }
            }

            _MapCollectFilter.OnCompare += CollectFilterCompare;
            _MapCollectFilter.Sort();
            _MapCollectFilter.OnCompare -= CollectFilterCompare;

            _AddressDisplays          = _MapAddress.Displays.ToArray();
            _PackDisplays             = _MapPacks.Displays.ToArray();
            _CollectFilterDisplays    = _MapCollectFilter.Displays.ToArray();
            _GT_CollectFilterDisplays = EditorGUIUtility.TrTempContent(_CollectFilterDisplays);
            _GT_PackDisplays          = EditorGUIUtility.TrTempContent(_PackDisplays);
            _GT_AddressDisplays       = EditorGUIUtility.TrTempContent(_AddressDisplays);
        }

        private static int CollectFilterCompare(string a, string b)
        {
            var a1 = _MapCollectFilter.GetValue(a);
            if (a1 == null) return 1;

            var b1 = _MapCollectFilter.GetValue(b);
            if (b1 == null) return string.Compare(a, b, StringComparison.Ordinal);

            if (a1.DisplayIndex > b1.DisplayIndex) return 1;
            return a1.DisplayIndex < b1.DisplayIndex ? -1 : 0;
        }
    }
}