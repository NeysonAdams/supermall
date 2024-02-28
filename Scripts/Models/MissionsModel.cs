using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MissionType
{
    NONE,
    LEVELUP,
    CUSTOMER,
    COLLECT_COINS,
    CARS_ON_PARKING
}

[Serializable]
public class MissionsModel
{
    public MissionType type = MissionType.NONE;
    public int current_count = 0;
    public int goal_count = 0;

    public RoomType room_type = RoomType.NONE;

    public string key;
    public bool is_compleate = false;
    public bool is_timer = false;

    public AsyncTimer timer;

    public MissionsModel(string key)
    {
        this.key = key;
        //GenerateMission();
        timer = new AsyncTimer(300, "timer"+key);
    }

    public MissionsModel (string key, MissionsModel model)
    {
        type = model.type;
        current_count = model.current_count;
        goal_count = model.goal_count;
        room_type = model.room_type;
        this.key = key;

        timer = new AsyncTimer(300, "timer" + key);
    }

    public void AddValue(int value, Action mission_update )
    {
        current_count+=value;
        if(goal_count >= current_count)
        {
            PlayerPrefs.DeleteKey(key);
            is_compleate = true;
            mission_update?.Invoke();
        }
    }

    public void pushTimer( Action mission_finished)
    {
        is_timer = true;
        timer.StartTimer(()=>
        {
            //GenerateMission();
            mission_finished?.Invoke();
            is_timer = false;
        });
    }

    public void GenerateMission()
    {
        is_compleate = false;
        this.type = (MissionType)Randomizer.Instance.ChouseNumber(1, Enum.GetValues(typeof(MissionType)).Length);
        current_count = 0;
        if (type == MissionType.COLLECT_COINS)
            goal_count = Randomizer.Instance.ChouseNumber(500, 100000);
        else
            goal_count = Randomizer.Instance.ChouseNumber(500, 100000);
        if (type==MissionType.CUSTOMER)
        {
            room_type = (RoomType)Randomizer.Instance.ChouseNumber(0, Enum.GetValues(typeof(RoomType)).Length);
        }
        if(type==MissionType.LEVELUP)
        {
            int chance = Randomizer.Instance.ChouseNumber(0, 100);
            if (chance > 50)
            {
                room_type = (RoomType)Randomizer.Instance.ChouseNumber(1, Enum.GetValues(typeof(RoomType)).Length);
            }
            else
                room_type = RoomType.NONE;
        }
    }

}
