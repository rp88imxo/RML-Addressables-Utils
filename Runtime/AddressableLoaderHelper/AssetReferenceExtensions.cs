using UnityEngine.AddressableAssets;

namespace RML.AddressablesUtils
{
    public static class AssetReferenceExtensions
    {
        public static bool Exists(this AssetReference assetReference)
        {
            return assetReference != null
                   && !string.IsNullOrEmpty(assetReference.AssetGUID)
                   && assetReference.RuntimeKeyIsValid();
        }
    }
}