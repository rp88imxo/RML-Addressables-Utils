using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RML.AddressablesUtils
{
    public class AddressableContext
    {
        public AddressableContext(
            IEnumerable<AsyncOperationHandle> asyncOperationHandles)
            : this()
        {
            AsyncOperationHandles.AddRange(asyncOperationHandles);
        }

        public AddressableContext()
        {
            AsyncOperationHandles = new List<AsyncOperationHandle>();
        }

        public List<AsyncOperationHandle> AsyncOperationHandles { get; set; }

        public void ReleaseAll()
        {
            foreach (var handle in AsyncOperationHandles)
            {
                Addressables.Release(handle);
            }

            AsyncOperationHandles.Clear();
        }
    }
}