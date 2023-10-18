


/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> Automatic Generate
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace AIO
{
    public partial class AssetSystem
    {

        #region Shader

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Shader LoadShader(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Shader>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadShader(string location, Action<UnityEngine.Shader> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Shader>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Shader> LoadShaderTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Shader>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Shader"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadShaderCO(string location, Action<UnityEngine.Shader> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region AudioClip

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.AudioClip LoadAudioClip(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.AudioClip>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAudioClip(string location, Action<UnityEngine.AudioClip> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.AudioClip>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.AudioClip> LoadAudioClipTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.AudioClip>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AudioClip"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAudioClipCO(string location, Action<UnityEngine.AudioClip> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region TextAsset

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.TextAsset LoadTextAsset(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.TextAsset>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadTextAsset(string location, Action<UnityEngine.TextAsset> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.TextAsset>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.TextAsset> LoadTextAssetTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.TextAsset>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.TextAsset"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadTextAssetCO(string location, Action<UnityEngine.TextAsset> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region Material

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Material LoadMaterial(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Material>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadMaterial(string location, Action<UnityEngine.Material> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Material>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Material> LoadMaterialTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Material>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Material"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadMaterialCO(string location, Action<UnityEngine.Material> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region AssetBundle

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.AssetBundle LoadAssetBundle(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.AssetBundle>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAssetBundle(string location, Action<UnityEngine.AssetBundle> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.AssetBundle>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.AssetBundle> LoadAssetBundleTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.AssetBundle>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.AssetBundle"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAssetBundleCO(string location, Action<UnityEngine.AssetBundle> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region Animation

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Animation LoadAnimation(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Animation>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadAnimation(string location, Action<UnityEngine.Animation> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Animation>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Animation> LoadAnimationTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Animation>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Animation"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadAnimationCO(string location, Action<UnityEngine.Animation> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region Texture

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Texture LoadTexture(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Texture>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadTexture(string location, Action<UnityEngine.Texture> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Texture>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Texture> LoadTextureTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Texture>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadTextureCO(string location, Action<UnityEngine.Texture> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region Texture2D

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Texture2D LoadTexture2D(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Texture2D>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadTexture2D(string location, Action<UnityEngine.Texture2D> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Texture2D>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Texture2D> LoadTexture2DTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Texture2D>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Texture2D"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadTexture2DCO(string location, Action<UnityEngine.Texture2D> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region Font

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Font LoadFont(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Font>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadFont(string location, Action<UnityEngine.Font> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Font>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Font> LoadFontTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Font>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Font"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadFontCO(string location, Action<UnityEngine.Font> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region Mesh

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.Mesh LoadMesh(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.Mesh>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadMesh(string location, Action<UnityEngine.Mesh> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.Mesh>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.Mesh> LoadMeshTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.Mesh>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.Mesh"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadMeshCO(string location, Action<UnityEngine.Mesh> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

        #region GameObject

        /// <summary>
        /// 同步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static UnityEngine.GameObject LoadGameObject(string location)
        {
            return Proxy.LoadAssetSync<UnityEngine.GameObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static async void LoadGameObject(string location, Action<UnityEngine.GameObject> cb)
        {
            cb.Invoke(await Proxy.LoadAssetTask<UnityEngine.GameObject>(Parameter.LoadPathToLower ? location.ToLower() : location));
        }

        /// <summary>
        /// 异步加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static Task<UnityEngine.GameObject> LoadGameObjectTask(string location)
        {
            return Proxy.LoadAssetTask<UnityEngine.GameObject>(Parameter.LoadPathToLower ? location.ToLower() : location);
        }

        /// <summary>
        /// 异步回调加载 <see cref="UnityEngine.GameObject"/>
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator LoadGameObjectCO(string location, Action<UnityEngine.GameObject> cb)
        {
            return Proxy.LoadAssetCO(Parameter.LoadPathToLower ? location.ToLower() : location, cb);
        }

       #endregion

    }
}
