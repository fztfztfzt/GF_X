using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityGameFramework.Runtime;
using static GridItemConfigs;

public class GridEntity: CombatUnitEntity
{
    public const string P_GridID = "P_GridID";

    public SpriteRenderer spriteRenderer;
    GridItemInfo gridItemInfo;
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        var data = Params.param as DestroyableItemGridData;
        gridItemInfo = GridItemConfigs.Instance.Items[data.dataId];
        Hp = data.hp;
        UpdateShow();
    }


    public override bool ApplyDamage(int damgeValue)
    {
        var ans = base.ApplyDamage(damgeValue);
        UpdateShow();
        return ans;
    }

    void UpdateShow()
    {
        int index = gridItemInfo.sprites.Length - Hp - 1;
        if(index >= 0)
        {
            spriteRenderer.sprite = gridItemInfo.sprites[index];
        }
        //GF.Resource.LoadAsset(UtilityBuiltin.AssetsPath.GetSpritesPath("grid_poop.png"), typeof(MultipleSprite), new GameFramework.Resource.LoadAssetCallbacks(OnLoadGFExtensionSuccess));
    }

    private void OnLoadGFExtensionSuccess(string assetName, object asset, float duration, object userData)
    {
        var spAtlas = asset as MultipleSprite;
        spriteRenderer.sprite = spAtlas.GetSprite("grid_poop_1");
    }

    protected override void OnBeKilled()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
