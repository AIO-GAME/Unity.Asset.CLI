using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    public class TreeViewItemDependencies : TreeViewItem, ITVItemDraw
    {
        private AssetCollectWindow.DependenciesInfo data;

        public bool  AllowChangeExpandedState             => false;
        public bool  AllowRename                          => false;
        public float GetHeight()                          => 20;
        public Rect  GetRenameRect(Rect rowRect, int row) => rowRect;

        public bool MatchSearch(string search) => search.Contains(data.Name, StringComparison.CurrentCultureIgnoreCase)
                                               || search.Contains(data.Type, StringComparison.CurrentCultureIgnoreCase)
                                               || data.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)
                                               || data.Type.Contains(search, StringComparison.CurrentCultureIgnoreCase);

        internal TreeViewItemDependencies(int id, AssetCollectWindow.DependenciesInfo data) : base(id, 1) { this.data = data; }

        /// <summary>
        ///     界面内容 - 实例物体选择打开
        /// </summary>
        private readonly GUIContent GC_LookMode_Object_Select = GEContent.NewBuiltin("d_scenepicking_pickable_hover", "选择指向指定资源");

        public void OnDraw(Rect cellRect, int col, ref RowGUIArgs args)
        {
            switch (col)
            {
                case 0:
                    cellRect.x     += 5;
                    cellRect.width -= 5;
                    GUI.enabled    =  false;
                    EditorGUI.ObjectField(cellRect, data.Object, data.GetType(), false);
                    GUI.enabled    =  true;
                    cellRect.x     += cellRect.width;
                    cellRect.width =  1;
                    EditorGUI.DrawRect(cellRect, TreeViewBasics.ColorLine);
                    break;
                case 1:
                    cellRect.x     += 5;
                    cellRect.width -= 5;
                    EditorGUI.LabelField(cellRect, data.Size.ToConverseStringFileSize());
                    cellRect.x     += cellRect.width + 2;
                    cellRect.width =  1;
                    EditorGUI.DrawRect(cellRect, TreeViewBasics.ColorLine);
                    break;
                case 2:
                    cellRect.x += cellRect.width / 4;
                    if (GUI.Button(cellRect, GC_LookMode_Object_Select, GEStyle.IconButton))
                    {
                        EditorUtility.RevealInFinder(data.AssetPath);
                        Selection.activeObject = data.Object;
                    }

                    break;
            }
        }
    }
}