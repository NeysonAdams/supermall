using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerTale : TaleView
{
    public override void Init(TaleModel model, TaleModel[,] models)
    {
        base.Init(model, models);
        int x = (int)Model.position.x;
        int y = (int)Model.position.y;
        int x_len = models.GetLength(0);
        int y_len = models.GetLength(1);
    }

    public void AddObject(GameObject obj, int id, Transform parent)
    {
        if (obj == null)
            return;
        GameObject element = (GameObject)Instantiate(obj, parent);
        Vector2 res_position = Model.position + Model.tale_objects[id].dop_position;

        element.transform.localPosition = new Vector3(
            res_position.x,
            0,
            res_position.y
            );
        Vector3 rotate = element.transform.localRotation.eulerAngles;
        element.transform.localRotation = Quaternion.Euler(new Vector3(rotate.x, Model.tale_objects[id].rotation, rotate.z));
    }
}
