using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MessageType
{
    COIN,
    EXP,
    TIMER
}

public class RoomMessageView : MonoBehaviour
{
    [Header("Message")]
    [SerializeField]
    private GameObject message_container;
    [SerializeField]
    private GameObject coin_icon;
    [SerializeField]
    private GameObject exp_icon;
    [Header("Timer")]
    [SerializeField]
    private GameObject timer_container;
    [SerializeField]
    private GameObject timer_line_icon;
    [SerializeField]
    private TextMeshPro timer_label;


    private void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 2.5f, transform.localPosition.z);
        transform.parent = transform.parent.parent;
        transform.localRotation = Quaternion.Euler(new Vector3(30, 45, 0));
    }

    public void UpdateTimerStr(string time)
    {
        timer_label.text = time;
    }

    public void UpdateTimer(float procent)
    {
        float x = -4+(4 * procent);
        timer_line_icon.transform.localPosition = new Vector3(x, timer_line_icon.transform.localPosition.y, timer_line_icon.transform.localPosition.z);
    }

    

    public void ShowMessage(MessageType type)
    {
        timer_container.SetActive(false);
        message_container.SetActive(false);
        exp_icon.SetActive(false);
        coin_icon.SetActive(false);
        switch (type)
        {
            case MessageType.COIN:
                message_container.SetActive(true);
                
                coin_icon.SetActive(true);
                break;
            case MessageType.EXP:
                message_container.SetActive(true);
                exp_icon.SetActive(true);
                break;
            case MessageType.TIMER:
                timer_container.SetActive(true);
                
                break;
        }
    }
}
