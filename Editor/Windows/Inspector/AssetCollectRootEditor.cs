using UnityEditor;

namespace AIO.UEditor
{
    /// <summary>
    ///     AssetCollectRoot
    /// </summary>
    [CustomEditor(typeof(AssetCollectRoot))]
    public class AssetCollectRootEditor : AFInspector<AssetCollectRoot>
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