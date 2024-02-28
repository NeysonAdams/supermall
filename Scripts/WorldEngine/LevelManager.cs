using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("Mstrials")]
    [SerializeField]
    private Material[] materials = new Material[Enum.GetNames(typeof(TaleType)).Length - 1];
    [Header("RoadMarking")]
    [SerializeField]
    private Sprite[] marking_sprites;
    /*[Header("Grounds")]
    [SerializeField]
    private GameObject[] grounds = new GameObject[Enum.GetNames(typeof(TaleType)).Length];*/
    [Header("Out Objects")]
    [SerializeField]
    private GameObject[] trees;
    [SerializeField]
    private GameObject[] bushes;
    [SerializeField]
    private GameObject[] benches;
    [SerializeField]
    private GameObject[] cans;
    [SerializeField]
    private GameObject[] flower_beds;
    [SerializeField]
    private GameObject[] latterns;
    [SerializeField]
    [Header("Mall Wall Objects")]
    private GameObject[] doors;
    [SerializeField]
    private GameObject[] walls;
    [SerializeField]
    private float wall_size = 2.8f;
    [Header("Food Shop Objects")]
    [SerializeField]
    private GameObject[] refrejerators;
    [SerializeField]
    private GameObject[] shafls;
    [SerializeField]
    private GameObject[] cashier;
    [SerializeField]
    private GameObject[] vegetables;
    [SerializeField]
    private GameObject[] protector;
    [Header("Managers")]
    [SerializeField]
    private RoadManager road_manager;
    [SerializeField]
    private RoomManager room_manager;
    [SerializeField]
    private CharactersManager character_manager;
    [SerializeField]
    private Respowner respowner;
    #endregion

    #region Private Fields
    private GameObject ground_parent;
    private GameObject ground_objects_parent;
    private GameObject roof_parent;
    private List<GameObject> floor_containers = new List<GameObject>();
    private List<GameObject> floor_object_containers = new List<GameObject>();
    #endregion

    
    public LevelModel H_level_model { get; set; }

    #region Get Map
    /// <summary>
    /// получи карту этажа на котором находится персонаж
    /// </summary>
    public static Func<int, TaleModel[,]> GetMapAction;
    /// <summary>
    /// получи карту этажа для наигации персонажа. Вызывай стичесикий делегат GetMapAction 
    /// </summary>
    /// <param name="stay_level"> какого по номеру этажа нужно получить карту</param>
    /// <returns></returns>
    private TaleModel[,] GetMap(int stay_level)
    {
        if(stay_level == 0)
        {
            return H_level_model.ground_model;
        }
        else
        {
            return H_level_model.flor_model[stay_level-1];
        }
        //return null;
    }
    #endregion

    /// <summary>
    /// метод Awake  отвечает за то что проиходит при загрузке
    /// в данном случае загружаем фаил с информацией об уровне! А после начинаем строить сам уровень
    /// </summary>
    private void Awake()
    {
        LevelManager.GetMapAction += GetMap; 
        DOTween.Init(true, true);
        
        StartCoroutine(JsonStorage.LoadFile((model)=> { // Запуск Карутины
            H_level_model = model;

            var m = H_level_model.Ground; // скажем так это несколько не правильно, 
            var g = H_level_model.Flors;  // однако мы переводим с формата Листа в формат массива!
            var r = H_level_model.Roof;   // ......
            UIManager.Instance.SetPreloaderValue = 0;
            StartCoroutine(BuildRoutine());

            
        }));
        
    }
    #region Show Flor
    /// <summary>
    /// Показываеет этаж и прячет не нужный
    /// </summary>
    /// <param name="flor"> этаж который нужно показать</param>
    private void ShowFlor( int flor)
    {
        ground_parent.SetActive(false);
        ground_objects_parent.SetActive(false);
        roof_parent.SetActive(false);
        for (int i = 0; i < floor_containers.Count; i++)
            floor_containers[i].SetActive(false);
        for (int i = 0; i < floor_object_containers.Count; i++)
            floor_object_containers[i].SetActive(false);

        if (flor >= 0)
        {
            ground_parent.SetActive(true);
            ground_objects_parent.SetActive(true);
        }

        if (flor >= 1)
        {
            floor_containers[0].SetActive(true);
            floor_object_containers[0].SetActive(true);
        }

        if (flor >= 2 && floor_containers.Count >=2)
        {
            floor_containers[1].SetActive(true);
            floor_object_containers[1].SetActive(true);
        }else if(flor >= 2)
        {
            roof_parent.SetActive(true);
        }
    }
    #endregion

    #region Getters
    public int BenchesCount => benches.Length;
    public int CansCount => cans.Length;
    public int FlowersBedCount => flower_beds.Length;
    public int LatternsCount => latterns.Length;
    public int DoorsCount => doors.Length;
    public int WallsCount => walls.Length;
    public int RefrejeratorCount => refrejerators.Length;
    public int ShaflCount => shafls.Length;
    public int CashierCount => cashier.Length;
    public int ProtectorCount => protector.Length;
    public int VejetablesCount => vegetables.Length;
    public RoomManager RoomManager => room_manager;

    public bool ShowRoof
    {
        set { 
            if(roof_parent != null)
                roof_parent?.SetActive(value); 
        }
    }
    #endregion

    #region Unity Editor Region
