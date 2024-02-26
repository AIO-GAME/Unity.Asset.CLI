/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-11-24
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System.Collections.Generic;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal static partial class YAssetSystem
    {
        private static T GetHandle<T>(string location) where T : OperationHandleBase
        {
            return ReferenceOPHandle.TryGetValue(location, out var value)
                ? (T)value
                : null;
        }

        private static T GetHandle<T>(AssetInfo location) where T : OperationHandleBase
        {
            return ReferenceOPHandle.TryGetValue(location.Address, out var value)
                ? (T)value
                : null;
        }

        private static void AddHandle<T>(string location, T operation) where T : OperationHandleBase
        {
            if (string.IsNullOrEmpty(location)) return;
            if (operation is null) return;
            ReferenceOPHandle[location] = operation;
        }

        private static void AddHandle<T>(AssetInfo location, T operation) where T : OperationHandleBase
        {
            if (location is null) return;
            if (string.IsNullOrEmpty(location.Address)) return;
            if (operation is null) return;
            ReferenceOPHandle[location.Address] = operation;
        }

        public static void FreeHandle(string location)
        {
            if (string.IsNullOrEmpty(location)) return;
            if (ReferenceOPHandle.TryGetValue(location, out var value))
            {
                ReleaseInternal?.Invoke(value, null);
                ReferenceOPHandle.Remove(location);
            }
        }

        public static void FreeHandle(AssetInfo location)
        {
            if (location is null) return;
            if (string.IsNullOrEmpty(location.Address)) return;
            if (ReferenceOPHandle.TryGetValue(location.Address, out var value))
            {
                ReleaseInternal?.Invoke(value, null);
                ReferenceOPHandle.Remove(location.Address);
            }
        }

        public static void FreeHandle(IEnumerable<string> locations)
        {
            foreach (var location in locations) FreeHandle(location);
        }

        public static void FreeHandle(IEnumerable<AssetInfo> locations)
        {
            foreach (var location in locations) FreeHandle(location);
        }
    }
}
#endif