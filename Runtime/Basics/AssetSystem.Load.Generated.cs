
/*|✩ - - - - - |||
|||✩ Date:     ||| -> Automatic Generate
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO
{
    partial class AssetSystem
    {

        #region Shader

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Shader LoadShader(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Shader>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadShader(string location, Action<UnityEngine.Shader> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Shader>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Shader> LoadShaderTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Shader>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Shader> LoadShaderTask(string location, Action<UnityEngine.Shader> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Shader>.Create(location, cb);
        }

       #endregion

        #region AudioClip

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.AudioClip LoadAudioClip(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.AudioClip>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAudioClip(string location, Action<UnityEngine.AudioClip> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.AudioClip>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AudioClip> LoadAudioClipTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.AudioClip>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AudioClip> LoadAudioClipTask(string location, Action<UnityEngine.AudioClip> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.AudioClip>.Create(location, cb);
        }

       #endregion

        #region TextAsset

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.TextAsset LoadTextAsset(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.TextAsset>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadTextAsset(string location, Action<UnityEngine.TextAsset> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.TextAsset>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.TextAsset> LoadTextAssetTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.TextAsset>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.TextAsset> LoadTextAssetTask(string location, Action<UnityEngine.TextAsset> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.TextAsset>.Create(location, cb);
        }

       #endregion

        #region Material

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Material LoadMaterial(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Material>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadMaterial(string location, Action<UnityEngine.Material> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Material>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Material> LoadMaterialTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Material>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Material> LoadMaterialTask(string location, Action<UnityEngine.Material> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Material>.Create(location, cb);
        }

       #endregion

        #region AssetBundle

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.AssetBundle LoadAssetBundle(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.AssetBundle>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAssetBundle(string location, Action<UnityEngine.AssetBundle> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.AssetBundle>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AssetBundle> LoadAssetBundleTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.AssetBundle>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AssetBundle> LoadAssetBundleTask(string location, Action<UnityEngine.AssetBundle> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.AssetBundle>.Create(location, cb);
        }

       #endregion

        #region Animation

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Animation LoadAnimation(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Animation>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAnimation(string location, Action<UnityEngine.Animation> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Animation>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Animation> LoadAnimationTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Animation>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Animation> LoadAnimationTask(string location, Action<UnityEngine.Animation> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Animation>.Create(location, cb);
        }

       #endregion

        #region Texture

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Texture LoadTexture(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Texture>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadTexture(string location, Action<UnityEngine.Texture> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Texture>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture> LoadTextureTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Texture>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture> LoadTextureTask(string location, Action<UnityEngine.Texture> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Texture>.Create(location, cb);
        }

       #endregion

        #region Texture2D

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Texture2D LoadTexture2D(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Texture2D>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadTexture2D(string location, Action<UnityEngine.Texture2D> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Texture2D>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture2D> LoadTexture2DTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Texture2D>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture2D> LoadTexture2DTask(string location, Action<UnityEngine.Texture2D> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Texture2D>.Create(location, cb);
        }

       #endregion

        #region Font

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Font LoadFont(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Font>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadFont(string location, Action<UnityEngine.Font> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Font>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Font> LoadFontTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Font>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Font> LoadFontTask(string location, Action<UnityEngine.Font> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Font>.Create(location, cb);
        }

       #endregion

        #region Mesh

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.Mesh LoadMesh(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Mesh>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadMesh(string location, Action<UnityEngine.Mesh> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.Mesh>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Mesh> LoadMeshTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.Mesh>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Mesh> LoadMeshTask(string location, Action<UnityEngine.Mesh> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.Mesh>.Create(location, cb);
        }

       #endregion

        #region GameObject

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static UnityEngine.GameObject LoadGameObject(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.GameObject>.Create(location).Invoke();
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadGameObject(string location, Action<UnityEngine.GameObject> cb)
        {
            await LoaderHandleLoadAsset<UnityEngine.GameObject>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.GameObject> LoadGameObjectTask(string location)
        {
            return LoaderHandleLoadAsset<UnityEngine.GameObject>.Create(location);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.GameObject> LoadGameObjectTask(string location, Action<UnityEngine.GameObject> cb)
        {
            return LoaderHandleLoadAsset<UnityEngine.GameObject>.Create(location, cb);
        }

       #endregion

    }
}
