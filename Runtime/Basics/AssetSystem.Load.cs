/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AIO
{
    partial class AssetSystem
    {
        #region 子资源加载

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadSubAssets<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            cb?.Invoke(
                await Proxy.LoadSubAssetsTask<TObject>(SettingToLocalPath(location)));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadSubAssets(string location, Action<Object[]> cb)
        {
            cb?.Invoke(await Proxy.LoadSubAssetsTask<Object>(Parameter.LoadPathToLower
                ? location.ToLower()
                : location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="type">子对象类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadSubAssets(string location, Type type, Action<Object[]> cb)
        {
            cb?.Invoke(await Proxy.LoadSubAssetsTask(SettingToLocalPath(location), type));
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadSubAssetsCO<TObject>(string location, Action<TObject[]> cb) where TObject : Object
        {
            return Proxy.LoadSubAssetsCO(SettingToLocalPath(location), cb);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadSubAssetsCO(string location, Type type, Action<Object[]> cb)
        {
            return Proxy.LoadSubAssetsCO(SettingToLocalPath(location), type, cb);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static TObject[] LoadSubAssets<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadSubAssetsSync<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object[] LoadSubAssets(string location, Type type)
        {
            return Proxy.LoadSubAssetsSync(SettingToLocalPath(location), type);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object[] LoadSubAssets(string location)
        {
            return Proxy.LoadSubAssetsSync(SettingToLocalPath(location), typeof(Object));
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<TObject[]> LoadSubAssetsTask<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadSubAssetsTask<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Object[]> LoadSubAssetsTask(string location, Type type)
        {
            return Proxy.LoadSubAssetsTask(SettingToLocalPath(location), type);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Object[]> LoadSubAssetsTask(string location)
        {
            return Proxy.LoadSubAssetsTask(SettingToLocalPath(location), typeof(Object));
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset<TObject>(string location, Action<TObject> cb) where TObject : Object
        {
            cb?.Invoke(await Proxy.LoadAssetTask<TObject>(SettingToLocalPath(location)));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset(string location, Action<Object> cb)
        {
            cb?.Invoke(await Proxy.LoadAssetTask<Object>(SettingToLocalPath(location)));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadAsset(string location, Type type, Action<Object> cb)
        {
            cb?.Invoke(await Proxy.LoadAssetTask(SettingToLocalPath(location), type));
        }

        public interface IAsyncHandle : IEnumerator, IDisposable
        {
            /// <summary>
            /// 是否已经完成
            /// </summary>
            bool IsDone { get; }

            /// <summary>
            /// 处理进度
            /// </summary>
            float Progress { get; }
        }

        public interface IAsyncHandle<T> : IAsyncHandle where T : Object
        {
            T Result { get; }

            TaskAwaiter<T> GetAwaiter();
        }

        internal class LoadAssetHandleCo<T> : IAsyncHandle<T> where T : Object
        {
            private IEnumerator CO => _CO ?? (_CO = Proxy.LoadAssetCO<T>(Location, OnCompletedCo));
            private IEnumerator _CO;

            internal readonly string Location;

            public bool IsDone { get; set; }

            public T Result { get; set; }

            public float Progress { get; private set; }

            public Action<T> OnCompleted;

            private void OnCompletedCo(T asset)
            {
                Progress = 1;
                Result = asset;
                IsDone = true;
                OnCompleted?.Invoke(Result);
            }

            private void OnCompletedTask()
            {
                Progress = 1;
                Result = Awaiter.GetResult();
                IsDone = true;
                OnCompleted?.Invoke(Result);
            }

            public LoadAssetHandleCo(string location, Action<T> onCompleted = null)
            {
                Location = location;
                OnCompleted = onCompleted;
                IsDone = false;
                Progress = 0;
                Result = null;
                _CO = null;
            }

            bool IEnumerator.MoveNext()
            {
                return CO.MoveNext();
            }

            void IEnumerator.Reset()
            {
                Progress = 0;
                IsDone = false;
                CO?.Reset();
            }

            object IEnumerator.Current => CO.Current;

            private TaskAwaiter<T> Awaiter;

            public TaskAwaiter<T> GetAwaiter()
            {
                var Task = Proxy.LoadAssetTask<T>(Location);
                Awaiter = Task.GetAwaiter();
                Awaiter.OnCompleted(OnCompletedTask);
                return Awaiter;
            }

            void IDisposable.Dispose()
            {
                _CO = null;
                Result = null;
            }
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IAsyncHandle<TObject> LoadAssetCO<TObject>(string location) where TObject : Object
        {
            return new LoadAssetHandleCo<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IAsyncHandle<TObject> LoadAssetCO<TObject>(string location, Action<TObject> cb)
            where TObject : Object
        {
            return new LoadAssetHandleCo<TObject>(SettingToLocalPath(location), cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadAssetCO(string location, Action<Object> cb)
        {
            return Proxy.LoadAssetCO(SettingToLocalPath(location), cb);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadAssetCO(string location, Type type, Action<Object> cb)
        {
            return Proxy.LoadAssetCO(SettingToLocalPath(location), type, cb);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static TObject LoadAsset<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadAssetSync<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object LoadAsset(string location, Type type)
        {
            return Proxy.LoadAssetSync(SettingToLocalPath(location), type);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Object LoadAsset(string location)
        {
            return Proxy.LoadAssetSync(SettingToLocalPath(location), typeof(Object));
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<TObject> LoadAssetTask<TObject>(string location) where TObject : Object
        {
            return Proxy.LoadAssetTask<TObject>(SettingToLocalPath(location));
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Object> LoadAssetTask(string location, Type type)
        {
            return Proxy.LoadAssetTask(SettingToLocalPath(location), type);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Object> LoadAssetTask(string location)
        {
            return Proxy.LoadAssetTask(SettingToLocalPath(location), typeof(Object));
        }

        #endregion

        #region 场景加载

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadScene(
            string location,
            Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            var scene = await Proxy.LoadSceneTask(SettingToLocalPath(location), sceneMode, suspendLoad, priority);
            cb?.Invoke(scene);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadSceneCO(
            string location,
            Action<Scene> cb,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            return Proxy.LoadSceneCO(SettingToLocalPath(location), cb, sceneMode,
                suspendLoad, priority);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<Scene> LoadSceneTask(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false,
            int priority = 100)
        {
            return Proxy.LoadSceneTask(SettingToLocalPath(location), sceneMode,
                suspendLoad, priority);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static string LoadRawFileText(string location)
        {
            return Proxy.LoadRawFileTextSync(SettingToLocalPath(location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadRawFileText(string location, Action<string> cb)
        {
            cb?.Invoke(await Proxy.LoadRawFileTextTask(SettingToLocalPath(location)));
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<string> LoadRawFileTextTask(string location)
        {
            return Proxy.LoadRawFileTextTask(SettingToLocalPath(location));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="cb">回调</param>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async void LoadRawFileData(string location, Action<byte[]> cb)
        {
            cb?.Invoke(await Proxy.LoadRawFileDataTask(SettingToLocalPath(location)));
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static byte[] LoadRawFileData(string location)
        {
            return Proxy.LoadRawFileDataSync(SettingToLocalPath(location));
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task<byte[]> LoadRawFileDataTask(string location)
        {
            return Proxy.LoadRawFileDataTask(SettingToLocalPath(location));
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb)
        {
            return Proxy.LoadRawFileDataCO(SettingToLocalPath(location), cb);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator LoadRawFileTextCO(string location, Action<string> cb)
        {
            return Proxy.LoadRawFileTextCO(SettingToLocalPath(location), cb);
        }

        #endregion
    }
}