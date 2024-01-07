using System.IO;

namespace AIO.UEngine
{
    public class ResolverBundleStream : FileStream
    {
        public const byte KEY = 64;

        public ResolverBundleStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize,
            bool useAsync) : base(path, mode, access, share, bufferSize, useAsync)
        {
        }

        public ResolverBundleStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            for (var i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }
            return index;
        }
    }
}
