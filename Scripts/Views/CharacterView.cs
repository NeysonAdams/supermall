using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public enum CharacterType
{
    AJ,
    BOY,
    MUSICGIRL,
    AJAYA,
    BOB,
    LUCY,
    CLAIRE,
    KAYA,
    GRANY,
    BOSS,
    TY
}

public enum CharacterAnimStatus
{
    IDLE,
    WALK,
    CHOUSE,
    EAT,
    MARKETIDLE,
    SPINAROUND,
    CHEERING,
    JOYFULLJUMP,
    BARSEATING,
    RUNNING,
    BICEPCURL,
    FRONTRAISES,
    JUMPINGJACK,
    KETTLESWANG,
    BOWLINGHURA,
    RELAXSEAT,
    RELAXLEY
}

public class CharacterView : MonoBehaviour
{
    private Animator animator;
    private int countigs = 0;
    private int stay_flor_level = 0;
    public int StayLevel { 
        set 
        { 
            stay_flor_level = value;
            gameObject.SetActive(stay_flor_level==UIManager.Instance.GM_MODEL.show_level);
        }
        get => stay_flor_level; 
    }

    public Action<CharacterView, TaleModel> OnCharacterGouesOut;
    GameObject avatar;
    CarView car;
    private CharacterAnimStatus animation_status = CharacterAnimStatus.IDLE;
    TaleModel pre_parking;

    [SerializeField]
    private List<GameObject> avatars;

    public TaleModel PreParking => pre_parking;
    public CarView Car => car;

    #region Getters Setters
    public CharacterAnimStatus AnimationStatus
    {
        set
        {
            animator.SetInteger("status", (int)value);
            animation_status = value;
            avatar.transform.localPosition = Vector3.zero;
            avatar.transform.localRotation = Quaternion.identity;
        }
    }

    public int Counting
    {
        get => countigs;
        set => countigs = value;
    }
    public int Place { get; set; } = 0;
    #endregion

    #region Initialization
    private void Awake()
    {
        UIManager.Instance.ShowFlor += (flor) => {
            gameObject.SetActive(stay_flor_level == flor);
            AnimationStatus = animation_status;
        };
    }

    public void Init(CharacterType _avatar)
    {
        avatar = Instantiate(avatars[(int)_avatar], transform);
        avatar.transform.localPosition = new Vector3(0, avatar.transform.localPosition.y, 0);
        animator = avatar.GetComponent<Animator>();

    }

    public void Init(CharacterType _avatar, CarView _car, TaleModel _pre_parking)
    {
        Init(_avatar);
        car = _car;
        pre_parking = _pre_parking;
    }

    #endregion

    #region Navigation
    public void GoTo(TaleModel start, TaleModel end, TaleModel[,] map, Action after_action)
    {
        List<TaleModel> path = AStar.Instance.FindPath(start, end, map, false);
        AnimationStatus = CharacterAnimStatus.WALK;
        transform.DOLocalPath(GetVector3(path), path.Count * 1.5f, PathType.Linear, PathMode.Full3D)
            .SetOptions(false)
            .SetLookAt(0.01f, Vector3.forward, Vector3.up)
            .SetEase(Ease.Linear)
            .OnComplete(() => after_action?.Invoke());
    }

    private Vector3[] GetVector3(List<TaleModel> path)
    {
        Vector3[] v_path = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            v_path[i] = new Vector3(path[i].position.x, stay_flor_level * 2.8f, path[i].position.y);
        }
        return v_path;
    }
    #endregion

    public void OnDestroy()
    {
        this.OnCharacterGouesOut = null;
    }

}
