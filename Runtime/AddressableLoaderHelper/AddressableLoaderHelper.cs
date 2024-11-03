using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RML.AddressablesUtils
{
    public enum LoadingAddressableState
    {
        FinishedSuccessfully,
        Failed,
        Canceled
    }

    public class AddressableLoaderHelper
    {
        private static readonly Dictionary<object, AddressableContext>
            AddressableContexts;

        static AddressableLoaderHelper()
        {
            AddressableContexts =
                new Dictionary<object, AddressableContext>();
        }

        public static TObject LoadAsset<TObject>(object context,
            object assetKey)
        {
            var handler = Addressables.LoadAssetAsync<TObject>(assetKey);

            if (AddressableContexts.TryGetValue(context,
                    out var addressableContext))
            {
                addressableContext.AsyncOperationHandles.Add(handler);
            }
            else
            {
                var addressableContextNew = new AddressableContext();
                addressableContextNew.AsyncOperationHandles.Add(handler);
                AddressableContexts.Add(context, addressableContextNew);
            }

            handler.WaitForCompletion();
            return handler.Result;
        }

        public static async Task<TObject> LoadAssetAsync<TObject>(
            object context,
            object assetKey)
        {
            var handler = Addressables.LoadAssetAsync<TObject>(assetKey);

            if (AddressableContexts.TryGetValue(context,
                    out var addressableContext))
            {
                addressableContext.AsyncOperationHandles.Add(handler);
            }
            else
            {
                var addressableContextNew = new AddressableContext();
                addressableContextNew.AsyncOperationHandles.Add(handler);
                AddressableContexts.Add(context, addressableContextNew);
            }

            var result = await handler.Task;
            return result;
        }

        public static async Task<(GameObject, LoadingAddressableState)>
            LoadAssetAsync(object context,
                object assetKey,
                CancellationToken ct)
        {
            AsyncOperationHandle<GameObject> handler =
                Addressables.LoadAssetAsync<GameObject>(assetKey);

            GameObject result = await handler.Task;

            if (ct.IsCancellationRequested)
            {
                Addressables.Release(handler);
                return (default, LoadingAddressableState.Canceled);
            }

            if (!AddressableContexts.TryGetValue(context,
                    out AddressableContext addressableContext))
            {
                addressableContext = new AddressableContext();
                AddressableContexts.Add(context, addressableContext);
            }

            addressableContext.AsyncOperationHandles.Add(handler);
            var state = LoadingAddressableState.FinishedSuccessfully;
            if (handler.Status == AsyncOperationStatus.Failed)
            {
                state = LoadingAddressableState.Failed;
            }

            return (result, state);
        }

        public static void UnloadAssets(object context)
        {
            if (AddressableContexts.TryGetValue(context,
                    out AddressableContext addressableContext))
            {
                addressableContext.ReleaseAll();
                AddressableContexts.Remove(context);
            }
            else
            {
                Debug.LogWarning(
                    $"Cannot find corresponding context {context}");
            }
        }
    }
}