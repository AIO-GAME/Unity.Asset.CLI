using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

#if !SUPPORT_YOOASSET

namespace AIO.UEditor
{
    internal static partial class Install
    {
        [MenuItem("AIO/CLI/Install/YooAsset", false, 0)]
        internal static async void YooAssetRunAsync()
        {
            var Packages = new DirectoryInfo(EHelper.Path.Packages);
            try
            {
                EditorUtility.DisplayProgressBar("插件", "正在安装插件 YooAsset 1.5.7", 0);
                var result = await PrGit.Clone.Tag(
                    Path.Combine(Packages.FullName),
                    "https://github.com/tuyoogame/YooAsset.git", "1.5.7");
                if (result.ExitCode != 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Git Error : com.tuyoogame.yooasset", result.StdError.ToString(), "确定");
                    return;
                }

                EditorUtility.DisplayProgressBar("com.tuyoogame.yooasset", "moveing folder", 0);
                result = await PrPlatform.Folder.Move(
                    Path.Combine(Packages.FullName, "com.tuyoogame.yooasset"),
                    Path.Combine(Packages.FullName, "YooAsset", "Assets", "YooAsset")
                );
                if (result.ExitCode != 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Git Error : com.tuyoogame.yooasset", result.StdError.ToString(), "确定");
                    return;
                }

                EditorUtility.DisplayProgressBar("com.tuyoogame.yooasset", "delete folder", 0);
                result = await PrPlatform.Folder.Del(Path.Combine(Packages.FullName, "YooAsset"));
                if (result.ExitCode != 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("Git Error : com.tuyoogame.yooasset", result.StdError.ToString(), "确定");
                    return;
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                CompilationPipeline.RequestScriptCompilation();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Git Error : com.tuyoogame.yooasset", e.Message, "确定");
            }
        }
    }
}
#else
namespace AIO.UEditor
{
    internal static partial class UnInstall
    {
        [MenuItem("AIO/CLI/UnInstall/YooAsset", false, 0)]
        internal static async void YooAssetRunAsync()
        {
            var Root = Directory.GetParent(Application.dataPath);
            if (Root is null) throw new Exception("Application.dataPath is null");
            var Packages = new DirectoryInfo(Path.Combine(Root.FullName, "Packages"));
            try
            {
                var target = Path.Combine(Packages.FullName, "com.tuyoogame.yooasset");
                if (Directory.Exists(target))
                {
                    EditorUtility.DisplayProgressBar("com.tuyoogame.yooasset", "uninstall", 0);
                    await PrPlatform.Folder.Del(target);
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.Refresh();
                    CompilationPipeline.RequestScriptCompilation();
                }
                else EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Git Error : com.tuyoogame.yooasset", e.Message, "确定");
            }
        }
    }
}
#endif