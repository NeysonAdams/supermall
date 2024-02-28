using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer
{
    private static Randomizer instance;

    public static Randomizer Instance
    {
        get {
            if (instance == null)
                instance = new Randomizer();
            return instance;
        }
    }

    public RoomManager Room_Manager { get; set; }

    public T Pick<T>(List<T> list)
    {
        int rand_num = UnityEngine.Random.Range(0, list.Count);
        return list[rand_num];
    }
    public T PickWithContidion<T>(List<T> list, Func<T, bool> condition)
    {
        var res = Pick<T>(list);
        while (!condition(res))
            res = Pick<T>(list);

        return res;
    }

    public int ChouseNumber(int count)
    {
        return UnityEngine.Random.Range(0, count);
    }

    public int ChouseNumber(int from, int to)
    {
        return UnityEngine.Random.Range(from, to);
    }

    public int ChouseNumber(int count, Func<int, bool> condition)
    {
        int res = ChouseNumber(count);
        int c = 0;
        while (!condition(res))
        {
            res = ChouseNumber(count);
            c++;
            if (c == 20) return -1;
        }
        return res;
    }

    public RoomView ChouseRoom ()
    {
        return PickWithContidion(Room_Manager.views, (room) =>
        {
            return room.Model.type != RoomType.ATM &&
            room.Model.type != RoomType.ESCOLATOR_UP &&
            room.Model.type != RoomType.ESCOLATOR_DOWN &&
            room.Model.type != RoomType.SERVISE &&
            room.Model.type != RoomType.TABLE &&
            room.Model.type != RoomType.NONE;
        });
    }

    public RoomView ChouseRoom(RoomType type)
    {
        return PickWithContidion(Room_Manager.views, (room) =>
        {
            return room.Model.type == type;
        });
    }

    public RoomView ChouseRoom(int index)
    {
        return Room_Manager.views[index];
    }
}
