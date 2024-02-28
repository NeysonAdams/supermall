using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofTale: TaleView
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

        if (x + 1 < x_len && models[x + 1, y].type != TaleType.ROOF)
            borders[3].SetActive(true);
        if (x != 0 && models[x - 1, y].type != TaleType.ROOF)
            borders[2].SetActive(true);
        if (y + 1 < y_len && models[x, y + 1].type != TaleType.ROOF)
            borders[1].SetActive(true);
        if (y != 0 && models[x, y - 1].type != TaleType.ROOF)
            borders[0].SetActive(true);
    }
}
