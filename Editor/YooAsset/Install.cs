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
        /// <summary>
        /// 设置功能函数
        /// </summary>
        private static class Setting
        {
            private static ICollection<string> GetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup)
            {
                var str =
#if UNITY_2023_1_OR_NEWER
                    PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));
#else
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif

                return string.IsNullOrEmpty(str) ? Array.Empty<string>() : str.Split(';');
            }

            private static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup,
                IEnumerable<string> verify)
            {
                var str = string.Join(";", verify);
#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), str);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, str);
#endif
            }

            /// <summary>
            /// 添加传入的宏定义
            /// </summary>
            public static void AddScriptingDefine(BuildTargetGroup buildTargetGroup, ICollection<string> value)
            {
                if (value is null || value.Count == 0) return;
                Debug.Log($"Plugins Data Editor : AddScriptingDefine -> {buildTargetGroup}");
                var verify = new List<string>(GetScriptingDefineSymbolsForGroup(buildTargetGroup));
                foreach (var v in value)
                {
                    if (string.IsNullOrEmpty(v) || verify.Contains(v)) continue;
                    verify.Add(v);
                }

                SetScriptingDefineSymbolsForGroup(buildTargetGroup, verify.RemoveRepeat());
            }

            /// <summary>
            /// 禁止传入的宏定义
            /// </summary>
            public static void DelScriptingDefine(BuildTargetGroup buildTargetGroup, ICollection<string> value)
            {
                if (value is null || value.Count == 0) return;
                Debug.Log($"Plugins Data Editor : DelScriptingDefine -> {buildTargetGroup}");
                var str = GetScriptingDefineSymbolsForGroup(buildTargetGroup);
                if (str.Count == 0) return;
                IList<string> verify = new List<string>(str);
                verify = verify.RemoveRepeat();
                foreach (var item in value) verify.Remove(item);
                SetScriptingDefineSymbolsForGroup(buildTargetGroup, verify);
            }
        }

        internal static DirectoryInfo GetValidDir(string rootDir, string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var dirInfo = new DirectoryInfo(value);
            if (dirInfo.Exists) return dirInfo;
            var root = new DirectoryInfo(Path.Combine(rootDir, Path.GetPathRoot(value)));

            var name = dirInfo.Name;
            try
            {
                var regex = new Regex(value);
                foreach (var directory in root.GetDirectories("*.*",
                             value.Contains("*") ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    if (directory.Name == name && regex.Match(directory.FullName).Success)
                        return directory;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return dirInfo;
        }

        [MenuItem("AIO/CLI/Install/YooAsset", false, 0)]
        public static async void Run()
        {
            var Root = Directory.GetParent(Application.dataPath);
            if (Root is null) throw new Exception("Application.dataPath is null");
            var Packages = new DirectoryInfo(Path.Combine(Root.FullName, "Packages"));
            if (!Packages.Exists) throw new Exception("Packages folder not found");
            var yooAsset = new DirectoryInfo(Path.Combine(Packages.FullName, "com.tuyoogame.yooasset"));
            if (yooAsset.Exists) return;

            var datas = AssetDatabase.FindAssets($"t:{nameof(PluginData)}", new string[] { "Packages" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PluginData>)
                .Where(value => value != null);
            foreach (var data in datas)
            {
                Debug.Log(data.Name);
                if (!data.Name.ToLower().Contains("yoo asset")) continue;
                var dataPath = Root.FullName;
                var macroList = new List<string>();
                var source = GetValidDir(dataPath, data.SourceRelativePath);
                if (!source.Exists) continue;

                var target = GetValidDir(dataPath, data.TargetRelativePath);
                if (target.Exists) continue;

                if (!string.IsNullOrEmpty(data.MacroDefinition))
                {
                    if (data.MacroDefinition.Contains(";"))
                    {
                        macroList.AddRange(data.MacroDefinition.Split(';'));
                    }
                    else macroList.Add(data.MacroDefinition);
                }

                var parent = Directory.GetParent(target.FullName);
                if (parent != null && !parent.Exists) parent.Create();
                EditorUtility.DisplayProgressBar("插件", "正在安装插件", 0);
                await PrPlatform.Folder.Symbolic(target.FullName, source.FullName).Async();
                Setting.AddScriptingDefine(EditorUserBuildSettings.selectedBuildTargetGroup, macroList);
                AssetDatabase.Refresh();
#if UNITY_2020_1_OR_NEWER
                AssetDatabase.RefreshSettings();
#endif
                CompilationPipeline.compilationStarted += CompilationPipelineCompilationStartedEnd;
                CompilationPipeline.RequestScriptCompilation();
                EditorUtility.ClearProgressBar();
                break;
            }
        }

        private static void CompilationPipelineCompilationStartedEnd(object o)
        {
            EditorUtility.DisplayProgressBar("插件", "正在编译", 0);
            CompilationPipeline.compilationStarted -= CompilationPipelineCompilationStartedEnd;
            CompilationPipeline.compilationFinished += CompilationPipelineCompilationFinishedEnd;
        }

        private static void CompilationPipelineCompilationFinishedEnd(object o)
        {
            CompilationPipeline.compilationFinished -= CompilationPipelineCompilationFinishedEnd;
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh(
                ImportAssetOptions.ForceSynchronousImport |
                ImportAssetOptions.ForceUpdate |
                ImportAssetOptions.ImportRecursive |
                ImportAssetOptions.DontDownloadFromCacheServer |
                ImportAssetOptions.ForceUncompressedImport
            );
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}
#endif