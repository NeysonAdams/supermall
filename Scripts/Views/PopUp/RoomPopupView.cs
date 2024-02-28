using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class RoomPopupView : PopupView
{
    [SerializeField]
    private Button upgrade_button;
    [SerializeField]
    private Image upgrade_button_image;
    [SerializeField]
    private Button close_button;

    [SerializeField]
    private TextMeshProUGUI name_label;
    [SerializeField]
    private TextMeshProUGUI level_label;
    [SerializeField]
    private TextMeshProUGUI time_label;
    [SerializeField]
    private TextMeshProUGUI profit_label;
    [SerializeField]
    private TextMeshProUGUI upgrade_label;

    [Header("Button Sprite")]
    [SerializeField]
    private Sprite inactive_sprite_button;
    [SerializeField]
    private Sprite active_sprite_button;
    

    [Header("Build Timer")]
    [SerializeField]
    private GameObject timer_view;
    [SerializeField]
    private Slider timer_slider;
    [SerializeField]
    private TextMeshProUGUI build_timer_label;

    AsyncTimer timer, upgrade_timer;
    public Action upgrade_button_click;

    protected override void Awake()
    {
        base.Awake();
        close_button.onClick.AddListener(()=>
        {
            Hide();
        });

        upgrade_button.onClick.AddListener(() => 
        {
            upgrade_button_click?.Invoke();
            Hide();
        });
    }

    //Инициализируем  
    public void Init(RoomModel model, AsyncTimer _timer, AsyncTimer _upgrade_timer)
    {
        if (is_shown) return;
        name_label.text = model.type.ToString();
        level_label.text = "LVL. " + model.level.ToString();
        profit_label.text = $"Profit:\t\t+{model.profit*0.2f}\t\t{model.profit}";
        time_label.text = (_timer!=null) ? _timer.GetRemainingTime(): $"Time:\t\t\t00:00:00";
        upgrade_label.text = $"{model.upgrate_prise}"; 
        timer = _timer;
        timer.updete_timer_str += UpdateCoinTimer;

        // Устанавливаем вид окна когда делаем апгрейт комнате
        if (_upgrade_timer != null)
        {
            upgrade_button.gameObject.SetActive(false);
            close_button.gameObject.SetActive(false);
            upgrade_timer = _upgrade_timer;
            upgrade_timer.updete_timer_str = UpdateBuilderTimer;
            upgrade_timer.updete_int_timer = UpdateSliderTimer;
            timer_slider.maxValue = model.upgrate_timer;
            time_label.text = $"Time:\t\t\t--:--:--";
            timer_view.SetActive(true);
            timer_slider.gameObject.SetActive(true);
        }
        else
        {
            upgrade_button.gameObject.SetActive(true);
            close_button.gameObject.SetActive(true);

            timer_view.SetActive(false);
            timer_slider.gameObject.SetActive(false);
        }
        Show();
    }

    public void CheckBalance(RoomModel model, int balance)
    {
        upgrade_button.enabled = (model.upgrate_prise < balance);
        upgrade_button_image.sprite = (model.upgrate_prise < balance) ? active_sprite_button :inactive_sprite_button;
    }

    private void UpdateCoinTimer(string time)
    {
        time_label.text = $"Time:\t\t\t{time}";
    }
    private void UpdateBuilderTimer(string time)
    {
        build_timer_label.text = time;
    }

    private void UpdateSliderTimer(int time)
    {
        timer_slider.value = time;
    }

    public override void Hide()
    {
        base.Hide();
        timer.updete_timer_str = null;
        upgrade_button_click = null;
        if (upgrade_timer != null)
        {
            upgrade_timer.updete_timer_str = null;
            
        }

    }
}
