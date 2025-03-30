
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class MultipleSpriteManager
{
    private static MultipleSpriteManager m_Instance;
    public static MultipleSpriteManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new ();
            }
            return m_Instance;
        }
    }

    private Dictionary<string, Sprite> m_SpriteDict = new ();
    HashSet<string> m_LoadedSprite = new HashSet<string>();

    public async UniTask LoadMultipleSprite(string path)
    {
        if (m_LoadedSprite.Contains(path))
        {
            return;
        }
        m_LoadedSprite.Add(path);
        if (!Application.isPlaying)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
            foreach (Sprite sprite in sprites)
            {
                m_SpriteDict.Add(sprite.name, sprite);
            }
            return;
        }
        var ans = await GF.Resource.LoadAssetAwait<MultipleSprite>(path);
        OnLoadGFExtensionSuccess(ans);
    }

    private void OnLoadGFExtensionSuccess(object asset)
    {
        var spAtlas = asset as MultipleSprite;
        foreach (var item in spAtlas.Sprites)
        {
            m_SpriteDict.Add(item.Key, item.Value);
        }
    }

    public Sprite GetSprite(string name)
    {
        if (m_SpriteDict.ContainsKey(name))
        {
            return m_SpriteDict[name];
        }
        GF.LogError($"MultipleSpriteManager.GetSprite: Sprite {name} not found.");
        return null;
    }
}

