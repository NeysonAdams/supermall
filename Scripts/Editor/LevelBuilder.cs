#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBuilder : EditorWindow
{
    Vector2Int field_size = new Vector2Int(0,0);

    private int pos_x = 0, pos_y = 0;
    private bool is_clickable_draw = false;
    private bool show_walls = false;
    private bool show_road_direction = true;
    private bool show_roof = true;
    private bool show_walkable = true;

    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 scroll_position_room = Vector2.zero;

    private int selected_flor = 0;
    private string[] flor_options; 

    private LevelManager levelManager;
    private LevelBuilderSetting settings;
    private TaleModel draw_model = new TaleModel();

    [MenuItem("Builders/Level Builder")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelBuilder));
    }

    void OnGUI()
    {
        GUILayout.Label("Map Field: ", EditorStyles.boldLabel);
        field_size = EditorGUILayout.Vector2IntField("Map Size:", field_size);
        levelManager = EditorGUILayout.ObjectField("Manager:", levelManager, typeof(LevelManager), true) as LevelManager;
        if (levelManager == null)
            levelManager = GameObject.Find("Level").GetComponent<LevelManager>();
        settings = EditorGUILayout.ObjectField("Settings:", settings, typeof(LevelBuilderSetting), true) as LevelBuilderSetting;

        if (settings == null) return;
        if (levelManager != null && levelManager.H_level_model != null)
            levelManager.H_level_model.out_size = EditorGUILayout.Vector2IntField("Out Size:", levelManager.H_level_model.out_size);

        if (GUILayout.Button("Build the Roof"))
        {
            levelManager.H_level_model.BuildTheRoof();
            var r = levelManager.H_level_model.roof_model;
        }
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            levelManager.H_level_model.SawePreSet();
            JsonStorage.SaveLevelData(levelManager.H_level_model, field_size);
        }

        if (GUILayout.Button("Load"))
        {
            levelManager.H_level_model = JsonStorage.LoadLevelData();
            field_size = levelManager.H_level_model.size;
            var m = levelManager.H_level_model.Ground;
            var g = levelManager.H_level_model.Flors;
            var r = levelManager.H_level_model.Roof;
            
        }
        EditorGUILayout.EndHorizontal();
        if (levelManager == null && levelManager.H_level_model == null) return;

        levelManager.H_level_model.max_charCount = EditorGUILayout.IntField("Max Characters: ", levelManager.H_level_model.max_charCount);

        int flor_count = levelManager.H_level_model.flor_count;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Flor Count: "+ flor_count, EditorStyles.boldLabel);

        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20)))
        {
            if (levelManager.H_level_model.flor_count < 3)
            {
                levelManager.H_level_model.flor_count++;
                levelManager.H_level_model.BuildLevel();
            }
            
        }
        if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
        {
            if (levelManager.H_level_model.flor_count != 0)
            {
                levelManager.H_level_model.flor_count--;
                levelManager.H_level_model.flor_model.RemoveAt(levelManager.H_level_model.flor_model.Count - 1);
            }
                
        }

        EditorGUILayout.EndHorizontal();

        flor_options = new string[flor_count + 2];
        flor_options[0] = "GROUND";
        for (int i =0; i< flor_count; i++)
        {
            flor_options[i + 1] = "LEVEL " + (i+1).ToString();
        }
        flor_options[flor_count + 1] = "ROOF";
        selected_flor = EditorGUILayout.Popup("FLORS:", selected_flor, flor_options);
        EditorGUILayout.EndVertical();

        GUILayout.Label("Map", EditorStyles.boldLabel);
        if (levelManager != null)
        {
            GUILayout.BeginHorizontal();
            if (selected_flor == 0)
            {
                DrawMap(ref levelManager.H_level_model.ground_model);
                ShowModel(ref levelManager.H_level_model.ground_model);
            }
            if (selected_flor > 0 && selected_flor < flor_count + 1)
            {
                var model = levelManager.H_level_model.flor_model[selected_flor - 1];
                DrawMap(ref model);
                ShowModel(ref model);
                levelManager.H_level_model.flor_model[selected_flor - 1] = model;
            }

            if (selected_flor >= flor_count + 1)
            {
                var model = levelManager.H_level_model.Roof;
                DrawMap(ref levelManager.H_level_model.roof_model);
                ShowModel(ref levelManager.H_level_model.roof_model, true);
            }

            GUILayout.EndHorizontal();
        }
        
    }

    private void DrawMap(ref TaleModel[,] models)
    {
        var m = new TaleModel[field_size.x, field_size.y];
        if (models != null)
        {
            int x_len = models.GetLength(0) > m.GetLength(0) ? m.GetLength(0) : models.GetLength(0);
            int y_len = models.GetLength(1) > m.GetLength(1) ? m.GetLength(1) : models.GetLength(1);

            for (int i = 0; i < x_len; i++)
                for (int j = 0; j < y_len; j++)
                    m[i, j] = models[i, j];
            models = m;
        }
        else
        {
            models = m;
        }

        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < field_size.x; i++)
        {
            
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < field_size.y; j++)
            {
                if (models[i, j] == null)
                {
                    models[i, j] = new TaleModel();
                    models[i, j].type = TaleType.ROAD;
                    models[i, j].position = new Vector2(i, j);
                }

                Color pcolor = GUI.backgroundColor;
                int color_num = (int)models[i, j].type;
                if (color_num >= settings.ground_colors.Length)
                    GUI.backgroundColor = Color.gray;
                else
                    GUI.backgroundColor = settings.ground_colors[color_num];
                string button_name = GetTile(models[i, j]);
                GUIStyle bs = ButtonStyle(models[i, j]);
                if (GUILayout.Button(button_name, bs, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    pos_x = i;
                    pos_y = j;
                    if (is_clickable_draw)
                    {
                        draw_model.position = new Vector2(i, j);
                        models[pos_x, pos_y] = new TaleModel(draw_model);

                    }
                }
                GUI.backgroundColor = pcolor;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        
    }

    private GUIStyle ButtonStyle(TaleModel model)
    {
        GUIStyle button_style = new GUIStyle(GUI.skin.button);
        if (model.type == TaleType.ROAD)
        {

            for (int i = 0; i < model.road_lines.Length; i++)
                if (model.road_lines[i])
                    button_style.normal.background = settings.road_marking_textures[i];
            if (model.zebra_h)
                button_style.normal.background = settings.road_marking_textures[4];
            if (model.zebra_w)
                button_style.normal.background = settings.road_marking_textures[5];

        }
        if (model.type == TaleType.PARKING)
        {
            for (int i = 0; i < model.road_lines.Length; i++)
                if (model.road_lines[i])
                    button_style.normal.background = settings.parking_marking_textures[i];
        }
        return button_style;
    }

    private Texture2D GetButtonTexture(TaleType type, Texture2D tex)
    {
        Color[] colors = tex.GetPixels();
        for (int x = 0; x < colors.Length; x++)
        {
            if (colors[x] != Color.white)
            {
                colors[x] = settings.ground_colors[(int)type];
            }
        }
        Texture2D tes = new Texture2D(20, 20);
        tes.SetPixels(colors);
        tes.Apply();
        return tes;
    }

    private string GetTile(TaleModel model)
    {
        string tale = "";
        switch (model.type)
        {
            case TaleType.GRASS:
                if (model.grass_type == GrassType.TREE)
                    tale = "T";
                if (model.grass_type == GrassType.BUSH)
                    tale = "B";
                break;
            case TaleType.SIDEWALK:
                if (model.tale_object.type == TaleObjectType.BENCH)
                    tale = "b";
                if (model.tale_object.type == TaleObjectType.CAN)
                    tale = "c";
                if (model.tale_object.type == TaleObjectType.FLOWERBAD)
                    tale = "f";
                if (model.tale_object.type == TaleObjectType.LATERN)
                    tale = "l";
                if (model.is_Respown)
                    tale = "s";
                
                break;
            case TaleType.FLOOR:
            case TaleType.CHECKER:

                

                if ( model.tale_objects.Count != 0)
                {
                    if (show_walls && model.tale_objects[0].type == TaleObjectType.WALL)
                        tale = "W";
                    if (show_walls && model.tale_objects[0].type == TaleObjectType.DOOR)
                        tale = "D";

                    if (model.tale_objects[0].type == TaleObjectType.SHAFL)
                        tale = "S";
                    if (model.tale_objects[0].type == TaleObjectType.REFREJERATOR)
                        tale = "R";
                    if (model.tale_objects[0].type == TaleObjectType.PROTECTOR)
                        tale = "P";
                    if (model.tale_objects[0].type == TaleObjectType.VEGETABLE)
                        tale = "V";
                    if (model.tale_objects[0].type == TaleObjectType.CASHIER)
                        tale = "C";
                }


                break;
        }

        #region Road Model
        if (show_road_direction)
        {
            if (model.type == TaleType.ROAD || model.type == TaleType.PARKING)
            {
                if (model.road_model[0].side != RoadSideDirection.NONE)
                {
                    if (model.road_model[0].side == RoadSideDirection.ONESIDE)
                    {
                        switch (model.road_model[0].direction)
                        {
                            case RoadDirection.UP:
                                tale = "↑";
                                break;
                            case RoadDirection.DOWN:
                                tale = "↓";
                                break;
                            case RoadDirection.LEFT:
                                tale = "←";
                                break;
                            case RoadDirection.RIGHT:
                                tale = "→";
                                break;
                        }
                    }
                    else if(model.road_model[0].side == RoadSideDirection.CROSSROAD)
                    {
                        if((model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.RIGHT))
                        {
                            tale = "⇘";
                        }
                        else if ((model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.UP) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.RIGHT)
                            )
                        {
                            tale = "⇗";
                        }
                        else if ((model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.DOWN)||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.LEFT))
                        {
                            tale = "⇙";
                        }
                        else if ((model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.UP) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.LEFT))
                        {
                            tale = "⇖";
                        }
                    }
                    else if (model.road_model[0].side == RoadSideDirection.TRIPPLE)
                    {
                        if ((model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.LEFT && model.road_model[2].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.RIGHT && model.road_model[2].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.RIGHT && model.road_model[2].direction == RoadDirection.LEFT) ||
                            (model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.DOWN && model.road_model[2].direction == RoadDirection.LEFT) ||
                            (model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.DOWN && model.road_model[2].direction == RoadDirection.RIGHT) ||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.LEFT && model.road_model[2].direction == RoadDirection.RIGHT))
                        {
                            tale = "⤩";
                        }
                        else if ((model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.UP && model.road_model[2].direction == RoadDirection.LEFT) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.RIGHT && model.road_model[2].direction == RoadDirection.LEFT) ||
                            (model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.UP && model.road_model[2].direction == RoadDirection.RIGHT) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.LEFT && model.road_model[2].direction == RoadDirection.RIGHT) ||
                            (model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.LEFT && model.road_model[2].direction == RoadDirection.UP) ||
                            (model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.RIGHT && model.road_model[2].direction == RoadDirection.UP)
                            )
                            
                        {
                            tale = "⤧";
                        }
                        else if ((model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.UP && model.road_model[2].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.RIGHT && model.road_model[2].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.RIGHT && model.road_model[2].direction == RoadDirection.UP) ||
                            (model.road_model[0].direction == RoadDirection.RIGHT && model.road_model[1].direction == RoadDirection.DOWN && model.road_model[2].direction == RoadDirection.UP) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.DOWN && model.road_model[2].direction == RoadDirection.RIGHT) ||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.UP && model.road_model[2].direction == RoadDirection.RIGHT))
                        {
                            tale = "⇛";
                        }
                        else if ((model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.UP && model.road_model[2].direction == RoadDirection.LEFT) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.DOWN && model.road_model[2].direction == RoadDirection.LEFT) ||
                            (model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.UP && model.road_model[2].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.UP && model.road_model[1].direction == RoadDirection.LEFT && model.road_model[2].direction == RoadDirection.DOWN) ||
                            (model.road_model[0].direction == RoadDirection.LEFT && model.road_model[1].direction == RoadDirection.DOWN && model.road_model[2].direction == RoadDirection.UP) ||
                            (model.road_model[0].direction == RoadDirection.DOWN && model.road_model[1].direction == RoadDirection.LEFT && model.road_model[2].direction == RoadDirection.UP)
                            )

                        {
                            tale = "⇚";
                        }

                    }
                }
            }
        }
        else
        {
            if (model.type == TaleType.ROAD)
            {
                if (model.road_model[0].is_start) tale = "S";
                if (model.road_model[0].is_end) tale = "E";
            }
        }
        #endregion

        if (show_walkable)
        {
            if (model.walkable)
                tale = "★";
            if (model.is_Respown)
                tale = "®";
        }
        return tale;
    }

    private void SetTaleObject(ref TaleObject tale_object)
    {
        tale_object.type = (TaleObjectType)EditorGUILayout.EnumPopup("Object:", tale_object.type);
        if (tale_object.type != TaleObjectType.NONE)
        {
            tale_object.dop_position.x = EditorGUILayout.Slider("W:", tale_object.dop_position.x, -0.5f, 0.5f);
            tale_object.dop_position.y = EditorGUILayout.Slider("H:", tale_object.dop_position.y, -0.5f, 0.5f);
            tale_object.rotation = EditorGUILayout.Slider("Rotate:", tale_object.rotation, 0, 360);
            switch (tale_object.type)
            {
                case TaleObjectType.BENCH:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.BenchesCount - 1);
                    break;
                case TaleObjectType.CAN:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.CansCount - 1);
                    break;
                case TaleObjectType.FLOWERBAD:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.FlowersBedCount - 1);
                    break;
                case TaleObjectType.LATERN:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.LatternsCount - 1);
                    break;
                case TaleObjectType.DOOR:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.DoorsCount - 1);
                    break;
                case TaleObjectType.WALL:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.WallsCount - 1);
                    break;
                case TaleObjectType.REFREJERATOR:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.RefrejeratorCount - 1);
                    break;
                case TaleObjectType.SHAFL:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.ShaflCount - 1);
                    break;
                case TaleObjectType.VEGETABLE:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.VejetablesCount - 1);
                    break;
                case TaleObjectType.PROTECTOR:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.ProtectorCount - 1);
                    break;
                case TaleObjectType.CASHIER:
                    tale_object.variants = EditorGUILayout.IntSlider("V:",
                        tale_object.variants, 0, levelManager.CashierCount - 1);
                    break;
            }
        }
    }

    private void SetModel(ref TaleModel current_model)
    {
        GUILayout.BeginHorizontal();
        show_walkable = EditorGUILayout.Toggle("Show Walk", show_walkable);
        current_model.walkable = EditorGUILayout.Toggle("Walkable: ", current_model.walkable);
        GUILayout.EndHorizontal();
        current_model.type = (TaleType)EditorGUILayout.EnumPopup("Type", current_model.type);
        if (current_model.type == TaleType.GRASS)
        {
            current_model.grass_type = (GrassType)EditorGUILayout.EnumPopup("Grass", current_model.grass_type);
            if (current_model.grass_type == GrassType.TREE)
            {
                current_model.tree_count = EditorGUILayout.IntSlider(current_model.tree_count, 0, 4);
            }
            if (current_model.grass_type == GrassType.BUSH)
            {
                current_model.bush_count = EditorGUILayout.IntSlider(current_model.bush_count, 0, 3);
            }
        }
        if (current_model.type == TaleType.ROAD)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("lines", EditorStyles.boldLabel);
            for (int i = 0; i < current_model.road_lines.Length; i++)
                current_model.road_lines[i] = EditorGUILayout.Toggle(current_model.road_lines[i]);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Zebra", EditorStyles.boldLabel);
            current_model.zebra_h = EditorGUILayout.Toggle("Horizontal",current_model.zebra_h);
            current_model.zebra_w = EditorGUILayout.Toggle("Vertical", current_model.zebra_w);
            if (current_model.zebra_h || current_model.zebra_w)
                for (int i = 0; i < current_model.road_lines.Length; i++) current_model.road_lines[i] = false;
            EditorGUILayout.EndVertical();
        }
        if (current_model.type == TaleType.SIDEWALK)
        {
            current_model.is_Respown = EditorGUILayout.Toggle("Respown:", current_model.is_Respown);
            SetTaleObject(ref current_model.tale_object);
        }
        if(current_model.type == TaleType.PARKING)
        {
            EditorGUILayout.BeginHorizontal();
            current_model.road_lines[0] = EditorGUILayout.Toggle("0", current_model.road_lines[0], GUILayout.Width(30));
            current_model.road_lines[1] = EditorGUILayout.Toggle("90", current_model.road_lines[1], GUILayout.Width(30));
            current_model.road_lines[2] = EditorGUILayout.Toggle("180", current_model.road_lines[2], GUILayout.Width(30));
            current_model.road_lines[3] = EditorGUILayout.Toggle("270", current_model.road_lines[3], GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();
            current_model.pre_parking_position = EditorGUILayout.Vector2IntField("Pre Parking: ", current_model.pre_parking_position);
        }
        if (current_model.type == TaleType.FLOOR || current_model.type == TaleType.CHECKER)
        {
            current_model.is_Respown = EditorGUILayout.Toggle("Respown:", current_model.is_Respown); 
            EditorGUILayout.BeginHorizontal();

            show_walls = EditorGUILayout.Toggle("Show_wall", show_walls);
            GUILayout.Label("Objects {"+current_model.tale_objects.Count+"}", EditorStyles.boldLabel);
            if (GUILayout.Button("+"))
            {
                current_model.tale_objects.Add(new TaleObject());
            }
            if (GUILayout.Button("-"))
            {
                current_model.tale_objects.RemoveAt(current_model.tale_objects.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < current_model.tale_objects.Count; i++)
            {
                var to = current_model.tale_objects[i];
                SetTaleObject(ref to);
            }
            EditorGUILayout.EndVertical();
        }

        if(current_model.type == TaleType.ROAD || current_model.type == TaleType.PARKING)
        {
            
            EditorGUILayout.BeginVertical();
            
            GUILayout.Label("Road Settings", EditorStyles.boldLabel);
            show_road_direction = EditorGUILayout.Toggle("Show Direction:", show_road_direction);

            current_model.road_model[0].side = (RoadSideDirection)EditorGUILayout.EnumPopup("Road Type:", current_model.road_model[0].side);
            current_model.road_model[0].direction = (RoadDirection)EditorGUILayout.EnumPopup("Direction:", current_model.road_model[0].direction);
            if (current_model.road_model[0].side != RoadSideDirection.ONESIDE)
            {
                current_model.road_model[1].side = current_model.road_model[0].side;
                current_model.road_model[1].direction = (RoadDirection)EditorGUILayout.EnumPopup("Direction:", current_model.road_model[1].direction);
            }
            if (current_model.road_model[0].side == RoadSideDirection.TRIPPLE)
            {
                if (current_model.road_model.Length == 2)
                {
                    RoadModel[] road_model = new RoadModel[3];
                    road_model[0] = current_model.road_model[0];
                    road_model[1] = current_model.road_model[1];
                    road_model[2] = new RoadModel();
                    current_model.road_model = road_model;
                }
                current_model.road_model[2].side = current_model.road_model[0].side;
                current_model.road_model[2].direction = (RoadDirection)EditorGUILayout.EnumPopup("Direction:", current_model.road_model[2].direction);
            }
            EditorGUILayout.BeginHorizontal();
            current_model.road_model[0].is_start = EditorGUILayout.Toggle("Start", current_model.road_model[0].is_start);
            if (current_model.road_model[0].is_start) current_model.road_model[0].is_end = false;
            current_model.road_model[0].is_end = EditorGUILayout.Toggle("End", current_model.road_model[0].is_end);
            if (current_model.road_model[0].is_end) current_model.road_model[0].is_start = false;
            EditorGUILayout.EndHorizontal();
            if(GUILayout.Button("Test Drive"))
            {
                levelManager.TestDrive();
            }
            EditorGUILayout.EndVertical();
        }
    }

    private void ShowModel(ref TaleModel[,] models, bool is_roof=false)
    {
        if (models.Length == 0) return;
        GUIStyle style = EditorStyles.helpBox;
        style.fixedHeight = 650;
        style.fixedWidth = 350;
        EditorGUILayout.BeginVertical(style);
        is_clickable_draw = EditorGUILayout.Toggle("Draw: ", is_clickable_draw);
        
        show_roof = EditorGUILayout.Toggle("Show Roof: ", show_roof);
        
        if (levelManager != null)
            levelManager.ShowRoof = show_roof;

        GUILayout.Label("Map {" + pos_x + ":" + pos_y + "}", EditorStyles.boldLabel);
        if (is_clickable_draw)
            SetModel(ref draw_model);
        else
            SetModel(ref models[pos_x, pos_y]);
        
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Build"))
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                levelManager.H_level_model.size = field_size;
                levelManager.Build();

            };
        }
        if (GUILayout.Button("Clear"))
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                levelManager.ClearAll();

            };
        }
        EditorGUILayout.EndHorizontal();
        if(!is_roof && levelManager!=null)
            ShowRoomModels(selected_flor, ref levelManager.H_level_model.rooms);
        EditorGUILayout.EndVertical();
    }

    private void ShowRoomModels(int levels, ref List<RoomModel> rooms)
    {
        

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Rooms :");
        if(GUILayout.Button("+"))
        {
            rooms.Add(new RoomModel { stay_level = selected_flor});
        }
        if (GUILayout.Button("-"))
        {
            int index = 0;
            for(int i =0; i< rooms.Count; i++)
                if (rooms[i].stay_level == selected_flor) index = i;
            rooms.RemoveAt(index);
        }
        EditorGUILayout.EndHorizontal();
        scroll_position_room = EditorGUILayout.BeginScrollView(scroll_position_room);
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stay_level != selected_flor) continue;
            rooms[i].colapsed = EditorGUILayout.BeginFoldoutHeaderGroup(rooms[i].colapsed, rooms[i].type.ToString()+"["+ rooms[i].position.x + ":"+ rooms[i].position.z + "]");
            if (rooms[i].colapsed)
            {
                rooms[i].type = (RoomType)EditorGUILayout.EnumPopup("Type", rooms[i].type);
                rooms[i].position.x = EditorGUILayout.Slider("X:", rooms[i].position.x, 0, field_size.x);
                rooms[i].position.z = EditorGUILayout.Slider("Y:", rooms[i].position.z, 0, field_size.y);
                rooms[i].rotation = EditorGUILayout.Slider("Rotate:", rooms[i].rotation, 0, 360);
                if (rooms[i].type != RoomType.TABLE && rooms[i].type != RoomType.FLOWERBED
                    && rooms[i].type != RoomType.ESCOLATOR_DOWN && rooms[i].type != RoomType.ESCOLATOR_UP)
                {
                    GUILayout.Label("Settigs: LVL:" + rooms[i].level);
                    rooms[i].coin_time = EditorGUILayout.IntField("Coin Timer", rooms[i].coin_time);
                    rooms[i].upgrate_timer = EditorGUILayout.IntField("UPGR Timer", rooms[i].upgrate_timer);
                    rooms[i].profit = EditorGUILayout.IntField("Profit: ", rooms[i].profit);
                    rooms[i].upgrate_prise = EditorGUILayout.IntField("Price: ", rooms[i].upgrate_prise);
                }
                rooms[i].stay_level = selected_flor;

                switch (rooms[i].type)
                {
                    case RoomType.FASTFOOD:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.FastFoodCount - 1);
                        break;
                    case RoomType.BAR:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.BarCount - 1);
                        break;
                    case RoomType.CAFE:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.CafeRoomCount - 1);
                        break;
                    case RoomType.CHILDRENROOM:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.ChildrenRoomCount - 1);
                        break;
                    case RoomType.CASINO:

                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.CasinoCount - 1);
                        break;
                    case RoomType.CINEMA:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.CinemaRoomCount - 1);
                        break;
                    case RoomType.FURNITURESHOP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.FurnitureShopCount - 1);
                        break;
                    case RoomType.FOODSHOP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.FoodShopCount - 1);
                        break;
                    case RoomType.GAMEROOM:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.GameRoomCount - 1);
                        break;
                    case RoomType.GYM:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.GYMRoom - 1);
                        break;
                    case RoomType.MINISHOP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.MiniShopCount - 1);
                        break;
                    case RoomType.OTHERSHOP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.ShopCount - 1);
                        break;
                    case RoomType.RELAX:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.RelaxRoom - 1);
                        break;
                    case RoomType.SPORTSHOP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.SportShopCount - 1);
                        break;
                    case RoomType.WEARSHOP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.WearShopCount - 1);
                        break;
                    case RoomType.TABLE:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.TablesVariantCount - 1);
                        break;
                    case RoomType.FLOWERBED:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.FlowerBedCount - 1);
                        break;
                    case RoomType.ATM:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.ATMCount - 1);
                        break;
                    case RoomType.ESCOLATOR_UP:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.EscolatorCount - 1);
                        break;
                    case RoomType.ESCOLATOR_DOWN:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.EscolatorCount - 1);
                        break;
                    case RoomType.SERVISE:
                        rooms[i].variant = EditorGUILayout.IntSlider("V:",
                            rooms[i].variant, 0, levelManager.RoomManager.ServicesCount - 1);
                        break;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Entery Position");
                if (GUILayout.Button("+"))
                    rooms[i].entery_position.Add(new Vector2Int(0, 0));
                if (GUILayout.Button("-"))
                    rooms[i].entery_position.RemoveAt(rooms[i].entery_position.Count - 1);
                EditorGUILayout.EndHorizontal();

                if (rooms[i].entery_position != null)
                {
                    for (int j = 0; j < rooms[i].entery_position.Count; j++)
                        rooms[i].entery_position[j] = EditorGUILayout.Vector2IntField("", rooms[i].entery_position[j]);
                }

                EditorGUILayout.BeginHorizontal();
                if(rooms[i].free_places != null)
                    GUILayout.Label("Free Place {" + rooms[i].free_places.Count +"}");
                else
                    GUILayout.Label("Free Place {" + 0 + "}");
                if (GUILayout.Button("+"))
                    rooms[i].free_places.Add(true);
                if (GUILayout.Button("-"))
                    rooms[i].free_places.RemoveAt(rooms[i].free_places.Count - 1);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }
    

}
#endif
