using UnityEngine;

public class PlayerSpriteSetter : MonoBehaviour
{
    public int Head_NormalIndex = 0;
    public int Head_RightIndex = 2;
    public int Head_BackIndex = 4;
    public int Head_AnimCount = 2;
    
    public Vector2Int Body_Normal = new(6,0);
    public Vector2Int Body_Right = new(0,2);

    public SpriteRenderer HeadSprite;
    public SpriteRenderer BodySprite;
    public int State = 0;
    public int BodyState = 0;
    public float cellSize = 0.0625f;
    private MaterialPropertyBlock headBlock;
    private MaterialPropertyBlock bodyBlock;

    public bool HeadAnim = false;
    public bool TestMode = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        headBlock= new MaterialPropertyBlock();
        bodyBlock = new MaterialPropertyBlock();
        HeadSprite.GetPropertyBlock(headBlock);
        BodySprite.GetPropertyBlock(bodyBlock);
        FaceNormal();
        BodyNormal();
    }

    // Update is called once per frame
    void Update()
    {
        if(!TestMode) return;
        if (State == 0)
        {
            Face(Head_RightIndex);
            headBlock.SetFloat("_FlipX", 0);
        }
        else if (State == 1)
        {
            Face(Head_NormalIndex);
        }
        else if (State == 2)
        {
            Face(Head_RightIndex);
            headBlock.SetFloat("_FlipX", 1);
        }
        if(HeadAnim)
        {
            PlayHeadAnim(true);
        }
        else
        {
            PlayHeadAnim(false);
        }

        if(BodyState == 0)
        {
            BodyWalkRight();
        }
        else if (BodyState == 1)
        {
            BodyWalkNormal();
        }
        else if (BodyState == 2)
        {
            BodyWalkLeft();
        }
    }

    public void FaceRight()
    {
        Face(Head_RightIndex);
        headBlock.SetFloat("_FlipX", 0);
        HeadSprite.SetPropertyBlock(headBlock);
    }

    public void FaceLeft()
    {
        Face(Head_RightIndex);
        headBlock.SetFloat("_FlipX", 1);
        HeadSprite.SetPropertyBlock(headBlock);
    }

    public void FaceNormal()
    {
        Face(Head_NormalIndex);
        HeadSprite.SetPropertyBlock(headBlock);
    }

    public void FaceBack()
    {
        Face(Head_BackIndex);
        headBlock.SetFloat("_FlipX", 0);
        HeadSprite.SetPropertyBlock(headBlock);
    }

    private void Face(int face)
    {
        headBlock.SetVector("_StartUV", new Vector2(face * cellSize, 1 - cellSize));
        HeadSprite.SetPropertyBlock(headBlock);
    }

    public void PlayHeadAnim(bool play)
    {
        if (play)
        {
            headBlock.SetFloat("_XCount", Head_AnimCount);
        }
        else
        {
            headBlock.SetFloat("_XCount", 1);
        }
        HeadSprite.SetPropertyBlock(headBlock);
    }

    public void BodyWalkNormal()
    {
        bodyBlock.SetVector("_StartUV", new Vector2(Body_Normal.x * cellSize, 1 - (Body_Normal.y * cellSize)));
        bodyBlock.SetInt("_XCount", 10);
        bodyBlock.SetInt("_FirstXLen", 2);
        bodyBlock.SetFloat("_FlipX", 0);
        BodySprite.SetPropertyBlock(bodyBlock);
    }

    public void BodyWalkRight()
    {
        bodyBlock.SetVector("_StartUV", new Vector2(Body_Right.x * cellSize, 1 - (Body_Right.y * cellSize)));
        bodyBlock.SetInt("_XCount", 10);
        bodyBlock.SetInt("_FirstXLen", 8);
        bodyBlock.SetFloat("_FlipX", 0);
        BodySprite.SetPropertyBlock(bodyBlock);
    }

    public void BodyWalkLeft()
    {
        BodyWalkRight();
        bodyBlock.SetFloat("_FlipX", 1);
        BodySprite.SetPropertyBlock(bodyBlock);
    }

    public void BodyNormal()
    {
        bodyBlock.SetVector("_StartUV", new Vector2(0, 1 - (2 * cellSize)));
        bodyBlock.SetInt("_XCount", 1);
        BodySprite.SetPropertyBlock(bodyBlock);
    }
}
