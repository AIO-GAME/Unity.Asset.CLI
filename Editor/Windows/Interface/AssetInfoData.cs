/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO.UEditor
{
    public struct AssetInfoData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 收集器 (分隔符标准: /)
        /// </summary>
        public string CollectPath;

        /// <summary>
        /// 资源路径 (不包含扩展名)
        /// </summary>
        public string AssetPath;
        
        /// <summary>
        /// 自定义匹配地址 (分隔符标准: /)
        /// </summary>
        public string RuleCustomAddress;
        
        /// <summary>
        /// 自定义过滤器 (分隔符标准: /)
        /// </summary>
        public string[] RuleCustomFilter;
        
        /// <summary>
        /// 自定义收集器 (分隔符标准: /)
        /// </summary>
        public string[] RuleCustomCollect;

        /// <summary>
        /// 自定义匹配地址 (分隔符标准: /)
        /// </summary>
        public string RuleAddress;
        
        /// <summary>
        /// 自定义过滤器 (分隔符标准: /)
        /// </summary>
        public string RuleFilter;

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