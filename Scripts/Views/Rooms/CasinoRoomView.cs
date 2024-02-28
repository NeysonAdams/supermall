using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CasinoRoomView : RoomView
{
    [SerializeField]
    private float put_time = 540; // время нахожения в комнате

    private Transform parent;
    Vector3[] position = {
        new Vector3( 0f, 0, -0.25f),
        new Vector3(-0.38f, 0, 0.6f),
        new Vector3(-1f, 0, 0.8f),
        new Vector3(-0.15f, 0, 0.7f),
        new Vector3(-0.6f, 0, -1.2f),
        new Vector3(-1f, 0, -1.84f),
        new Vector3( -1.3f, 0, -2.33f),
        new Vector3(-1.85f, 0, 2.64f),
        new Vector3(-2.46f, 0, 2.6f),
        new Vector3(-2.97f, 0, -2.2f),
        new Vector3(-1.2f, 0, 1.66f),
        new Vector3(-1.7f, 0, 2.1f)
    }; // позиция песоонажа

    Vector3[] rotation = {
        new Vector3(0, -90, 0),
        new Vector3(0, -30, 0),
        new Vector3(0, 74, 0),
        new Vector3(0, 33, 0),
        new Vector3(0, 65, 0),
        new Vector3(0, 32, 0),
        new Vector3(0, 54, 0),
        new Vector3(0, 82, 0),
        new Vector3(0, 115, 0),
        new Vector3(0, 160, 0),
        new Vector3(0, 350, 0),
        new Vector3(0, 315, 0)
    }; // позиция оси персоонажа

     public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>   //проверка этажа и нужности эскалатора
        {
            var map = LevelManager.GetMapAction(character.StayLevel); 
            Vector2Int pos = Model.EntryPosition;
            character.GoTo(map[positon.x, positon.y], map[pos.x, pos.y], map, () =>
            {
                character.AnimationStatus = CharacterAnimStatus.WALK;

                DOTween.Sequence()
                .AppendCallback(() => Put(character))
                .AppendInterval(put_time)
                .AppendCallback(() => Out(character))
                .AppendCallback(() =>
                {
                    Vector2Int pos = Model.EntryPosition;
                    character.transform.localPosition = new Vector3(pos.x, character.transform.localPosition.y, pos.y);
                    character.transform.localRotation = Quaternion.identity;
                    character.OnCharacterGouesOut(character, map[pos.x, pos.y]);
                })
                .Play();
            });
        });
    }    

    public override void Put(CharacterView character)
    {
        parent = character.transform.parent;
        character.transform.parent = transform;
        character.AnimationStatus = CharacterAnimStatus.MARKETIDLE;
        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count - 1, (count) =>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;
        Model.free_places[character.Place] = true;
        character.transform.localPosition = position[character.Place];
        character.transform.localRotation = Quaternion.Euler(rotation[character.Place]);
    }

    public override void Out(CharacterView character)
    {
        character.transform.parent = parent;
        if (character.Place == -1) return;
        Model.free_places[character.Place] = false;
        character.transform.parent = transform.parent;
    }
}


