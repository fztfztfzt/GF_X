using System.Collections.Generic;
using UnityEngine;

public class MultipleSprite
{
    Dictionary<string, Sprite> sprites = new();
    public void Init(Object[] imgs)
    {
        for (int i = 1; i < imgs.Length; i++)
        {
            var sprite = imgs[i] as Sprite;
            sprites.Add(sprite.name, sprite);
        }
    }

    public Sprite GetSprite(string name)
    {
        if (sprites.ContainsKey(name))
        {
            return sprites[name];
        }
        return null;
    }
}
