using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

#if !SUPPORT_WHOOTHOT
namespace AIO.UEditor
{
    internal static partial class Install
    {
        [MenuItem("AIO/CLI/Install/Whoot HOT", false, 0)]
        internal static async void WhootHOTRunAsync()
        {
            var Root = Directory.GetParent(Application.dataPath);
            if (Root is null) throw new Exception("Application.dataPath is null");
            var Packages = new DirectoryInfo(Path.Combine(Root.FullName, "Packages"));
            try
            {
                EditorUtility.DisplayProgressBar("插件", "正在安装插件", 0);
                var result = await PrGit.Clone.Branch(
                    Path.Combine(Packages.FullName),
                    "https://git.ingcreations.com/whoot-games-hot/client/hot_addressables.git",
                    "com.unity.addressables",
                    "master"
                );
                if (result.ExitCode != 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Git Error : com.unity.addressables", result.StdError.ToString(), "确定");
                    return;
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                CompilationPipeline.RequestScriptCompilation();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Git Error : com.unity.addressables", e.Message, "确定");
            }
        }
    }
}

#else

namespace AIO.UEditor
{
    internal static partial class UnInstall
    {
        [MenuItem("AIO/CLI/UnInstall/Whoot HOT", false, 0)]
        internal static async void WhootHOTRunAsync()
        {
            var Root = Directory.GetParent(Application.dataPath);
            if (Root is null) throw new Exception("Application.dataPath is null");
            try
            {
                EditorUtility.DisplayProgressBar("com.unity.addressables", "uninstall", 0);
                var target = Path.Combine(Root.FullName, "Packages", "com.unity.addressables");
                if (Directory.Exists(target)) await PrPlatform.Folder.Del(target);

                target = Path.Combine(Root.FullName, "Library", "com.unity.addressables");
                if (Directory.Exists(target)) await PrPlatform.Folder.Del(target);

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                CompilationPipeline.RequestScriptCompilation();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Git Error : com.unity.addressables", e.Message, "确定");
            }
        }
    }
}

#endif