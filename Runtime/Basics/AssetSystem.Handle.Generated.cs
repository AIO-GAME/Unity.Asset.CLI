using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Object = UnityEngine.Object;

namespace AIO
{
    public static class IHandleExtensions
    {
        /// <summary>
        /// 安全转换
        /// </summary>
        /// <returns> 转换后的对象 </returns>
        public static Object SafeCast(this AssetSystem.IHandle handle)
        {
            return handle.Result;
        }

        /// <summary>
        /// 安全转换
        /// </summary>
        /// <returns> 转换后的对象 </returns>
        public static TObject SafeCast<TObject>(this AssetSystem.IHandle<TObject> handle) where TObject : Object
        {
            return handle.Result;
        }
    }
}