#if UNITY_EDITOR
    public void ClearAll()
    {
        while (transform.childCount != 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        while (room_manager.transform.childCount != 0)
        {
            for (int i = 0; i < room_manager.transform.childCount; i++)
            {
                DestroyImmediate(room_manager.transform.GetChild(i).gameObject);
            }
        }
    }
#endif

    #endregion

    #region INIT

    private IEnumerator BuildRoutine()
    {
        BuildParents();
        yield return new WaitForFixedUpdate();
        UIManager.Instance.AddPreloaderValue = 10;
        UIManager.Instance.LoadingLabel = "Ground Initialize";
        InitMesh(ground_parent, ground_objects_parent, H_level_model.ground_model, H_level_model.size);
        yield return new WaitForFixedUpdate();
        UIManager.Instance.AddPreloaderValue = 10;

        UIManager.Instance.LoadingLabel = "Out Ground Initialize";
        //InitOutSize(H_level_model.size, H_level_model.out_size, H_level_model.ground_model, ground_parent);
        yield return new WaitForFixedUpdate();
        UIManager.Instance.AddPreloaderValue = 10;
        UIManager.Instance.LoadingLabel = "Levels Initialize";
        var flors = H_level_model.Flors;
        for (int i = 0; i < H_level_model.flor_count; i++)
        {
            BuildTheFlor(i, flors[i]);
            yield return new WaitForFixedUpdate();
            UIManager.Instance.AddPreloaderValue = 10;
        }
        //Init(H_level_model.size, H_level_model.Roof, roof_parent, roof_parent);
        yield return new WaitForFixedUpdate();
        UIManager.Instance.AddPreloaderValue = 10;
        UIManager.Instance.LoadingLabel = "Rooms Initialize";
        PutRooms();
        yield return new WaitForFixedUpdate();
        UIManager.Instance.AddPreloaderValue = 10;

        UIManager.Instance.LoadingLabel = "Visual settings Initialize";
        UIManager.Instance.ShowFlor += ShowFlor;
        UIManager.Instance.ShowFlor += room_manager.ShowFlor;

        UIManager.Instance.HideWalls();
        yield return new WaitForFixedUpdate();
        UIManager.Instance.AddPreloaderValue = 10;
        UIManager.Instance.LoadingLabel = "Reswon Initialize Initialize";

        respowner.Tales = H_level_model.models;
        respowner.Map = H_level_model.ground_model;
        respowner.Max_count = H_level_model.max_charCount;
        respowner.Init();

        UIManager.Instance.SetPreloaderValue = 100;
        yield return new WaitForFixedUpdate();
        UIManager.Instance.HidePreloader();
        UIManager.Instance.HideShowRoomWals();
    }

    private void BuildParents()
    {
        transform.localPosition = new Vector3(0 - H_level_model.size.x / 2, 0, 0 - H_level_model.size.y / 2);
        road_manager.Init(transform.localPosition, H_level_model.ground_model);

        #region Init Parents
        // Создайём Родительские контейнеры для каждого типа Тайлов
        ground_parent = new GameObject();
        ground_parent.transform.parent = transform;
        ground_parent.transform.localPosition = Vector3.zero;
        ground_parent.name = "ground_parent"; //контйнер для превого этажа

        ground_objects_parent = new GameObject();
        ground_objects_parent.transform.parent = transform;
        ground_objects_parent.transform.localPosition = Vector3.zero;
        ground_objects_parent.name = "ground_objects_parent"; // контейнер для объектоа дерeво скамейка

        roof_parent = new GameObject();
        roof_parent.transform.parent = transform;
        roof_parent.name = "roof_parent";
        roof_parent.transform.localPosition = new Vector3(0, (H_level_model.flor_count + 1) * wall_size, 0);
        #endregion
    }

    public void BuildTheFlor(int index, TaleModel[,] flor)
    {
        GameObject parent = new GameObject();
        parent.transform.parent = transform;
        parent.name = "level_" + index.ToString();
        parent.transform.localPosition = new Vector3(0, wall_size * (index + 1), 0);
        floor_containers.Add(parent);
        GameObject obj_parent = new GameObject();
        obj_parent.transform.parent = transform;
        obj_parent.name = "obj_level_" + index.ToString();
        obj_parent.transform.localPosition = new Vector3(0, wall_size * (index + 1), 0);
        floor_object_containers.Add(obj_parent);
        InitMesh(parent, obj_parent, flor, H_level_model.size);
    }

    public void Build()
    {
#if UNITY_EDITOR
        ClearAll();
#endif
        BuildParents();

        InitMesh(ground_parent, ground_objects_parent, H_level_model.ground_model, H_level_model.size);
        //InitOutSize(H_level_model.size, H_level_model.out_size, H_level_model.ground_model, ground_parent);
        if (H_level_model.flor_count > 0)
        {
            var flors = H_level_model.Flors;
            for (int i = 0; i < H_level_model.flor_count; i++)
            {
                BuildTheFlor(i, flors[i]);
            }
        }
        /*if(H_level_model.roof.Count > 0)
            Init(H_level_model.size, H_level_model.Roof, roof_parent, roof_parent);*/
        PutRooms();
    }

    private void PutRooms()
    {
        room_manager.Init(transform.position, H_level_model.rooms);
        character_manager.Init(transform.position);
        Randomizer.Instance.Room_Manager = room_manager;
    }

    public void TestDrive()
    {
        if(Application.isPlaying)
            road_manager.PutTheCar();
    }
/*
    public void InitOutSize(Vector2 size, Vector2Int out_size, TaleModel[,] m, GameObject parrent)
    {
        var models = m;
        if (m[0, 0] == null) return;
        for (int j = 0; j < models.GetLength(1); j++)
        {
            int pt = (int)models[0, j].type;
            for (int x = 0; x < out_size.x; x++)
            {
                GameObject element = (GameObject)Instantiate(grounds[pt], parrent.transform);
                element.transform.localPosition = new Vector3(
                    0-x,
                    element.transform.localPosition.y,
                    models[0, j].position.y);
            }
            pt = (int)models[(int)size.x-1, j].type;
            for (int x = 0; x < out_size.x; x++)
            {
                GameObject element = (GameObject)Instantiate(grounds[pt], parrent.transform);
                element.transform.localPosition = new Vector3(
                    size.x + x,
                    element.transform.localPosition.y,
                    models[(int)size.x-1, j].position.y);
            }
        }

        for (int i = 0; i < models.GetLength(0); i++)
        {
            int pt = (int)models[i, 0].type;
            for (int x = 0; x < out_size.y; x++)
            {
                GameObject element = (GameObject)Instantiate(grounds[pt], parrent.transform);
                element.transform.localPosition = new Vector3(
                    models[i, 0].position.x,
                    element.transform.localPosition.y,
                    0 - x);
            }
            pt = (int)models[i, (int)size.y-1].type;
            for (int x = 0; x < out_size.y; x++)
            {
                GameObject element = (GameObject)Instantiate(grounds[pt], parrent.transform);
                element.transform.localPosition = new Vector3(
                    models[i, (int)size.y-1].position.x,
                    element.transform.localPosition.y,
                    size.y + x);
            }
        }
    }*/

    public void InitMesh(GameObject parrent, GameObject obj_parrent, TaleModel[,] models, Vector2 size)
    {
        if (models == null) return;
        MeshBuilder builder = parrent.AddComponent<MeshBuilder>();
        builder.InitTaleObjects();
        for (int i = 0; i < models.GetLength(0); i++)
        {
            for (int j = 0; j < models.GetLength(1); j++)
            {
                if (models[i, j].type == TaleType.NONE)
                    continue;
                builder.AddVertises(models[i, j].type, models[i, j].position);

                builder.AddMarking(marking_sprites, models[i, j]);

                var to = models[i, j].tale_objects;
                for (int k = 0; k < to.Count; k++)
                {
                    builder.AddObject(AddObject(to[k]), k, obj_parrent.transform, models[i, j]);
                }
                if (models[i, j].type == TaleType.GRASS)
                {
                    builder.AddObject(AddObject(models[i, j]), -1, obj_parrent.transform, models[i, j]);
                }
                if (models[i, j].is_Respown)
                    character_manager.AddRespownPosition(ref models[i, j]);
                if (Application.isPlaying && models[i, j].walkable) GetHumanNeighbors(models[i, j], models);
                if ((models[i, j].type == TaleType.ROAD || models[i, j].type == TaleType.PARKING) && Application.isPlaying)
                    GetNeighbors(models[i, j], models);

            }
        }
        builder.Apply(materials);
        //builder.GenerateBorders();
    }

   /* public void Init(Vector2 size, TaleModel[,] m, GameObject parrent, GameObject objects_parrent)
    {
        var models = m;
        if (m[0, 0] == null) return;
        for (int i =0; i<models.GetLength(0);i++)
        {
            for (int j = 0; j < models.GetLength(1); j++)
            {
                if (models[i, j].type == TaleType.NONE)
                    continue;
                int pt = (int)models[i,j].type;
                GameObject element = (GameObject)Instantiate(grounds[pt],
                    (models[i, j].type == TaleType.ROAD || models[i, j].type == TaleType.PARKING) ?
                    ground_parent.transform :  parrent.transform);

                element.transform.localPosition = new Vector3(
                    models[i, j].position.x, 
                    element.transform.localPosition.y, 
                    models[i, j].position.y);

                if (models[i, j].type==TaleType.GRASS)
                {
                    var tale = element.GetComponent<GrassTale>();
                    tale.Init(models[i, j], models);
                }
                else if (models[i, j].type == TaleType.ROAD)
                {
                    var tale = element.GetComponent<RoadTale>();
                    tale.Init(models[i, j], models);
                    if (models[i, j].road_model[0].is_start) road_manager.Add_start = models[i, j];
                    if (models[i, j].road_model[0].is_end) road_manager.Add_end = models[i, j];
//#if UNITY_EDITOR
                    if (Application.isPlaying)
                        GetNeighbors(models[i, j], models);
//#endif
                }
                else if (models[i, j].type == TaleType.SIDEWALK)
                {
                    var tale = element.GetComponent<SideRoadTale>();
                    tale.Init(models[i, j], models);
                    tale.AddObject(AddObject(models[i, j].tale_object), objects_parrent.transform);
                    if (models[i, j].is_Respown)
                        character_manager.AddRespownPosition(ref models[i, j]);
                }
                else if (models[i, j].type == TaleType.PARKING)
                {
                    var tale = element.GetComponent<ParkingTale>();
                    tale.Init(models[i, j], models);
                    road_manager.Add_parking = models[i, j];
//#if UNITY_EDITOR
                    if (Application.isPlaying)
                        GetNeighbors(models[i, j], models);
//#endif
                }
                else if (models[i, j].type == TaleType.FLOOR)
                {
                    var tale = element.GetComponent<FloorTale>();
                    tale.Init(models[i, j], models);
                    var to = models[i, j].tale_objects;
                    for (int k = 0; k < to.Count; k++)
                    {
                        tale.AddObject(AddObject(to[k]),k, objects_parrent.transform);
                    }

                }
                else if (models[i, j].type == TaleType.CHECKER)
                {
                    var tale = element.GetComponent<CheckerTale>();
                    tale.Init(models[i, j], models);
                    var to = models[i, j].tale_objects;
                    for (int k = 0; k < to.Count; k++)
                    {
                        tale.AddObject(AddObject(to[k]), k, objects_parrent.transform);
                    }
                }
                else if (models[i, j].type == TaleType.ROOF)
                {
                    var tale = element.GetComponent<RoofTale>();
                    tale.Init(models[i, j], models);
                }
                else
                {
                    var tale = element.GetComponent<TaleView>();
                    tale.Init(models[i, j], models);
                }
//#if UNITY_EDITOR
                if(Application.isPlaying && models[i,j].walkable) GetHumanNeighbors(models[i, j], models);
//#endif
            }
        }
    }*/
    
    private GameObject AddObject(TaleModel model)
    {
        switch (model.grass_type)
        {
            case GrassType.TREE:
                return trees[model.tree_count];
                break;
            case GrassType.BUSH:
                return bushes[model.bush_count];
        }
        return null;
    }

    private GameObject AddObject(TaleObject tale_object)
    {
        switch (tale_object.type)
        {
            case TaleObjectType.BENCH:
                return benches[tale_object.variants];
                break;
            case TaleObjectType.LATERN:
                return latterns[tale_object.variants];
                break;
            case TaleObjectType.CAN:
                return cans[tale_object.variants];
                break;
            case TaleObjectType.FLOWERBAD:
                return flower_beds[tale_object.variants];
                break;
            case TaleObjectType.DOOR:
                return doors[tale_object.variants];
                break;
            case TaleObjectType.WALL:
                return walls[tale_object.variants];
                break;
            case TaleObjectType.REFREJERATOR:
                return refrejerators[tale_object.variants];
                break;
            case TaleObjectType.SHAFL:
                return shafls[tale_object.variants];
                break;
            case TaleObjectType.VEGETABLE:
                return vegetables[tale_object.variants];
                break;
            case TaleObjectType.CASHIER:
                return cashier[tale_object.variants];
                break;
            case TaleObjectType.PROTECTOR:
                return protector[tale_object.variants];
                break;
        }
        return null;
    }
    #endregion

//#if UNITY_EDITOR
    #region AStart Initialization
    private bool CheckWallOption(TaleModel node, float rotation)
    {
        if (node.type == TaleType.FLOOR)
        {
            if (node.tale_objects == null || node.tale_objects.Count == 0) return false;
            if (node.tale_objects.Count >= 1 && node.tale_objects[0].type == TaleObjectType.WALL && node.tale_objects[0].rotation == rotation) return true;
            if (node.tale_objects.Count >= 2 && node.tale_objects[1].type == TaleObjectType.WALL && node.tale_objects[1].rotation == rotation) return true;
        }
        return false;
    }
    

    public void GetHumanNeighbors(TaleModel node, TaleModel[,] obstacleMap)
    {
        node.WalkNeighbors = new List<TaleModel>();
        Action<int, int, float> set_argue = (x, y, rotation) =>
        {
            int checkX = (int)node.position.x + x;
            int checkY = (int)node.position.y + y;

            if (checkX >= 0 && checkX < H_level_model.size.x && checkY >= 0 && checkY < H_level_model.size.y)
                if (!CheckWallOption(obstacleMap[checkX, checkY], rotation))
                    node.WalkNeighbors.Add(obstacleMap[checkX, checkY]);
        };

        if (!CheckWallOption(node, 0)) set_argue.Invoke(-1, 0, 180);
        if (!CheckWallOption(node, 180)) set_argue.Invoke(1, 0, 0);
        if (!CheckWallOption(node, 270)) set_argue.Invoke(0, -1, 90);
        if (!CheckWallOption(node, 90)) set_argue.Invoke(0, 1, 270);
    }

    public void GetNeighbors(TaleModel node, TaleModel[,] obstacleMap)
    {
        //if (node.type != TaleType.ROAD || node.type != TaleType.PARKING) return;
        node.Neighbors = new List<TaleModel>();

        Action<int, int> set_argue = (x, y) =>
        {
            int checkX = (int)node.position.x + x;
            int checkY = (int)node.position.y + y;

            if (checkX >= 0 && checkX < H_level_model.size.x && checkY >= 0 && checkY < H_level_model.size.y)
                if (obstacleMap[checkX, checkY].type == TaleType.ROAD || obstacleMap[checkX, checkY].type == TaleType.PARKING)
                    node.Neighbors.Add(obstacleMap[checkX, checkY]);
        };
        
        switch (node.road_model[0].direction)
        {
            case RoadDirection.RIGHT:
                set_argue.Invoke(0, 1);
                break;
            case RoadDirection.LEFT:
                set_argue.Invoke(0, -1);
                break;
            case RoadDirection.UP:
                set_argue.Invoke(-1, 0);
                break;
            case RoadDirection.DOWN:
                set_argue.Invoke(1, 0);
                break;
        }
        if (node.road_model[0].side != RoadSideDirection.ONESIDE)
        {
            switch (node.road_model[1].direction)
            {
                case RoadDirection.RIGHT:
                    set_argue.Invoke(0, 1);
                    break;
                case RoadDirection.LEFT:
                    set_argue.Invoke(0, -1);
                    break;
                case RoadDirection.UP:
                    set_argue.Invoke(-1, 0);
                    break;
                case RoadDirection.DOWN:
                    set_argue.Invoke(1, 0);
                    break;
            }
        }

        if (node.road_model[0].side == RoadSideDirection.TRIPPLE)
        {
            switch (node.road_model[2].direction)
            {
                case RoadDirection.RIGHT:
                    set_argue.Invoke(0, 1);
                    break;
                case RoadDirection.LEFT:
                    set_argue.Invoke(0, -1);
                    break;
                case RoadDirection.UP:
                    set_argue.Invoke(-1, 0);
                    break;
                case RoadDirection.DOWN:
                    set_argue.Invoke(1, 0);
                    break;
            }
        }

    }
    #endregion
//#endif
}
