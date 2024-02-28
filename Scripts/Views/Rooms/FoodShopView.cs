using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoodShopView : RoomView
{
    [SerializeField]
    private float put_time = 120;
    Vector3[] position = {
        new Vector3(1.2f, 0.05f, .35f),
        new Vector3(1.2f, 0.05f, -1.85f),
        new Vector3(1.2f, 0.05f, -2.355f)
    };
    Vector3[] rotation = {
        new Vector3(0, 180, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, 180, 0)
    };

    Vector3[] seating_position = {
        new Vector3(3.5f, 0, 1.35f),
        new Vector3(4.62f, 0, 1.35f),
        new Vector3(4.62f, 0, 0.4f),
        new Vector3(3.5f, 0, 0.4f),
        new Vector3(4.35f, 0, -1.75f),
        new Vector3(3.4f, 0, -1.75f),
        new Vector3(2.4f, 0, -1.75f),
        new Vector3(5.4f, 0, -1.75f),
        new Vector3(5.4f, 0, 0f)
    };

    Vector3[] seating_rotation = {
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 180, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0),
        new Vector3(0, 90, 0)
    };

    Transform parent;

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        Vector2Int pos = Model.EntryPosition;
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            var map = LevelManager.GetMapAction(character.StayLevel);
            character.GoTo(map[positon.x, positon.y], map[pos.x, pos.y], map, () =>
            {
                DOTween.Sequence()
                .AppendCallback(() => Put(character))
                .AppendInterval(put_time)
                .AppendCallback(() => GoPurchace(character))
                .AppendInterval(2)
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
        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count, (count) =>
        {
            return Model.free_places[count];
        });
        if (character.Place == -1) return;
        Model.free_places[character.Place] = true;
        character.transform.localPosition = seating_position[character.Place];
        character.transform.localRotation = Quaternion.Euler(seating_rotation[character.Place]);
    }

    private void GoPurchace(CharacterView character)
    {
        int p = Randomizer.Instance.ChouseNumber(position.Length - 1);
        character.transform.localPosition = position[character.Place];
        character.transform.localRotation = Quaternion.Euler(rotation[character.Place]);
    }

    public override void Out(CharacterView character)
    {
        character.transform.parent = parent;
        if (character.Place == -1) return;
        Model.free_places[character.Place] = false;

    }
}
