/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-11-24
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YooAsset;
using Debug = UnityEngine.Debug;

namespace AIO.UEngine.YooAsset
{
    internal static partial class YAssetSystem
    {
        private static T GetHandle<T>(string location) where T : OperationHandleBase
        {
            if (ReferenceOPHandle.ContainsKey(location)) return (T)ReferenceOPHandle[location];
            return null;
        }

        private static T GetHandle<T>(AssetInfo location) where T : OperationHandleBase
        {
            return GetHandle<T>(location.Address);
        }

        private static void AddHandle<T>(string location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            ReferenceOPHandle.Set(location, operation);
        }

        private static void AddHandle<T>(AssetInfo location, T operation) where T : OperationHandleBase
        {
            if (operation is null) return;
            ReferenceOPHandle.Set(location.Address, operation);
        }

        public static void FreeHandle(string location)
        {
            if (!ReferenceOPHandle.TryGetValue(location, out var value)) return;
            ReleaseInternal?.Invoke(value, null);
            ReferenceOPHandle.Remove(location);
        }

        public static void FreeHandle(IEnumerable<string> locations)
        {
            foreach (var location in locations)
            {
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