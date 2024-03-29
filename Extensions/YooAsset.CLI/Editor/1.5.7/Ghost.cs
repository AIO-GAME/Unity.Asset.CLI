﻿using UnityEditor;

namespace AIO.UEditor.CLI
{
    internal static partial class Ghost
    {
        internal static partial class YooAsset
        {
            public const string Version = "1.5.7";
            public const string Scopes = "com.tuyoogame.yooasset";
            public const string Name = "YooAsset";
        }
    }
}


#if SUPPORT_YOOASSET
namespace AIO.UEditor.CLI
{
    internal static partial class Ghost
    {
        internal static partial class YooAsset
        {
            [MenuItem("AIO/CLI/UnInstall/YooAsset[" + Version + "](CN)", false, 0)]
            internal static void UnInstallCN()
            {
                EHelper.Ghost.UnInstallOpenUpm(Scopes, true);
            }

            [MenuItem("AIO/CLI/UnInstall/YooAsset[" + Version + "]", false, 0)]
            internal static void UnInstall()
            {
                EHelper.Ghost.UnInstallOpenUpm(Scopes);
            }
        }
    }
}
#else
namespace AIO.UEditor.CLI
{
    internal static partial class Ghost
    {
        internal static partial class YooAsset
        {
            [MenuItem("AIO/CLI/Install/YooAsset[" + Version + "](CN)", false, 0)]
            internal static void InstallCN()
            {
                EHelper.Ghost.OpenupmInstall(Scopes, Version, true);
            }

            [MenuItem("AIO/CLI/Install/YooAsset[" + Version + "]", false, 0)]
            internal static void Install()
            {
                EHelper.Ghost.OpenupmInstall(Scopes, Version);
            }
        }
    }
}
#endif