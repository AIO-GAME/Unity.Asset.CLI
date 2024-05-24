
/*|✩ - - - - - |||
|||✩ Date:     ||| -> Automatic Generate
|||✩ - - - - - |*/

using UnityEngine;

namespace AIO.UEditor
{
    partial class RuleCollect
    {
        #region Nested type: Collect Audio

        private struct CollectAudio : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Audio/default";
            public int DisplayIndex => 100;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "mixer" 
                    || data.Extension.ToLower() == "mpeg" 
                    || data.Extension.ToLower() == "aac" 
                    || data.Extension.ToLower() == "wmv" 
                    || data.Extension.ToLower() == "wma" 
                    || data.Extension.ToLower() == "mp3" 
                    || data.Extension.ToLower() == "ogg" 
                    || data.Extension.ToLower() == "flac" 
                    ;
            }
        }

        #region Nested type: Collect Audio MIXER

        private struct CollectAudioMIXER : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Audio/*.mixer";
            public int DisplayIndex => 100;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "mixer";
        }

        #endregion

        #endregion

        #region Nested type: Collect Font

        private struct CollectFont : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Font/default";
            public int DisplayIndex => 200;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "ttf" 
                    || data.Extension.ToLower() == "otf" 
                    || data.Extension.ToLower() == "fon" 
                    || data.Extension.ToLower() == "fnt" 
                    ;
            }
        }

        #endregion

        #region Nested type: Collect Text

        private struct CollectText : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Text/default";
            public int DisplayIndex => 300;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "lua" 
                    || data.Extension.ToLower() == "json" 
                    || data.Extension.ToLower() == "xml" 
                    || data.Extension.ToLower() == "bytes" 
                    || data.Extension.ToLower() == "txt" 
                    || data.Extension.ToLower() == "csv" 
                    || data.Extension.ToLower() == "yaml" 
                    || data.Extension.ToLower() == "asset" 
                    || data.Extension.ToLower() == "md" 
                    ;
            }
        }

        #region Nested type: Collect Text LUA

        private struct CollectTextLUA : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Text/*.lua";
            public int DisplayIndex => 300;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "lua";
        }

        #endregion

        #region Nested type: Collect Text JSON

        private struct CollectTextJSON : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Text/*.json";
            public int DisplayIndex => 301;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "json";
        }

        #endregion

        #region Nested type: Collect Text XML

        private struct CollectTextXML : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Text/*.xml";
            public int DisplayIndex => 302;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "xml";
        }

        #endregion

        #region Nested type: Collect Text BYTES

        private struct CollectTextBYTES : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Text/*.bytes";
            public int DisplayIndex => 303;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "bytes";
        }

        #endregion

        #endregion

        #region Nested type: Collect Texture

        private struct CollectTexture : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Texture/default";
            public int DisplayIndex => 400;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "png" 
                    || data.Extension.ToLower() == "jpg" 
                    || data.Extension.ToLower() == "spriteatlas" 
                    || data.Extension.ToLower() == "jpeg" 
                    || data.Extension.ToLower() == "tga" 
                    || data.Extension.ToLower() == "bmp" 
                    || data.Extension.ToLower() == "psd" 
                    || data.Extension.ToLower() == "gif" 
                    || data.Extension.ToLower() == "hdr" 
                    || data.Extension.ToLower() == "pic" 
                    || data.Extension.ToLower() == "pvr" 
                    || data.Extension.ToLower() == "dds" 
                    || data.Extension.ToLower() == "pkm" 
                    || data.Extension.ToLower() == "ktx" 
                    || data.Extension.ToLower() == "astc" 
                    || data.Extension.ToLower() == "webp" 
                    || data.Extension.ToLower() == "svg" 
                    || data.Extension.ToLower() == "exr" 
                    || data.Extension.ToLower() == "tiff" 
                    ;
            }
        }

        #region Nested type: Collect Texture PNG

        private struct CollectTexturePNG : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Texture/*.png";
            public int DisplayIndex => 400;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "png";
        }

        #endregion

        #region Nested type: Collect Texture JPG

        private struct CollectTextureJPG : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Texture/*.jpg";
            public int DisplayIndex => 401;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "jpg";
        }

        #endregion

        #region Nested type: Collect Texture SPRITEATLAS

        private struct CollectTextureSPRITEATLAS : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Texture/*.spriteatlas";
            public int DisplayIndex => 402;
            public bool IsCollectAsset(AssetRuleData data) => data.Extension.ToLower() == "spriteatlas";
        }

        #endregion

        #endregion

        #region Nested type: Collect Scene

        private struct CollectScene : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Scene/default";
            public int DisplayIndex => 500;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "unity" 
                    ;
            }
        }

        #endregion

        #region Nested type: Collect Material

        private struct CollectMaterial : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Material/default";
            public int DisplayIndex => 600;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "mat" 
                    ;
            }
        }

        #endregion

        #region Nested type: Collect Shader

        private struct CollectShader : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Shader/default";
            public int DisplayIndex => 700;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "shader" 
                    || data.Extension.ToLower() == "shadervariants" 
                    ;
            }
        }

        #endregion

        #region Nested type: Collect Prefab

        private struct CollectPrefab : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Prefab/default";
            public int DisplayIndex => 800;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "prefab" 
                    ;
            }
        }

        #endregion

        #region Nested type: Collect Model

        private struct CollectModel : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Model/default";
            public int DisplayIndex => 900;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "fbx" 
                    ;
            }
        }

        #endregion

        #region Nested type: Collect Terrain

        private struct CollectTerrain : AIO.UEditor.IAssetRuleFilter
        {
            public string DisplayFilterName => "Terrain/default";
            public int DisplayIndex => 1000;
            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension.ToLower() == "terrain" 
                    ;
            }
        }

        #endregion

    }
}