using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewItemDependencies : TreeViewItem, ITVItemDraw
    {
        private AssetPageLook.DependenciesInfo data;

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => 20;
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        public bool MatchSearch(string search) => search.Contains(data.Name, StringComparison.CurrentCultureIgnoreCase)
                                               || search.Contains(data.Type, StringComparison.CurrentCultureIgnoreCase)
                                               || data.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                                               || data.Type.Contains(search, StringComparison.CurrentCultureIgnoreCase);

        internal TreeViewItemDependencies(int id, AssetPageLook.DependenciesInfo data) : base(id, 1) { this.data = data; }

        /// <summary>
        ///     界面内容 - 实例物体选择打开
        /// </summary>
        private readonly GUIContent GC_LookMode_Object_Select = GEContent.NewBuiltin("d_scenepicking_pickable_hover", "选择指向指定资源");

        public void OnDraw(Rect cell, int col, ref RowGUIArgs args)
        {
            switch (col)
            {
                case 0:
                    cell.x      += 5;
                    cell.width  -= 5;
                    GUI.enabled =  false;
                    EditorGUI.ObjectField(cell, data.Object, data.GetType(), false);
                    GUI.enabled =  true;
                    cell.x      += cell.width;
                    cell.width  =  1;
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 1:
                    cell.x     += 5;
                    cell.width -= 5;
                    EditorGUI.LabelField(cell, data.Size.ToConverseStringFileSize());
                    cell.x     += cell.width + 2;
                    cell.width =  1;
                    EditorGUI.DrawRect(cell, TreeViewBasics.ColorLine);
                    break;
                case 2:
                    cell.x += cell.width / 4;
                    if (GUI.Button(cell, GC_LookMode_Object_Select, GEStyle.IconButton))
                    {
                        EditorUtility.RevealInFinder(data.AssetPath);
                        Selection.activeObject = data.Object;
                    }

                    break;
            }
        }
    }
}