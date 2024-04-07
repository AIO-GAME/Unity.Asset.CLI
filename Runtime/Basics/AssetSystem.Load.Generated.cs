
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
            return ASHandleLoadAsset<UnityEngine.Shader>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Shader>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Shader> LoadShaderTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Shader>.Create(location);
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
        public static IHandle<UnityEngine.Shader> LoadShaderTask(string location, Action<UnityEngine.Shader> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Shader>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.AudioClip>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.AudioClip>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.AudioClip> LoadAudioClipTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.AudioClip>.Create(location);
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
        public static IHandle<UnityEngine.AudioClip> LoadAudioClipTask(string location, Action<UnityEngine.AudioClip> cb)
        {
            return ASHandleLoadAsset<UnityEngine.AudioClip>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.TextAsset>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.TextAsset>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.TextAsset> LoadTextAssetTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.TextAsset>.Create(location);
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
        public static IHandle<UnityEngine.TextAsset> LoadTextAssetTask(string location, Action<UnityEngine.TextAsset> cb)
        {
            return ASHandleLoadAsset<UnityEngine.TextAsset>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.Material>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Material>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Material> LoadMaterialTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Material>.Create(location);
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
        public static IHandle<UnityEngine.Material> LoadMaterialTask(string location, Action<UnityEngine.Material> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Material>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.AssetBundle>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.AssetBundle>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.AssetBundle> LoadAssetBundleTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.AssetBundle>.Create(location);
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
        public static IHandle<UnityEngine.AssetBundle> LoadAssetBundleTask(string location, Action<UnityEngine.AssetBundle> cb)
        {
            return ASHandleLoadAsset<UnityEngine.AssetBundle>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.Animation>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Animation>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Animation> LoadAnimationTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Animation>.Create(location);
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
        public static IHandle<UnityEngine.Animation> LoadAnimationTask(string location, Action<UnityEngine.Animation> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Animation>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.Texture>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Texture>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Texture> LoadTextureTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Texture>.Create(location);
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
        public static IHandle<UnityEngine.Texture> LoadTextureTask(string location, Action<UnityEngine.Texture> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Texture>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.Texture2D>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Texture2D>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Texture2D> LoadTexture2DTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Texture2D>.Create(location);
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
        public static IHandle<UnityEngine.Texture2D> LoadTexture2DTask(string location, Action<UnityEngine.Texture2D> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Texture2D>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.Font>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Font>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Font> LoadFontTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Font>.Create(location);
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
        public static IHandle<UnityEngine.Font> LoadFontTask(string location, Action<UnityEngine.Font> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Font>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.Mesh>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.Mesh>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.Mesh> LoadMeshTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.Mesh>.Create(location);
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
        public static IHandle<UnityEngine.Mesh> LoadMeshTask(string location, Action<UnityEngine.Mesh> cb)
        {
            return ASHandleLoadAsset<UnityEngine.Mesh>.Create(location, cb);
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
            return ASHandleLoadAsset<UnityEngine.GameObject>.Create(location).Invoke();
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
            await ASHandleLoadAsset<UnityEngine.GameObject>.Create(location, cb);
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IHandle<UnityEngine.GameObject> LoadGameObjectTask(string location)
        {
            return ASHandleLoadAsset<UnityEngine.GameObject>.Create(location);
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
        public static IHandle<UnityEngine.GameObject> LoadGameObjectTask(string location, Action<UnityEngine.GameObject> cb)
        {
            return ASHandleLoadAsset<UnityEngine.GameObject>.Create(location, cb);
        }

       #endregion

    }
}
