// RoomLayoutAssetDoubleClickListener.cs
using UnityEditor;
using UnityEngine;

public class RoomLayoutAssetDoubleClickListener
{
    [InitializeOnLoadMethod]
    static void Initialize()
    {
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
    }

    static void ProjectWindowItemOnGUI(string guid, Rect rect)
    {
        // 检查是否是双击事件
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && currentEvent.clickCount == 2)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(assetPath) && assetPath.EndsWith(".asset"))
            {
                RoomLayoutAsset layout = AssetDatabase.LoadAssetAtPath<RoomLayoutAsset>(assetPath);
                if (layout != null)
                {
                    layout.Initialize();
                    // 打开编辑器窗口并加载布局
                    RoomLayoutEditorWindow window = EditorWindow.GetWindow<RoomLayoutEditorWindow>("Room Layout Editor");
                    window.SetCurrentLayout(layout);
                }
            }
        }
    }
}