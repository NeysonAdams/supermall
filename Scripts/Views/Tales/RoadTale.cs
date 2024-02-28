using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadTale : TaleView
{
    [SerializeField]
    private GameObject road_marning;
    [SerializeField]
    private GameObject zebra;

    public override void Init(TaleModel model, TaleModel[,] models)
    {
        base.Init(model, models);
        if (model.zebra_h || model.zebra_w)
        {

            float rotate = model.zebra_w ? 0f : 90f;

            zebra.SetActive(model.zebra_h || model.zebra_w);
            road_marning.gameObject.SetActive(false);
            zebra.transform.localRotation = Quaternion.Euler(new Vector3(90, rotate, 0));
        }
        else
        {
            road_marning.SetActive(model.road_lines[0] || model.road_lines[1] || model.road_lines[2] || model.road_lines[3]);
            if (model.road_lines[0]) road_marning.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0)); 
            if (model.road_lines[1]) road_marning.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
            if (model.road_lines[2]) road_marning.transform.localRotation = Quaternion.Euler(new Vector3(90, 270, 0));
            if (model.road_lines[3]) road_marning.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
        }
    }
}
