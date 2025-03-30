using System.Collections.Generic;
using UnityEngine;

public class MultipleSprite
{
    public Dictionary<string, Sprite> Sprites = new();
    public void Init(Object[] imgs)
    {
        for (int i = 0; i < imgs.Length; i++)
        {
            var sprite = imgs[i] as Sprite;
            if(sprite)
            {
                Sprites.Add(sprite.name, sprite);
            }
        }
    }

    public Sprite GetSprite(string name)
    {
        if (Sprites.ContainsKey(name))
        {
            return Sprites[name];
        }
        return null;
    }
}
