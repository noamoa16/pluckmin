using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static HashSet<string> prefabNames { get; private set; } = new HashSet<string>();

    public static readonly MersenneTwister random = new MersenneTwister();

    public static bool isQuitting { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;

        // Test
        MyObject.Create("Strawberry", new Vector2(3, 0));
        MyObject.Create("Strawberry", new Vector2(4, 0));
        MyObject.Create("Strawberry", new Vector2(5, 0));

        // オブジェクト一覧マップの作成
        GameObject[] prefabs = Resources.LoadAll<GameObject>(@"Prefabs");
        System.Array.ForEach(prefabs, prefab => prefabNames.Add(prefab.name));

        prefabNames.Remove("HealthBar");


        SaveCurrentStageData("test.json");
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public static void SaveCurrentStageData(string path)
    {
        StageData savedObjectsData = new StageData();

        GameObject[] objects = (GameObject[])FindObjectsOfType(typeof(GameObject));
        foreach (GameObject obj in objects)
        {
            // 対応するprefabが存在すれば
            if (Main.prefabNames.Contains(obj.name))
            {
                savedObjectsData.Add(new MyObjectData(obj));
            }
        }

        string json = JsonUtility.ToJson(savedObjectsData);
        json = ConvertToReadableJson(json);

        File.WriteAllText(ToStageFilePath(path), json);
    }

    public static void LoadStageData(string path)
    {
        string json = File.ReadAllText(ToStageFilePath(path));
        StageData savedObjectsData = JsonUtility.FromJson<StageData>(json);

        foreach (MyObjectData myObjectData in savedObjectsData)
        {
            MyObject.Create(myObjectData.name, myObjectData.position);
        }
    }

    private static string ToStageFilePath(string path)
    {
        return Application.dataPath + "/Data/" + path;
    }

    private static string ConvertToReadableJson(string json)
    {
        string ret = string.Empty;
        bool isString = false;
        int tabs = 0;

        string multiTabs(int n)
        {
            string s = string.Empty;
            for (int i = 0; i < n; i++)
            {
                s += "  ";
            }
            return s;
        }

        foreach (char c in json)
        {
            string before = string.Empty;
            string after = string.Empty;
            if (c == '"') isString ^= true;

            if (!isString)
            {
                switch (c)
                {
                    case '{':
                    case '[':
                        after += '\n';
                        tabs++;
                        after += multiTabs(tabs);
                        break;
                    case '}':
                    case ']':
                        before += '\n';
                        tabs--;
                        before += multiTabs(tabs);
                        break;
                    case ':':
                        before += ' ';
                        after += ' ';
                        break;
                    case ',':
                        after += '\n';
                        after += multiTabs(tabs);
                        break;
                }
            }

            ret += before + c + after;
        }

        return ret;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
