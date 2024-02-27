/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-21
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Diagnostics;
using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        [DebuggerNonUserCode]
        internal class YALogger : ILogger
        {
            [IgnoreConsoleJump]
            public void Log(string message)
            {
                AssetSystem.Log(message);
            }

            [IgnoreConsoleJump]
            public void Warning(string message)
            {
                AssetSystem.LogWarning(message);
            }

            [IgnoreConsoleJump]
            public void Error(string message)
            {
                AssetSystem.LogError(message);
            }

            [IgnoreConsoleJump]
            public void Exception(Exception exception)
            {
                AssetSystem.LogException(exception);
            }
        }

        private enum LoadType
        {
            Sync,
            Coroutine,
            Async
        }

#if UNITY_EDITOR
        private string GetLocation(string location)
        {
            return (
                from asset in Dic.Values
                where asset.CheckLocationValid(location)
                select asset.GetAssetInfo(location)
            ).FirstOrDefault()?.AssetPath;
        }

        private string GetType(LoadType type)
        {
            switch (type)
            {
                case LoadType.Sync:
                    return "<b><color=#AF7AC5>[Sync]</color></b> Load ";
                case LoadType.Coroutine:
                    return "<b><color=#F7DC6F>[Coroutine]</color></b> Load ";
                case LoadType.Async:
                    return "<b><color=#B3E5FC>[Async]</color></b> Load ";
                default:
                    return $"[{type}] Load ";
            }
        }

#endif

        [Conditional("DEBUG"), IgnoreConsoleJump]
        private void PackageDebug(LoadType type, string location)
        {
#if UNITY_EDITOR
            AssetSystem.Log($"{type} : [auto : {location}] -> {GetLocation(location)}");
#else
            AssetSystem.Log("{0} : [auto : {1}]", type, location);
#endif
        }

        [Conditional("DEBUG"), IgnoreConsoleJump]
        private void PackageDebug(LoadType type, string packageName, string location)
        {
#if UNITY_EDITOR
            AssetSystem.Log($"Load {type} : [{packageName} : {location}] -> {GetLocation(location)}");
#else
            AssetSystem.Log("{0} : [{1} : {2}]", type.ToString(), packageName, location);
#endif
        }
    }
}
#endif