using System;
using UnityEngine;
using UnityEngine.U2D;

public class GridEntity: CombatUnitEntity
{
    public const string P_GridID = "P_GridID";

    public SpriteRenderer spriteRenderer;
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        UpdateShow();
    }


    public override bool ApplyDamage(CombatUnitEntity attacker, int damgeValue)
    {
        var ans = base.ApplyDamage(attacker, damgeValue);
        UpdateShow();
        return ans;
    }

    void UpdateShow()
    {
        GF.Resource.LoadAsset(UtilityBuiltin.AssetsPath.GetSpritesPath("grid_poop.png"), typeof(MultipleSprite), new GameFramework.Resource.LoadAssetCallbacks(OnLoadGFExtensionSuccess));
    }

    private void OnLoadGFExtensionSuccess(string assetName, object asset, float duration, object userData)
    {
        var spAtlas = asset as MultipleSprite;
        spriteRenderer.sprite = spAtlas.GetSprite("grid_poop_1");
    }

    protected override void OnBeKilled()
    {
        base.OnBeKilled();

    }
}
