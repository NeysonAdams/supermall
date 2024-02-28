using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum TaleType
{
    ROAD,
    GRASS,
    SIDEWALK,
    PARKING,
    MARBLE,
    FLOOR,
    ROOF,
    SPEAKER,
    CHECKER,
    NONE
}


[Serializable]
public enum GrassType
{
    NONE,
    TREE,
    BUSH

}

[Serializable]
public enum TaleObjectType
{
    NONE,
    BENCH,
    CAN,
    FLOWERBAD,
    LATERN,
    WALL,
    DOOR,
    REFREJERATOR,
    CASHIER,
    SHAFL,
    PROTECTOR,
    VEGETABLE,
    TABLE
}

[Serializable]
public enum AreaType
{
    NONE,
    FASTFOOD,
    MINISHOP,
    SERVICEROOM,
    RESTORAN,
    WHEARSHOP,
    SUPERMARKET,
    LUXARY
}

[Serializable]
public class TaleObject
{
    public TaleObjectType type = TaleObjectType.NONE;
    public Vector2 dop_position = new Vector2(0,0);
    public float rotation = 0;
    public int variants = 0;
    public TaleObject() { }
    public TaleObject(TaleObject model)
    {
        this.type = model.type;
        this.dop_position = model.dop_position;
        this.rotation = model.rotation;
        this.variants = model.variants;
    }
}

[Serializable]
public class TaleModel
{
    public TaleType type = TaleType.ROAD;
    public Vector2 position;

    public GrassType grass_type = GrassType.NONE;
    public int tree_count = 0;
    public int bush_count=0;

    public bool[] road_lines = { false, false, false, false };
    public bool zebra_h = false;
    public bool zebra_w = false;
    public bool walkable = false;
    public bool is_Respown = false;

    public TaleObject tale_object = new TaleObject();
    public List<TaleObject> tale_objects = new List<TaleObject>();

    public Vector2Int pre_parking_position;

    public RoadModel[] road_model = {
        new RoadModel(),
        new RoadModel(),
        new RoadModel()
    };

    #region For AStar
    public TaleModel Parent;
    public float DistanceToTarget;
    public float Cost;
    public float Weight;
    public List<TaleModel> Neighbors = new List<TaleModel>();
    public List<TaleModel> WalkNeighbors = new List<TaleModel>();

    public bool Walkable
    {
        get
        {
            /*if ((type == TaleType.SIDEWALK && tale_objects.Count == 0) ||
                (type == TaleType.ROAD && (zebra_h || zebra_w)) ||
                type == TaleType.MARBLE || type == TaleType.FLOOR)
                return true;*/
            if (type != TaleType.PARKING || type == TaleType.GRASS)
                return walkable;
            else if (type == TaleType.ROAD || type == TaleType.PARKING)
                return true;
            
            return false;
        }
    }
    public float FCost
    {
        get
        {
            if (DistanceToTarget != -1 && Cost != -1)
                return DistanceToTarget + Cost;
            else
                return -1;
        }
    }
    #endregion

    public TaleModel() {
        Parent = null;
        DistanceToTarget = -1;
        Cost = 1;
        Weight = 1;
        Neighbors = new List<TaleModel>();
        road_model[0] = new RoadModel();
        road_model[1] = new RoadModel();
        road_model[2] = new RoadModel();
        AStar.Instance.Recalculate += Recakculate;
    }
    public TaleModel (TaleModel model)
    {
        walkable = model.walkable;
        is_Respown = model.is_Respown;
        road_model[0] = new RoadModel();
        road_model[1] = new RoadModel();
        road_model[2] = new RoadModel();
        this.type = model.type;
        this.position = model.position;
        this.grass_type = model.grass_type;
        this.tree_count = model.tree_count;
        this.bush_count = model.bush_count;
        for(int i = 0;i<road_lines.Length;i++)
            this.road_lines[i] = model.road_lines[i];
        zebra_h = model.zebra_h;
        zebra_w = model.zebra_w;
        tale_object = new TaleObject(model.tale_object);
        for (int i = 0; i < model.tale_objects.Count; i++)
            tale_objects.Add(new TaleObject(model.tale_objects[i]));
        road_model[0] = new RoadModel(model.road_model[0]);
        road_model[1] = new RoadModel(model.road_model[1]);
        if (model.road_model.Length>2)
            road_model[2] = new RoadModel(model.road_model[2]);
        Parent = null;
        DistanceToTarget = -1;
        Cost = 1;
        Weight = 1;
        Neighbors = new List<TaleModel>();

    }

    ~TaleModel()
    {
        AStar.Instance.Recalculate -= Recakculate;
    }

    private void Recakculate()
    {
        Parent = null;
        DistanceToTarget = -1;
        Cost = 1;
        Weight = 1;
    }


}
