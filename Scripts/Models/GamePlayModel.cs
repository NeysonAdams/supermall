using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GamePlayModel
{
    public int show_level = 0;
    public int coins = 0;
    public int gold = 0;
    public int level = 1;
    public int current_experience = 0;
    public int bound_experience = 10;
    public bool show_room_walls;
}
