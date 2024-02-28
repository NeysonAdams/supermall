using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GYMRoomView : RoomView
{
    [SerializeField]
    private float put_time = 360;
    Vector3[] position = {
        new Vector3(4.25f, 0.18f, 2.45f),
        new Vector3(4.25f, 0.18f, 1.58f),
        new Vector3(4.25f, 0.18f, 0.7f),
        new Vector3(4.25f, 0.18f, -0.16f),
        new Vector3(4.25f, 0.18f, -1.05f),
        new Vector3(4.25f, 0.18f, -1.91f),
        new Vector3(3f, 0.05f, 2.2f),
        new Vector3(1.71f, 0.05f, 2.2f),
        new Vector3(0.8f, 0.05f, -0.25f),
        new Vector3(-0.7f, 0.05f, -0.25f),
        new Vector3(-0.7f, 0.05f, 1.1f),
        new Vector3(0.65f, 0.05f, 1.1f),
        new Vector3(2.21f, 0.05f, 1.1f)
    };
    Vector3[] rotation = {
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 270, 0)
    };

    private Transform parent;

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            character.AnimationStatus = CharacterAnimStatus.WALK;
            Vector2Int pos = Model.EntryPosition;
            var map = LevelManager.GetMapAction(character.StayLevel);
            character.GoTo(map[positon.x, positon.y], map[pos.x, pos.y], map, () =>
            {
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
        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count, (count) =>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;
        Model.free_places[character.Place] = true;
        if(character.Place <=5)
            character.AnimationStatus = CharacterAnimStatus.RUNNING;
        else if(character.Place == 6 || character.Place == 7)
            character.AnimationStatus = CharacterAnimStatus.BICEPCURL;
        else if (character.Place ==8 || character.Place == 9)
            character.AnimationStatus = CharacterAnimStatus.KETTLESWANG;
        else if (character.Place == 10)
            character.AnimationStatus = CharacterAnimStatus.FRONTRAISES;
        else if (character.Place == 11 || character.Place == 12)
            character.AnimationStatus = CharacterAnimStatus.JUMPINGJACK;
        character.transform.localPosition = position[character.Place];
        character.transform.localRotation = Quaternion.Euler(rotation[character.Place]);
    }

    public override void Out(CharacterView character)
    {
        character.AnimationStatus = CharacterAnimStatus.IDLE;
        character.transform.parent = parent;
        if (character.Place == -1) return;
        Model.free_places[character.Place] = false;
    }
}
