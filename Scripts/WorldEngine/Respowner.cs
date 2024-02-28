using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respowner : MonoBehaviour
{
    [SerializeField]
    private RoadManager road_manager;
    [SerializeField]
    private CharactersManager character_manager;

    private int current_characters_init_count = 0;
    
    public List<TaleModel> Tales { get; set; }
    public TaleModel[,] Map { get; set; }
    public int Max_count { get; set; }

    public void Init()
    {
        int first_initialization_cout = Mathf.RoundToInt(Max_count * 0.3f);

        for(int i =0; i<first_initialization_cout; i++)
        {
            character_manager.PushHuman(Randomizer.Instance.PickWithContidion<TaleModel>(Tales, (tale) =>
            {
                return tale.walkable;
            }), Map);
            current_characters_init_count++;
        }
        StartCoroutine(Respown());
    }

    IEnumerator Respown()
    {
        while (current_characters_init_count <Max_count)
        {

            int waiting_time = Randomizer.Instance.ChouseNumber(5, 15);
            yield return new WaitForSeconds(waiting_time);
            int chanse_initializtion = Randomizer.Instance.ChouseNumber(100);
            if(chanse_initializtion<50)
            {
                character_manager.PushHuman(Map);
                
            }
            else
            {
                road_manager.PutTheCar();
            }
            current_characters_init_count++;
        }
    }

    public void CharacterGouesOut()
    {
        current_characters_init_count--;
        if (current_characters_init_count - 1 < Max_count)
            StartCoroutine(Respown());
    }
}
