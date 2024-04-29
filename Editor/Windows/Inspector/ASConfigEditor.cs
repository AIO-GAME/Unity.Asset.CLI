using AIO.UEngine;
using UnityEditor;

namespace AIO.UEditor
{
    [CustomEditor(typeof(ASConfig))]
    public class ASConfigEditor : AFInspector<ASConfig>
    {
        private AssetConfigCommonEditor editor;

        [MenuItem(AssetWindow.MENU_CONFIG)]
        public static void Create() => Selection.activeObject = ASConfig.GetOrCreate();

        protected override void OnHeaderGUI() { editor.OnHeaderGUI(); }

        protected override void Awake() { editor = new AssetConfigCommonEditor(); }

        protected override void OnActivation()
        {
            if (editor is null) editor = new AssetConfigCommonEditor();
            editor.OnActivation();
        }

        protected override void OnGUI() { editor.OnGUI(); }
    }
}