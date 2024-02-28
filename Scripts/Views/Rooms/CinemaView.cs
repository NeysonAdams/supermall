using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CinemaView : RoomView
{
    [SerializeField]
    private float put_time = 180;

    Vector3[] position = {
        new Vector3(5.32f, 0, 0.29f),
        new Vector3(5.32f, 0, -0.08f),
        new Vector3(5.32f, 0, -0.62f),
        new Vector3(5.32f, 0, -0.97f),
        new Vector3(5.32f, 0, -1.51f),
        new Vector3(5.32f, 0, -1.88f),
        new Vector3(5.32f, 0, -2.41f),
        new Vector3(5.32f, 0, -2.78f),

        new Vector3(4.6f, 0.161f, -2.7f),
        new Vector3(4.6f, 0.161f, -1.9f),
        new Vector3(4.6f, 0.161f, -1.5f),
        new Vector3(4.68f, 0.161f, -1f),
        new Vector3(4.6f, 0.161f, -0.6f),
        new Vector3(4.6f, 0.161f, -0.2f),
        new Vector3(4.6f, 0.161f, 0.2f),
        new Vector3(4.6f, 0.161f, -2.3f),

        new Vector3(3.9f, 0.282f, -2.7f),
        new Vector3(3.9f, 0.282f, -1.9f),
        new Vector3(3.9f, 0.282f, -1.5f),
        new Vector3(3.9f, 0.282f, -1f),
        new Vector3(3.9f, 0.282f, -0.6f),
        new Vector3(3.9f, 0.282f, -0.2f),
        new Vector3(3.9f, 0.282f, 0.2f),
        new Vector3(3.9f, 0.282f, -2.3f),

        new Vector3(3.156f, 0.397f, -2.7f),
        new Vector3(3.156f, 0.397f, -1.9f),
        new Vector3(3.156f, 0.397f, -1.5f),
        new Vector3(3.156f, 0.397f, -1f),
        new Vector3(3.156f, 0.397f, -0.6f),
        new Vector3(3.156f, 0.397f, -0.2f),
        new Vector3(3.156f, 0.397f, 0.2f),
        new Vector3(3.156f, 0.397f, -2.3f),

        new Vector3(2.484f, 0.524f, -2.7f),
        new Vector3(2.484f, 0.524f, -1.9f),
        new Vector3(2.484f, 0.524f, -1.5f),
        new Vector3(2.484f, 0.524f, -1f),
        new Vector3(2.484f, 0.524f, -0.6f),
        new Vector3(2.484f, 0.524f, -0.2f),
        new Vector3(2.484f, 0.524f, 0.2f),
        new Vector3(2.484f, 0.524f, -2.3f),
    };

    Vector3[] rotation = {
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),

        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),

        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),

        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
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

        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count - 1, (count) =>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;

        if (character.Place >= 0 && character.Place < 2)
            character.AnimationStatus = CharacterAnimStatus.BARSEATING;
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
