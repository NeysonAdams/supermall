using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup bg_canvas_group;
    [SerializeField]
    private CanvasGroup popup_canvas_group;
    [SerializeField]
    private Button bg_button;

    protected bool is_shown = false;

    protected virtual void Awake()
    {
        bg_button.onClick.AddListener(()=> {
            Hide();
        });
    }

    public virtual void Show()
    {
        if (is_shown) return;
        bg_canvas_group.gameObject.SetActive(true);
        popup_canvas_group.gameObject.SetActive(true);
        DOTween.Sequence()
            .Append(bg_canvas_group.DOFade(1, 0.2f))
            .Append(popup_canvas_group.DOFade(1, 0.2f))
            .AppendInterval(0.2f)
            .AppendCallback(()=> { 
                is_shown = true;
                bg_canvas_group.alpha = 1;
                popup_canvas_group.alpha = 1;
            }).Play();
    }

    public virtual void Hide()
    {
        if (!is_shown) return;
        DOTween.Sequence()
            .Append(bg_canvas_group.DOFade(0, 0.2f))
            .Append(popup_canvas_group.DOFade(0, 0.2f))
            .AppendInterval(0.2f)
            .AppendCallback(() => {
                is_shown = false;
                bg_canvas_group.alpha = 0;
                popup_canvas_group.alpha = 0;
                bg_canvas_group.gameObject.SetActive(false);
                popup_canvas_group.gameObject.SetActive(false);
            }).Play();
    }
}
