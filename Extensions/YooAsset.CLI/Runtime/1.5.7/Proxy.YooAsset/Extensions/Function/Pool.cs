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
            if (ReferenceOPHandle.TryGetValue(location, out var value)) return (T)value;
            return null;
        }

        private static T GetHandle<T>(AssetInfo location) where T : OperationHandleBase
        {
            return GetHandle<T>(location.Address);
        }

        private static void AddHandle<T>(string location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            if (string.IsNullOrEmpty(location)) return;
            ReferenceOPHandle[location] = operation;
        }

        private static void AddHandle<T>(AssetInfo location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            if (location is null) return;
            ReferenceOPHandle[location.Address] = operation;
        }

        public static void FreeHandle(string location)
        {
            if (string.IsNullOrEmpty(location)) return;
            if (!ReferenceOPHandle.TryGetValue(location, out var value)) return;
            ReleaseInternal?.Invoke(value, null);
            ReferenceOPHandle.Remove(location);
        }

        public static void FreeHandle(IEnumerable<string> locations)
        {
            foreach (var location in locations)
            {
                if (string.IsNullOrEmpty(location)) continue;
                if (!ReferenceOPHandle.TryGetValue(location, out var value)) continue;
                ReleaseInternal?.Invoke(value, null);
                ReferenceOPHandle.Remove(location);
            }
        }

        public static void FreeHandle(IEnumerable<AssetInfo> locations)
        {
            foreach (var location in locations) FreeHandle(location.Address);
        }

        public static void FreeHandle(AssetInfo location)
        {
            FreeHandle(location.Address);
        }
    }
}
#endif