#if !SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;
using UnityEngine;

namespace AIO.Editor
{
    internal static class Install
    {
        [MenuItem("AIO/CLI/Install/YooAsset", false, 0)]
        internal static async void RunAsync()
        {
            var Root = Directory.GetParent(Application.dataPath);
            if (Root is null) throw new Exception("Application.dataPath is null");
            var Packages = new DirectoryInfo(Path.Combine(Root.FullName, "Packages"));
            try
            {
                EditorUtility.DisplayProgressBar("插件", "正在安装插件", 0);
                var result = await PrGit.Clone.Tag(
                    Path.Combine(Packages.FullName),
                    "https://github.com/tuyoogame/YooAsset.git", "1.5.3-preview");
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
#endif