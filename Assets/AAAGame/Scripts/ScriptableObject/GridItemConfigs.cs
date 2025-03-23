using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(fileName = "GridItemConfigs", menuName = "ScriptableObject/格子道具信息")]
public class GridItemConfigs :ScriptableObject
{
    public static GridItemConfigs Instance = null;
    private void Awake()
    {
        Instance = this;
    }

    [Serializable]
    public class GridItemInfo
    {
        [SerializeField]public int Id;
        [SerializeField]public Sprite[] sprites;
        [SerializeField]public int DropId;
        [SerializeField] public Vector2 size;
    }
    [SerializeField] public List<GridItemInfo> itemsL;
    public Dictionary<int, GridItemInfo> Items = new();

    public void Init()
    {
        Items.Clear();
        foreach (var item in itemsL)
        {
            Items[item.Id] = item;
        }
    }

    public GridItemInfo GetItem(int id)
    {
        return Items[id]; 
    }
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下获取实例
    /// </summary>
    /// <returns></returns>
    public static GridItemConfigs GetInstanceEditor()
    {
        var configAsset = UtilityBuiltin.AssetsPath.GetScriptableAsset("GridItemConfigs");
        Instance = AssetDatabase.LoadAssetAtPath<GridItemConfigs>(configAsset);
        Instance.Init();
        return Instance;
    }
#endif
    /// <summary>
    /// 运行时获取实例
    /// </summary>
    /// <returns></returns>
    public static async Task<GridItemConfigs> GetInstanceSync()
    {
        var configAsset = UtilityBuiltin.AssetsPath.GetScriptableAsset("GridItemConfigs");
        if (Instance == null)
        {
            Instance = await GFBuiltin.Resource.LoadAssetAwait<GridItemConfigs>(configAsset);
            Instance.Init();
        }
        return Instance;
    }
}
