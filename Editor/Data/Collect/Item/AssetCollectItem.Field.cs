using UnityEngine.Serialization;

namespace AIO.UEditor
{
    partial class AssetCollectItem
    {
        /// <summary>
        ///    是否启用
        /// </summary>
        public bool Enable = true;

        /// <summary>
        ///     是否折叠 True:折叠 False:展开
        /// </summary>
        public bool Folded;

        /// <summary>
        ///     收集器类型
        /// </summary>
        public EAssetCollectItemType Type;

        /// <summary>
        ///     资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        ///     加载类型
        /// </summary>
        public EAssetLoadType LoadType = EAssetLoadType.Always;

        /// <summary>
        ///     收集器名称
        /// </summary>
        public string FileName;

        /// <summary>
        ///     资源GUID
        /// </summary>
        public string GUID;

        /// <summary>
        ///     收集器路径
        /// </summary>
        public string CollectPath;

        /// <summary>
        ///     自定义数据
        /// </summary>
        public string UserData;

        /// <summary>
        ///     定位规则
        /// </summary>
        [FormerlySerializedAs("Address")]
        public int AddressIndex;

        /// <summary>
        ///     定位格式
        /// </summary>
        public EAssetLocationFormat LocationFormat;

        /// <summary>
        ///     是否有后缀
        /// </summary>
        public bool HasExtension;

        /// <summary>
        ///     打包规则
        /// </summary>
        public int RulePackIndex = 1;

        /// <summary>
        ///     资源包名称
        /// </summary>
        public string PackageName { get; internal set; }

        /// <summary>
        ///     资源组名称
        /// </summary>
        public string GroupName { get; internal set; }

        #region Collect

        /// <summary>
        ///     收集规则(获取收集规则下标)
        /// </summary>
        public int RuleCollectIndex;

        /// <summary>
        ///     开启自定义收集规则
        /// </summary>
        public bool RuleUseCollectCustom;

        /// <summary>
        ///     收集规则 自定义
        /// </summary>
        public string RuleCollect;

        #endregion

        #region Filter

        /// <summary>
        ///     过滤规则(获取收集规则下标)
        /// </summary>
        public int RuleFilterIndex;

        /// <summary>
        ///     开启自定义过滤规则
        /// </summary>
        public bool RuleUseFilterCustom;

        /// <summary>
        ///     过滤规则 自定义
        /// </summary>
        public string RuleFilter;

        #endregion
    }
}