using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TableView : RoomView
{
    [SerializeField]
    private float put_time = 120;
    Vector3[] position = { 
        new Vector3(-0.18f, 0, 0.145f), 
        new Vector3(0.18f, 0, 0.145f), 
        new Vector3(-0.18f, 0, -0.145f), 
        new Vector3(0.18f, 0, -0.145f) 
    };
    Vector3[] rotation = { 
        new Vector3(0, 180, 0), 
        new Vector3(0, 180, 0), 
        new Vector3(0, 0, 0), 
        new Vector3(0, 0, 0) 
    };
    Transform parent;
    public override void Come(CharacterView character, TaleModel start_tale)
    {
        character.AnimationStatus = CharacterAnimStatus.WALK;
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {
            var map = LevelManager.GetMapAction(character.StayLevel);
            character.AnimationStatus = CharacterAnimStatus.WALK;
            Vector2Int pos = Model.EntryPosition;

            character.GoTo(map[positon.x, positon.y], map[pos.x, pos.y], map, () =>
            {
                character.AnimationStatus = CharacterAnimStatus.EAT;
                DOTween.Sequence()
                .AppendCallback(() => Put(character))
                .AppendInterval(put_time)
                .AppendCallback(() => Out(character))
                .AppendCallback(() =>
                {
                    Vector2Int pos = Model.EntryPosition;
                    character.transform.localPosition = new Vector3(pos.x, character.transform.localPosition.y, pos.y);
                    character.transform.localRotation = Quaternion.identity;
                    character.OnCharacterGouesOut?.Invoke(character, map[pos.x, pos.y]);
                })
                .Play();
            });
        });
    }

    public override void Put(CharacterView character)
    {
        character.AnimationStatus = CharacterAnimStatus.EAT;
        parent = character.transform.parent;
        character.transform.parent = transform;
        for (int i = 0; i< Model.free_places.Count; i++)
        {
            if (Model.free_places[i])
            {
                Model.free_places[i] = false;
                character.transform.localPosition = position[i];
                character.transform.localRotation = Quaternion.Euler(rotation[i]);
                break;
            }
        }
    }

    public override void Out(CharacterView character)
    {
        character.AnimationStatus = CharacterAnimStatus.IDLE;
        character.transform.parent = parent;
        for (int i = 0; i < Model.free_places.Count; i++)
        {
            if (!Model.free_places[i])
            {
                Model.free_places[i] = true;
                break;
            }
        }
    }
}
