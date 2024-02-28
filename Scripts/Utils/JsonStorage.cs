using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using UnityEngine.Networking;

public class JsonStorage
{
    private static readonly string level_json_path = $"{Application.streamingAssetsPath}/Levels/level.json";

    public static void SaveLevelData(LevelModel level, Vector2 size)
    {
        Debug.Log(level_json_path);

        string json = JsonUtility.ToJson(level);
        File.WriteAllText(level_json_path, json);
    }

    public static LevelModel LoadLevelData()
    {
        Debug.Log(level_json_path);
        if (File.Exists(level_json_path))
        {
            string jsonData = File.ReadAllText(level_json_path);
            return JsonUtility.FromJson<LevelModel>(jsonData);
        }
        else
        {
            return null;
        }
    }

    public static LevelModel RunTimeLoadLevelData()
    {
        string jsonData;
        var targetFile = Resources.Load<TextAsset>("Levels/level");

        //string json = targetFile.text.Replace("\"", "'");
        return JsonUtility.FromJson<LevelModel>(targetFile.text);
    }

    public static IEnumerator LoadFile(Action<LevelModel> loaded)
    {
        string path = $"{Application.streamingAssetsPath}/Levels/level.json";

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(path);
                Debug.LogError(www.error);
            }
            else
            {
                string fileContents = www.downloadHandler.text;
                loaded?.Invoke(JsonUtility.FromJson<LevelModel>(fileContents));
            }
        }
    }
}
