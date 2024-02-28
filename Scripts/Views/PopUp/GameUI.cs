using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coins_label;
    [SerializeField]
    private Slider experiance;
    [SerializeField]
    private TextMeshProUGUI level_label;

    [Header("Side Buttons")]
    [SerializeField]
    private Button up_level_button;
    [SerializeField]
    private Button down_level_button;
    [SerializeField]
    private Button hide_room_walls_button;

    [Header("Down Buttons")]
    [SerializeField]
    private Button mission_button;
    [SerializeField]
    private Button vachers_button;

    private void Awake()
    {
        up_level_button.onClick.AddListener(()=> 
        {
            if (UIManager.Instance.GM_MODEL.show_level < 3)
            {
                UIManager.Instance.GM_MODEL.show_level++;
                UIManager.Instance.HideWalls();
            }
        });

        down_level_button.onClick.AddListener(() =>
        {
            if (UIManager.Instance.GM_MODEL.show_level > 0)
            {
                UIManager.Instance.GM_MODEL.show_level--;
                UIManager.Instance.HideWalls();
            }
        });

        hide_room_walls_button.onClick.AddListener(()=> 
        {
            UIManager.Instance.GM_MODEL.show_room_walls = !UIManager.Instance.GM_MODEL.show_room_walls;
            UIManager.Instance.HideShowRoomWals();
        });

        mission_button.onClick.AddListener(() => 
        {
            UIManager.Instance.ShowMissions();
        });

        vachers_button.onClick.AddListener(() => 
        {
            UIManager.Instance.ShowCooupons();
        });
    }

    public void AddCoins(int coins)
    {
        DOTween.To(() => 0f, x => coins_label.text = x.ToString("F0"), coins, 1f);
    }

    public void SetCurrntMaxExp(int current, int max)
    {
        experiance.maxValue = max;
        experiance.value = current;
    }

    public void SetCurrentExp(int current)
    {
        experiance.value = current;
    }

    public void SetLevel(int level)
    {
        level_label.text = level.ToString();
    }

    public void SetCoins(int coins)
    {
        coins_label.text = coins.ToString();
    }

}
