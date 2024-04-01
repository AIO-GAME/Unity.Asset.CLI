using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class AssetCollectWindow
    {
        /// <summary>
        ///     当前导航栏高度
        /// </summary>
        private const float DrawHeaderHeight = 25;

        /// <summary>
        ///     资源收集根节点
        /// </summary>
        public AssetCollectRoot Data;

        /// <summary>
        ///     资源系统配置
        /// </summary>
        public ASConfig Config;

        /// <summary>
        ///     资源系统打包配置
        /// </summary>
        public ASBuildConfig BuildConfig;

        /// <summary>
        ///     当前选择的收集器
        /// </summary>
        private int CurrentCurrentCollectorsIndex;

        /// <summary>
        ///     当前选择的资源
        /// </summary>
        private int CurrentSelectAssetIndex;

        /// <summary>
        ///     宽度偏移量
        /// </summary>
        private Rect DoConfigDrawRect = Rect.zero;

        /// <summary>
        ///     宽度偏移量
        /// </summary>
        private Rect DoEditorDrawRect = Rect.zero;

        /// <summary>
        ///     界面 - 收集器选择下标
        /// </summary>
        private int LookModeCollectorsPageIndex;

        private Vector2 OnDrawConfigFTPScroll = Vector2.zero;

        private Vector2 OnDrawConfigGCScroll  = Vector2.zero;
        private Vector2 OnDrawConfigScroll    = Vector2.zero;
        private Vector2 OnDrawGroupListScroll = Vector2.zero;
        private Vector2 OnDrawGroupScroll     = Vector2.zero;
        private Vector2 OnDrawItemListScroll  = Vector2.zero;

        /// <summary>
        ///     界面 - 查询模式 显示ICON
        /// </summary>
        private GenericMenu onDrawLookDataItemMenu;

        private Vector2 OnDrawLookDataScroll = Vector2.zero;
        private Vector2 OnDrawPackageScroll  = Vector2.zero;
        private Vector2 OnDrawSettingScroll  = Vector2.zero;

        /// <summary>
        ///     界面 - 收集器列表
        /// </summary>
        private ViewRect ViewCollectorsList;

        /// <summary>
        ///     界面 - 配置界面
        /// </summary>
        private ViewRect ViewConfig;

        /// <summary>
        ///     界面 - 详情界面
        /// </summary>
        private ViewRect ViewDetailList;

        /// <summary>
        ///     界面 - 详情界面
        /// </summary>
        private ViewRect ViewDetails;

        /// <summary>
        ///     界面 - 组列表
        /// </summary>
        private ViewRect ViewGroupList;

        /// <summary>
        ///     界面 - 包列表
        /// </summary>
        private ViewRect ViewPackageList;

        /// <summary>
        ///     界面 - 配置界面
        /// </summary>
        private ViewRect ViewSetting;

        private void GPInit()
        {
            GP_MAX_Width_100 = GUILayout.MaxWidth(100);
            GP_MIN_Width_50  = GUILayout.MinWidth(50);
            GP_Width_EXPAND  = GUILayout.ExpandWidth(true);
            GP_Width_120     = GUILayout.Width(120);
            GP_Width_150     = GUILayout.Width(150);
            GP_Width_160     = GUILayout.Width(160);
            GP_Width_100     = GUILayout.Width(100);
            GP_Width_75      = GUILayout.Width(75);
            GP_Width_80      = GUILayout.Width(80);
            GP_Width_50      = GUILayout.Width(50);
            GP_Width_40      = GUILayout.Width(40);
            GP_Width_30      = GUILayout.Width(30);
            GP_Width_25      = GUILayout.Width(25);
            GP_Width_20      = GUILayout.Width(20);
            GP_Height_30     = GUILayout.Height(30);
            GP_Height_25     = GUILayout.Height(25);
            GP_Height_20     = GUILayout.Height(20);
        }

        partial void GUIContentInit();

        partial void GCInit()
        {
            ViewRectUpdate();
            GPInit();
            GUIContentInit();

            if (CurrentPageValues is null) CurrentPageValues = new PageList<AssetDataInfo> { PageSize = 30 };
            if (LookDataPageSizeMenu is null) UpdatePageSizeMenu();
        }

        private void ViewRectUpdate()
        {
            DoEditorDrawRect = new Rect(5, DrawHeaderHeight, 0, CurrentHeight - DrawHeaderHeight);
            DoConfigDrawRect = new Rect(5, DrawHeaderHeight, 0, CurrentHeight - DrawHeaderHeight);

            ViewSetting = new ViewRect(250, DoEditorDrawRect.height)
            {
                IsShow              = true,
                IsAllowHorizontal   = false,
                DragHorizontalWidth = 5,
                width               = 250
            };
            ViewConfig = new ViewRect(550, DoEditorDrawRect.height)
            {
                IsShow              = true,
                IsAllowHorizontal   = true,
                DragHorizontalWidth = 5,
                width               = CurrentWidth - ViewSetting.width
            };
            ViewPackageList = new ViewRect(100, DoEditorDrawRect.height)
            {
                IsShow              = true,
                IsAllowHorizontal   = true,
                DragHorizontalWidth = 5,
                width               = 150
            };
            ViewGroupList = new ViewRect(100, DoEditorDrawRect.height)
            {
                IsShow              = true,
                IsAllowHorizontal   = true,
                DragHorizontalWidth = 5,
                width               = 150
            };
            ViewCollectorsList = new ViewRect(100, DoEditorDrawRect.height)
            {
                IsShow            = true,
                IsAllowHorizontal = false,
                width             = 400
            };

            ViewDetailList = new ViewRect(300, DoEditorDrawRect.height)
            {
                IsShow              = true,
                IsAllowHorizontal   = false,
                DragHorizontalWidth = 10,
                width               = 400,
                x                   = 0,
                y                   = DrawHeaderHeight
            };

            ViewDetails = new ViewRect(300, DoEditorDrawRect.height)
            {
                IsShow            = false,
                IsAllowHorizontal = false,
                width             = 400,
                y                 = ViewDetailList.y + 3
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

        #region Build

        /// <summary>
        ///     当前打包资源标签列表
        /// </summary>
        private string[] Tags;

        /// <summary>
        ///     当前打包资源标签下标
        /// </summary>
        private int CurrentTagIndex;

        /// <summary>
        ///     折叠栏 - 打包设置
        /// </summary>
        private bool FoldoutBuildSetting = true;

        /// <summary>
        ///     折叠栏 - FTP上传
        /// </summary>
        private bool FoldoutUploadFTP = true;

        /// <summary>
        ///     折叠栏 - Google Cloud 上传
        /// </summary>
        private bool FoldoutUploadGCloud = true;

        #endregion

        #region Draw Group List

        /// <summary>
        ///     折叠栏 - 资源包信息
        /// </summary>
        private bool FoldoutPackageInfo = true;

        /// <summary>
        ///     折叠栏 - 收集器列表
        /// </summary>
        private bool FoldoutCollectors = true;

        #endregion

        #region Header

        /// <summary>
        ///     界面模式
        /// </summary>
        public enum Mode
        {
            [InspectorName(" 编辑模式 [Ctrl + Alpha1\\Number1]")]
            Editor = 0,

            [InspectorName(" 配置管理 [Ctrl + Alpha2\\Number2]")]
            Config = 1,

            [InspectorName(" 查询模式 [Ctrl + Alpha3\\Number3]")]
            Look = 2,

            [InspectorName(" 查询标签 [Ctrl + Alpha4\\Number4]")]
            LookTags = 3,

            [InspectorName(" 查询首包 [Ctrl + Alpha5\\Number5]")]
            LookFirstPackage = 4,

            [InspectorName(" 打包工具 [Ctrl + Alpha6\\Number6]")]
            Build = 5
        }

        /// <summary>
        ///     当前界面模式
        /// </summary>
        public static Mode WindowMode = Mode.Editor;

        /// <summary>
        ///     Header中间显示信息
        /// </summary>
        private readonly StringBuilder TempBuilder = new StringBuilder();

        /// <summary>
        ///     磁盘信息
        /// </summary>
        private DriveInfo Disk;

        #endregion

        #region LookMode

        /// <summary>
        ///     界面内容 - 第一页
        /// </summary>
        private GUIContent GC_LookMode_Page_MaxLeft;

        /// <summary>
        ///     界面内容 - 左边一页
        /// </summary>
        private GUIContent GC_LookMode_Page_Left;

        /// <summary>
        ///     界面内容 - 最后一页
        /// </summary>
        private GUIContent GC_LookMode_Page_MaxRight;

        /// <summary>
        ///     界面内容 - 右边一页
        /// </summary>
        private GUIContent GC_LookMode_Page_Right;

        /// <summary>
        ///     界面内容 - 页面大小
        /// </summary>
        private GUIContent GC_LookMode_Page_Size;

        /// <summary>
        ///     界面内容 - 实例物体选择打开
        /// </summary>
        private GUIContent GC_LookMode_Object_Select;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_Size;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_Asset;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_GUID;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_Type;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_Path;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_LastWriteTime;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_IsSubAsset;

        /// <summary>
        ///     界面内容 -
        /// </summary>
        private GUIContent GC_LookMode_Detail_Tags;

        /// <summary>
        ///     界面内容 - 排序方式
        /// </summary>
        private GUIContent GC_LookMode_Data_Sort;

        /// <summary>
        ///     排序方式
        /// </summary>
        public enum ESort
        {
            [InspectorName("大小")]     FileSize,
            [InspectorName("最后修改时间")] LastWrite,
            [InspectorName("名称")]     AssetName,
            [InspectorName("资源类型")]   ObjectType
        }

        /// <summary>
        ///     查看模式 资源排序
        /// </summary>
        private ESort LookModeSort = ESort.LastWrite;

        /// <summary>
        ///     查询模式 数据
        /// </summary>
        private Dictionary<(int, int), List<AssetDataInfo>> LookModeData;

        /// <summary>
        ///     查询模式 当前选择包索引
        /// </summary>
        private PageList<AssetDataInfo> CurrentPageValues;

        /// <summary>
        ///     包列表
        /// </summary>
        private string[] LookModeDisplayPackages;

        /// <summary>
        ///     组列表
        /// </summary>
        private Dictionary<string, string[]> LookModeDisplayGroups;

        /// <summary>
        ///     收集器列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayCollectors;

        /// <summary>
        ///     收集器类型列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayTypes;

        /// <summary>
        ///     收集器标签列表
        /// </summary>
        private Dictionary<(int, int), string[]> LookModeDisplayTags;

        /// <summary>
        ///     搜索文本
        /// </summary>
        private string SearchText = string.Empty;

        /// <summary>
        ///     当前选择包类型索引
        /// </summary>
        private int LookModeDisplayTypeIndex;

        /// <summary>
        ///     当前选择收集器索引
        /// </summary>
        private int LookModeDisplayCollectorsIndex = -1;

        /// <summary>
        ///     当前标签列表索引
        /// </summary>
        private int LookModeDisplayTagsIndex;

        /// <summary>
        ///     收集器全部资源大小
        /// </summary>
        private long LookModeCollectorsALLSize;

        /// <summary>
        ///     收集器页签资源大小
        /// </summary>
        private long LookModeCollectorsPageSize;

        /// <summary>
        ///     是否显示资源详情
        /// </summary>
        private bool LookModeShowAssetDetail => !string.IsNullOrEmpty(LookCurrentSelectAssetDataInfo.GUID) &&
                                                LookCurrentSelectAsset != null;

        /// <summary>
        ///     资源展示模式 当前页数量选项
        /// </summary>
        private GenericMenu LookDataPageSizeMenu;

        /// <summary>
        ///     用户当前选择的资源实体
        /// </summary>
        private Object LookCurrentSelectAsset;

        /// <summary>
        ///     选择的资源实体配置
        /// </summary>
        private AssetDataInfo LookCurrentSelectAssetDataInfo;

        /// <summary>
        ///     依赖资源
        /// </summary>
        private readonly Dictionary<string, DependenciesInfo> Dependencies = new Dictionary<string, DependenciesInfo>();

        /// <summary>
        ///     依赖资源大小
        /// </summary>
        private long DependenciesSize;

        /// <summary>
        ///     依赖资源搜索文本
        /// </summary>
        private string DependenciesSearchText = string.Empty;

        private class DependenciesInfo
        {
            private string _Type;

            public string AssetPath;

            public string Name;
            public Object Object;

            public long Size;

            public string Type
            {
                get
                {
                    if (string.IsNullOrEmpty(_Type)) return _Type;
                    _Type = Object is null ? "Unknown" : Object.GetType().FullName;
                    return _Type;
                }
            }
        }

        private Rect    LookModeShowAssetListView;
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
        ///     收集器标签列表
        /// </summary>
        private string[] TagsModeDisplayTags;
        private string[] TagsModeDisplayCollectors;
        private string[] TagsModeDisplayTypes;

        /// <summary>
        ///     标签模式 当前所有资源
        /// </summary>
        private readonly List<AssetDataInfo> CurrentTagValues = new List<AssetDataInfo>();

        #endregion

        #region Common

        private readonly Hashtable TempTable = new Hashtable();

        private GUILayoutOption GP_MAX_Width_100;
        private GUILayoutOption GP_MIN_Width_50;
        private GUILayoutOption GP_Width_100;
        private GUILayoutOption GP_Width_120;
        private GUILayoutOption GP_Width_150;
        private GUILayoutOption GP_Width_160;
        private GUILayoutOption GP_Width_80;
        private GUILayoutOption GP_Width_75;
        private GUILayoutOption GP_Width_50;
        private GUILayoutOption GP_Width_30;
        private GUILayoutOption GP_Width_40;
        private GUILayoutOption GP_Width_25;
        private GUILayoutOption GP_Width_20;
        private GUILayoutOption GP_Width_EXPAND;
        private GUILayoutOption GP_Height_20;
        private GUILayoutOption GP_Height_25;
        private GUILayoutOption GP_Height_30;

        #endregion
    }
}