using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum RoadDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

[Serializable]
public enum RoadSideDirection
{
    NONE,
    ONESIDE,
    CROSSROAD,
    TRIPPLE
}

[Serializable]
public class RoadModel
{
    public RoadDirection direction;
    public RoadSideDirection side;
    public bool is_start = false;
    public bool is_end = false;
    public bool is_busy = false;

    public RoadModel()
    {

    }
    public RoadModel(RoadModel road_model)
    {
        direction = road_model.direction;
        side = road_model.side;
        is_start = road_model.is_start;
        is_end = road_model.is_end;
    }
}
