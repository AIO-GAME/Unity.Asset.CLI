#if SUPPORT_YOOASSET
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine
{
    /// <summary>
    ///     nameof(DeliveryQueryServices)
    /// </summary>
    [Preserve]
    public class ResolverDeliveryQueryServices : IDeliveryQueryServices
    {
        #region IDeliveryQueryServices Members

        public bool QueryDeliveryFiles(string packageName, string fileName)
        {
            return false;
        }

        public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
        {
            return new DeliveryFileInfo();
        }

        #endregion
    }
}
#endif