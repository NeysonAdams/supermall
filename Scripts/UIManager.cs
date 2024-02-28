using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    public Action<int> ShowFlor;
    public Action<bool> HideRoomsWalls;
    [SerializeField]
    private GamePlayModel game_play_model;
    [Header("Popups")]
    [SerializeField]
    private RoomPopupView room_popup;
    [SerializeField]
    private TaskPopupView task_popup;
    [SerializeField]
    private CouponsPopupView coupon_popup;
    [SerializeField]
    private PreloaderView preloader_popup;
    [SerializeField]
    private CouponsPopupView coupons_popup;
    [Header("Buttons")]
    [SerializeField]
    private Button task_button;
    [SerializeField]
    private Button coupons_button;
    [Header("Game Play UI")]
    [SerializeField]
    private GameUI gameui;


    public GamePlayModel GM_MODEL { get => game_play_model; set => game_play_model = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadGameData();
            SetGameData();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMissions()
    {
        task_popup.Init(MissionsManager.Instanse.Init());
        task_popup.Show();
        
    }

    public void ShowCooupons()
    {
        coupons_popup.Show();
    }

    #region Preloader Conrollers
    public int AddPreloaderValue
    {
        set => preloader_popup.AddPreloaderValue = value;
    }

    public string LoadingLabel
    {
        set => preloader_popup.LoadingLabel = value;
    }

    public int SetPreloaderValue
    {
        set => preloader_popup.SetPreloaderValue = value;
    }

    public void HidePreloader()
    {
        preloader_popup.HidePreloader();
    }

    #endregion

    #region Sawe/Load data
    private void SaweGameData()
    {
        PlayerPrefs.SetString("gamedata", JsonUtility.ToJson(game_play_model));
    }

    private void LoadGameData()
    {
        if (PlayerPrefs.HasKey("gamedata"))
            game_play_model = JsonUtility.FromJson<GamePlayModel>(PlayerPrefs.GetString("gamedata"));
    }
    

    private void SetGameData()
    {
        gameui.SetLevel(game_play_model.level);
        gameui.SetCurrntMaxExp(0, game_play_model.bound_experience);
        gameui.SetCoins(game_play_model.coins);

    }
    #endregion

    #region Game UI Controllers
    public void AddCoins(int coins)
    {
        game_play_model.coins += coins;
        gameui.AddCoins(game_play_model.coins);
        SaweGameData();
    }
    public void AdExp(int exp)
    {
        game_play_model.current_experience += exp;
        if (game_play_model.bound_experience <= game_play_model.current_experience) {

            int current_exp = game_play_model.current_experience - game_play_model.bound_experience;
            game_play_model.level++;
            gameui.SetLevel(game_play_model.level);
            game_play_model.bound_experience = Mathf.RoundToInt(game_play_model.bound_experience * 1.5f);
            SaweGameData();
            gameui.SetCurrntMaxExp(current_exp, game_play_model.bound_experience);
        }else
            gameui.SetCurrentExp(game_play_model.current_experience);
    }
    #endregion

    #region Room Controll Methods
    public void HideWalls()
    {
        ShowFlor?.Invoke(game_play_model.show_level);
    }

    public void HideShowRoomWals()
    {
        HideRoomsWalls?.Invoke(game_play_model.show_room_walls);
    }

    public void ShowRoom(RoomView room, AsyncTimer timer, AsyncTimer upgrade_timer)
    {
        room_popup.Init(room.Model, timer, upgrade_timer);
        room_popup.CheckBalance(room.Model, game_play_model.coins);
        room_popup.upgrade_button_click += () =>
        {
            room.SetUpgrateTimer();
            game_play_model.coins -= room.Model.upgrate_prise;
        };
    }
    #endregion
}
