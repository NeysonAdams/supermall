using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum RoomType
{
    NONE,
    FASTFOOD,
    CHILDRENROOM,
    GAMEROOM,
    CINEMA,
    CASINO,
    FOODSHOP,
    CAFE,
    OTHERSHOP,
    WEARSHOP,
    MINISHOP,
    SPORTSHOP,
    FURNITURESHOP,
    GYM,
    BAR,
    RELAX,
    TABLE,
    FLOWERBED,
    ATM,
    ESCOLATOR_UP,
    ESCOLATOR_DOWN,
    SERVISE
}

[Serializable]
public class RoomModel
{
    public RoomType type;
    public Vector3 position;
    public int variant;
    public int stay_level;
    public float rotation = 0;
    public List<Vector2Int> entery_position=new List<Vector2Int>();
    public List<bool> free_places = new List<bool>();

    public int level = 1;
    public int profit = 10;
    public int coin_time = 300;
    public int upgrate_timer = 300;
    public int upgrate_prise = 1000;

    public bool colapsed = false;

    public RoomModel() { }
    public RoomModel(RoomModel model)
    {
        this.type = model.type;
        this.position = model.position;
        this.variant = model.variant;
        this.stay_level = model.stay_level;
        this.rotation = model.rotation;
        entery_position = model.entery_position;
        free_places = model.free_places;
    }

    public Vector2Int EntryPosition => entery_position[Randomizer.Instance.ChouseNumber(entery_position.Count - 1)];

    public bool FreePlace {
        get
        {
            for(int i=0; i<free_places.Count;i++)
            {
                if (free_places[i]) return true;
            }
            return false;
        }
    }

}
