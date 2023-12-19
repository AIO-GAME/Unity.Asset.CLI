/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-11
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections;
using System.Collections.Generic;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        /// 资源收集根节点
        /// </summary>
        public AssetCollectRoot Data;

        /// <summary>
        /// 资源系统配置
        /// </summary>
        public ASConfig Config;

        /// <summary>
        /// 资源系统打包配置
        /// </summary>
        public ASBuildConfig BuildConfig;

        /// <summary>
        /// 宽度偏移量
        /// </summary>
        private Rect DoEditorDrawRect = Rect.zero;

        /// <summary>
        /// 宽度偏移量
        /// </summary>
        private Rect DoConfigDrawRect = Rect.zero;

        /// <summary>
        /// 当前选择包下标
        /// </summary>
        private int CurrentPackageIndex;

        /// <summary>
        /// 当前选择组下标
        /// </summary>
        private int CurrentGroupIndex;

        /// <summary>
        /// 当前导航栏高度
        /// </summary>
        private const float DrawHeaderHeight = 25;

        private Vector2 OnDrawConfigScroll = Vector2.zero;
        private Vector2 OnDrawSettingScroll = Vector2.zero;
        private Vector2 OnDrawPackageScroll = Vector2.zero;
        private Vector2 OnDrawGroupScroll = Vector2.zero;
        private Vector2 OnDrawGroupListScroll = Vector2.zero;
        private Vector2 OnDrawListScroll = Vector2.zero;
        private Vector2 OnDrawLookDataScroll = Vector2.zero;

        #region Build

        /// <summary>
        /// 当前打包资源标签列表
        /// </summary>
        private string[] Tags;

        /// <summary>
        /// 当前打包资源标签下标
        /// </summary>
        private int CurrentTagIndex;

        /// <summary>
        /// 折叠栏 - 打包设置
        /// </summary>
        private bool FoldoutBuildSetting = true;

        /// <summary>
        /// 折叠栏 - FTP上传
        /// </summary>
        private bool FoldoutUploadFTP = true;

        /// <summary>
        /// 折叠栏 - 钉钉通知
        /// </summary>
        private bool FoldoutNoticeDingDing = true;

        #endregion

        #region Draw Group List

        /// <summary>
        /// 折叠栏 - 资源包信息
        /// </summary>
        private bool FoldoutPackageInfo = true;

        /// <summary>
        /// 折叠栏 - 收集器列表
        /// </summary>
        private bool FoldoutCollectors = true;

        #endregion

        #region Header

        /// <summary>
        /// 界面模式
        /// </summary>
        public enum Mode
        {
            [InspectorName("编辑模式")] Editor,
            [InspectorName("配置模式")] Config,
            [InspectorName("查询模式")] Look,
            [InspectorName("标签模式[等待优化]")] Tags,
            [InspectorName("打包模式")] Build,
            [InspectorName("首包模式")] FirstPackage,
        }

        /// <summary>
        /// 当前界面模式
        /// </summary>
        private Mode WindowMode = Mode.Editor;

        /// <summary>
        /// Header中间显示信息
        /// </summary>
        private readonly StringBuilder TempBuilder = new StringBuilder();

        #endregion

        #region LookMode

        /// <summary>
        /// 界面内容 - 第一页
        /// </summary>
        private GUIContent GC_LookMode_Page_MaxLeft;

        /// <summary>
        /// 界面内容 - 左边一页
        /// </summary>
        private GUIContent GC_LookMode_Page_Left;

        /// <summary>
        /// 界面内容 - 最后一页
        /// </summary>
        private GUIContent GC_LookMode_Page_MaxRight;

        /// <summary>
        /// 界面内容 - 右边一页
        /// </summary>
        private GUIContent GC_LookMode_Page_Right;

        /// <summary>
        /// 界面内容 - 页面大小
        /// </summary>
        private GUIContent GC_LookMode_Page_Size;

        /// <summary>
        /// 界面内容 - 实例物体选择打开
        /// </summary>
        private GUIContent GC_LookMode_Object_Select;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_Size;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_Asset;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_GUID;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_Type;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_Path;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_LastWriteTime;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_IsSubAsset;

        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_LookMode_Detail_Tags;

        /// <summary>
        /// 界面内容 - 排序方式
        /// </summary>
        private GUIContent GC_LookMode_Data_Sort;

        /// <summary>
        /// 排序方式
        /// </summary>
        public enum ESort
        {
            [InspectorName("大小")] FileSize,
            [InspectorName("最后修改时间")] LastWrite,
            [InspectorName("名称")] AssetName,
            [InspectorName("资源类型")] ObjectType,
        }

        /// <summary>
        /// 查看模式 资源排序
        /// </summary>
        private ESort LookModeSort = ESort.LastWrite;

        /// <summary>
        /// 查询模式 数据
        /// </summary>
        private Dictionary<(int, int), List<AssetDataInfo>> LookModeData;

        /// <summary>
        /// 查询模式 当前选择包索引
        /// </summary>
        private PageList<AssetDataInfo> CurrentPageValues;

        /// <summary>
        /// 包列表
        /// </summary>
        private string[] LookModeDisplayPackages;

        /// <summary>
        /// 组列表
        /// </summary>
        private Dictionary<string, string[]> LookModeDisplayGroups;

        /// <summary>
        /// 收集器列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayCollectors;

        /// <summary>
        /// 收集器类型列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayTypes;

        /// <summary>
        /// 收集器标签列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayTags;

        /// <summary>
        /// 搜索文本
        /// </summary>
        private string SearchText = string.Empty;

        /// <summary>
        /// 当前选择包类型索引
        /// </summary>
        private int LookModeDisplayTypeIndex = -1;

        /// <summary>
        /// 当前选择收集器索引
        /// </summary>
        private int LookModeDisplayCollectorsIndex = -1;

        /// <summary>
        /// 当前标签列表索引
        /// </summary>
        private int LookModeDisplayTagsIndex = -1;

        /// <summary>
        /// 收集器全部资源大小
        /// </summary>
        private long LookModeCollectorsALLSize;

        /// <summary>
        /// 收集器页签资源大小
        /// </summary>
        private long LookModeCollectorsPageSize;

        /// <summary>
        /// 是否显示资源详情
        /// </summary>
        private bool LookModeShowAssetDetail => !string.IsNullOrEmpty(LookModeCurrentSelectAssetDataInfo.GUID) &&
                                                LookModeCurrentSelectAsset != null;

        /// <summary>
        /// 资源展示模式 当前页数量选项
        /// </summary>
        private GenericMenu LookDataPageSizeMenu;

        /// <summary>
        /// 用户当前选择的资源实体
        /// </summary>
        private Object LookModeCurrentSelectAsset;

        /// <summary>
        /// 选择的资源实体配置
        /// </summary>
        private AssetDataInfo LookModeCurrentSelectAssetDataInfo;

        /// <summary>
        /// 依赖资源
        /// </summary>
        private Dictionary<string, DependenciesInfo> Dependencies = new Dictionary<string, DependenciesInfo>();

        /// <summary>
        /// 依赖资源大小
        /// </summary>
        private long DependenciesSize;

        /// <summary>
        /// 依赖资源搜索文本
        /// </summary>
        private string DependenciesSearchText = string.Empty;

        private class DependenciesInfo
        {
            public Object Object;

            public string AssetPath;

            public string Name;

            public string Type;

            public long Size;
        }

        private Rect LookModeShowAssetListView;
        private Vector2 LookModeShowAssetDetailScroll;

        private static readonly Color ROW_SHADING_COLOR = new Color(0f, 0f, 0f, 0.2f);

        private bool LookModeSortEnableAssetName;
        private bool LookModeSortEnableAssetNameToMin;

        private bool LookModeSortEnableLastWrite;
        private bool LookModeSortEnableLastWriteToMin;

        private bool LookModeSortEnableSize;
        private bool LookModeSortEnableSizeToMin;

        #endregion

        #region Tags Mode

        /// <summary>
        /// 收集器标签列表
        /// </summary>
        private string[] TagsModeDisplayTags;

        private string[] TagsModeDisplayCollectors;

        private string[] TagsModeDisplayTypes;

        /// <summary>
        /// 标签模式 当前所有资源
        /// </summary>
        private List<AssetDataInfo> CurrentTagValues = new List<AssetDataInfo>();

        #endregion

        #region Common

        private Hashtable TempTable = new Hashtable();

        private GUILayoutOption GP_Width_100;
        private GUILayoutOption GP_Width_150;
        private GUILayoutOption GP_Width_75;
        private GUILayoutOption GP_Width_50;
        private GUILayoutOption GP_Width_30;
        private GUILayoutOption GP_Width_40;
        private GUILayoutOption GP_Width_25;
        private GUILayoutOption GP_Width_20;

        private GUILayoutOption GP_Height_20;

        private GUILayoutOption GP_Height_25;

        private GUILayoutOption GP_Height_30;

        /// <summary>
        /// 界面内容 - 添加
        /// </summary>
        private GUIContent GC_ADD;

        /// <summary>
        /// 界面内容 - 收缩
        /// </summary>
        private GUIContent GC_FOLDOUT;

        /// <summary>
        /// 界面内容 - 展开
        /// </summary>
        private GUIContent GC_FOLDOUT_ON;

        /// <summary>
        /// 界面内容 - 删除
        /// </summary>
        private GUIContent GC_DEL;

        /// <summary>
        /// 界面内容 - 清空
        /// </summary>
        private GUIContent GC_CLEAR;

        /// <summary>
        /// 界面内容 - 上传
        /// </summary>
        private GUIContent GC_UPLOAD;

        /// <summary>
        /// 界面内容 - 下载
        /// </summary>
        private GUIContent GC_DOWNLOAD;

        /// <summary>
        /// 界面内容 - 云端
        /// </summary>
        private GUIContent GC_NET;

        /// <summary>
        /// 界面内容 - 打开
        /// </summary>
        private GUIContent GC_OPEN;

        /// <summary>
        /// 界面内容 - 打开文件夹
        /// </summary>
        private GUIContent GC_OPEN_FOLDER;

        /// <summary>
        /// 界面内容 - 刷新
        /// </summary>
        private GUIContent GC_REFRESH;

        /// <summary>
        /// 界面内容 - 同步
        /// </summary>
        private GUIContent GC_SyncData;

        /// <summary>
        /// 界面内容 - 复制
        /// </summary>
        private GUIContent GC_COPY;

        /// <summary>
        /// 界面内容 - 保存
        /// </summary>
        private GUIContent GC_SAVE;

