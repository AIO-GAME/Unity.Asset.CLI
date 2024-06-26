﻿// using System;
// using System.IO;
// using YooAsset.Editor;
//
// [DisplayName("打包特效纹理（自定义）")]
// public class PackEffectTexture : IPackRule
// {
//     private const string PackDirectory = "Assets/Effect/Textures/";
//
//     PackRuleResult IPackRule.GetPackRuleResult(PackRuleData data)
//     {
//         var assetPath = data.AssetPath;
//         if (assetPath.StartsWith(PackDirectory) == false)
//             throw new Exception($"Only support folder : {PackDirectory}");
//
//         var assetName = Path.GetFileName(assetPath).ToLower();
//         var firstChar = assetName.Substring(0, 1);
//         var bundleName = $"{PackDirectory}effect_texture_{firstChar}";
//         var packRuleResult = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
//         return packRuleResult;
//     }
//
//     bool IPackRule.IsRawFilePackRule()
//     {
//         return false;
//     }
// }
//
// [DisplayName("打包视频（自定义）")]
// public class PackVideo : IPackRule
// {
//     public PackRuleResult GetPackRuleResult(PackRuleData data)
//     {
//         var bundleName = RemoveExtension(data.AssetPath);
//         var fileExtension = Path.GetExtension(data.AssetPath);
//         fileExtension = fileExtension.Remove(0, 1);
//         var result = new PackRuleResult(bundleName, fileExtension);
//         return result;
//     }
//
//     bool IPackRule.IsRawFilePackRule()
//     {
//         return true;
//     }
//
//     private string RemoveExtension(string str)
//     {
//         if (string.IsNullOrEmpty(str))
//             return str;
//
//         var index = str.LastIndexOf(".", StringComparison.CurrentCulture);
//         return index == -1 ? str : str.Remove(index); //"assets/config/test.unity3d" --> "assets/config/test"
//     }
// }

