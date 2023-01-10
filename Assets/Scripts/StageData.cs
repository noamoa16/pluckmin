using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class StageData : IEnumerable<MyObjectData>
{
    public List<MyObjectData> objects;
    
    public StageData()
    {
        objects = new List<MyObjectData>();
    }

    public StageData(List<MyObjectData> objects)
    {
        this.objects = objects;
    }

    public void Add(MyObjectData obj)
    {
        objects.Add(obj);
    }

    public IEnumerator<MyObjectData> GetEnumerator()
    {
        return objects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

[System.Serializable]
public class MyObjectData
{
    public string name;
    public Vector2 position;
    public MyObjectData(string name, Vector2 position)
    {
        this.name = name;
        this.position = position;
    }
    public MyObjectData(GameObject gameObject)
    {
        this.name = gameObject.name;
        this.position = gameObject.transform.position;
    }

    public override string ToString()
    {
        return "{name : " + name + ", position : " + position + "}";
    }
}
