using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEntity : EntityBase
{
    public SpriteRenderer spriteRenderer;
    cfg.item gridItemInfo;
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        spriteRenderer = GetComponent<SpriteRenderer>();
        var data = Params.param as ItemGridData;
        gridItemInfo = cfg.Tables.Instance.Tbitem[data.dataId];
        UpdateShow();
    }


    async void UpdateShow()
    {
        await MultipleSpriteManager.Instance.LoadMultipleSprite(UtilityBuiltin.AssetsPath.GetSpritesPath(gridItemInfo.MultipleSprite));
        spriteRenderer.sprite = MultipleSpriteManager.Instance.GetSprite(gridItemInfo.Image);
    }

    public bool Use()
    {
        if (gridItemInfo.Effect1.Length>0)
        {
            for (int i = 0; i < gridItemInfo.Effect1.Length; i++)
            {
                switch (gridItemInfo.Effect1[0])
                {
                    case 1://hp
                        if (GF.Floor.PlayerEntity.IsFullHp) return false;
                        GF.Floor.PlayerEntity.ApplyHeal(gridItemInfo.Param1[0]);
                        break;
                    case 2://添加资源
                        GF.Floor.PlayerEntity.AddResource(gridItemInfo.Param1[0], gridItemInfo.Param1[1]);
                        break;
                }
            }
        }
        return true;
    }

}