#if SUPPORT_YOOASSET
        /// <summary>
        /// 界面内容 - 
        /// </summary>
        private GUIContent GC_ToConvert_YooAsset;
#endif

        private GUIContent GC_Select_ASConfig;

        #endregion

        /// <summary>
        /// 界面 - 配置界面
        /// </summary>
        private ViewRect ViewConfig;

        /// <summary>
        /// 界面 - 配置界面
        /// </summary>
        private ViewRect ViewSetting;

        /// <summary>
        /// 界面 - 详情界面
        /// </summary>
        private ViewRect ViewDetails;

        /// <summary>
        /// 界面 - 详情界面
        /// </summary>
        private ViewRect ViewDetailList;

        /// <summary>
        /// 界面 - 包列表
        /// </summary>
        private ViewRect ViewPackageList;

        /// <summary>
        /// 界面 - 组列表
        /// </summary>
        private ViewRect ViewGroupList;

        /// <summary>
        /// 界面 - 收集器列表
        /// </summary>
        private ViewRect ViewCollectorsList;

        private AssetSystem.SequenceRecordQueue SequenceRecords;

        private void GPInit()
        {
            GP_Width_150 = GUILayout.Width(150);
            GP_Width_100 = GUILayout.Width(100);
            GP_Width_75 = GUILayout.Width(75);
            GP_Width_50 = GUILayout.Width(50);
            GP_Width_40 = GUILayout.Width(40);
            GP_Width_30 = GUILayout.Width(30);
            GP_Width_25 = GUILayout.Width(25);
            GP_Width_20 = GUILayout.Width(20);
            GP_Height_30 = GUILayout.Height(30);
            GP_Height_25 = GUILayout.Height(25);
            GP_Height_20 = GUILayout.Height(20);
        }

        partial void GCInit()
        {
            ViewRectUpdate();
            GPInit();

#if SUPPORT_YOOASSET
            GC_ToConvert_YooAsset = GEContent.NewApp("Yooasset", "转换为Yooasset资源");
#endif

            GC_SAVE = GEContent.NewBuiltin("d_SaveAs", "保存");
            GC_LookMode_Object_Select = GEContent.NewBuiltin("d_scenepicking_pickable_hover", "选择指向指定资源");

            GC_OPEN_FOLDER = GEContent.NewSetting("bangdingliucheng", "打开文件");
            GC_NET = GEContent.NewSetting("国际", "云端");
            GC_UPLOAD = GEContent.NewSetting("上传", "上传");
            GC_DOWNLOAD = GEContent.NewSetting("下载", "下载");
            GC_FOLDOUT = GEContent.NewSetting("quanping-shouqi-xian", "收缩");
            GC_FOLDOUT_ON = GEContent.NewSetting("quanping-zhankai-xian", "展开");
            GC_Select_ASConfig = GEContent.NewSetting("ic_Eyes", "选择资源配置文件");
            GC_REFRESH = GEContent.NewSetting("重置", "刷新数据");
            GC_COPY = GEContent.NewSetting("ic_copy", "复制资源路径");
            GC_ADD = GEContent.NewSetting("新增", "添加元素");
            GC_DEL = GEContent.NewSetting("删除", "删除元素");
            GC_CLEAR = GEContent.NewSetting("cancel", "清空元素");
            GC_OPEN = GEContent.NewSetting("操作", "打开");
            GC_SyncData = GEContent.NewSetting("下载", "下载");
            GC_LookMode_Data_Sort = GEContent.NewSetting("ic_sort", "排序方式");
            GC_LookMode_Page_MaxLeft = GEContent.NewSetting("Arrows_Arrow_Big_Left", "跳转到第一页");
            GC_LookMode_Page_Left = GEContent.NewSetting("Arrows_Arrow_Left_Bar", "上一页");
            GC_LookMode_Page_MaxRight = GEContent.NewSetting("Arrows_Arrow_Right_Bar", "跳转到最后一页");
            GC_LookMode_Page_Right = GEContent.NewSetting("Arrows_Arrow_Big_Right", "下一页");
            GC_LookMode_Page_Size = GEContent.NewSetting("操作", "设置页面大小");

            GC_LookMode_Detail_Tags = new GUIContent("Tags", "资源标签");
            GC_LookMode_Detail_GUID = new GUIContent("GUID", "资源GUID");
            GC_LookMode_Detail_Asset = new GUIContent("Asset", "资源实例");
            GC_LookMode_Detail_Type = new GUIContent("Type", "资源类型");
            GC_LookMode_Detail_Path = new GUIContent("Path", "资源文件路径");
            GC_LookMode_Detail_Size = new GUIContent("Size", "文件大小");
            GC_LookMode_Detail_LastWriteTime = new GUIContent("Last Write Time", "最后写入时间");
            GC_LookMode_Detail_IsSubAsset = new GUIContent("IsSubAsset", "资源是否构成了其他资源的一部分？");

            if (CurrentPageValues is null) CurrentPageValues = new PageList<AssetDataInfo> { PageSize = 25 };
            if (LookDataPageSizeMenu is null) UpdatePageSizeMenu();
        }

        private void ViewRectUpdate()
        {
            DoEditorDrawRect = new Rect(5, DrawHeaderHeight, 0, CurrentHeight - DrawHeaderHeight);
            DoConfigDrawRect = new Rect(5, DrawHeaderHeight, 0, CurrentHeight - DrawHeaderHeight);

            ViewSetting = new ViewRect(150, DoEditorDrawRect.height)
            {
                IsShow = true,
                IsAllowHorizontal = false,
                DragHorizontalWidth = 5,
                width = 150
            };
            ViewConfig = new ViewRect(550, DoEditorDrawRect.height)
            {
                IsShow = true,
                IsAllowHorizontal = true,
                DragHorizontalWidth = 5,
                width = CurrentWidth - ViewSetting.width
            };
            ViewPackageList = new ViewRect(100, DoEditorDrawRect.height)
            {
                IsShow = true,
                IsAllowHorizontal = true,
                DragHorizontalWidth = 5,
                width = 150
            };
            ViewGroupList = new ViewRect(100, DoEditorDrawRect.height)
            {
                IsShow = true,
                IsAllowHorizontal = true,
                DragHorizontalWidth = 5,
                width = 150
            };
            ViewCollectorsList = new ViewRect(100, DoEditorDrawRect.height)
            {
                IsShow = true,
                IsAllowHorizontal = false,
                width = 400
            };

            ViewDetailList = new ViewRect(300, DoEditorDrawRect.height)
            {
                IsShow = true,
                IsAllowHorizontal = false,
                DragHorizontalWidth = 10,
                width = 400,
                x = 0,
                y = DrawHeaderHeight,
            };

            ViewDetails = new ViewRect(300, DoEditorDrawRect.height)
            {
                IsShow = false,
                IsAllowHorizontal = false,
                width = 400,
                y = ViewDetailList.y + 3,
            };
        }

        private void UpdatePageSizeMenu()
        {
            LookDataPageSizeMenu = new GenericMenu();
            LookDataPageSizeMenu.AddItem(new GUIContent("25"), CurrentPageValues.PageSize == 25,
                () =>
                {
                    CurrentPageValues.PageSize = 25;
                    UpdatePageSizeMenu();
                });
            LookDataPageSizeMenu.AddItem(new GUIContent("30"), CurrentPageValues.PageSize == 30,
                () =>
                {
                    CurrentPageValues.PageSize = 30;
                    UpdatePageSizeMenu();
                });
            LookDataPageSizeMenu.AddItem(new GUIContent("40"), CurrentPageValues.PageSize == 40,
                () =>
                {
                    CurrentPageValues.PageSize = 40;
                    UpdatePageSizeMenu();
                });
            LookDataPageSizeMenu.AddItem(new GUIContent("50"), CurrentPageValues.PageSize == 50,
                () =>
                {
                    CurrentPageValues.PageSize = 50;
                    UpdatePageSizeMenu();
                });
        }
    }
}