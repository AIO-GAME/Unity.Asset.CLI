#if SUPPORT_YOOASSET
using System;
using System.IO;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 资源文件解密服务类
    /// </summary>
    public class ResolverDecryptionServices : IDecryptionServices
    {
        public ulong LoadFromFileOffset(DecryptFileInfo fileInfo)
        {
            return 32;
        }

        public byte[] LoadFromMemory(DecryptFileInfo fileInfo)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream LoadFromStream(DecryptFileInfo fileInfo)
        {
            return new ResolverBundleStream(fileInfo.FilePath, FileMode.Open);
        }

        public uint GetManagedReadBufferSize()
        {
            return 1024;
        }
    }
}
#endif
