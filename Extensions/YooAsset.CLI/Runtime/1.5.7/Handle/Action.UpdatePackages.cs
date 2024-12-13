#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <inheritdoc />
        public override IOperationAction<bool> UpdatePackagesTask(ASConfig config, Action<bool> completed = null) { return new ActionUpdatePackages(config, completed); }

        private static bool CheckPackages(string remote, string content, out AssetsPackageConfig[] packages)
        {
            if (string.IsNullOrEmpty(content))
            {
#if UNITY_EDITOR
                throw new Exception($"{remote} Request failed");
#else
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionRequestFailure);
                AssetSystem.LogError($"{remote} Request failed");
                packages = Array.Empty<AssetsPackageConfig>();
                return false;
#endif
            }

            try
            {
                packages = AHelper.Json.Deserialize<AssetsPackageConfig[]>(content);
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                throw new Exception($"ASConfig Remote Version Parsing Json Failure : {e}");
            }
#else
            catch (Exception)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionParsingJsonFailure);
                packages = Array.Empty<AssetsPackageConfig>();
                return false;
            }
#endif

            if (packages is null || packages.Length == 0)
            {
#if UNITY_EDITOR
                throw new ArgumentNullException($"Please set the ASConfig Packages configuration");
#else
                AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                packages = Array.Empty<AssetsPackageConfig>();
                return false;
#endif
            }

            return true;
        }

        private static string GetPackageManifestVersionUrl(ASConfig config, AssetsPackageConfig item) => $"{config.URL}/{AssetSystem.PlatformNameStr}/{item.Name}/{item.Version}/PackageManifest_{item.Name}.version?t={DateTime.Now.Ticks}";

        /// <summary>
        ///     更新资源包列表
        /// </summary>
        private static bool UpdatePackagesRemoteSync(ASConfig config)
        {
            if (string.IsNullOrEmpty(config.URL))
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlIsNull);
                return false;
            }

            var    remote = config.FullURL;
            string content;
            try
            {
                content = AHelper.HTTP.Get(remote, Encoding.UTF8);
            }
            catch (Exception)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionRequestFailure);
                return false;
            }

            if (!CheckPackages(remote, content, out config.Packages)) return false;

            foreach (var item in config.Packages)
            {
                item.IsLatest = item.Version == "Latest"; // 如果使用Latest则认为是最新版本 同时需要获取最新版本号
                if (!item.IsLatest) continue;
                var url  = GetPackageManifestVersionUrl(config, item);
                var temp = AHelper.HTTP.Get(url, Encoding.UTF8);
                if (string.IsNullOrEmpty(temp))
                {
                    AssetSystem.LogError($"{url} Request failed");
#if !UNITY_EDITOR
                    AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionRequestFailure);
#endif
                    return false;
                }

                item.Version = temp;
            }

            return true;
        }

        /// <summary>
        ///     更新资源包列表
        /// </summary>
        private static async Task<bool> UpdatePackagesRemoteTask(ASConfig config)
        {
            if (string.IsNullOrEmpty(config.URL))
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlIsNull);
                return false;
            }

            var    remote = $"{config.URL}/Version/{AssetSystem.PlatformNameStr}.json?t={DateTime.Now.Ticks}";
            string content;
            try
            {
                content = await AHelper.HTTP.GetAsync(remote, Encoding.UTF8);
            }
            catch (Exception e)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionRequestFailure);
                AssetSystem.LogError($"{remote} Request failed : {e}");
                return false;
            }

            if (!CheckPackages(remote, content, out config.Packages)) return false;

            foreach (var item in config.Packages)
            {
                item.IsLatest = item.Version == "Latest"; // 如果使用Latest则认为是最新版本 同时需要获取最新版本号
                if (!item.IsLatest) continue;
                var url  = GetPackageManifestVersionUrl(config, item);
                var temp = await AHelper.HTTP.GetAsync(url, Encoding.UTF8);
                if (string.IsNullOrEmpty(temp))
                {
                    AssetSystem.LogError($"{url} Request failed");
#if !UNITY_EDITOR
                    AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionRequestFailure);
#endif
                    return false;
                }

                item.Version = temp;
            }

            return true;
        }

#if UNITY_EDITOR

        private static bool UpdatePackagesEditor(ASConfig config)
        {
            var assembly    = Assembly.Load("AIO.Asset.Editor");
            var type        = assembly.GetType("AIO.UEditor.AssetCollectRoot", true);
            var getOrCreate = type.GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.Public);
            var collectRoot = getOrCreate?.Invoke(null, new object[] { });
            if (collectRoot is null)
            {
                AssetSystem.ExceptionEvent(ASException.NoFoundAssetCollectRoot);
                return false;
            }

            var packages = type.GetField("Packages", BindingFlags.Instance | BindingFlags.Public)?.GetValue(collectRoot);
            if (!(packages is Array array))
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                return false;
            }

            var fieldInfo = assembly.GetType("AIO.UEditor.AssetCollectPackage", true).GetField("Name", BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo is null)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                return false;
            }

            var list = (
                from object item in array
                where item != null
                select new AssetsPackageConfig
                {
                    Name    = fieldInfo.GetValue(item) as string,
                    Version = "-.-.-"
                }).ToArray();

            if (list.Length <= 0)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                return false;
            }

            config.Packages              = list;
            config.Packages[0].IsDefault = true;
            return true;
        }

