using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideRoadTale : TaleView
{
    [SerializeField]
    private List<GameObject> borders;

    public override void Init(TaleModel model, TaleModel[,] models)
    {
        base.Init(model, models);
        int x = (int)Model.position.x;
        int y = (int)Model.position.y;
        int x_len = models.GetLength(0);
        int y_len = models.GetLength(1);

        if (x + 1 < x_len && (models[x + 1, y].type == TaleType.ROAD || models[x + 1, y].type == TaleType.PARKING))
            borders[2].SetActive(true);
        if (x != 0 && (models[x - 1, y].type == TaleType.ROAD || models[x - 1, y].type == TaleType.PARKING))
            borders[3].SetActive(true);
        if (y + 1 < y_len && (models[x, y + 1].type == TaleType.ROAD || models[x, y + 1].type == TaleType.PARKING))
            borders[0].SetActive(true);
        if (y != 0 && (models[x, y - 1].type == TaleType.ROAD || models[x, y - 1].type == TaleType.PARKING))
            borders[1].SetActive(true);

    }

    public void AddObject(GameObject obj, Transform parent)
    {
        if (obj == null)
            return;
        GameObject element = (GameObject)Instantiate(obj, parent);
        Vector2 res_position = Model.position + Model.tale_object.dop_position;

        element.transform.localPosition = new Vector3(
            res_position.x,
            element.transform.localPosition.y,
            res_position.y
            );
        Vector3 rotate = element.transform.localRotation.eulerAngles;
        element.transform.localRotation = Quaternion.Euler(new Vector3(rotate.x, rotate.y, Model.tale_object.rotation));
    }
}
