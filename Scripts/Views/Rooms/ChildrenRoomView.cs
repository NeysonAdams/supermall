using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChildrenRoomView : RoomView
{
    [SerializeField]
    private float put_time = 300;
    Vector3[] position = {
        new Vector3(0.8f, 0.05f, 1.75f),
        new Vector3(0f, 0.05f, -0.5f),
        new Vector3(2.8f, 0.05f, -1.2f),
        new Vector3(5.25f, 0.05f, -1f),
        new Vector3(0.21f, 0.05f, -1.81f),
        new Vector3(3.1f, 0.05f, 1.7f)
    };
    Vector3[] rotation = {
        new Vector3(0, 125, 0),
        new Vector3(0, 56, 0),
        new Vector3(0, 112, 0),
        new Vector3(0, -65, 0),
        new Vector3(0, -65, 0),
        new Vector3(0, -145, 0)
    };

    /*private void OnEnable()
    {
        for (int i = 0; i <= rotation.Length; i++)
            Model.free_places.Add(false);
    }*/

    public override void Come(CharacterView character, TaleModel start_tale)
    {
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            var map = LevelManager.GetMapAction(character.StayLevel);
            Vector2Int pos = Model.EntryPosition;
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
        character.Place = Randomizer.Instance.ChouseNumber(Model.free_places.Count, (count) =>
        {
            return !Model.free_places[count];
        });
        if (character.Place == -1) return;
        if (character.Place >= 0 && character.Place < 2) character.AnimationStatus = CharacterAnimStatus.CHEERING;
        if (character.Place >= 2 && character.Place < 4) character.AnimationStatus = CharacterAnimStatus.JOYFULLJUMP;
        if (character.Place >= 4) character.AnimationStatus = CharacterAnimStatus.SPINAROUND;
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
