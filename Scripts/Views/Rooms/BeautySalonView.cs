using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeautySalonView : RoomView
{
    [SerializeField]
    private float put_time = 240;

    Vector3[] position = {
        new Vector3(1.41f, 0, 1.26f),
        new Vector3(1.41f, 0, -0.19f),
        new Vector3(1.41f, 0, -1.73f),
        new Vector3(1.28f, 0, 2.1f),
        new Vector3(0.14f, 0, -1.76f)
    };

    Vector3[] rotation = {
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 240, 0),
        new Vector3(0, 0, 0)
    };

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            Vector2Int pos = Model.EntryPosition;
            var map = LevelManager.GetMapAction(character.StayLevel);
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

        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count - 1, (count) =>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;

        character.AnimationStatus = CharacterAnimStatus.RELAXSEAT;

        Model.free_places[character.Place] = true;
        character.transform.parent = transform;
        character.transform.localPosition = position[character.Place];
        character.transform.localRotation = Quaternion.Euler(rotation[character.Place]);
    }

    public override void Out(CharacterView character)
    {
        if (character.Place == -1) return;
        Model.free_places[character.Place] = false;
        character.transform.parent = transform.parent;
    }
}
