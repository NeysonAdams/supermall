using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharactersManager : MonoBehaviour
{
    [SerializeField]
    private CharacterView character_view;

    private List<TaleModel> respowne_position = new List<TaleModel>();
    private CharacterView test_character;

    public Action ChharacterGoesOutAction;

    public void Init(Vector3 position)
    {
        transform.localPosition = position;
    }
    public void AddRespownPosition(ref TaleModel tale)
    {
        respowne_position.Add(tale);
    }

#if UNITY_EDITOR
    public TaleModel SetRespown { set => respowne_position.Add(value); }

    public void TestPushHuman(TaleModel[,] map, Vector2Int position, int stay_level, Vector2Int finish_position, Action after_action)
    {
        if (test_character == null)
        {
            test_character = Instantiate(character_view, transform);
            CharacterType ch_type = (CharacterType)Randomizer.Instance.ChouseNumber(Enum.GetNames(typeof(CharacterType)).Length - 1);
            
            test_character.Init(ch_type);
        }
        test_character.transform.localPosition = new Vector3(position.x, 2.8f * stay_level, position.y);
        test_character.GoTo(map[position.x, position.y], map[finish_position.x, finish_position.y], map, ()=> {
            test_character.AnimationStatus = CharacterAnimStatus.IDLE;
            after_action?.Invoke();
        });
    }

    public void TestPushHuman(TaleModel[,] map, Vector2Int position, int stay_level, RoomView view, Action<CharacterView, TaleModel> after_action)
    {
        if (test_character == null)
        {
            test_character = Instantiate(character_view, transform);
            CharacterType ch_type = (CharacterType)Randomizer.Instance.ChouseNumber(Enum.GetNames(typeof(CharacterType)).Length - 1);
            test_character.Init(ch_type);
            test_character.OnCharacterGouesOut +=  after_action;
        }
        test_character.transform.localPosition = new Vector3(position.x, 2.8f * stay_level, position.y);
        Debug.Log(view.Model.type);
        view.Come(test_character, map[position.x, position.y]);
    }

    public void RemoveTestCharacter()
    {
        Destroy(test_character.gameObject);
        test_character = null;
    }
#endif

    public void PushHuman(TaleModel[,] map)
    {
        try
        {
            CharacterView character = Instantiate(character_view, transform);
            CharacterType ch_type = (CharacterType)Randomizer.Instance.ChouseNumber(Enum.GetNames(typeof(CharacterType)).Length - 2);

            TaleModel start_tale = Randomizer.Instance.Pick(respowne_position);
            character.transform.localPosition = new Vector3(start_tale.position.x, 0, start_tale.position.y);
            character.Init(ch_type);
            character.OnCharacterGouesOut += CharacterGoesOut;
            RoomView view = Randomizer.Instance.ChouseRoom();
            view.Come(character, start_tale);
        }
        catch { }
    }

    public void PushHuman(TaleModel start_tale, TaleModel[,] map)
    {
        CharacterView character = Instantiate(character_view, transform);
        CharacterType ch_type = (CharacterType)Randomizer.Instance.ChouseNumber(Enum.GetNames(typeof(CharacterType)).Length - 2);
        character.transform.localPosition = new Vector3(start_tale.position.x, 0, start_tale.position.y);
        character.Init(ch_type);
        character.OnCharacterGouesOut += CharacterGoesOut;
        RoomView view = Randomizer.Instance.ChouseRoom();
        view.Come(character, start_tale);
    }

    public void PushHuman(TaleModel[,] map, CarView _car, TaleModel parking)
    {
        CharacterView character = Instantiate(character_view, transform);
        CharacterType ch_type = (CharacterType)Randomizer.Instance.ChouseNumber(Enum.GetNames(typeof(CharacterType)).Length - 1);

        TaleModel start_tale = Randomizer.Instance.Pick(respowne_position);
        character.transform.localPosition = new Vector3(start_tale.position.x, 0, start_tale.position.y);
        character.Init(ch_type, _car, parking);
        character.OnCharacterGouesOut += CharacterGoesToCar;
        RoomView view = Randomizer.Instance.ChouseRoom();
        view.Come(character, start_tale);
    }

    private void CharacterGoesOut(CharacterView character, TaleModel start_tale)
    {
        
        character.AnimationStatus = CharacterAnimStatus.WALK;
        character.Counting = character.Counting + 1;
        if (character.Counting == 4)
        {

            Escolator(character, start_tale);
        }
        else
        {
            RoomView view = Randomizer.Instance.ChouseRoom();
            view.Come(character, start_tale);
        }
    }

    private void Escolator(CharacterView character, TaleModel start_tale, bool with_car = false)
    {
        if (character.StayLevel != 0)
        {
            Escolator view = Randomizer.Instance.ChouseRoom(RoomType.ESCOLATOR_DOWN) as Escolator;
            view.EscCome(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (position)=> 
            {
                var map = LevelManager.GetMapAction(character.StayLevel);
                Escolator(character, map[position.x, position.y], with_car);
            });
        }
        else
        {
            if (!with_car)
            {
                TaleModel end_tale = Randomizer.Instance.Pick(respowne_position);
                var map = LevelManager.GetMapAction(character.StayLevel);
                character.GoTo(start_tale, end_tale, map, () =>
                {
                    Destroy(character.gameObject);

                    ChharacterGoesOutAction?.Invoke();
                });
            }
            else
            {
                var map = LevelManager.GetMapAction(character.StayLevel);
                character.GoTo(start_tale, character.PreParking, map, () =>
                {
                    Destroy(character.gameObject);
                    character.Car.DriveToEnd(map, () => {
                        Destroy(character.Car.gameObject);
                        ChharacterGoesOutAction?.Invoke();
                    });
                });
            }
        }
    }

    private void CharacterGoesToCar(CharacterView character, TaleModel start_tale)
    {

        character.AnimationStatus = CharacterAnimStatus.WALK;
        character.Counting = character.Counting + 1;
        if (character.Counting == 4)
        {
            Escolator(character, start_tale, true);
        }
        else
        {
            RoomView view = Randomizer.Instance.ChouseRoom();
            view.Come(character, start_tale);
        }
    }


}
