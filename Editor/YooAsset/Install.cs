using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
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
                //获得当前平台已有的的宏定义
                var GetScriptingDefineSymbols = typeof(PlayerSettings).GetMethod("GetScriptingDefineSymbolsInternal",
                    BindingFlags.Static | BindingFlags.NonPublic);
                string str = null;
                if (GetScriptingDefineSymbols != null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (!assembly.GetName().Name.StartsWith("UnityEditor.Build")) continue;
                        var namedBuildTargetType = assembly.GetType("UnityEditor.Build.NamedBuildTarget");
                        var FromBuildTargetGroupMethod = namedBuildTargetType?.GetMethod("FromBuildTargetGroup",
                            BindingFlags.Static | BindingFlags.Public);
                        if (FromBuildTargetGroupMethod is null) continue;
                        var symbols = FromBuildTargetGroupMethod.Invoke(null, new object[] { buildTargetGroup });
                        str = GetScriptingDefineSymbols.Invoke(null, new object[] { symbols }) as string;
                        break;
                    }
                }
                else str = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

                return string.IsNullOrEmpty(str) ? Array.Empty<string>() : str.Split(';');
            }

            private static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup buildTargetGroup,
                IEnumerable<string> verify)
            {
                //获得当前平台已有的的宏定义
                MethodInfo SetScriptingDefineSymbols = null;
                foreach (var methodInfo in typeof(PlayerSettings).GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (methodInfo.Name != "SetScriptingDefineSymbols") continue;
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length != 2) continue;
                    if (parameters[0].ParameterType != typeof(string)) continue;
                    if (parameters[1].ParameterType != typeof(string)) continue;
                    SetScriptingDefineSymbols = methodInfo;
                }

                var str = string.Join(";", verify);
                if (SetScriptingDefineSymbols != null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (!assembly.GetName().Name.StartsWith("UnityEditor.Build")) continue;
                        var namedBuildTargetType = assembly.GetType("UnityEditor.Build.NamedBuildTarget");
                        var FromBuildTargetGroupMethod = namedBuildTargetType?.GetMethod("FromBuildTargetGroup",
                            BindingFlags.Static | BindingFlags.Public);
                        if (FromBuildTargetGroupMethod is null) continue;
                        var Symbols = FromBuildTargetGroupMethod.Invoke(null, new object[] { buildTargetGroup });
                        SetScriptingDefineSymbols.Invoke(null, new object[] { Symbols, str });

                        break;
                    }
                }
                else PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, str);
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

        [InitializeOnLoadMethod]
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