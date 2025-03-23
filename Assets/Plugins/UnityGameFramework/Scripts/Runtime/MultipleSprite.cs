using System.Collections.Generic;
using UnityEngine;

public class MultipleSprite
{
    Dictionary<string, Sprite> sprites = new();
    public void Init(Object[] imgs)
    {
        for (int i = 0; i < imgs.Length; i++)
        {
            var sprite = imgs[i] as Sprite;
            if(sprite)
            {
                sprites.Add(sprite.name, sprite);
            }
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
