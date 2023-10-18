/*|==========|*|
|*|Author:   |*| -> XINAN
|*|Date:     |*| -> 2023-05-24
|*|==========|*/
#if SUPPORT_YOOASSET

using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("InstanceID = 定位地址")]
    public class AddressRuleInstanceID : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(data.AssetPath);
            if (obj != null)
                return obj.GetInstanceID().ToString();
            return Path.GetFileName(data.AssetPath);
        }
    }
}
#endif