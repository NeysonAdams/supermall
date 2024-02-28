using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuilderSettings", menuName = "Level Builder Settings", order = 1)]
public class LevelBuilderSetting : ScriptableObject
{
    public Color[] ground_colors = new Color[Enum.GetNames(typeof(TaleType)).Length];
    public string[] names = Enum.GetNames(typeof(TaleType));
    public Texture2D[] road_marking_textures = new Texture2D[6];
    public Texture2D[] parking_marking_textures = new Texture2D[4];

}
