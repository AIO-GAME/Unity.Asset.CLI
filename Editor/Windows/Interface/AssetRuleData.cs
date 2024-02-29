namespace AIO.UEditor
{
    public struct AssetRuleData
    {
        /// <summary>
        /// 收集器 (分隔符标准: /)
        /// </summary>
        public string CollectPath;

        /// <summary>
        /// 资源路径 (不包含扩展名)
        /// </summary>
        public string AssetPath;

        /// <summary>
        /// 扩展名 (小写 .xxx)
        /// </summary>
        public string Extension;

        /// <summary>
        /// 用户数据
        /// </summary>
        public string UserData;

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags;

        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName;

        /// <summary>
        /// 包名称
        /// </summary>
        public string PackageName;
    }
}