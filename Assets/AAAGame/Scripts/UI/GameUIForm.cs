using GameFramework.Event;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public partial class GameUIForm : UIFormBase
{
    public Image HeartTemplate;
    public Sprite FullSprite;
    public Sprite HalfSprite;
    public Sprite EmptySprite;
    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        RefreshCoinsText();
        GF.Event.Subscribe(PlayerDataChangedEventArgs.EventId, OnDataChange);

    }

    private void OnDataChange(object sender, GameEventArgs e)
    {
        var args = e as PlayerDataChangedEventArgs;
        switch (args.DataType)
        {
            case PlayerDataType.Coins:
                varGoldNum.text = args.Value.ToString();
                break;
            case PlayerDataType.Diamond:
                varBombNum.text = args.Value.ToString();
                break;
            case PlayerDataType.Energy:
                varKeyNum.text = args.Value.ToString();
                break;
            case PlayerDataType.Hp:
                RefreshHp();
                break;
        }
    }

    private void RefreshCoinsText()
    {
        varGoldNum.text = "00";
        varBombNum.text = "00";
        varKeyNum.text = "00";
        RefreshHp();

    }

    private void RefreshHp()
    {
        var playerEntity = GF.Floor.PlayerEntity;
        var maxHp = playerEntity.MaxHp/2;
        var hp = playerEntity.Hp;
        int fullHeart = hp / 2;
        int halfHeart = hp % 2;
        int count = maxHp;
        for (int i = varHeart.childCount; i < count; ++i)
        {
            Instantiate(HeartTemplate, varHeart);
        }

        for (int i = count; i < varHeart.childCount; ++i)
        {
            var obj = varHeart.GetChild(i);
            obj.GetComponent<Image>().enabled = false;
        }
        for (int i = 0; i < count; ++i)
        {
            var obj = varHeart.GetChild(i);
            var image = obj.GetComponent<Image>();
            image.enabled = true;
            if (i < fullHeart)
            {
                image.sprite = FullSprite;
            }
            else if (i < fullHeart + halfHeart)
            {
                image.sprite = HalfSprite;
            }
            else
            {
                image.sprite = EmptySprite;
            }

        }
    }
}
