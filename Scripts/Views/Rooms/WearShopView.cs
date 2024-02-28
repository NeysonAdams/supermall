using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WearShopView : RoomView
{
    [SerializeField]
    private float put_time = 240;

    private Transform parent;

    Vector3[] position = {
        new Vector3(-0.9f, 0, 1.23f),
        new Vector3(-0.9f, 0, 0.157f),
        new Vector3(-0.9f, 0, -1f),
        new Vector3(-2.25f, 0, 0.1f),
        new Vector3(-2.25f, 0, -1f),
        new Vector3(-2.25f, 0, 1.2f)
    };

    Vector3[] rotation = {
        new Vector3(0, 270, 0),
        new Vector3(0, 270, 0),
        new Vector3(0, 270, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0)
    };

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
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