#endif
        private static bool UpdatePackagesLocal(ASConfig config)
        {
            var temp =
                AHelper.IO.ReadJsonUTF8<AssetsPackageConfig[]>($"{AssetSystem.BuildInRootDirectory}/Version/{AssetSystem.PlatformNameStr}.json");
            if (temp != null)
            {
                config.Packages = temp;
                return true;
            }

            if (config.Packages is null)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                return false;
            }

            return true;
        }

        private static IEnumerator UpdatePackagesRemoteCoroutine(ASConfig config, Action<bool> cb)
        {
            if (string.IsNullOrEmpty(config.URL))
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlIsNull);
                cb.Invoke(false);
                yield break;
            }

            var remote  = config.FullURL;
            var content = string.Empty;
            using (var uwr = UnityWebRequest.Get(remote))
            {
                yield return uwr.SendWebRequest();
                if (AssetSystem.LoadCheckNet(uwr)) content = uwr.downloadHandler.text;
            }

            if (!CheckPackages(remote, content, out config.Packages))
            {
                cb.Invoke(false);
                yield break;
            }

            foreach (var item in config.Packages)
            {
                item.IsLatest = item.Version == "Latest"; // 如果使用Latest则认为是最新版本 同时需要获取最新版本号
                if (!item.IsLatest) continue;
                var url = GetPackageManifestVersionUrl(config, item);
                using (var uwr = UnityWebRequest.Get(url))
                {
                    yield return uwr.SendWebRequest();
                    if (AssetSystem.LoadCheckNet(uwr))
                    {
                        var temp = uwr.downloadHandler.text;
                        if (string.IsNullOrEmpty(temp))
                        {
                            AssetSystem.LogError($"{url} Request failed");
#if !UNITY_EDITOR
                            AssetSystem.ExceptionEvent(ASException.ASConfigRemoteUrlRemoteVersionRequestFailure);
#endif
                            cb.Invoke(false);
                            yield break;
                        }

                        item.Version = temp;
                    }
                }
            }

            cb.Invoke(true);
        }

        private class ActionUpdatePackages : OperationAction<bool>
        {
            private ASConfig config { get; }

            public ActionUpdatePackages(ASConfig config, Action<bool> completed) : base(completed) => this.config = config;

            /// <inheritdoc />
            protected override TaskAwaiter<bool> CreateAsync()
            {
                TaskAwaiter<bool> Awaiter;
                switch (config.ASMode)
                {
                    case EASMode.Remote:
                        Awaiter = UpdatePackagesRemoteTask(config).GetAwaiter();
                        break;
                    case EASMode.Editor:
#if UNITY_EDITOR
                        Awaiter = Task.FromResult(UpdatePackagesEditor(config)).GetAwaiter();
                        break;
#endif
                    case EASMode.Local:
                        Awaiter = Task.FromResult(UpdatePackagesLocal(config)).GetAwaiter();
                        break;
                    default:
                        AssetSystem.ExceptionEvent(ASException.NoSupportEASMode);
                        Awaiter = Task.FromResult(false).GetAwaiter();
                        break;
                }

                Awaiter.OnCompleted(() => Result = Awaiter.GetResult());
                return Awaiter;
            }

            /// <inheritdoc />
            protected override IEnumerator CreateCoroutine()
            {
                switch (config.ASMode)
                {
                    case EASMode.Remote:
                        yield return UpdatePackagesRemoteCoroutine(config, b => Result = b);
                        break;
                    case EASMode.Editor:
#if UNITY_EDITOR
                        Result = UpdatePackagesEditor(config);
                        break;
#endif
                    case EASMode.Local:
                        Result = UpdatePackagesLocal(config);
                        break;
                    default:
                        Result = false;
                        AssetSystem.ExceptionEvent(ASException.NoSupportEASMode);
                        break;
                }

                InvokeOnCompleted();
            }

            /// <inheritdoc />
            protected override void CreateSync()
            {
                switch (config.ASMode)
                {
                    case EASMode.Remote:
                        Result = UpdatePackagesRemoteSync(config);
                        break;
                    case EASMode.Editor:
#if UNITY_EDITOR
                        Result = UpdatePackagesEditor(config);
                        break;
#endif
                    case EASMode.Local:
                        Result = UpdatePackagesLocal(config);
                        break;
                    default:
                        Result = false;
                        AssetSystem.ExceptionEvent(ASException.NoSupportEASMode);
                        break;
                }

                IsDone = true;
            }
        }
    }
}
#endif