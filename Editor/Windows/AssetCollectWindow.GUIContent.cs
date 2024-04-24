using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        ///     界面内容 - 清空
        /// </summary>
        private GUIContent GC_CLEAR;

        /// <summary>
        ///     界面内容 - 复制
        /// </summary>
        private GUIContent GC_COPY;

        /// <summary>
        ///     界面内容 - 删除
        /// </summary>
        private GUIContent GC_DEL;

        /// <summary>
        ///     界面内容 - 下载
        /// </summary>
        private GUIContent GC_DOWNLOAD;

        /// <summary>
        ///     界面内容 - 收缩
        /// </summary>
        private GUIContent GC_FOLDOUT;

        /// <summary>
        ///     界面内容 - 展开
        /// </summary>
        private GUIContent GC_FOLDOUT_ON;

        /// <summary>
        ///     界面内容 - 首包 取消
        /// </summary>
        private GUIContent GC_FP_Cancel;

        /// <summary>
        ///     界面内容 - 首包 确定
        /// </summary>
        private GUIContent GC_FP_OK;

        /// <summary>
        ///     界面内容 - 合并
        /// </summary>
        private GUIContent GC_MERGE;

        /// <summary>
        ///     界面内容 - 云端
        /// </summary>
        private GUIContent GC_NET;

        /// <summary>
        ///     界面内容 - 打开文件夹
        /// </summary>
        private GUIContent GC_OPEN_FOLDER;

        /// <summary>
        ///     界面内容 - 刷新
        /// </summary>
        private GUIContent GC_REFRESH;

        /// <summary>
        ///     界面内容 - 报告
        /// </summary>
        private GUIContent GC_REPORT;

        /// <summary>
        ///     界面内容 - 保存
        /// </summary>
        private GUIContent GC_SAVE;

        /// <summary>
        ///     选择指定配置
        /// </summary>
        private GUIContent GC_Select_ASConfig;

        /// <summary>
        ///     界面内容 - 数据排序
        /// </summary>
        private GUIContent GC_SORT;

        /// <summary>
        ///     界面内容 - 转化
        /// </summary>
        private GUIContent GC_ToConvert;

        partial void GUIContentInit()
        {
            GC_SAVE                          = GEContent.NewBuiltin("d_SaveAs", "保存");
            GC_OPEN_FOLDER                   = GEContent.NewSetting("bangdingliucheng", "打开文件");
            GC_NET                           = GEContent.NewSetting("国际", "云端");
            GC_DOWNLOAD                      = GEContent.NewSetting("下载", "下载");
            GC_FOLDOUT                       = GEContent.NewSetting("quanping-shouqi-xian", "收缩");
            GC_FOLDOUT_ON                    = GEContent.NewSetting("quanping-zhankai-xian", "展开");
            GC_Select_ASConfig               = GEContent.NewSetting("ic_Eyes", "选择资源配置文件");
            GC_REFRESH                       = EditorGUIUtility.IconContent("Refresh").SetTooltips("刷新");
            GC_COPY                          = GEContent.NewSetting("ic_copy", "复制资源路径");
            GC_DEL                           = GEContent.NewSetting("删除", "删除元素");
            GC_CLEAR                         = GEContent.NewSetting("cancel", "清空元素");
            GC_LookMode_Page_MaxLeft         = EditorGUIUtility.IconContent("d_scrollleft").SetTooltips("跳转到第一页");
            GC_LookMode_Page_Left            = EditorGUIUtility.IconContent("ArrowNavigationLeft").SetTooltips("上一页");
            GC_LookMode_Page_MaxRight        = EditorGUIUtility.IconContent("d_scrollright").SetTooltips("跳转到最后一页");
            GC_LookMode_Page_Right           = EditorGUIUtility.IconContent("ArrowNavigationRight").SetTooltips("下一页");
            GC_LookMode_Page_Size            = EditorGUIUtility.IconContent("d_Preset.Context").SetTooltips("设置页面大小");
            GC_FP_OK                         = GEContent.NewSettingCustom("Editor/Icon/Setting/add-to-list", "添加进入首包列表");
            GC_FP_Cancel                     = GEContent.NewSettingCustom("Editor/Icon/Setting/cancel", "从首包列表删除");
            GC_SORT                          = EditorGUIUtility.IconContent("d_DefaultSorting").SetTooltips("数据排序");
            GC_MERGE                         = new GUIContent("合并配置", "合并当前资源包的所有组和收集器");
            GC_ToConvert                     = new GUIContent("转换配置", "转换为第三方配置文件");
            GC_REPORT                        = new GUIContent("报", "资源报告工具");
            GC_LookMode_Detail_Tags          = new GUIContent("资源标签", "资源标签");
            GC_LookMode_Detail_GUID          = new GUIContent("GUID", "资源GUID");
            GC_LookMode_Detail_Type          = new GUIContent("资源类型", "资源类型");
            GC_LookMode_Detail_Path          = new GUIContent("资源路径", "资源文件路径");
            GC_LookMode_Detail_Size          = new GUIContent("资源大小", "文件大小");
            GC_LookMode_Detail_LastWriteTime = new GUIContent("最后修改时间", "最后写入时间");
            GC_LookMode_Detail_IsSubAsset    = new GUIContent("是否被引用", "资源是否构成了其他资源的一部分？");
        }
    }
}