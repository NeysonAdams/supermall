using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FastFoodView : RoomView
{
    public override void Come(CharacterView character, TaleModel start_tale)
    {
        character.AnimationStatus = CharacterAnimStatus.WALK;
        GoEscolator(character, start_tale, LevelManager.GetMapAction(character.StayLevel), (positon) =>
        {

            var map = LevelManager.GetMapAction(character.StayLevel);
            Vector2Int pos = Model.EntryPosition;
            character.GoTo(map[positon.x, positon.y], map[pos.x, pos.y], map, () =>
            {
                character.AnimationStatus = CharacterAnimStatus.CHOUSE;
                DOTween.Sequence()
                .AppendInterval(3)
                .AppendCallback(() =>
                {
                    RoomView table = Randomizer.Instance.ChouseRoom(RoomType.TABLE);
                    table.Come(character, map[pos.x, pos.y]);
                })
                .Play();
            });
        });
    }
}
