using GameFramework.Event;
using GameFramework;
using UnityEngine;

public class SwitchRoomEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(SwitchRoomEventArgs).GetHashCode();
    public override int Id => EventId;
    public Vector2Int oldPos { get; private set; }
    public Vector2Int dir { get; private set; }
    public override void Clear()
    {

    }
    public static SwitchRoomEventArgs Create(Vector2Int pos, Vector2Int dir)
    {
        var instance = ReferencePool.Acquire<SwitchRoomEventArgs>();
        instance.oldPos = pos;
        instance.dir = dir;
        return instance;
    }
}
