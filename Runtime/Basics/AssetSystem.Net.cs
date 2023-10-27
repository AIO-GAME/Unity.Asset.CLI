﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-15
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
#if SUPPORT_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 检测网络请求
        /// </summary>
        /// <returns>Ture:成功 False:异常</returns>
        private static bool LoadCheckNet(UnityWebRequest operation)
        {
#if UNITY_2020_1_OR_NEWER
            switch (operation.result)
            {
                case UnityWebRequest.Result.InProgress:
                    Debug.LogError($"{ERROR_NET}请求正在进行中");
                    return false;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError($"{ERROR_NET}无法连接到服务器");
                    return false;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"{ERROR_NET}服务器返回响应错误");
                    return false;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError($"{ERROR_NET}数据处理异常");
                    return false;
            }

            if (operation.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{ERROR_NET_UNKNOWN}{operation.result} -> {operation.error}");
                return false;
            }
#else
            if (operation.isHttpError || operation.isNetworkError)
            {
                Debug.LogError(operation.error);
                Debug.Log($"{ERROR_NET_UNKNOWN} {operation.error}");
                return false;
            }
#endif

            if (operation.isDone) return true;
            Debug.LogError($"{ERROR_NET}请求未完成");
            return false;
        }

        #region Async

#if SUPPORT_UNITASK
        /// <summary>
        /// 网上加载图片
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static async void NetLoadTexture(string url, Action<Texture2D> cb)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    cb?.Invoke(DownloadHandlerTexture.GetContent(uwr));
                else
                    cb?.Invoke(null);
            }
        }

        /// <summary>
        /// 网上加载图片
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="rect">矩形</param>
        /// <param name="pivot">中心点</param>
        /// <param name="cb">回调</param>
        public static async void NetLoadSprite(string url, Rect rect, Vector2 pivot, Action<Sprite> cb)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    cb?.Invoke(Sprite.Create(DownloadHandlerTexture.GetContent(uwr), rect, pivot));
                else
                    cb?.Invoke(null);
            }
        }

        /// <summary>
        /// 网上加载文本
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static async void NetLoadString(string url, Action<string> cb)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    cb?.Invoke(uwr.downloadHandler.text);
                else
                    cb?.Invoke(null);
            }
        }

        /// <summary>
        /// 网上加载字节
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static async void NetLoadBytes(string url, Action<byte[]> cb)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    cb?.Invoke(uwr.downloadHandler.data);
                else
                    cb?.Invoke(null);
            }
        }

        /// <summary>
        /// 网上加载音频
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="audioType">音频类型</param>
        /// <param name="cb">回调</param>
        public static async void NetLoadAudioClip(string url, AudioType audioType, Action<AudioClip> cb)
        {
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    cb?.Invoke(DownloadHandlerAudioClip.GetContent(uwr));
                else
                    cb?.Invoke(null);
            }
        }

        /// <summary>
        /// 网上加载AB包
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static async void NetLoadAssetBundle(string url, Action<AssetBundle> cb)
        {
            using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    cb?.Invoke(DownloadHandlerAssetBundle.GetContent(uwr));
                else
                    cb?.Invoke(null);
            }
        }

#endif

        #endregion

        #region CO

        /// <summary>
        /// 网上加载图片
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator NetLoadTextureCO(string url, Action<Texture2D> cb)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) cb.Invoke(DownloadHandlerTexture.GetContent(uwr));
            }
        }

        /// <summary>
        /// 网上加载精灵
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="rect">矩形</param>
        /// <param name="pivot">中心点</param>
        /// <param name="cb">回调</param>
        public static IEnumerator NetLoadSpriteCO(string url, Rect rect, Vector2 pivot, Action<Sprite> cb)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                {
                    var tempSprite = Sprite.Create(DownloadHandlerTexture.GetContent(uwr), rect, pivot);
                    cb?.Invoke(tempSprite);
                }
            }
        }

        /// <summary>
        /// 网上加载AB
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator NetLoadAssetBundleCO(string url, Action<AssetBundle> cb)
        {
            using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                yield return uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) cb?.Invoke(DownloadHandlerAssetBundle.GetContent(uwr));
            }
        }

        /// <summary>
        /// 网上加载音频
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="audioType">音频类型</param>
        /// <param name="cb">回调</param>
        public static IEnumerator NetLoadAudioCO(string url, AudioType audioType, Action<AudioClip> cb)
        {
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                yield return uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) cb?.Invoke(DownloadHandlerAudioClip.GetContent(uwr));
            }
        }

        /// <summary>
        /// 网上加载音频
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator NetLoadStringCO(string url, Action<string> cb)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                yield return uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) cb?.Invoke(uwr.downloadHandler.text);
            }
        }

        /// <summary>
        /// 网上加载流数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="cb">回调</param>
        public static IEnumerator NetLoadBytesCO(string url, Action<byte[]> cb)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                yield return uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) cb?.Invoke(uwr.downloadHandler.data);
            }
        }

        #endregion

        #region UniTask

#if SUPPORT_UNITASK
        /// <summary>
        /// 网上加载图片
        /// </summary>
        /// <param name="url">网址</param>
        internal static async UniTask<Texture2D> NetLoadTextureTask(string url)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    return DownloadHandlerTexture.GetContent(uwr);
                return null;
            }
        }

        /// <summary>
        /// 网上加载图片
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="rect">矩形</param>
        /// <param name="pivot">中心点</param>
        public static async UniTask<Sprite> NetLoadSpriteTask(string url, Rect rect, Vector2 pivot)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    return Sprite.Create(DownloadHandlerTexture.GetContent(uwr), rect, pivot);
                return null;
            }
        }

        /// <summary>
        /// 网上加载文本
        /// </summary>
        /// <param name="url">网址</param>
        public static async UniTask<string> NetLoadStringTask(string url)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) return uwr.downloadHandler.text;
                return null;
            }
        }

        /// <summary>
        /// 网上加载字节
        /// </summary>
        /// <param name="url">网址</param>
        public static async UniTask<byte[]> NetLoadBytesTask(string url)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) return uwr.downloadHandler.data;
                return null;
            }
        }

        /// <summary>
        /// 网上加载音频
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="audioType">音频类型</param>
        public static async UniTask<AudioClip> NetLoadAudioClipTask(string url, AudioType audioType)
        {
            using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr))
                    return DownloadHandlerAudioClip.GetContent(uwr);
                return null;
            }
        }

        /// <summary>
        /// 网上加载AB包
        /// </summary>
        /// <param name="url">网址</param>
        public static async UniTask<AssetBundle> NetLoadAssetBundleTask(string url)
        {
            using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                await uwr.SendWebRequest();
                if (LoadCheckNet(uwr)) return DownloadHandlerAssetBundle.GetContent(uwr);
                return null;
            }
        }

#endif

        #endregion
    }
}