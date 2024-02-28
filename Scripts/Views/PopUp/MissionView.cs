using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MissionView : MonoBehaviour
{
    [SerializeField]
    private Sprite[] icons;

    [SerializeField]
    private Image icon_image;

    [Header("Progress")]
    [SerializeField]
    private Slider progress_slider;
    [SerializeField]
    private TextMeshProUGUI mission_counting_label;
    [Header("MissionLabel")]
    [SerializeField]
    private GameObject mission_label_container;
    [SerializeField]
    private TextMeshProUGUI mission_label;

    [Header("Timer")]
    [SerializeField]
    private GameObject timer_container;
    [SerializeField]
    private TextMeshProUGUI timer_label;

    [SerializeField]
    private Button claim_button;


    public Action claim_action;

    private MissionsModel model;

    private void Awake()
    {

        claim_button.onClick.AddListener(() => 
        {
            claim_action?.Invoke();
            if (model != null) model.pushTimer(() => {
                mission_label_container.gameObject.SetActive(false);
                icon_image.transform.parent.gameObject.SetActive(false);
                mission_label.transform.parent.gameObject.SetActive(false);
                timer_container.SetActive(true);

                claim_button.gameObject.SetActive(false);
            });
        });
    }

    public void Show()
    {
        if(model.timer!= null) model.timer.updete_timer_str += SetTimer;
    }

    public void Hide()
    {
        if (model.timer != null) model.timer.updete_timer_str -= SetTimer;
    }

    public MissionsModel Model
    {
        set
        {
            model = value;
            if (model.is_timer)
            {
                mission_label_container.gameObject.SetActive(false);
                icon_image.transform.parent.gameObject.SetActive(false);
                mission_label.transform.parent.gameObject.SetActive(false);
                timer_container.SetActive(true);

                claim_button.gameObject.SetActive(false);
            }
            else if (!value.is_compleate) 
            { 
                icon_image.sprite = icons[(int)value.type];
                progress_slider.maxValue = value.goal_count;
                progress_slider.value = value.current_count;

                mission_counting_label.text = $"{value.current_count}/{value.goal_count}";

                mission_label_container.gameObject.SetActive(true);
                icon_image.transform.parent.gameObject.SetActive(true);
                mission_label.transform.parent.gameObject.SetActive(true);
                timer_container.SetActive(false);

                claim_button.gameObject.SetActive(false);


                mission_label.text = SetMissionDiscription(value);
            } 
            else 
            {
                mission_label_container.gameObject.SetActive(false);
                icon_image.transform.parent.gameObject.SetActive(true);
                mission_label.transform.parent.gameObject.SetActive(false);
                timer_container.SetActive(false);
                claim_button.gameObject.SetActive(true);
            }
            

        }
    }

    private void SetTimer(string value)
    {
        timer_label.text = value;
    }

    private string SetMissionDiscription(MissionsModel mission)
    {
        string str = "";
        switch(mission.type)
        {
            case MissionType.LEVELUP:
                string val = (mission.room_type == RoomType.NONE) ? "любое" : mission.room_type.ToString();
                str = $"Улучшите {val} здание";
                break;
            case MissionType.CUSTOMER:
                val = (mission.room_type == RoomType.NONE) ? "любое" : mission.room_type.ToString();
                str = $"Посетили {val} здание";
                break;
            case MissionType.COLLECT_COINS:
                str = "Соберите монетки";
                break;
            case MissionType.CARS_ON_PARKING:
                str = "Машин прибыло на парковку";
                break;
        }
        return str;
    }

}
