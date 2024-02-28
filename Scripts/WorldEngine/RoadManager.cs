using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    [SerializeField]
    private List<CarView> cars = new List<CarView>();
    [SerializeField]
    private CharactersManager character_manager;
    private List<TaleModel> start_points = new List<TaleModel>();
    private List<TaleModel> end_points = new List<TaleModel>();
    private List<TaleModel> parkings = new List<TaleModel>();
    
    private TaleModel[,] map { get; set; }

    public TaleModel Add_start { set => start_points.Add(value); }
    public TaleModel Add_end { set => end_points.Add(value); }
    public TaleModel Add_parking { set => parkings.Add(value); }

    public void Init(Vector3 position, TaleModel[,] _map)
    {
        transform.localPosition = position;
        map = _map;
        start_points.Clear();
        end_points.Clear();
        parkings.Clear();
    }
    public void PutTheCar(bool test = false)
    {
        try
        {
            TaleModel start = Randomizer.Instance.Pick<TaleModel>(start_points);
            TaleModel parking = Randomizer.Instance.PickWithContidion<TaleModel>(parkings, (tale) =>
            {
                return !tale.road_model[0].is_busy;
            });
            TaleModel end = Randomizer.Instance.Pick<TaleModel>(end_points);
            CarView car = Instantiate(Randomizer.Instance.Pick<CarView>(cars), transform);
            car.Init(start, end, parking);
            car.DriveToParking(map, () =>
            {
                character_manager.PushHuman(map, car, map[parking.pre_parking_position.x, parking.pre_parking_position.y]);
            });
        }
        catch { }
    }
}
