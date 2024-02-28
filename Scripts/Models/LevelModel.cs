using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelModel
{
    public List<TaleModel> models = new List<TaleModel>();
    public List<TaleModel> flor = new List<TaleModel>();
    public List<TaleModel> flor_1 = new List<TaleModel>();
    public List<TaleModel> flor_2 = new List<TaleModel>();
    public List<TaleModel> roof = new List<TaleModel>();
    public List<RoomModel> rooms = new List<RoomModel>();
    public Vector2Int size;
    public Vector2Int out_size;
    public int flor_count = 0;
    public int max_charCount;

    public TaleModel[,] ground_model;
    public List<TaleModel[,]> flor_model = new List<TaleModel[,]>();
    public TaleModel[,] roof_model;
    

    public void SawePreSet()
    {
        SET(ref ground_model, ref models);
        if(roof_model != null) SET(ref roof_model, ref roof);
        for (int i =0; i< flor_count; i++)
        {
            List<TaleModel> storage = new List<TaleModel>();
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    storage.Add(flor_model[i][x, y]);
            if (i == 0) flor = storage;
            else if (i == 1) flor_1 = storage;
            else if (i == 2) flor_2 = storage;
        }
    }

    private void GET(out TaleModel[,] model, ref List<TaleModel> storage)
    {
        int i = 0;
        model = new TaleModel[(int)size.x, (int)size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                model[x, y] = storage[i];
                i++;
            }
        }
    }

    private void SET(ref TaleModel[,] model, ref List<TaleModel> storage)
    {
        if (model == null) return;
        storage.Clear();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                storage.Add(model[x, y]);
            }
        }
    }

    public void BuildTheRoof()
    {
        if (ground_model == null)
            return;

        roof_model = new TaleModel[size.x, size.y];
        int i = 0;
        roof.Clear();
        for (int x =0; x< roof_model.GetLength(0); x++)
        {
            for(int y =0; y< roof_model.GetLength(1); y++)
            {
                roof_model[x, y] = new TaleModel();
                if (ground_model[x, y].type == TaleType.MARBLE || ground_model[x, y].type == TaleType.FLOOR)
                {
                    roof_model[x, y].type = TaleType.ROOF;
                    roof_model[x, y].position = ground_model[x,y].position;
                }
                else
                {
                    roof_model[x, y].type = TaleType.NONE;
                }
                roof.Add(roof_model[x, y]);
            }
        }
    }

    public void BuildLevel()
    {
        var tm = new TaleModel[size.x, size.y];
        for (int x = 0; x < tm.GetLength(0); x++)
        {
            for (int y = 0; y < tm.GetLength(1); y++)
            {
                tm[x, y] = new TaleModel();
                if (ground_model[x, y].type == TaleType.FLOOR)
                    tm[x, y] = new TaleModel(ground_model[x, y]);
                else
                    tm[x, y].type = TaleType.NONE;
            }
        }
        flor_model.Add(tm);
    }

    public List<TaleModel[,]> Flors
    {
        get
        {
            if (flor_model.Count != 0)
                return flor_model;
            if (flor.Count == 0)
                return null;
            TaleModel[,] model;
            GET(out model, ref flor);
            flor_model.Add(model);
            if(flor_count >1)
            {
                TaleModel[,] model1;
                GET(out model1, ref flor_1);
                flor_model.Add(model1);
            }
            if (flor_count > 2)
            {
                TaleModel[,] model2;
                GET(out model2, ref flor_2);
                flor_model.Add(model2);
            }
            return flor_model;
        }
        set
        {
            flor_model.Clear();
            for (int i = 0; i < value.Count; i++)
            {
                TaleModel[,] model = value[i];
                List<TaleModel> storage = new List<TaleModel>();
                SET(ref model,  ref storage);
                if (i == 0) flor = storage;
                else if (i == 1) flor_1 = storage;
                else if (i == 2) flor_2 = storage;
            }
        }
    }

    public TaleModel[,] Ground
    {
        get
        {
            if (ground_model != null)
                return ground_model;
            GET(out ground_model, ref models);
            return ground_model;
        }
        set
        {
            //ground_model = null;
            SET(ref value, ref models);
        }
    }

    public TaleModel[,] Roof
    {
        get
        {
            if (roof_model != null)
                return roof_model;
            GET(out roof_model, ref roof);
            return roof_model;
        }
        set
        {
            //roof_model = null;
            SET(ref value, ref roof);
        }
    }
}
