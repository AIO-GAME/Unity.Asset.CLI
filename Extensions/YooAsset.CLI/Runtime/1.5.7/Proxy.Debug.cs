#if SUPPORT_YOOASSET

#region

using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Scripting;
using ILogger = YooAsset.ILogger;

#if UNITY_2022_1_OR_NEWER
using Unity.Profiling;
using UnityEngine;
#endif

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        [DebuggerNonUserCode]
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
#endif
        [Preserve]
        internal class YALogger : ILogger
        {
#if UNITY_2022_1_OR_NEWER
            [HideInCallstack]
#endif
            [IgnoreConsoleJump]
            [Preserve]
            public void Log(string message) { AssetSystem.LOG.I(message); }
#if UNITY_2022_1_OR_NEWER
            [HideInCallstack]
#endif
            [IgnoreConsoleJump]
            [Preserve]
            public void Warning(string message) { AssetSystem.LOG.W(message); }
#if UNITY_2022_1_OR_NEWER
            [HideInCallstack]
#endif
            [IgnoreConsoleJump]
            [Preserve]
            public void Error(string message) { AssetSystem.LOG.E(message); }
#if UNITY_2022_1_OR_NEWER
            [HideInCallstack]
#endif
            [IgnoreConsoleJump]
            [Preserve]
            public void Exception(Exception exception) { AssetSystem.LOG.Exception(exception); }
        }

        [Preserve]
        private enum LoadType
        {
            Sync,
            Coroutine,
            Async
        }

#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler, HideInCallstack]
#endif
        [DebuggerHidden]
        private string GetType(LoadType type)
        {
            switch (type)
            {
                case LoadType.Sync:
                    return "<b><color=#AF7AC5>【同步】</color></b> ";
                case LoadType.Coroutine:
                    return "<b><color=#F7DC6F>【协程】</color></b> ";
                case LoadType.Async:
                    return "<b><color=#B3E5FC>【异步】</color></b> ";
                default:
                    return $"【{type}】 ";
            }
        }

#endif

#if UNITY_EDITOR

#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler, HideInCallstack]
#endif
        [DebuggerHidden]
        private string GetLocation(string location)
        {
#if UNITY_EDITOR
            return (from asset in Dic.Values
                    where asset.CheckLocationValid(location)
                    select asset.GetAssetInfo(location)).FirstOrDefault()?.
                                                         AssetPath;
#else
            return location;
#endif
        }

        [IgnoreConsoleJump, DebuggerHidden]
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
        [HideInCallstack]
#endif
        private void PackageDebug(LoadType type, string location) { AssetSystem.LOG.I($"{GetType(type)} : [auto : {location}] -> {GetLocation(location)}"); }

        [IgnoreConsoleJump, DebuggerHidden]
#if UNITY_2022_1_OR_NEWER
        [IgnoredByDeepProfiler]
        [HideInCallstack]
#endif
        private void PackageDebug(LoadType type, string packageName, string location) { AssetSystem.LOG.I($"Load {GetType(type)} : [{packageName} : {location}] -> {GetLocation(location)}"); }
    }
}
#endif