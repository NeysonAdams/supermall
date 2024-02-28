#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterController : EditorWindow
{
    private LevelManager levelManager;
    private CharactersManager characterManager;
    private Vector2Int start_position = new Vector2Int(0,0);
    private int stay_level = 0;
    private bool to_point = false;
    private Vector2Int finish_position = new Vector2Int(0, 0);
    private List<string> rooms_list = new List<string>();
    private List<int> rooms_id_list = new List<int>();
    private int chousen_room = 0;

    [MenuItem("Builders/Character Controller")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CharacterController));
    }

    void OnGUI()
    {
        GUILayout.Label("Character: ", EditorStyles.boldLabel);
        levelManager = EditorGUILayout.ObjectField("Level Manager:", levelManager, typeof(LevelManager), true) as LevelManager;
        characterManager = EditorGUILayout.ObjectField("Characters Manager:", characterManager, typeof(CharactersManager), true) as CharactersManager;

        if (levelManager == null) levelManager = GameObject.Find("Level").GetComponent<LevelManager>();
        if (characterManager == null) characterManager = GameObject.Find("CharacterMaping").GetComponent<CharactersManager>();

        GUILayout.Label("Start Position: ", EditorStyles.boldLabel);
        start_position = EditorGUILayout.Vector2IntField("", start_position);
        stay_level = EditorGUILayout.IntField("Flor: ", stay_level);

        to_point = EditorGUILayout.Toggle("To Point: ", to_point);
        EditorGUILayout.BeginHorizontal();
        
        if (to_point)
        {
            finish_position = EditorGUILayout.Vector2IntField("", finish_position);
        }
        else
        {
            rooms_list.Clear();
            rooms_id_list.Clear();
            for (int i =0; i< Randomizer.Instance.Room_Manager.views.Count; i++)
            {
                var v = Randomizer.Instance.ChouseRoom(i);
                if (v.Model.type != RoomType.TABLE &&
                    v.Model.type != RoomType.FLOWERBED &&
                    v.Model.type != RoomType.ESCOLATOR_DOWN &&
                    v.Model.type != RoomType.ATM &&
                    v.Model.type != RoomType.ESCOLATOR_UP &&
                    v.Model.type != RoomType.SERVISE &&
                    v.Model.type != RoomType.NONE)
                {

                    rooms_list.Add(v.Model.type.ToString()+":"+i.ToString());
                    rooms_id_list.Add(i);
                }
            }
            //Debug.Log(rooms_list);
            chousen_room = EditorGUILayout.Popup("To Room: ", chousen_room, rooms_list.ToArray());
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Push Character"))
        {
            if (to_point)
            {
                characterManager.TestPushHuman(levelManager.H_level_model.ground_model,
                    start_position, stay_level, finish_position, () =>
                    {
                        start_position = finish_position;
                    });
            }
            else
            {
                //Debug.Log(Randomizer.Instance.Room_Manager.views.Count);
                RoomView v = Randomizer.Instance.ChouseRoom(rooms_id_list[chousen_room]);
                
                //Debug.Log(v.Model.type+ " : " + chousen_room + " : "+ rooms_id_list[chousen_room]);
                characterManager.TestPushHuman(levelManager.H_level_model.ground_model,
                    start_position, stay_level, v, (c, t) => {
                        
                        start_position = new Vector2Int((int)t.position.x, (int)t.position.y);
                    });
            }
        }
        if(GUILayout.Button("Remove Test Character"))
        {
            characterManager.RemoveTestCharacter();
        }
    }
}
#endif