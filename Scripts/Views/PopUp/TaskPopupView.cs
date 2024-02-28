using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskPopupView : PopupView
{
    [SerializeField]
    private List<MissionView> views;
    [SerializeField]
    private Button ok_button;

    protected override void Awake()
    {
        base.Awake();
        ok_button.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Init(List<MissionsModel> models)
    {
        for (int i =0;i<models.Count;i++)
        {
            views[i].Model = models[i];
        }
    }

    public override void Show()
    {
        base.Show();
        for (int i = 0; i < views.Count; i++)
        {
            views[i].Show();
        }
    }

    public override void Hide()
    {
        base.Hide();
        for (int i = 0; i < views.Count; i++)
        {
            views[i].Hide();
        }
    }
}
