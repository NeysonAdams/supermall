using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class CarView : MonoBehaviour
{
    private TaleModel start, end, parking;
    private float time_step = 0.25f;
    [SerializeField]
    private RoadDirection direction = RoadDirection.RIGHT;
    private List<TaleModel> back_parking_path = new List<TaleModel>();
    List<TaleModel> path_to_end = new List<TaleModel>();
    private bool reverse = false;
    public void Init(TaleModel _start, TaleModel _end, TaleModel _parking)
    {
        start = _start;
        end = _end;
        parking = _parking;
        transform.localPosition = new Vector3(start.position.x, 0, start.position.y);
        direction = start.road_model[0].direction;

        transform.localRotation= GetRotation(direction);
    }

    public void DriveToParking(TaleModel[,] map, Action after_action)
    {
        List<TaleModel> path = AStar.Instance.FindPath(start, parking, map);
        if (path == null) return;
        back_parking_path.Add(parking);
        back_parking_path.Add(path[path.Count - 2]);
        back_parking_path.Add(path[path.Count - 3]);
        path_to_end = AStar.Instance.FindPath(path[path.Count - 3], end, map);
        //StartCoroutine(Drive(path, after_action));

        transform.DOLocalPath(GetVector3(path), path.Count * 0.2f, PathType.Linear, PathMode.Full3D)
            .SetOptions(false)
            .SetLookAt(0.01f, Vector3.forward, Vector3.up)
            //.SetEase(Ease.Linear)
            .OnComplete(() => after_action?.Invoke());

    }

    private Vector3[] GetVector3(List<TaleModel> path)
    {
        Vector3[] v_path = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            v_path[i] = new Vector3(path[i].position.x, 0, path[i].position.y);
        }
        return v_path;
    }

    public void DriveToEnd(TaleModel[,] map, Action after_action)
    {
        reverse = true;
        
        transform.DOLocalPath(GetVector3(back_parking_path), back_parking_path.Count * 0.2f, PathType.Linear, PathMode.Full3D)
            .SetOptions(false)
            .SetLookAt(0.01f, Vector3.back, Vector3.up)
            .OnComplete(() => {
                transform.DOLocalPath(GetVector3(path_to_end), path_to_end.Count * 0.2f, PathType.Linear, PathMode.Full3D)
                .SetOptions(false)
                .SetLookAt(0.01f, Vector3.forward, Vector3.up)
                .SetEase(Ease.Linear)
                .OnComplete(() => after_action?.Invoke());
            });
    }

    

    private Quaternion GetRotation(RoadDirection d)
    {
        switch (d)
        {
            case RoadDirection.RIGHT:
                return Quaternion.Euler(0, -90, 0);
                break;
            case RoadDirection.LEFT:
                return Quaternion.Euler(0,-270,0);
                break;
            case RoadDirection.DOWN:
                return Quaternion.identity;
                break;
            case RoadDirection.UP:
                return Quaternion.Euler(0, -180, 0);
                break;
        }
        return Quaternion.identity;
    }
}
