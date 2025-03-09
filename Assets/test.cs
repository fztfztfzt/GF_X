using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public Animator animator;
    public List<Sprite> sprites; // 要设置的 Sprite 列表
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var a = animator.GetCurrentAnimatorClipInfo(0);
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "Circle (1)",
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Count];
        keyframes[0] = new ObjectReferenceKeyframe
        {
            time = 0f,
            value = sprites[0]
        };
        keyframes[1] = new ObjectReferenceKeyframe
        {
            time = 0.5f,
            value = sprites[1]
        };
        keyframes[2] = new ObjectReferenceKeyframe
        {
            time = 1f,
            value = sprites[2]
        };
        AnimationUtility.SetObjectReferenceCurve(a[0].clip, binding, keyframes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
