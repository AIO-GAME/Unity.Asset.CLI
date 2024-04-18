using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AIO.UEditor
{
    partial class ViewTreeCollect
    {
        private static readonly GUIContent GC_FOLDOUT_ON = EditorGUIUtility.IconContent("d_Folder Icon").SetTooltips("折叠");
        private static readonly GUIContent GC_FOLDOUT    = EditorGUIUtility.IconContent("d_FolderOpened Icon").SetTooltips("展开");
        private static readonly GUIContent GC_OPEN       = EditorGUIUtility.IconContent("d_RectTransform Icon").SetTooltips("打开");
        private static readonly GUIContent GC_DEL        = EditorGUIUtility.IconContent("d_ol_minus_act").SetTooltips("删除");

        protected sealed class TreeViewItemCollect : TreeViewItem, IGraphDraw
        {
            public readonly AssetCollectItem Item;

            /// <summary>
            ///    收缩状态变更事件
            /// </summary>
            public event Action<bool> OnChangedFold;

            public TreeViewItemCollect(int id, AssetCollectItem item) : base(id, 1)
            {
                Item = item;
            }

            #region IGraphDraw

            bool IGraphDraw.AllowChangeExpandedState => false;
            bool IGraphDraw.AllowRename              => false;

            Rect IGraphDraw.GetRenameRect(Rect cellRect, int row) => Rect.zero;

            public float GetHeight()
            {
                if (!Item.Path || !Item.Folded) return 27;
                var temp = 127;
                if (Item.Type == EAssetCollectItemType.MainAssetCollector) temp += 27;
                return temp;
            }

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

            public void OP_Open()
            {
                GUI.FocusControl(null);
                AssetCollectWindow.OpenCollectItem(Item);
            }

            public void OP_DEL()
            {
                GUI.FocusControl(null);
                if (EditorUtility.DisplayDialog("删除", "确定删除当前收集器?", "确定", "取消"))
                {
                    AssetCollectRoot.GetOrCreate().CurrentGroup.Remove(Item);
                }
            }

            private void OnDrawHeader(Rect rect, ref RowGUIArgs args)
            {
                if (!Item.Path) GUI.enabled = false;

                var rect1 = new Rect(rect.x + 5, rect.y + 3, 24, 20);
                if (GUI.Button(rect1, Item.Folded ? GC_FOLDOUT.image : GC_FOLDOUT_ON.image, GEStyle.IconButton))
                {
                    Item.Folded = !Item.Folded;
                    OnChangedFold?.Invoke(Item.Folded);
                }

                var rectType = new Rect(rect1.x + rect1.width, rect.y + 1, 70, 20);
                Item.Type = (EAssetCollectItemType)EditorGUI.EnumPopup(rectType, Item.Type, GEStyle.PreDropDown);
                if (!Item.Path) GUI.enabled = true;
                var spaceTemp = Item.Folded ? 0 : 450;
                var rectObjBox = new Rect(rectType.x + rectType.width, rectType.y, rect.width - rectType.x - rectType.width - 45 - spaceTemp, 20);
                Item.Path = GELayout.FieldObject(rectObjBox, Item.Path, GEStyle.ToolbarDropDownToggle, GEStyle.DDItemStyle);
                if (!Item.Path) GUI.enabled = false;
                if (!Item.Folded)
                {
                    var rectAddress = new Rect(rectObjBox.x + rectObjBox.width, rectType.y, 200, 20);
                    var rectRulePack = new Rect(rectAddress.x + rectAddress.width, rectType.y, 200, 20);
                    var rectLoadType = new Rect(rectRulePack.x + rectRulePack.width, rectType.y, 50, 20);
                    Item.Address       = EditorGUI.IntPopup(rectAddress, Item.Address, AssetCollectSetting.MapAddress.Displays.ToArray(), null, GEStyle.PreDropDown);
                    Item.RulePackIndex = EditorGUI.IntPopup(rectRulePack, Item.RulePackIndex, AssetCollectSetting.MapPacks.Displays.ToArray(), null, GEStyle.PreDropDown);
                    Item.LoadType      = (EAssetLoadType)EditorGUI.EnumPopup(rectLoadType, Item.LoadType, GEStyle.PreDropDown);
                }

                var rectOpen = new Rect(rectObjBox.x + rectObjBox.width + 5 + spaceTemp, rectType.y + 2, 20, 20);
                if (GELayout.Button(rectOpen, GC_OPEN, GEStyle.IconButton))
                {
                    OP_Open();
                    return;
                }

                if (!Item.Path) GUI.enabled = true;
                var rectDel = new Rect(rectOpen.x + rectOpen.width, rectType.y + 2, 20, 20);
                if (GELayout.Button(rectDel, GC_DEL, GEStyle.IconButton)) OP_DEL();
            }

            private void OnDrawBody1(Rect rect, ref RowGUIArgs args)
            {
                var rectLabel = new Rect(rect.x + 32, rect.y + 3, 70, 20);
                GUI.Label(rectLabel, "寻址规则", GEStyle.HeaderLabel);

                var rectAddress = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, rect.width - rectLabel.x - rectLabel.width - 392, 20);
                Item.Address = EditorGUI.IntPopup(rectAddress, Item.Address, AssetCollectSetting.MapAddress.Displays.ToArray(), null, GEStyle.PreDropDown);

                var rectRulePack = new Rect(rect.width - 45 - 75 - 75 - 200, rectLabel.y, 200, 20);
                Item.RulePackIndex = EditorGUI.IntPopup(rectRulePack, Item.RulePackIndex, AssetCollectSetting.MapPacks.Displays.ToArray(), null, GEStyle.PreDropDown);

                var rectFormat = new Rect(rect.width - 45 - 75 - 75, rectLabel.y, 75, 20);
                if (ASConfig.GetOrCreate().LoadPathToLower)
                {
                    GUI.enabled = false;
                    EditorGUI.EnumPopup(rectFormat, EAssetLocationFormat.ToLower, GEStyle.PreDropDown);
                    GUI.enabled = true;
                }
                else
                {
                    Item.LocationFormat = (EAssetLocationFormat)EditorGUI.EnumPopup(rectFormat, Item.LocationFormat, GEStyle.PreDropDown);
                }

                var rectLoadType = new Rect(rect.width - 45 - 75, rectLabel.y, 75, 20);
                Item.LoadType = (EAssetLoadType)EditorGUI.EnumPopup(rectLoadType, Item.LoadType, GEStyle.PreDropDown);

                var rectExtension = new Rect(rect.width - 45, rectLabel.y, 45, 20);
                if (ASConfig.GetOrCreate().HasExtension)
                {
                    GUI.enabled       = false;
                    Item.HasExtension = EditorGUI.ToggleLeft(rectExtension, "后缀", true);
                    GUI.enabled       = true;
                }
                else Item.HasExtension = EditorGUI.ToggleLeft(rectExtension, "后缀", Item.HasExtension);
            }

            private void OnDrawBody2(Rect rect, ref RowGUIArgs args)
            {
                var rectLabel = new Rect(rect.x + 32, rect.y + 3, 70, 20);
                GUI.Label(rectLabel, "收集规则", GEStyle.HeaderLabel);
                if (Item.RuleUseCollectCustom)
                {
                    var rectCollect = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, rect.width - rectLabel.x - rectLabel.width - 45, 20);
                    Item.RuleCollect = EditorGUI.DelayedTextField(rectCollect, Item.RuleCollect);
                }
                else
                {
                    var rectCollect = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, 200, 20);
                    Item.RuleCollectIndex = EditorGUI.MaskField(
                        rectCollect, Item.RuleCollectIndex,
                        AssetCollectSetting.MapCollect.Displays.ToArray(),
                        GEStyle.PreDropDown);
                    var rectHelp = new Rect(rectCollect.x + rectCollect.width, rectLabel.y, rect.width - rectCollect.x - rectCollect.width - 45, 20);
                    EditorGUI.HelpBox(rectHelp, GetInfo(AssetCollectSetting.MapCollect.Displays, Item.RuleCollectIndex), MessageType.None);
                }

                var rectCustom = new Rect(rect.width - 45, rectLabel.y, 45, 20);
                Item.RuleUseCollectCustom = EditorGUI.ToggleLeft(
                    rectCustom,
                    new GUIContent("自定", "自定义收集规则 \n传入文件后缀 \n[冒号(;)/空格( )/逗号(,)]隔开 \n可无需填写点(.)"),
                    Item.RuleUseCollectCustom);
            }

            private void OnDrawBody3(Rect rect, ref RowGUIArgs args)
            {
                var rectLabel = new Rect(rect.x + 32, rect.y + 3, 70, 20);
                GUI.Label(rectLabel, "过滤规则", GEStyle.HeaderLabel);
                if (Item.RuleUseFilterCustom)
                {
                    var rectCollect = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, rect.width - rectLabel.x - rectLabel.width - 45, 20);
                    Item.RuleFilter = EditorGUI.DelayedTextField(rectCollect, Item.RuleFilter);
                }
                else
                {
                    var rectCollect = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, 200, 20);
                    Item.RuleFilterIndex = EditorGUI.MaskField(
                        rectCollect, Item.RuleFilterIndex,
                        AssetCollectSetting.MapCollect.Displays.ToArray(),
                        GEStyle.PreDropDown);
                    var rectHelp = new Rect(rectCollect.x + rectCollect.width, rectLabel.y, rect.width - rectCollect.x - rectCollect.width - 45, 20);
                    EditorGUI.HelpBox(rectHelp, GetInfo(AssetCollectSetting.MapFilter.Displays, Item.RuleFilterIndex), MessageType.None);
                }

                var rectCustom = new Rect(rect.width - 45, rectLabel.y, 45, 20);
                Item.RuleUseFilterCustom = EditorGUI.ToggleLeft(
                    rectCustom,
                    new GUIContent("自定", "自定义过滤规则 \n传入文件后缀 \n[冒号(;)/空格( )/逗号(,)]隔开\n可无需填写点(.)"),
                    Item.RuleUseFilterCustom);
            }

            private void OnDrawBody4(Rect rect, ref RowGUIArgs args)
            {
                var rectLabel = new Rect(rect.x + 32, rect.y + 3, 70, 20);
                GUI.Label(rectLabel, "自定数据", GEStyle.HeaderLabel);
                var rectUserData = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, rect.width - rectLabel.x - rectLabel.width, 20);
                Item.UserData = EditorGUI.DelayedTextField(rectUserData, Item.UserData);
            }

            private void OnDrawBody5(Rect rect, ref RowGUIArgs args)
            {
                var rectLabel = new Rect(rect.x + 32, rect.y + 3, 70, 20);
                GUI.Label(rectLabel, "资源标签", GEStyle.HeaderLabel);
                var rectTags = new Rect(rectLabel.x + rectLabel.width - 3, rectLabel.y, rect.width - rectLabel.x - rectLabel.width, 20);
                Item.Tags = EditorGUI.DelayedTextField(rectTags, Item.Tags);
            }

            private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
            {
                EditorGUI.DrawRect(args.rowRect, args.row % 2 == 0 ? ColorAlternatingA : ColorAlternatingB);
                GUI.Box(args.rowRect, string.Empty, GEStyle.INThumbnailShadow);
                if (args.selected) GUI.Box(args.rowRect, string.Empty, GEStyle.SelectionRect);
                if (Item.Folded)
                {
                    GUI.Box(new Rect(args.rowRect.x, args.rowRect.y + 27, args.rowRect.width, args.rowRect.height - 27), string.Empty, GEStyle.InnerShadowBg);
                }

                var rect = new Rect(args.rowRect.x, args.rowRect.y + 5, cellRect.width, 22);
                OnDrawHeader(rect, ref args);
                var rectRightLine = new Rect(cellRect.x + cellRect.width - 11, args.rowRect.y, 1, args.rowRect.height);
                EditorGUI.DrawRect(rectRightLine, ColorLine);
                if (Item.Folded)
                {
                    rect = new Rect(args.rowRect.x, args.rowRect.y + 27, cellRect.width, args.rowRect.height - 27);
                    OnDrawBody1(rect, ref args);
                    rectRightLine = new Rect(args.rowRect.x, rect.y + 23, cellRect.width, 1);
                    EditorGUI.DrawRect(rectRightLine, ColorLine);


                    rect = new Rect(args.rowRect.x, rect.y + 24, cellRect.width, args.rowRect.height - 24);
                    OnDrawBody2(rect, ref args);
                    rectRightLine = new Rect(args.rowRect.x, rect.y + 23, cellRect.width, 1);
                    EditorGUI.DrawRect(rectRightLine, ColorLine);

                    rect = new Rect(args.rowRect.x, rect.y + 24, cellRect.width, args.rowRect.height - 24);
                    OnDrawBody3(rect, ref args);
                    rectRightLine = new Rect(args.rowRect.x, rect.y + 23, cellRect.width, 1);
                    EditorGUI.DrawRect(rectRightLine, ColorLine);

                    rect = new Rect(args.rowRect.x, rect.y + 24, cellRect.width, args.rowRect.height - 24);
                    OnDrawBody4(rect, ref args);
                    rectRightLine = new Rect(args.rowRect.x, rect.y + 23, cellRect.width, 1);
                    EditorGUI.DrawRect(rectRightLine, ColorLine);

                    if (Item.Type != EAssetCollectItemType.MainAssetCollector) return;
                    rect = new Rect(args.rowRect.x, rect.y + 24, cellRect.width, args.rowRect.height - 24);
                    OnDrawBody5(rect, ref args);
                }
            }

            private static string GetInfo(IEnumerable<string> Displays, long source)
            {
                if (source <= -1) return "全选所有条件";
                if (source == 0) return "忽略当前条件";

                var builder = new StringBuilder().Append("当前选中: ").Append(source).Append(" -> ");
                var status = 1L;
                foreach (var display in Displays)
                {
                    if ((source & status) == status) builder.Append(display).Append(";");
                    status *= 2;
                }

                return builder.ToString();
            }
        }
    }
}