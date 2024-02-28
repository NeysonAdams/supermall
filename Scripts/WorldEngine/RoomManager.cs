using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField]
    private List<GameObject> fast_food;
    [SerializeField]
    private List<GameObject> children_room;
    [SerializeField]
    private List<GameObject> game_room;
    [SerializeField]
    private List<GameObject> cinema_room;
    [SerializeField]
    private List<GameObject> food_shop_room;
    [SerializeField]
    private List<GameObject> cafe_room;
    [SerializeField]
    private List<GameObject> shop_room;
    [SerializeField]
    private List<GameObject> wear_shop_room;
    [SerializeField]
    private List<GameObject> sport_shop_room;
    [SerializeField]
    private List<GameObject> furniture_shop_room;
    [SerializeField]
    private List<GameObject> bar_room;
    [SerializeField]
    private List<GameObject> minishop_room;
    [SerializeField]
    private List<GameObject> gym_room;
    [SerializeField]
    private List<GameObject> relax_room;
    [SerializeField]
    private List<GameObject> tables;
    [SerializeField]
    private List<GameObject> flower_bed;
    [SerializeField]
    private List<GameObject> atm;
    [SerializeField]
    private List<GameObject> escolator;
    [SerializeField]
    private List<GameObject> servises;
    [SerializeField]
    private List<GameObject> casino;
    #endregion

    #region Getters
    public int FastFoodCount => fast_food.Count;
    public int ChildrenRoomCount => children_room.Count;
    public int GameRoomCount => game_room.Count;
    public int CinemaRoomCount => cinema_room.Count;
    public int FoodShopCount => food_shop_room.Count;
    public int CafeRoomCount => cafe_room.Count;
    public int ShopCount => shop_room.Count;
    public int WearShopCount => wear_shop_room.Count;
    public int SportShopCount => sport_shop_room.Count;
    public int FurnitureShopCount => furniture_shop_room.Count;
    public int BarCount => bar_room.Count;
    public int MiniShopCount => minishop_room.Count;
    public int GYMRoom => gym_room.Count;
    public int RelaxRoom => relax_room.Count;
    public int TablesVariantCount => tables.Count;
    public int FlowerBedCount => flower_bed.Count;
    public int ATMCount => atm.Count;
    public int EscolatorCount => escolator.Count;
    public int ServicesCount => servises.Count;
    public int CasinoCount => casino.Count;

    #endregion

    public List<RoomView> views = new List<RoomView>();

    private GameObject ground, flor_one, flor_two;

    public void ShowFlor(int flor)
    {
        flor_one.SetActive(flor >= 1);
        flor_two.SetActive(flor >= 2);
    }

    #region Init

    public void Init(Vector3 position, List<RoomModel> models)
    {
        transform.localPosition = position;
        views.Clear();

        ground = new GameObject();
        ground.transform.parent = transform;
        ground.name = "Ground";
        flor_one = new GameObject();
        flor_one.transform.parent = transform;
        flor_one.name = "Flor_one";
        flor_two = new GameObject();
        flor_two.transform.parent = transform;
        flor_two.name = "Flor_two";

        for (int i = 0; i<models.Count; i++)
        {
            GameObject element = Choouse(models[i].type, models[i].variant, models[i]);
            if (element == null) continue;

            element.transform.localPosition = models[i].position + new Vector3(0, models[i].stay_level * 2.8f, 0);
            element.transform.localRotation = Quaternion.Euler(new Vector3(0, models[i].rotation, 0));

            switch (models[i].stay_level)
            {
                case 0:
                    element.transform.parent = ground.transform;
                    break;
                case 1:
                    element.transform.parent = flor_one.transform;
                    break;
                case 2:
                    element.transform.parent = flor_two.transform;
                    break;
            }
        }
    }

    

    private GameObject Choouse(RoomType type, int variant, RoomModel model)
    {
        if (type == RoomType.NONE) return null;
        List<GameObject> list;
        GameObject go = null;
        switch (type)
        {
            case RoomType.FASTFOOD:
                go = Instantiate(fast_food[variant], transform);
                views.Add(go.GetComponent<FastFoodView>());
                views[views.Count - 1].Model = model;
                
                break;
            case RoomType.BAR:
                go = Instantiate(bar_room[variant], transform);
                if (variant == 0) views.Add(go.GetComponent<VinoBarView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.CAFE:
                go = Instantiate(cafe_room[variant], transform);
                if (variant == 0)views.Add(go.GetComponent<CoffeShopView>());
                if (variant == 1) views.Add(go.GetComponent<CoffeShopBigView>());
                if (variant == 2) views.Add(go.GetComponent<JusyShopView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.CHILDRENROOM:
                go = Instantiate(children_room[variant], transform);
                views.Add(go.GetComponent<ChildrenRoomView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.CASINO:
                go = Instantiate(casino[variant], transform);
                views.Add(go.GetComponent<CasinoRoomView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.CINEMA:
                go = Instantiate(cinema_room[variant], transform);
                views.Add(go.GetComponent<CinemaView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.FURNITURESHOP:
                go = Instantiate(furniture_shop_room[variant], transform);
                break;
            case RoomType.FOODSHOP:
                go = Instantiate(food_shop_room[variant], transform);
                views.Add(go.GetComponent<FoodShopView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.GAMEROOM:
                go = Instantiate(game_room[variant], transform);
                views.Add(go.GetComponent<BowlingView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.GYM:
                go = Instantiate(gym_room[variant], transform);
                views.Add(go.GetComponent<GYMRoomView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.MINISHOP:
                go = Instantiate(minishop_room[variant], transform);
                break;
            case RoomType.OTHERSHOP:
                go = Instantiate(shop_room[variant], transform);
                if(variant ==0)views.Add(go.GetComponent<ToyShopView>());
                views[views.Count - 1].Model = model;
                break;
                break;
            case RoomType.RELAX:
                go = Instantiate(relax_room[variant], transform);
                if (variant == 0) views.Add(go.GetComponent<BeautySalonView>());
                if (variant == 1) views.Add(go.GetComponent<MassageView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.SPORTSHOP:
                go = Instantiate(sport_shop_room[variant], transform);
                if (variant == 0) views.Add(go.GetComponent<SportShopView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.WEARSHOP:
                go = Instantiate(wear_shop_room[variant], transform);
                if (variant == 0) views.Add(go.GetComponent<WearShopView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.TABLE:
                go = Instantiate(tables[variant], transform);
                views.Add(go.GetComponent<TableView>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.FLOWERBED:
                go = Instantiate(flower_bed[variant], transform);
                break;
            case RoomType.ATM:
                go = Instantiate(atm[variant], transform);
                break;
            case RoomType.ESCOLATOR_UP:
                go = Instantiate(escolator[variant], transform);
                views.Add(go.GetComponent<Escolator>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.ESCOLATOR_DOWN:
                go = Instantiate(escolator[variant], transform);
                views.Add(go.GetComponent<Escolator>());
                views[views.Count - 1].Model = model;
                break;
            case RoomType.SERVISE:
                go = Instantiate(servises[variant], transform);
                break;

        }

        if(type!=RoomType.NONE && type != RoomType.TABLE && type != RoomType.ATM && 
            type != RoomType.ESCOLATOR_UP && type != RoomType.ESCOLATOR_DOWN)
        {
            views[views.Count - 1].index = views.Count - 1;
            views[views.Count - 1].SetTimer();
            views[views.Count - 1].StartTimer();
        }

        return go;
    }
    #endregion
}
