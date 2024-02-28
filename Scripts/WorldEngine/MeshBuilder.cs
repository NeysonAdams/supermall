using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeshBuilder : MonoBehaviour
{
    private GameObject[] tale_ground_objects = new GameObject[Enum.GetNames(typeof(TaleType)).Length-1];
    private List<Vector3>[] poligons = new List<Vector3>[Enum.GetNames(typeof(TaleType)).Length - 1];
    private List<int>[] triangles = new List<int>[Enum.GetNames(typeof(TaleType)).Length - 1];
    private List<Vector3>[] normals = new List<Vector3>[Enum.GetNames(typeof(TaleType)).Length - 1];
    private List<Vector2>[] uvs = new List<Vector2>[Enum.GetNames(typeof(TaleType)).Length - 1];

    #region Init Grounds
    public void InitTaleObjects()
    {
        TaleGroungIterate((i, type) =>
            {
                tale_ground_objects[i] = new GameObject();
                tale_ground_objects[i].transform.parent = transform;
                tale_ground_objects[i].transform.localPosition = new Vector3(0, 0, 0);
                tale_ground_objects[i].name = type;
                poligons[i] = new List<Vector3>();
                triangles[i] = new List<int>();
                normals[i] = new List<Vector3>();
                uvs[i] = new List<Vector2>();
            });
    }
    public void AddVertises(TaleType type, Vector2 position)
    {
        int index = (int)type;
        float size = 1.0f;
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-size / 2 + position.x, 0, -size / 2+ position.y),
            new Vector3(size / 2 + position.x, 0, -size / 2 + position.y),
            new Vector3(-size / 2 + position.x, 0, size / 2 + position.y),
            new Vector3(size / 2 + position.x, 0, size / 2 + position.y)
        };

        Vector2[] uv = new Vector2[4]
          {
              new Vector2(position.x, position.y),
              new Vector2(position.x+1, position.y),
              new Vector2(position.x, position.y+1),
              new Vector2(position.x+1, position.y+1)
          };

        uvs[index].AddRange(uv);

        poligons[index].AddRange(vertices);

        // Определяем индексы треугольников для текущей позиции
        int startIndex = poligons[index].Count - vertices.Length;
        int[] _triangles = new int[]
        {
                startIndex, startIndex + 1, startIndex + 2,
                startIndex, startIndex + 2, startIndex + 3,
                startIndex+3, startIndex + 2, startIndex + 1,
                startIndex, startIndex + 3, startIndex + 1,

        };

        // Добавляем индексы треугольников к списку всех индексов треугольников
        triangles[index].AddRange(_triangles);

        Vector3[] normal = new Vector3[4]
        {
            Vector3.one,
            Vector3.one,
            Vector3.one,
            Vector3.one
        };
        normals[index].AddRange(normal);
    }

    public void Apply(Material[] materials)
    {
        TaleGroungIterate((i, type) =>
        {
            Mesh mesh = new Mesh();

            mesh.vertices = poligons[i].ToArray();
            mesh.triangles = triangles[i].ToArray();
            mesh.normals = normals[i].ToArray();
            mesh.uv = uvs[i].ToArray();

            MeshFilter meshFilter = tale_ground_objects[i].AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = tale_ground_objects[i].AddComponent<MeshRenderer>();
            meshRenderer.material = materials[i];

            meshRenderer.sortingLayerID = 0;

        });
    }
    #endregion

    public void AddObject(GameObject obj, int id, Transform parent, TaleModel model)
    {
        if (obj == null)
            return;
        GameObject element = (GameObject)Instantiate(obj, parent);
        Vector2 res_position = model.position + ((id==-1)?Vector2.zero:model.tale_objects[id].dop_position);

        element.transform.localPosition = new Vector3(
            res_position.x,
            0,
            res_position.y
            );
        Vector3 rotate = element.transform.localRotation.eulerAngles;
        element.transform.localRotation = Quaternion.Euler(new Vector3(rotate.x, (id == -1) ? 0 : model.tale_objects[id].rotation, rotate.z));
    }

    public void AddMarking(Sprite[] sprite, TaleModel model)
    {
        GameObject marking = new GameObject();
        marking.transform.parent = tale_ground_objects[(int)TaleType.ROAD].transform;
        marking.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        var render = marking.AddComponent<SpriteRenderer>();
        marking.transform.localPosition = new Vector3(model.position.x, 0.01f, model.position.y);
        if (model.type==TaleType.PARKING)
        {
            render.sprite = sprite[0];
            float rotate = 0;
            if (model.road_lines[0]) rotate = -90;
            else if (model.road_lines[1]) rotate = 0;
            else if (model.road_lines[2]) rotate = 90;
            else if (model.road_lines[3]) rotate = 180;

            marking.transform.localRotation = Quaternion.Euler(new Vector3(90, rotate, 0));
        }
        if (model.type == TaleType.ROAD)
        {
            if (model.zebra_h || model.zebra_w)
            {

                float rotate = model.zebra_w ? 0f : 90f;
                marking.transform.localScale = new Vector3(4f, 4f, 4f);
                render.sprite = sprite[1];
                marking.transform.localRotation = Quaternion.Euler(new Vector3(90, rotate, 0));
            }
            else if(model.road_lines[0] || model.road_lines[1] || model.road_lines[2] || model.road_lines[3])
            {
                render.sprite = sprite[2];
                if (model.road_lines[0]) marking.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                if (model.road_lines[1]) marking.transform.localRotation = Quaternion.Euler(new Vector3(90, 180, 0));
                if (model.road_lines[2]) marking.transform.localRotation = Quaternion.Euler(new Vector3(90, 270, 0));
                if (model.road_lines[3]) marking.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
            }
        }
    }

    public void GenerateBorders()
    {
        GameObject borderObject = new GameObject("BORDER");
        borderObject.transform.parent = transform;
        float borderWidth = 0.2f;
        float borderHeight = 0.5f;
        MeshFilter mesh_filter = tale_ground_objects[(int)TaleType.GRASS].GetComponent<MeshFilter>();
        Mesh mesh = mesh_filter.mesh;
        Bounds bounds = mesh.bounds;

        Vector3[] borderVertices = new Vector3[]
        {
            new Vector3(bounds.min.x - borderWidth, bounds.min.y, bounds.min.z - borderWidth),
            new Vector3(bounds.max.x + borderWidth, bounds.min.y, bounds.min.z - borderWidth),
            new Vector3(bounds.max.x + borderWidth, bounds.min.y, bounds.max.z + borderWidth),
            new Vector3(bounds.min.x - borderWidth, bounds.min.y, bounds.max.z + borderWidth),
            new Vector3(bounds.min.x - borderWidth, bounds.min.y + borderHeight, bounds.min.z - borderWidth),
            new Vector3(bounds.max.x + borderWidth, bounds.min.y + borderHeight, bounds.min.z - borderWidth),
            new Vector3(bounds.max.x + borderWidth, bounds.min.y + borderHeight, bounds.max.z + borderWidth),
            new Vector3(bounds.min.x - borderWidth, bounds.min.y + borderHeight, bounds.max.z + borderWidth)
        };

        // Создаем треугольники бордюра
        int[] borderTriangles = new int[]
        {
            0, 1, 2, 2, 3, 0, // Bottom face
            4, 5, 6, 6, 7, 4, // Top face
            0, 4, 7, 7, 3, 0, // Left side
            1, 5, 6, 6, 2, 1, // Right side
            0, 1, 5, 5, 4, 0, // Front side
            2, 3, 7, 7, 6, 2  // Back side
        };

        // Создаем новый меш для бордюра
        Mesh borderMesh = new Mesh();
        borderMesh.vertices = borderVertices;
        borderMesh.triangles = borderTriangles;

        // Рассчитываем нормали и тангенты для корректного отображения материала
        borderMesh.RecalculateNormals();
        borderMesh.RecalculateTangents();

        MeshFilter borderMeshFilter = borderObject.AddComponent<MeshFilter>();
        borderMeshFilter.mesh = borderMesh;

        // Добавляем компонент MeshRenderer для отображения бордюра
        MeshRenderer borderMeshRenderer = borderObject.AddComponent<MeshRenderer>();
        borderMeshRenderer.material = new Material(Shader.Find("Standard"));
    }

    private void TaleGroungIterate(Action<int, string> iterateAction)
    {
        int i = 0;
        foreach (string type in Enum.GetNames(typeof(TaleType)))
        {
            if (!type.Equals("NONE"))
            {
                iterateAction?.Invoke(i, type);
                i++;
            }
        }
    }

}
