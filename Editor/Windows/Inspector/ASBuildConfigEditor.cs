using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    ///     ASBuildConfig
    /// </summary>
    [CustomEditor(typeof(AssetBuildConfig))]
    public class ASBuildConfigEditor : AFInspector<AssetBuildConfig>
    {
        private            AssetConfigCommonEditor editor;
        protected override void                    OnHeaderGUI() { editor.OnHeaderGUI(); }

        protected override void Awake() { editor = new AssetConfigCommonEditor(); }

        protected override void OnActivation()
        {
            if (editor is null) editor = new AssetConfigCommonEditor();
            editor.OnActivation();
        }

        protected override void OnGUI() { editor.OnGUI(); }
    }
}