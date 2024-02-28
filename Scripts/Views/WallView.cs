using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallView : MonoBehaviour
{
    [SerializeField]
    private GameObject main, little;

    private void Start()
    {
        UIManager.Instance.ShowFlor += SetHiddenWall;
    }

    public void Show()
    {
        main.SetActive(true);
        little.SetActive(false);
    }

    public void Hide()
    {
        main.SetActive(false);
        little.SetActive(true);
    }

    

    private void SetHiddenWall(int flor)
    {
        bool active = ((transform.parent.localPosition.y / 2.8f) == flor);
        main?.SetActive(!active);
        little?.SetActive(active);
    }
}
