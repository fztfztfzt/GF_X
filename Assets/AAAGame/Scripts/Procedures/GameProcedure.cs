using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine;
using System;


public class GameProcedure : ProcedureBase
{
    private GameUIForm m_GameUI;
    private LevelEntity m_Level;
    private IFsm<IProcedureManager> procedure;
    PlayerEntity playerEntity;

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
        ShowRooms();
        var miniMap = await GF.UI.OpenUIFormAwait(UIViews.MiniMap) as MiniMapForm;
        miniMap.RefreshAll();
        GF.BuiltinView.HideLoadingProgress();
    }

    private async void ShowRooms()
    {
        var curPos = GF.Floor.GetCurRoomPos();
        var roomType = GF.Floor.GetRoomType(curPos);
        //从随机池中随机一个房间
        var rooms = GF.Floor.Rooms;
        var FloorWidth = GF.Floor.FloorWidth;
        var FloorHeight = GF.Floor.FloorHeight;
        for (int i = 1; i <= FloorWidth; i++)
        {
            for (int j = 1; j <= FloorHeight; j++)
            {
                var room = rooms[i, j];
                if (room != null )
                {
                    var path = "BaseRoom";
                    var pos = new Vector2Int(i, j);
                    var roomEntity = await GF.Entity.ShowEntityAwait<RoomEntity>(path, Const.EntityGroup.Level,
                        EntityParams.Create(new Vector3(16 * i, 10 * j, 0), Vector3.zero)) as RoomEntity;
                    roomEntity.SetData(pos);
                }
            }
        }

        // 初始化游戏
        var playerParams = EntityParams.Create(new Vector3(curPos.x * 16, curPos.y *10,0), Vector3.zero);
        playerEntity = await GF.Entity.ShowEntityAwait<PlayerEntity>("player", Const.EntityGroup.Player, playerParams) as PlayerEntity;
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
                playerEntity.SetPosition(movePos_player);
                updateCamera = false;
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
