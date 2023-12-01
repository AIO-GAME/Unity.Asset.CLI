/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    public enum AssetCollectItemType
    {
        Main,
        Dependence,
        Static,
    }

    public enum AssetLocationFormat
    {
        [InspectorName("无")] None,
        [InspectorName("小写")] ToLower,
        [InspectorName("大写")] ToUpper,
    }

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class AssetCollectItem : IEqualityComparer<AssetCollectItem>
    {
        /// <summary>
        /// 收集器类型
        /// </summary>
        public AssetCollectItemType Type;

        /// <summary>
        /// 资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        /// 收集器路径
        /// </summary>
        public Object Path;

        /// <summary>
        /// 自定义数据
        /// </summary>
        public string UserData;

        /// <summary>
        /// 收集器名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 定位规则
        /// </summary>
        public string Address;

        /// <summary>
        /// 收集规则
        /// </summary>
        public string Collect;

        /// <summary>
        /// 过滤规则
        /// </summary>
        public string Filter;

        /// <summary>
        /// 显示列表
        /// </summary>
        public bool ShowList;

        /// <summary>
        /// 定位格式
        /// </summary>
        public AssetLocationFormat LocationFormat;
        
        public bool Equals(AssetCollectItem x, AssetCollectItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Type == y.Type &&
                   x.Tags == y.Tags &&
                   Equals(x.Path, y.Path) &&
                   x.UserData == y.UserData &&
                   x.Name == y.Name &&
                   x.Address == y.Address &&
                   x.Collect == y.Collect &&
                   x.Filter == y.Filter &&
                   x.ShowList == y.ShowList;
        }

        public int GetHashCode(AssetCollectItem obj)
        {
            unchecked
            {
                var hashCode = (int)obj.Type;
                hashCode = (hashCode * 397) ^ (obj.Tags != null ? obj.Tags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Path != null ? obj.Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.UserData != null ? obj.UserData.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Address != null ? obj.Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Collect != null ? obj.Collect.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Filter != null ? obj.Filter.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.ShowList.GetHashCode();
                return hashCode;
            }
        }
    }
}