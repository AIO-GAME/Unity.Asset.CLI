#region

using System;
using System.Collections.Generic;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

#endregion

namespace AIO.UEditor
{
    public sealed class TreeViewItemCollect : TreeViewItem, ITVItemDraw
    {
        private static readonly GUIContent GC_FOLDOUT_ON = EditorGUIUtility.IconContent("d_Folder Icon").SetTooltips("折叠");
        private static readonly GUIContent GC_FOLDOUT    = EditorGUIUtility.IconContent("d_FolderOpened Icon").SetTooltips("展开");
        private static readonly GUIContent GC_OPEN       = EditorGUIUtility.IconContent("d_RectTransform Icon").SetTooltips("打开");
        private static readonly GUIContent GC_DEL        = EditorGUIUtility.IconContent("d_ol_minus_act").SetTooltips("删除");

        private static readonly GUIContent GC_LABEL_RuleUseCollectCustom =
            new GUIContent("自定", "自定义收集规则 \n传入文件后缀 \n[冒号(;)/空格( )/逗号(,)]隔开 \n可无需填写点(.)");

        private static readonly GUIContent GC_LABEL_RuleUseFilterCustom =
            new GUIContent("自定", "自定义过滤规则 \n传入文件后缀 \n[冒号(;)/空格( )/逗号(,)]隔开\n可无需填写点(.)");

        public AssetCollectItem Item;

        public TreeViewItemCollect(int id, AssetCollectItem item) : base(id, 3) { Item = item; }

        /// <summary>
        ///     收缩状态变更事件
        /// </summary>
        public Action<bool> OnChangedFold;

        private void OP_Open()
        {
            GUI.FocusControl(null);
            AssetPageEditCollect.OpenCollectItem(Item);
        }

        public void OP_DEL()
        {
            GUI.FocusControl(null);
            if (EditorUtility.DisplayDialog("删除", "确定删除当前收集器?", "确定", "取消"))
                AssetCollectRoot.GetOrCreate().CurrentGroup.Remove(Item);
        }

        private void OnDrawHeader(Rect rect)
        {
            var cell = new Rect(rect.width - 20, rect.y + 5, 20, 20);
            if (GELayout.Button(cell, GC_DEL, GEStyle.IconButton))
            {
                OP_DEL();
                return;
            }

            using (new EditorGUI.DisabledGroupScope(isNull))
            {
                cell.x -= 20;
                if (GELayout.Button(cell, GC_OPEN, GEStyle.IconButton)) OP_Open();

                cell.x     = rect.x + 5;
                cell.width = 24;
                if (GUI.Button(cell, Item.Folded ? GC_FOLDOUT.image : GC_FOLDOUT_ON.image, GEStyle.IconButton))
                {
                    Item.Folded = !Item.Folded;
                    OnChangedFold?.Invoke(Item.Folded);
                }
            }


            cell.y     -= 2;
            cell.x     += cell.width;
            cell.width =  70;
            Item.Type  =  (EAssetCollectItemType)EditorGUI.EnumPopup(cell, Item.Type, GEStyle.PreDropDown);

            cell.x     += cell.width;
            cell.width =  rect.width - cell.x - (Item.Folded ? 0 : 450) - 45;
            Item.Path  =  GELayout.FieldObject(cell, Item.Path, GEStyle.ToolbarDropDownToggle, GEStyle.DDItemStyle);

            if (Item.Folded) return;

            using (new EditorGUI.DisabledGroupScope(isNull))
            {
                cell.x            += cell.width;
                cell.width        =  200;
                Item.AddressIndex =  EditorGUI.IntPopup(cell, Item.AddressIndex, AssetCollectSetting.GT_AddressDisplays, null, GEStyle.PreDropDown);

                cell.x             += cell.width;
                Item.RulePackIndex =  EditorGUI.IntPopup(cell, Item.RulePackIndex, AssetCollectSetting.GT_PackDisplays, null, GEStyle.PreDropDown);

                cell.x        += cell.width;
                cell.width    =  50;
                Item.LoadType =  (EAssetLoadType)EditorGUI.EnumPopup(cell, Item.LoadType, GEStyle.PreDropDown);
            }
        }

