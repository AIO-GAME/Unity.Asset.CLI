
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
            return Proxy.LoadAssetSync<UnityEngine.Shader>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Shader>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Shader>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.AudioClip>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.AudioClip>(location);
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
            return CreateLoadAssetHandle<UnityEngine.AudioClip>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.TextAsset>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.TextAsset>(location);
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
            return CreateLoadAssetHandle<UnityEngine.TextAsset>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.Material>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Material>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Material>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.AssetBundle>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.AssetBundle>(location);
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
            return CreateLoadAssetHandle<UnityEngine.AssetBundle>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.Animation>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Animation>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Animation>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.Texture>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Texture>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Texture>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.Texture2D>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Texture2D>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Texture2D>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.Font>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Font>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Font>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.Mesh>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.Mesh>(location);
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
            return CreateLoadAssetHandle<UnityEngine.Mesh>(location, cb);
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
            return Proxy.LoadAssetSync<UnityEngine.GameObject>(SettingToLocalPath(location));
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
            await CreateLoadAssetHandle(location, cb);
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
            return CreateLoadAssetHandle<UnityEngine.GameObject>(location);
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
            return CreateLoadAssetHandle<UnityEngine.GameObject>(location, cb);
        }

       #endregion

    }
}
