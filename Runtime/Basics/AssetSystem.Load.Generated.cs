
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
            => Proxy.LoadAssetTask<UnityEngine.Shader>(location, typeof(UnityEngine.Shader)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadShader(string location, Action<UnityEngine.Shader> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Shader>(location, typeof(UnityEngine.Shader), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Shader> LoadShaderTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Shader>(location, typeof(UnityEngine.Shader));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Shader> LoadShaderTask(string location, Action<UnityEngine.Shader> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Shader>(location, typeof(UnityEngine.Shader), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.AudioClip>(location, typeof(UnityEngine.AudioClip)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAudioClip(string location, Action<UnityEngine.AudioClip> completed)
            => await Proxy.LoadAssetTask<UnityEngine.AudioClip>(location, typeof(UnityEngine.AudioClip), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AudioClip> LoadAudioClipTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.AudioClip>(location, typeof(UnityEngine.AudioClip));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AudioClip> LoadAudioClipTask(string location, Action<UnityEngine.AudioClip> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.AudioClip>(location, typeof(UnityEngine.AudioClip), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.TextAsset>(location, typeof(UnityEngine.TextAsset)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadTextAsset(string location, Action<UnityEngine.TextAsset> completed)
            => await Proxy.LoadAssetTask<UnityEngine.TextAsset>(location, typeof(UnityEngine.TextAsset), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.TextAsset> LoadTextAssetTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.TextAsset>(location, typeof(UnityEngine.TextAsset));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.TextAsset> LoadTextAssetTask(string location, Action<UnityEngine.TextAsset> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.TextAsset>(location, typeof(UnityEngine.TextAsset), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.Material>(location, typeof(UnityEngine.Material)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadMaterial(string location, Action<UnityEngine.Material> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Material>(location, typeof(UnityEngine.Material), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Material> LoadMaterialTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Material>(location, typeof(UnityEngine.Material));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Material> LoadMaterialTask(string location, Action<UnityEngine.Material> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Material>(location, typeof(UnityEngine.Material), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.AssetBundle>(location, typeof(UnityEngine.AssetBundle)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAssetBundle(string location, Action<UnityEngine.AssetBundle> completed)
            => await Proxy.LoadAssetTask<UnityEngine.AssetBundle>(location, typeof(UnityEngine.AssetBundle), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AssetBundle> LoadAssetBundleTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.AssetBundle>(location, typeof(UnityEngine.AssetBundle));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.AssetBundle> LoadAssetBundleTask(string location, Action<UnityEngine.AssetBundle> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.AssetBundle>(location, typeof(UnityEngine.AssetBundle), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.Animation>(location, typeof(UnityEngine.Animation)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadAnimation(string location, Action<UnityEngine.Animation> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Animation>(location, typeof(UnityEngine.Animation), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Animation> LoadAnimationTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Animation>(location, typeof(UnityEngine.Animation));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Animation> LoadAnimationTask(string location, Action<UnityEngine.Animation> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Animation>(location, typeof(UnityEngine.Animation), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.Texture>(location, typeof(UnityEngine.Texture)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadTexture(string location, Action<UnityEngine.Texture> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Texture>(location, typeof(UnityEngine.Texture), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture> LoadTextureTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Texture>(location, typeof(UnityEngine.Texture));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture> LoadTextureTask(string location, Action<UnityEngine.Texture> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Texture>(location, typeof(UnityEngine.Texture), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.Texture2D>(location, typeof(UnityEngine.Texture2D)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadTexture2D(string location, Action<UnityEngine.Texture2D> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Texture2D>(location, typeof(UnityEngine.Texture2D), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture2D> LoadTexture2DTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Texture2D>(location, typeof(UnityEngine.Texture2D));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Texture2D> LoadTexture2DTask(string location, Action<UnityEngine.Texture2D> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Texture2D>(location, typeof(UnityEngine.Texture2D), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.Font>(location, typeof(UnityEngine.Font)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadFont(string location, Action<UnityEngine.Font> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Font>(location, typeof(UnityEngine.Font), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Font> LoadFontTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Font>(location, typeof(UnityEngine.Font));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Font> LoadFontTask(string location, Action<UnityEngine.Font> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Font>(location, typeof(UnityEngine.Font), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.Mesh>(location, typeof(UnityEngine.Mesh)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadMesh(string location, Action<UnityEngine.Mesh> completed)
            => await Proxy.LoadAssetTask<UnityEngine.Mesh>(location, typeof(UnityEngine.Mesh), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Mesh> LoadMeshTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.Mesh>(location, typeof(UnityEngine.Mesh));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.Mesh> LoadMeshTask(string location, Action<UnityEngine.Mesh> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.Mesh>(location, typeof(UnityEngine.Mesh), completed);

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
            => Proxy.LoadAssetTask<UnityEngine.GameObject>(location, typeof(UnityEngine.GameObject)).Invoke();

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static async void LoadGameObject(string location, Action<UnityEngine.GameObject> completed)
            => await Proxy.LoadAssetTask<UnityEngine.GameObject>(location, typeof(UnityEngine.GameObject), completed);

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.GameObject> LoadGameObjectTask(string location)
            =>  Proxy.LoadAssetTask<UnityEngine.GameObject>(location, typeof(UnityEngine.GameObject));

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="completed">回调</param>
        [DebuggerNonUserCode, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static ILoaderHandle<UnityEngine.GameObject> LoadGameObjectTask(string location, Action<UnityEngine.GameObject> completed)
            =>  Proxy.LoadAssetTask<UnityEngine.GameObject>(location, typeof(UnityEngine.GameObject), completed);

       #endregion

    }
}
