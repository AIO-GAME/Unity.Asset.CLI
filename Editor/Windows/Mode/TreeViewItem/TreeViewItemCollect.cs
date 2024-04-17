using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    partial class ViewTreeCollect
    {
        protected sealed class TreeViewItemCollect : TreeViewItem, IGraphDraw
        {
            private static Color LineColor = RHelper.Color.HexToColor("#212121");

            /// <summary>
            ///     界面内容 - 收缩
            /// </summary>
            private static GUIContent GC_FOLDOUT = GEContent.NewSetting("quanping-shouqi-xian", "收缩");

            /// <summary>
            ///     界面内容 - 展开
            /// </summary>
            private static GUIContent GC_FOLDOUT_ON = GEContent.NewSetting("quanping-zhankai-xian", "展开");

            private static GUIContent GC_OPEN = GEContent.NewSettingCustom("Editor/Setting/icon_information", "打开指定查询模式");
            private static GUIContent GC_DEL  = GEContent.NewSetting("删除", "删除元素");

            private GUILayoutOption  GP_Width_80 = GTOption.Width(80);
            private GUILayoutOption  GP_Width_30 = GTOption.Width(30);
            private AssetCollectItem Item;

            /// <summary>
            ///    收缩状态变更事件
            /// </summary>
            public event Action<bool> OnChangedFold;

            public TreeViewItemCollect(int id, AssetCollectItem item) : base(id, 1)
            {
                Item = item;
            }

            #region IGraphDraw

            public float    Height                                                   => Item.Folded ? 105 : 27;
            bool IGraphDraw.AllowChangeExpandedState                                 => false;
            bool IGraphDraw.AllowRename                                              => false;
            Rect IGraphDraw.GetRenameRect(Rect cellRect, int row, TreeViewItem item) => Rect.zero;

            void IGraphDraw.OnDraw(Rect cellRect, ref RowGUIArgs args)
            {
                if (Item.Enable)
                {
                    OnDrawContent(cellRect, ref args);
                }
                else
                {
                    var oldColor = GUI.color;
                    GUI.color = Color.gray;
                    OnDrawContent(cellRect, ref args);
                    GUI.color = oldColor;
                }
            }

            #endregion

            private void OP_Open()
            {
                GUI.FocusControl(null);
                AssetCollectWindow.OpenCollectItem(Item);
            }

            private void OP_DEL()
            {
                GUI.FocusControl(null);
                if (EditorUtility.DisplayDialog("删除", "确定删除当前收集器?", "确定", "取消"))
                {
                    AssetCollectRoot.GetOrCreate().CurrentGroup.Remove(Item);
                }
            }

            private void OnDrawHeader(Rect rect, ref RowGUIArgs args)
            {
                if (Item.Path is null) GUI.enabled = false;

                var rect1 = new Rect(rect.x + 10, rect.y + 1, 20, 20);
                if (GUI.Button(rect1, Item.Folded ? GC_FOLDOUT : GC_FOLDOUT_ON, GEStyle.toolbarbutton))
                {
                    Item.Folded = !Item.Folded;
                    OnChangedFold?.Invoke(Item.Folded);
                }

                var rect2 = new Rect(rect1.x + 20, rect1.y + 1, 80, 20);
                Item.Type = (EAssetCollectItemType)EditorGUI.EnumPopup(rect2, Item.Type, GEStyle.PreDropDown);

                var rectObj = new Rect(rect2.x + 85, rect2.y, 150, 20);
                Item.Path = EditorGUI.ObjectField(rectObj, Item.Path, typeof(Object), false);

                //
                // if (!Item.Folded)
                // {
                //     Item.Address       = GELayout.Popup(Item.Address, AssetCollectSetting.MapAddress.Displays, GEStyle.PreDropDown);
                //     Item.RulePackIndex = GELayout.Popup(Item.RulePackIndex, AssetCollectSetting.MapPacks.Displays, GEStyle.PreDropDown);
                //     Item.LoadType      = GELayout.Popup(Item.LoadType, GEStyle.PreDropDown, GTOption.Width(75));
                // }
                // else
                // {
                //     if (!string.IsNullOrEmpty(Item.CollectPath))
                //     {
                //         if (GUILayout.Button(Item.CollectPath, GEStyle.toolbarbutton))
                //             _ = PrPlatform.Open.Path(Item.CollectPath).Async();
                //     }
                //     else
                //     {
                //         Item.Folded = false;
                //         return;
                //     }
                // }
                //
                // if (GELayout.Button(GC_OPEN, GEStyle.TEtoolbarbutton, 24))
                // {
                //     OP_Open();
                //     return;
                // }
                //
                if (Item.Path is null) GUI.enabled = true;
                // if (GELayout.Button(GC_DEL, GEStyle.TEtoolbarbutton, 24))
                // {
                //     OP_DEL();
                //     return;
                // }
            }

            private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
            {
                GUI.Box(new Rect(args.rowRect.x, args.rowRect.y + 1, cellRect.width, 25), string.Empty, GEStyle.Toolbar);
                var rect = new Rect(args.rowRect.x, args.rowRect.y + 1, cellRect.width, Height - 5);
                if (args.selected)
                    GUI.Box(rect, string.Empty, GEStyle.SelectionRect);
                else
                    GUI.Box(rect, string.Empty, GEStyle.Badge);

                OnDrawHeader(rect, ref args);
                if (Item.Folded)
                {
                    EditorGUI.DrawRect(new Rect(rect.x, rect.y + 21, rect.width, 1), LineColor);
                }
            }
        }
    }
}