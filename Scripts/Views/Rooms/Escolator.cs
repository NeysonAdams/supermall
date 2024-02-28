using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Escolator : RoomView
{

    Vector3[] up_path = { 
        new Vector3(0, 0, 1), 
        new Vector3(-1f, 0, 1), 
        new Vector3(-3.1f, 1.37f, 1),
        new Vector3(-4.5f, 1.37f, 1),
        new Vector3(-6.3f, 2.8f, 1),
        new Vector3(-7f, 2.8f, 1) };

    Vector3[] down_path = {
        new Vector3(-7f, 2.8f, 1),
        new Vector3(-6.3f, 2.8f, 1),
        new Vector3(-4.5f, 1.37f, 1),
        new Vector3(-3.1f, 1.37f, 1),
        new Vector3(-1f, 0, 1),
        new Vector3(0, 0, 1)
    };

    public void EscCome(CharacterView character, TaleModel start_tale, TaleModel[,] map, Action<Vector2Int> after_action)
    {
        character.AnimationStatus = CharacterAnimStatus.IDLE;
        Vector2Int pos = (Model.type == RoomType.ESCOLATOR_UP) ?Model.entery_position[0]: Model.entery_position[1];
        character.GoTo(start_tale, map[pos.x, pos.y], map, () => {
            if (Model.type == RoomType.ESCOLATOR_UP) GoUp(character, after_action, Model.entery_position[1], character.transform.parent);
            else if (Model.type == RoomType.ESCOLATOR_DOWN) GoDown(character, after_action, Model.entery_position[1], character.transform.parent);
        });
    }

    public void GoUp(CharacterView character, Action<Vector2Int> after_action, Vector2Int pos, Transform parent)
    {
        character.transform.parent = transform;
        character.AnimationStatus = CharacterAnimStatus.IDLE;
        character.transform.parent = transform;
        character.transform.DOLocalPath(up_path, 10, PathType.Linear, PathMode.Full3D)
            .SetOptions(false)
            .SetLookAt(0.01f, Vector3.forward, Vector3.forward)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                character.StayLevel += 1;
                character.transform.parent = parent;
                after_action?.Invoke(pos);
                });
    }

    public void GoDown(CharacterView character, Action<Vector2Int> after_action, Vector2Int pos, Transform parent)
    {
        character.transform.parent = transform;
        character.AnimationStatus = CharacterAnimStatus.IDLE;
        character.transform.parent = transform;
        character.transform.DOLocalPath(down_path, 10, PathType.Linear, PathMode.Ignore)
            .SetOptions(false)
            .SetLookAt(0.01f, Vector3.forward, Vector3.forward)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                character.StayLevel -= 1;
                character.transform.parent = parent;
                after_action?.Invoke(pos);
            });
    }
}
