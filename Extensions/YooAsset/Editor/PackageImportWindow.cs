// using System.IO;
// using UnityEditor;
// using UnityEngine;
//
// namespace YooAsset.Editor
// {
//     public class PackageImportWindow : EditorWindow
//     {
//         private static PackageImportWindow _thisInstance;
//
//         [MenuItem("YooAsset/补丁包导入工具", false)]
//         private static void ShowWindow()
//         {
//             if (_thisInstance == null)
//             {
//                 _thisInstance = GetWindow<PackageImportWindow>(false, "补丁包导入工具", true);
//                 _thisInstance.minSize = new Vector2(800, 600);
//             }
//
//             _thisInstance.Show();
//         }
//
//         private string _manifestPath = string.Empty;
//         private string _packageName = "DefaultPackage";
//
//         private void OnGUI()
//         {
//             GUILayout.Space(10);
//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button("选择补丁包", GUILayout.MaxWidth(150)))
//             {
//                 var resultPath = EditorUtility.OpenFilePanel("Find", "Assets/", "bytes");
//                 if (string.IsNullOrEmpty(resultPath))
//                     return;
//                 _manifestPath = resultPath;
//             }
//
//             EditorGUILayout.LabelField(_manifestPath);
//             EditorGUILayout.EndHorizontal();
//
//             if (string.IsNullOrEmpty(_manifestPath)) return;
//             if (GUILayout.Button("导入补丁包（全部文件）", GUILayout.MaxWidth(150)))
//             {
//                 string streamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot();
//                 EditorTools.ClearFolder(streamingAssetsRoot);
//                 CopyPackageFiles(_manifestPath);
//             }
//         }
//
//         private void CopyPackageFiles(string manifestFilePath)
//         {
//             var manifestFileName = Path.GetFileNameWithoutExtension(manifestFilePath);
//             var outputDirectory = Path.GetDirectoryName(manifestFilePath);
//
//             // 加载补丁清单
//             var bytesData = FileUtility.ReadAllBytes(manifestFilePath);
//             var manifest = ManifestTools.DeserializeFromBinary(bytesData);
//
//             // 拷贝核心文件
//             {
//                 var sourcePath = $"{outputDirectory}/{manifestFileName}.bytes";
//                 var destPath =
//                     $"{AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot()}/{_packageName}/{manifestFileName}.bytes";
//                 EditorTools.CopyFile(sourcePath, destPath, true);
//             }
//             {
//                 var sourcePath = $"{outputDirectory}/{manifestFileName}.hash";
//                 var destPath =
//                     $"{AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot()}/{_packageName}/{manifestFileName}.hash";
//                 EditorTools.CopyFile(sourcePath, destPath, true);
//             }
//             {
//                 var fileName = YooAssetSettingsData.GetPackageVersionFileName(manifest.PackageName);
//                 var sourcePath = $"{outputDirectory}/{fileName}";
//                 var destPath = $"{AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot()}/{_packageName}/{fileName}";
//                 EditorTools.CopyFile(sourcePath, destPath, true);
//             }
//
//             // 拷贝文件列表
//             var fileCount = 0;
//             foreach (var packageBundle in manifest.BundleList)
//             {
//                 fileCount++;
//                 var sourcePath = $"{outputDirectory}/{packageBundle.FileName}";
//                 var destPath =
//                     $"{AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot()}/{_packageName}/{packageBundle.FileName}";
//                 EditorTools.CopyFile(sourcePath, destPath, true);
//             }
//
//             Debug.Log($"补丁包拷贝完成，一共拷贝了{fileCount}个资源文件");
//             AssetDatabase.Refresh();
//         }
//     }
// }