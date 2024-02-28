using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    [SerializeField]
    private List<MissionsModel> test_missions;

    private static MissionsManager instance;
    public static MissionsManager Instanse => instance;

    private List<MissionsModel> missions = new List<MissionsModel>();

    private string[] keys = { "mission_one", "mission_two", "mission_three" };

    public  MissionsModel Load(string key)
    {
        return JsonUtility.FromJson<MissionsModel>(PlayerPrefs.GetString(key));
    }

    public  void Sawe(string key, MissionsModel model)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(model));
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<MissionsModel> Init()
    {
        missions.Clear();
        for (int i = 0;i<keys.Length; i++)
        {
            if(PlayerPrefs.HasKey(keys[i]))
            {
                missions.Add(Load(keys[i]));
                missions[i].timer = new AsyncTimer(300, "timer" + keys[i]);
                if (missions[i].is_timer)
                    missions[i].pushTimer(null);
            }
            else
            {
                missions.Add(new MissionsModel(keys[i], test_missions[i]));
                Sawe(missions[i].key, missions[i]);
            }
        }
        return missions;
    }

    public void CheckMission(int value, MissionType m_type, RoomType r_type=RoomType.NONE)
    {
        for(int i =0; i<missions.Count;i++)
        {
            if(missions[i].type == m_type)
            {
                if((m_type == MissionType.LEVELUP || m_type == MissionType.CUSTOMER) &&
                    missions[i].room_type == r_type)
                {
                    missions[i].AddValue(value, () => Sawe(missions[i].key, missions[i]));
                }
            }
        }
    }
}
