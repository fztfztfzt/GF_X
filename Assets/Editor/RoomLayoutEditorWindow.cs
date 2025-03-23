// RoomLayoutEditorWindow.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomLayoutEditorWindow : EditorWindow
{
    private RoomLayoutAsset currentLayout;
    private RoomGridDef selectedObject;
    private List<RoomGridDef> objectDefinitions = new List<RoomGridDef>();
    private Vector2 scrollPosition;

    #region data
    private Dictionary<string,List<RoomGridDef>> roomLayouts = new();
    private Dictionary<int,RoomGridDef> roomDatas = new();
    #endregion

    [MenuItem("Window/Room Layout Editor")]
    public static void ShowWindow()
    {
        GetWindow<RoomLayoutEditorWindow>("Room Layout Editor");
    }

    void OnEnable()
    {
        LoadObjectDefinitions();
    }

    void LoadObjectDefinitions()
    {
        GridItemConfigs.GetInstanceEditor();
        roomLayouts.Clear();
        var curItems = new List<RoomGridDef>();
        roomLayouts.Add("可破坏", curItems);
        var destroyableItems = GridItemConfigs.Instance.Items;
        foreach (var item in destroyableItems)
        {
            Texture2D subTexture = new Texture2D((int)32, (int)32);
            var sprite = item.Value.sprites[0];
            var spriteRect = sprite.rect;
            subTexture.SetPixels(sprite.texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, 32, 32));
            subTexture.Apply();
            RoomGridDef data = new()
            {
                dataId = item.Key,
                type = 1,
                sprite = subTexture
            };
            curItems.Add(data);
            roomDatas.Add(item.Key, data);
        }

    }
    string selectedCategory = "";

    void OnGUI()
    {
        GUILayout.Label("房间布局编辑器", EditorStyles.boldLabel);

        // 选择布局文件
        currentLayout = EditorGUILayout.ObjectField("布局文件", currentLayout, typeof(RoomLayoutAsset), false) as RoomLayoutAsset;
        if (currentLayout == null)
        {
            if (GUILayout.Button("创建新布局"))
            {
                currentLayout = ScriptableObject.CreateInstance<RoomLayoutAsset>();
                currentLayout.Initialize();
                // 指定保存路径为 Assets/Editor/RoomLayout
                string savePath = "Assets/AAAGame/RoomLayout/New Room Layout.asset";
                // 确保目录存在
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(savePath));
                AssetDatabase.CreateAsset(currentLayout, savePath);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            // 对象选择区域
            GUILayout.Label("对象选择:", EditorStyles.boldLabel);
            // 类别选择
            foreach (var category in roomLayouts)
            {
                if (GUILayout.Button(category.Key))
                {
                    selectedCategory = category.Key;
                }
            }
            // 显示选定类别的对象
            if (selectedCategory != null && roomLayouts.TryGetValue(selectedCategory, out var objectsInCategory))
            {
                foreach (var obj in objectsInCategory)
                {
                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                    if (selectedObject == obj)
                    {
                        // 如果是选中的对象，设置背景为绿色
                        buttonStyle.normal.background = MakeTexture(100, 100, new Color(0.0f, 1.0f, 0.0f, 0.3f));
                    }

                    if (GUILayout.Button(new GUIContent(obj.sprite), buttonStyle, GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        selectedObject = obj;
                    }
                }
            }

            // 房间布局显示区域
            GUILayout.Label("房间布局:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();

            // 绘制纵坐标刻度
            GUILayout.BeginVertical();
            GUILayout.Label("", GUILayout.Width(32), GUILayout.Height(32));
            for (int y = 1; y <= currentLayout.height; y++)
            {
                GUILayout.Label(y.ToString(), GUILayout.Width(32), GUILayout.Height(32));
            }
            GUILayout.EndVertical();

            // 绘制房间布局网格
            GUILayout.BeginVertical();
            for (int y = 0; y < currentLayout.height; y++)
            {
                // 绘制横坐标刻度
                if (y == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(30));
                    for (int x = 1; x <= currentLayout.width; x++)
                    {
                        GUILayout.Label(x.ToString(), GUILayout.Width(64), GUILayout.Height(32));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(30));
                for (int x = 0; x < currentLayout.width; x++)
                {
                    Texture2D texture2D = null;
                    var ans = GetCellLabel(x, y);
                    if (ans != null)
                    {
                        texture2D = roomDatas[ans.dataId].sprite;
                    }
                    if (GUILayout.Button(texture2D, GUILayout.Width(64), GUILayout.Height(32)))
                    {
                        if (selectedObject != null)
                        {
                            currentLayout.SetObjectAtPosition(x, y, selectedObject);
                            EditorUtility.SetDirty(currentLayout);
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            // 保存按钮
            if (GUILayout.Button("保存布局"))
            {
                currentLayout.SaveLayout();
                EditorUtility.SetDirty(currentLayout);
                AssetDatabase.SaveAssets();
            }
        }
    }
    // 辅助方法：创建一个纯色纹理
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    private RoomGridDef GetCellLabel(int x, int y)
    {
        RoomGridDef obj = currentLayout.GetObjectAtPosition(x, y);
        if (obj != null)
        {
            return obj;
        }
        return null;
    }

    // 添加公共方法来设置currentLayout
    public void SetCurrentLayout(RoomLayoutAsset layout)
    {
        currentLayout = layout;
        Repaint(); // 刷新窗口
    }
}