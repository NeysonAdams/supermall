using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BowlingView : RoomView
{
    [SerializeField]
    private float put_time = 280;

    Vector3[] position = {
        new Vector3(3.41f, 0, 0.29f),
        new Vector3(3.41f, 0, -0.89f),
        new Vector3(3.41f, 0, -1.98f),

        new Vector3(5.59f, 0, -0.53f),
        new Vector3(6.18f, 0, -0.53f),
        new Vector3(7.08f, 0, -0.53f),
        new Vector3(7.66f, 0, -0.53f),

        new Vector3(5.59f, 0, 0.74f),
        new Vector3(6.18f, 0, 0.74f),
        new Vector3(7.08f, 0, 0.74f),
        new Vector3(7.66f, 0, 0.74f),

        new Vector3(5.59f, 0, -0.06f),
        new Vector3(6.18f, 0, -0.06f),
        new Vector3(7.08f, 0, -0.06f),
        new Vector3(7.66f, 0, -0.06f),

        new Vector3(5.59f, 0, 1.29f),
        new Vector3(6.18f, 0, 1.29f),
        new Vector3(7.08f, 0, 1.29f),
        new Vector3(7.66f, 0, 1.29f),
    };

    Vector3[] rotation = {
        new Vector3(0, -90, 0),
        new Vector3(0, -90, 0),
        new Vector3(0, -90, 0),

        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),

        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),

        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),

        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0)
    };

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            Vector2Int pos = Model.EntryPosition;
            var map = LevelManager.GetMapAction(character.StayLevel);
            character.AnimationStatus = CharacterAnimStatus.WALK;
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

        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count - 1, (count) =>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;

        if (character.Place >= 0 && character.Place < 3)
            character.AnimationStatus = CharacterAnimStatus.BOWLINGHURA;
        else
            character.AnimationStatus = CharacterAnimStatus.EAT;

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
