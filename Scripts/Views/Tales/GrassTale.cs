using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTale : TaleView
{
    [SerializeField]
    private GrassType grass_type;
    [SerializeField]
    private List<GameObject> trees;
    [SerializeField]
    private List<GameObject> bushes;
    [SerializeField]
    private List<GameObject> borders;

    public override void Init(TaleModel model, TaleModel[,] models)
    {
        base.Init(model, models);
        int x = (int)Model.position.x;
        int y = (int)Model.position.y;
        int x_len = models.GetLength(0);
        int y_len = models.GetLength(1);

        if (x + 1 < x_len && models[x + 1, y].type != TaleType.GRASS)
            borders[3].SetActive(true);
        if(x != 0 && models[x - 1, y].type != TaleType.GRASS)
            borders[2].SetActive(true);
        if (y + 1 < y_len && models[x, y + 1].type != TaleType.GRASS)
            borders[1].SetActive(true);
        if (y != 0 && models[x, y - 1].type != TaleType.GRASS)
            borders[0].SetActive(true);

        switch (model.grass_type)
        {
            case GrassType.TREE:
                trees[model.tree_count].SetActive(true);
                break;
            case GrassType.BUSH:
                if (model.bush_count < 4)
                    bushes[model.bush_count].SetActive(true);
                else
                {
                    for (int i = 0; i < bushes.Count; i++)
                        bushes[i].SetActive(true);
                }
                break;
        }

    }

}
