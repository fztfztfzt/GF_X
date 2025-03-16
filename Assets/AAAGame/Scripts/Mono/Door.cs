
using UnityEngine;

public class Door:MonoBehaviour
{
    public SpriteRenderer doorSprite;
    public GameObject CloseObj;
    public Vector2Int Dir;
    public BoxCollider2D trigger;
    public BoxCollider2D collider2d;

    public void Show(bool show)
    {
        doorSprite.gameObject.SetActive(show);
        CloseObj.SetActive(show);
        collider2d.enabled = !show;
        trigger.enabled = show;
    }

    public void Close()
    {
        CloseObj.SetActive(true);
        collider2d.enabled = true;
        trigger.enabled = false;
    }

    public void Open()
    {
        CloseObj.SetActive(false);
        collider2d.enabled = false;
        trigger.enabled = true;
    }

}