        private void OnDrawBody1(Rect rect)
        {
            var cell = new Rect(rect.x + 32, rect.y + 3, 70, 20);
            GUI.Label(cell, "寻址规则", GEStyle.HeaderLabel);

            cell.x            += cell.width - 3;
            cell.width        =  rect.width - cell.x - 394;
            Item.AddressIndex =  EditorGUI.IntPopup(cell, Item.AddressIndex, AssetCollectSetting.GT_AddressDisplays, null, GEStyle.PreDropDown);

            cell.width = 45;
            cell.x     = rect.width - cell.width;
            if (ASConfig.GetOrCreate().HasExtension)
            {
                GUI.enabled       = false;
                Item.HasExtension = EditorGUI.ToggleLeft(cell, "后缀", true);
                GUI.enabled       = true;
            }
            else
            {
                Item.HasExtension = EditorGUI.ToggleLeft(cell, "后缀", Item.HasExtension);
            }

            cell.width    =  75;
            cell.x        -= cell.width;
            Item.LoadType =  (EAssetLoadType)EditorGUI.EnumPopup(cell, Item.LoadType, GEStyle.PreDropDown);

            cell.x -= 75;
            if (ASConfig.GetOrCreate().LoadPathToLower)
            {
                GUI.enabled = false;
                EditorGUI.EnumPopup(cell, EAssetLocationFormat.ToLower, GEStyle.PreDropDown);
                GUI.enabled = true;
            }
            else
            {
                Item.LocationFormat = (EAssetLocationFormat)EditorGUI.EnumPopup(cell, Item.LocationFormat, GEStyle.PreDropDown);
            }

            cell.width         =  200;
            cell.x             -= cell.width;
            Item.RulePackIndex =  EditorGUI.IntPopup(cell, Item.RulePackIndex, AssetCollectSetting.GT_PackDisplays, null, GEStyle.PreDropDown);
        }

        private void OnDrawBody2(Rect rect)
        {
            var cell = new Rect(rect.x + 32, rect.y + 3, 70, 20);
            GUI.Label(cell, "收集规则", GEStyle.HeaderLabel);

            if (Item.RuleUseCollectCustom)
            {
                cell.x           += cell.width - 3;
                cell.width       =  rect.width - cell.x - 45;
                Item.RuleCollect =  EditorGUI.DelayedTextField(cell, Item.RuleCollect);
            }
            else
            {
                cell.x                += cell.width - 3;
                cell.width            =  200;
                Item.RuleCollectIndex =  EditorGUI.MaskField(cell, Item.RuleCollectIndex, AssetCollectSetting.CollectFilterDisplays, GEStyle.PreDropDown);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x - 45;
                EditorGUI.HelpBox(cell, GetInfo(AssetCollectSetting.MapCollect.Displays, Item.RuleCollectIndex), MessageType.None);
            }

            cell.width                = 45;
            cell.x                    = rect.width - 45;
            Item.RuleUseCollectCustom = EditorGUI.ToggleLeft(cell, GC_LABEL_RuleUseCollectCustom, Item.RuleUseCollectCustom);
        }

        private void OnDrawBody3(Rect rect)
        {
            var cell = new Rect(rect.x + 32, rect.y + 3, 70, 20);
            GUI.Label(cell, "过滤规则", GEStyle.HeaderLabel);
            if (Item.RuleUseFilterCustom)
            {
                cell.x          += cell.width - 3;
                cell.width      =  rect.width - cell.x - 45;
                Item.RuleFilter =  EditorGUI.DelayedTextField(cell, Item.RuleFilter);
            }
            else
            {
                cell.x               += cell.width - 3;
                cell.width           =  200;
                Item.RuleFilterIndex =  EditorGUI.MaskField(cell, Item.RuleFilterIndex, AssetCollectSetting.CollectFilterDisplays, GEStyle.PreDropDown);

                cell.x     += cell.width;
                cell.width =  rect.width - cell.x - 45;
                EditorGUI.HelpBox(cell, GetInfo(AssetCollectSetting.CollectFilterDisplays, Item.RuleFilterIndex), MessageType.None);
            }

            cell.width               = 45;
            cell.x                   = rect.width - 45;
            Item.RuleUseFilterCustom = EditorGUI.ToggleLeft(cell, GC_LABEL_RuleUseFilterCustom, Item.RuleUseFilterCustom);
        }

