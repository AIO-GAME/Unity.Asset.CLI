/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-02-27
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        public override IEnumerator LoadRawFileTextCO(string location, Action<string> cb)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, yp => package = yp);
                if (package is null)
                {
                    cb?.Invoke(string.Empty);
                    yield break;
                }

                operation = package.LoadRawFileAsync(location);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb?.Invoke(string.Empty);
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb?.Invoke(operation?.GetRawFileText());
        }

        public override string LoadRawFileTextSync(string location)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return string.Empty;
                operation = package.LoadRawFileSync(location);
                if (!LoadCheckOPSync(operation)) return string.Empty;
                HandleAdd(location, operation);
            }

            return operation?.GetRawFileText();
        }

        public override async Task<string> LoadRawFileTextTask(string location)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return string.Empty;
                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return string.Empty;
                HandleAdd(location, operation);
            }

            return operation?.GetRawFileText();
        }

        public override byte[] LoadRawFileDataSync(string location)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = GetAutoPackageSync(location);
                if (package is null) return Array.Empty<byte>();
                operation = package.LoadRawFileSync(location);
                if (!LoadCheckOPSync(operation)) return Array.Empty<byte>();
                HandleAdd(location, operation);
            }

            return operation?.GetRawFileData();
        }

        public override async Task<byte[]> LoadRawFileDataTask(string location)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                var package = await GetAutoPackageTask(location);
                if (package is null) return Array.Empty<byte>();
                operation = package.LoadRawFileAsync(location);
                if (!await LoadCheckOPTask(operation)) return Array.Empty<byte>();
                HandleAdd(location, operation);
            }

            return operation?.GetRawFileData();
        }

        public override IEnumerator LoadRawFileDataCO(string location, Action<byte[]> cb)
        {
            var operation = HandleGet<RawFileOperationHandle>(location);
            if (operation is null)
            {
                ResPackage package = null;
                yield return GetAutoPackageCO(location, ya => package = ya);
                if (package is null)
                {
                    cb?.Invoke(Array.Empty<byte>());
                    yield break;
                }

                operation = package.LoadRawFileAsync(location);
                var check = false;
                yield return LoadCheckOPCo(operation, ya => check = ya);
                if (!check)
                {
                    cb?.Invoke(Array.Empty<byte>());
                    yield break;
                }

                HandleAdd(location, operation);
            }

            cb?.Invoke(operation?.GetRawFileData());
        }
    }
}
#endif