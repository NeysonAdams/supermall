using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingTale : TaleView
{
    [SerializeField]
    private GameObject parking_mark;

    public override void Init(TaleModel model, TaleModel[,] models)
    {
        base.Init(model, models);
        float rotate = 0;
        if (model.road_lines[0]) rotate = 180;
        else if(model.road_lines[1]) rotate = -90;
        else if (model.road_lines[2]) rotate = 0;
        else if (model.road_lines[3]) rotate = 90;

        Vector3 current_rotate = parking_mark.transform.localRotation.eulerAngles;
        parking_mark.transform.localRotation = Quaternion.Euler(new Vector3(current_rotate.x, rotate, current_rotate.z));

    }
}