        private void OnDrawBody4(Rect rect)
        {
            var cell = new Rect(rect.x + 32, rect.y + 3, 70, 20);
            GUI.Label(cell, "自定数据", GEStyle.HeaderLabel);
            cell.x        += cell.width - 3;
            cell.width    =  rect.width - cell.x - 3;
            Item.UserData =  EditorGUI.DelayedTextField(cell, Item.UserData);
        }

        private void OnDrawBody5(Rect rect)
        {
            var cell = new Rect(rect.x + 32, rect.y + 3, 70, 20);
            GUI.Label(cell, "资源标签", GEStyle.HeaderLabel);
            cell.x     += cell.width - 3;
            cell.width =  rect.width - cell.x - 3;
            Item.Tags  =  EditorGUI.DelayedTextField(cell, Item.Tags);
        }

        private void OnDrawContent(Rect cellRect, ref RowGUIArgs args)
        {
            var cell = new Rect(args.rowRect);
            cell.height -= 2;
            GUI.Box(cell, string.Empty, GEStyle.INThumbnailShadow);
            if (args.selected) GUI.Box(cell, string.Empty, GEStyle.SelectionRect);
            if (Item.Folded)
            {
                cell.y      += 27;
                cell.height -= 27;
                GUI.Box(cell, string.Empty, GEStyle.InnerShadowBg);
            }

            cell.Set(args.rowRect.x, args.rowRect.y + 2, cellRect.width, 22);
            OnDrawHeader(cell);

            if (!Item.Folded) return;

            cell.y += 23;
            OnDrawBody1(cell);

            cell.y += 23;
            OnDrawBody2(cell);

            cell.y += 23;
            OnDrawBody3(cell);

            cell.y += 23;
            OnDrawBody4(cell);

            if (Item.Type == EAssetCollectItemType.MainAssetCollector)
            {
                cell.y += 24;
                OnDrawBody5(cell);
            }

            var line = new Rect(args.rowRect.x, args.rowRect.y + 48, cellRect.width, 1);
            EditorGUI.DrawRect(line, TreeViewBasics.ColorLine);
            line.y += 23;
            EditorGUI.DrawRect(line, TreeViewBasics.ColorLine);
            line.y += 23;
            EditorGUI.DrawRect(line, TreeViewBasics.ColorLine);
            line.y += 23;
            EditorGUI.DrawRect(line, TreeViewBasics.ColorLine);

            line.Set(args.rowRect.x + 28, args.rowRect.y + 28, 1, args.rowRect.height - 27);
            EditorGUI.DrawRect(line, TreeViewBasics.ColorLine);
            line.x += 70;
            EditorGUI.DrawRect(line, TreeViewBasics.ColorLine);
        }

        private static string GetInfo(IEnumerable<string> Displays, long source)
        {
            if (source <= -1) return "全选所有条件";
            if (source == 0) return "忽略当前条件";

            var builder = new StringBuilder().Append("当前选中: ").Append(source).Append(" -> ");
            var status  = 1L;
            foreach (var display in Displays)
            {
                if ((source & status) == status) builder.Append(display).Append(";");
                status *= 2;
            }

            return builder.ToString();
        }

        #region IGraphDraw

        bool ITVItemDraw.AllowChangeExpandedState              => false;
        bool ITVItemDraw.AllowRename                           => false;
        Rect ITVItemDraw.GetRenameRect(Rect cellRect, int row) => Rect.zero;

        bool ITVItemDraw.MatchSearch(string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return Item.CollectPath.Contains(search) || Item.UserData.Contains(search) || Item.Tags.Contains(search);
        }

        public float GetHeight()
        {
            if (!Item.Path || !Item.Folded) return 27;
            var temp                                                        = 127;
            if (Item.Type == EAssetCollectItemType.MainAssetCollector) temp += 16;
            return temp;
        }

        private bool isNull;

        void ITVItemDraw.OnDraw(Rect cell, int col, ref RowGUIArgs args)
        {
            isNull = !Item.Path;
            EditorGUI.DrawRect(args.rowRect, args.row % 2 == 0 ? TreeViewBasics.ColorAlternatingA : TreeViewBasics.ColorAlternatingB);
            if (Item.Enable)
            {
                OnDrawContent(cell, ref args);
            }
            else
            {
                var oldColor = GUI.color;
                GUI.color = Color.gray;
                OnDrawContent(cell, ref args);
                GUI.color = oldColor;
            }
        }

        #endregion
    }
}