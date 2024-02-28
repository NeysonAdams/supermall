using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JusyShopView : RoomView
{
    [SerializeField]
    private float put_time = 120;
    [SerializeField]
    private float chouse_time = 10;
    Vector3[] position = { 
        new Vector3(2, 0.05f, -0.25f), 
        new Vector3(2, 0.05f, 0.75f), 
        new Vector3(2, 0.05f, -0.125f), 
        new Vector3(-2.21f, 0.05f, -0.6f) };
    Vector3[] rotation = { 
        new Vector3(0, -90, 0), 
        new Vector3(0, -90, 0), 
        new Vector3(0, -90, 0), 
        new Vector3(0, 90, 0) };

    Vector3[] seating_position = {
        new Vector3(-1.8f, 0, 2.1f),
        new Vector3(-0.217f, 0, 2.1f),
        new Vector3(1.30f, 0, 2.1f),

        new Vector3(-1.8f, 0, 3.5f),
        new Vector3(-0.217f, 0, 3.5f),
        new Vector3(1.3f, 0, 3.5f),

        new Vector3(-1.1f, 0, 2.1f),
        new Vector3(-0.35f, 0, 2.1f),
        new Vector3(1.9f, 0, 2.1f),

        new Vector3(-1.3f, 0, 3.6f),
        new Vector3(0.24f, 0, 3.6f),
        new Vector3(1.9f, 0, 3.5f),

        new Vector3(-1.8f, 0, 2.75f),
        new Vector3(-0.3f, 0, 2.75f),
        new Vector3(1.3f, 0, 2.75f),

        new Vector3(-1.8f, 0, 4.1f),
        new Vector3(-0.3f, 0, 4.1f),
        new Vector3(1.3f, 0, 4.1f),

        new Vector3(-1.8f, 0, 2.75f),
        new Vector3(-0.35f, 0, 2.75f),
        new Vector3(1.2f, 0, 2.75f),

        new Vector3(-1.15f, 0, 3.5f),
        new Vector3(0.3f, 0, 3.5f),
        new Vector3(1.8f, 0, 3.5f)
    };

    Vector3[] seating_rotation = {
        new Vector3(0, 45, 0),
        new Vector3(0, 45, 0),
        new Vector3(0, 45, 0),
        new Vector3(0, 45, 0),
        new Vector3(0, 45, 0),
        new Vector3(0, 45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),

        new Vector3(0, 135, 0),
        new Vector3(0, 135, 0),
        new Vector3(0, 135, 0),
        new Vector3(0, 135, 0),
        new Vector3(0, 135, 0),
        new Vector3(0, 135, 0),

        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0),
        new Vector3(0, -45, 0)
    };

    private bool[] chousing_place = { false, false, false, false };

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            var map = LevelManager.GetMapAction(character.StayLevel);
            Vector2Int pos = Model.EntryPosition;
            character.GoTo(map[positon.x, positon.y], map[pos.x, pos.y], map, () =>
            {
                character.AnimationStatus = CharacterAnimStatus.CHOUSE;
                for (int i = 0; i < chousing_place.Length; i++)
                {
                    if (chousing_place[i])
                    {
                        chousing_place[i] = false;
                        character.transform.localPosition = transform.localPosition + position[i];
                        character.transform.localRotation = Quaternion.Euler(rotation[i]);
                        DOTween.Sequence()
                            .AppendInterval(chouse_time)
                            .AppendCallback(() => chousing_place[i] = true).Play();
                    }
                }
                DOTween.Sequence()
                .AppendInterval(chouse_time)
                .AppendCallback(() => character.AnimationStatus = CharacterAnimStatus.IDLE)
                .AppendInterval(0.1f)
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
        character.AnimationStatus = CharacterAnimStatus.EAT;
        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count-1, (count)=>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;
        Model.free_places[character.Place] = true;
        character.transform.parent = transform;
        Debug.Log(seating_rotation[character.Place]);
        character.transform.localPosition =  seating_position[character.Place];
        character.transform.localRotation = Quaternion.Euler(seating_rotation[character.Place]);
    }

    public override void Out (CharacterView character)
    {
        if (character.Place == -1) return;
        Model.free_places[character.Place] = false;
        character.transform.parent = transform.parent;

    }
}
