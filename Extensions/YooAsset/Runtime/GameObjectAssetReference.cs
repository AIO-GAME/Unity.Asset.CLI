using UnityEditor;
using UnityEngine;
using YooAsset;

#if UNITY_EDITOR
[CustomEditor(typeof(GameObjectAssetReference), true)]
public class GameObjectAssetReferenceInspector : Editor
{
    private GameObject _cacheObject;
    private bool       _init;

    public override void OnInspectorGUI()
    {
        var mono = (GameObjectAssetReference)target;

        if (_init == false)
        {
            _init = true;
            if (string.IsNullOrEmpty(mono.AssetGUID) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(mono.AssetGUID);
                if (string.IsNullOrEmpty(assetPath) == false)
                    _cacheObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            }
        }

        var go = (GameObject)EditorGUILayout.ObjectField(_cacheObject, typeof(GameObject), false);
        if (go != _cacheObject)
        {
            _cacheObject = go;
            var assetPath = AssetDatabase.GetAssetPath(go);
            mono.AssetGUID = AssetDatabase.AssetPathToGUID(assetPath);
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.LabelField("Asset GUID", mono.AssetGUID);
    }
}
#endif

public class GameObjectAssetReference : MonoBehaviour
{
    [HideInInspector] public string AssetGUID = "";

    private AssetOperationHandle _handle;

    public void Start()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var assetInfo = package.GetAssetInfoByGUID(AssetGUID);
        _handle           =  package.LoadAssetAsync(assetInfo);
        _handle.Completed += Handle_Completed;
    }

    public void OnDestroy()
    {
        if (_handle != null)
        {
            _handle.Release();
            _handle = null;
        }
    }

    private void Handle_Completed(AssetOperationHandle handle)
    {
        if (handle.Status == EOperationStatus.Succeed) handle.InstantiateSync(transform);
    }
}