using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GraphEditor.Core
{
    public class AddressablePreLoader : IDisposable
    {
        public Dictionary<string, Sprite> SpriteAssets { get; private set; }

        private readonly List<AssetReference> references = new();
        public AddressablePreLoader(IEnumerable<string> guids) 
        {
            _ = PreLoad(guids);   
        }

        private async Task PreLoad(IEnumerable<string> guids)
        {
            SpriteAssets = new();
            foreach (var guid in guids)
            {
                AssetReference reference = new(guid);
                try
                {
                    Sprite sprite = await reference.LoadAssetAsync<Sprite>().Task;
                    SpriteAssets.TryAdd(guid, sprite);
                    references.Add(reference);
                }
                catch (Exception)
                {
                    Debug.LogError($"AssetReference {reference.RuntimeKey} failed to load.");
                    SpriteAssets.TryAdd(guid, null);
                }
            }
        }


        public void Dispose()
        {
            foreach (var item in references)
            {
                item.ReleaseAsset();
            }
        }
    }
}

