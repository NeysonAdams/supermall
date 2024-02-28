using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public enum RoomState
{
    NONE,
    COIN,
    UPGRADE,
}

public class RoomView : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private GameObject main_wall, little_wall;
    [SerializeField]
    private RoomMessageView message;
    #endregion

    #region Private fields
    private AsyncTimer coin_timer;
    private AsyncTimer upgrade_timer;
    private RoomState state = RoomState.NONE;

    private bool is_clicked = false;
    #endregion

    #region Geters/Setters
    public RoomModel Model { get; set; }
    public int index { get; set; }
    #endregion

    #region UI Controllers Methods
    private void Start()
    {
        UIManager.Instance.HideRoomsWalls += SetHiddenWall;
        UIManager.Instance.ShowFlor += SetHideWall;
    }
    private void SetHiddenWall(bool active)
    {
        if (main_wall != null) main_wall.SetActive(!active);
        if (little_wall != null) little_wall.SetActive(active);
    }

    private void SetHideWall(int flor)
    {
        gameObject.SetActive(Model.stay_level == flor);
    }
    #endregion

    #region Timers Controllers
    public void SetTimer()
    {
        coin_timer = new AsyncTimer(Model.coin_time, $"coin{index}");
        upgrade_timer = new AsyncTimer(Model.upgrate_timer, $"upgrade{index}");
    }

    public void SetUpgrateTimer()
    {
        if (message == null)
            return;

        upgrade_timer.TotalTime = Model.upgrate_timer;
        coin_timer.StopTimer();
        message.gameObject.SetActive(true);
        message.ShowMessage(MessageType.TIMER);
        message.UpdateTimerStr(upgrade_timer.GetRemainingTime());

        state = RoomState.UPGRADE;
        
        upgrade_timer.m_updete_timer_str = message.UpdateTimerStr;
        upgrade_timer.m_updete_int_timer = (time) =>
        {
            message.UpdateTimer(time / Model.upgrate_timer);
        };
        upgrade_timer.StartTimer(() => {
            
            
            Model.level++;
            Model.profit = Mathf.RoundToInt(Model.profit + Model.profit * 0.2f);
            Model.upgrate_timer = Mathf.RoundToInt(Model.upgrate_timer + Model.upgrate_timer * 0.2f);
            Model.upgrate_prise = Mathf.RoundToInt(Model.upgrate_prise + Model.upgrate_prise * 0.2f);
            StartTimer();
            upgrade_timer.m_updete_timer_str = null;
            upgrade_timer.m_updete_int_timer = null;
            upgrade_timer = null;
            state = RoomState.NONE;
        });
    }
    
    public void StartTimer()
    {
        upgrade_timer.CheckTimer(() => coin_timer.StartTimer(() =>
        {
            if (message == null)
                return;
            message.gameObject.SetActive(true);
            message.ShowMessage(MessageType.COIN);
            state = RoomState.COIN;
        }),
        ()=>SetUpgrateTimer());
        
    }
    #endregion

    #region Virtual Methods
    public virtual void Come(CharacterView character, TaleModel start_tale)
    {

    }

    public virtual void Put(CharacterView character)
    {

    }

    public virtual void Out(CharacterView character)
    {

    }
    #endregion

    #region Escolator
    public void GoEscolator(CharacterView character, TaleModel start_tale, TaleModel[,] map, Action<Vector2Int> after)
    {
        if (Model.stay_level > character.StayLevel)
        {
            Escolator view = Randomizer.Instance.ChouseRoom(RoomType.ESCOLATOR_UP)as Escolator;
            view.EscCome(character, start_tale, map, after);

        }
        else if (Model.stay_level < character.StayLevel)
        {
            Escolator view = Randomizer.Instance.ChouseRoom(RoomType.ESCOLATOR_DOWN) as Escolator;
            view.EscCome(character, start_tale, map, after);
        }
        else
        {
            after?.Invoke(new Vector2Int((int) start_tale.position.x, (int) start_tale.position.y));
        }
    }
    #endregion


    public void onClickHandler()
    {
        if (is_clicked) return;

        if (Model.type == RoomType.TABLE ||
            Model.type == RoomType.ATM ||
            Model.type == RoomType.FLOWERBED || 
            Model.type == RoomType.ESCOLATOR_UP || 
            Model.type == RoomType.ESCOLATOR_DOWN)
            return;
        is_clicked = true;
        switch (state)
        {
            case RoomState.NONE:
                UIManager.Instance.ShowRoom(this, coin_timer, null);
                break;
            case RoomState.COIN:
                
                coin_timer.TotalTime = Model.coin_time;
                StartTimer();
                UIManager.Instance.AddCoins(Model.profit);
                message.gameObject.SetActive(false);
                state = RoomState.NONE;
                //To Do: Coins
                break;
            case RoomState.UPGRADE:
                UIManager.Instance.ShowRoom(this, coin_timer, upgrade_timer);
                break;
        }

        DOTween.Sequence()
            .Append(transform.DOScale(1.05f, 0.05f))
            .AppendInterval(0.05f)
            .Append(transform.DOScale(1f, 0.05f))
            .AppendInterval(0.2f)
            .AppendCallback(()=>is_clicked = false)
            .Play();
    }

}
