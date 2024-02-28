using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PreloaderView : MonoBehaviour
{
    [SerializeField]
    private Slider preloader;
    [SerializeField]
    private CanvasGroup group;
    [SerializeField]
    private TextMeshProUGUI loading_label;

    public int AddPreloaderValue
    {
        set => preloader.value += value;
    }

    public string LoadingLabel
    {
        set => loading_label.text = value;
    }

    public int SetPreloaderValue
    {
        set => preloader.value = value;
    }

    public void HidePreloader()
    {
        DOTween.Sequence()
            .Append( group.DOFade(0, 0.5f))
            .AppendInterval(0.5f)
            .AppendCallback(()=>gameObject.SetActive(false))
            .Play();
    }

}
