using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine;
using System;
using static CombatUnitEntity;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;


public class GameProcedure : ProcedureBase
{
    private GameUIForm m_GameUI;
    private LevelEntity m_Level;
    private IFsm<IProcedureManager> procedure;
    PlayerEntity playerEntity;
    Dictionary<int, RoomEntity> roomEntities = new Dictionary<int, RoomEntity>(10);

    protected override async void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        this.procedure = procedureOwner;

        if (GF.Base.IsGamePaused)
        {
            GF.Base.ResumeGame();
        }

        GF.Event.Subscribe(SwitchRoomEventArgs.EventId, SwitchRoom);

        GF.UI.CloseAllLoadingUIForms();
        GF.UI.CloseAllLoadedUIForms();
        GF.Entity.HideAllLoadingEntities();
        GF.Entity.HideAllLoadedEntities();
        InitGame();
    }

    private async void InitGame()
    {

        GF.Floor.GenFloor();
        await ShowRooms();
        var miniMap = await GF.UI.OpenUIFormAwait(UIViews.MiniMap) as MiniMapForm;
        miniMap.RefreshAll();
        GF.UI.OpenUIForm(UIViews.GameUIForm);
        GF.BuiltinView.HideLoadingProgress();
    }

    private async UniTask ShowRooms()
    {
        var curPos = GF.Floor.GetCurRoomPos();
        var roomType = GF.Floor.GetRoomType(curPos);
        //从随机池中随机一个房间
        var rooms = GF.Floor.Rooms;
        var FloorWidth = GF.Floor.FloorWidth;
        var FloorHeight = GF.Floor.FloorHeight;
        foreach(var info in rooms)
        {
            var room = info.Value;
            if (room != null )
            {
                var path = "BaseRoom";
                int i = room.x;
                int j = room.y;
                var pos = new Vector2Int(i, j);
                var roomEntity = await GF.Entity.ShowEntityAwait<RoomEntity>(path, Const.EntityGroup.Level,
                    EntityParams.Create(new Vector3(16 * i, 10 * j, 0), Vector3.zero)) as RoomEntity;
                await roomEntity.SetData(room,pos);
                roomEntity.Leave();
                roomEntities.Add(GF.Floor.GetRoomId(i, j), roomEntity);
            }
        }

        // 初始化游戏
        var playerParams = EntityParams.Create(new Vector3(curPos.x * 16, curPos.y *10,0), Vector3.zero);
        playerParams.Set<VarInt32>(P_CombatFlag, (int)CombatFlag.Player);
        playerParams.Set(P_DataTableRow, Tables.Instance.TbcombatUnit.Get(1));
        playerEntity = await GF.Entity.ShowEntityAwait<PlayerEntity>("player", Const.EntityGroup.Player, playerParams) as PlayerEntity;
        GF.Floor.PlayerEntity = playerEntity;
        var curRoom = roomEntities[GF.Floor.GetRoomId(curPos.x, curPos.y)];
        if (curRoom != null)
        {
            curRoom.Enter();
        }
        //CameraController.Instance.SetFollowTarget(m_PlayerEntity.CachedTransform);
        CameraController.Instance.mainCam.transform.position = new Vector3(curPos.x * 16, curPos.y * 10, -20);
    }

    Vector3 movePos_old;
    Vector3 movePos_target;
    Vector3 movePos_player;
    float spendTime = 0.5f;
    float curTime = 1;
    bool updateCamera = false;
    void SwitchRoom(object sender, GameEventArgs e)
    {
        var args = e as SwitchRoomEventArgs;
        var curPos = GF.Floor.GetCurRoomPos();
        var oldPos = args.oldPos;
        var oldRoom = roomEntities[GF.Floor.GetRoomId(oldPos.x, oldPos.y)];
        if(oldRoom != null)
        {
            oldRoom.Leave();
        }
        var dir = args.dir;
        movePos_old = new Vector3(oldPos.x * 16, oldPos.y * 10, -20);
        movePos_target = new Vector3(curPos.x * 16, curPos.y * 10, -20);
        curTime = 0;
        updateCamera = true;
        movePos_player = new Vector3(movePos_target.x - dir.x * 5, movePos_target.y - 3*dir.y, 0);
    }

    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        if(updateCamera)
        {
            curTime += Time.deltaTime;
            var delta = Mathf.Min(1,curTime / spendTime);
            var pos = Vector3.Lerp(movePos_old, movePos_target, delta);
            CameraController.Instance.mainCam.transform.position = pos;
            if(delta == 1)
            {
                playerEntity.SetPosition (movePos_player);
                updateCamera = false;
                var curPos = GF.Floor.GetCurRoomPos();
                var curRoom = roomEntities[GF.Floor.GetRoomId(curPos.x, curPos.y)];
                if (curRoom != null)
                {
                    curRoom.Enter();
                }
            }
        }

    }
    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        if (GF.Base.IsGamePaused)
        {
            GF.Base.ResumeGame();
        }
        base.OnLeave(procedureOwner, isShutdown);
    }


    public void Restart()
    {
        ChangeState<MenuProcedure>(procedure);
    }
    public void BackHome()
    {
        ChangeState<MenuProcedure>(procedure);
    }

    private void OnGameplayEvent(object sender, GameEventArgs e)
    {
        var args = e as GameplayEventArgs;
        if(args.EventType == GameplayEventType.GameOver)
        {
            OnGameOver(args.Params.Get<VarBoolean>("IsWin"));
        }
    }
    private void OnGameOver(bool isWin)
    {
        Log.Info("Game Over, isWin:{0}", isWin);
        procedure.SetData<VarBoolean>("IsWin", isWin);
        ChangeState<GameOverProcedure>(procedure);
    }
    private void CheckGamePause()
    {
        if (m_GameUI == null) return;

        if (GF.UI.GetTopUIFormId() != m_GameUI.UIForm.SerialId)
        {
            if (!GF.Base.IsGamePaused)
            {
                GF.Base.PauseGame();
            }
        }
        else
        {
            if (GF.Base.IsGamePaused)
            {
                GF.Base.ResumeGame();
            }
        }
    }
    private void OnCloseUIForm(object sender, GameEventArgs e)
    {
        CheckGamePause();
    }

    private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
    {
        var args = e as OpenUIFormSuccessEventArgs;
        CheckGamePause();
    }
}
