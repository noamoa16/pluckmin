using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            cameraPosition = player.transform.position;
        }
    }

    private Vector2 cameraPosition
    {
        get => 
            new Vector2(transform.position.x, transform.position.y);
        set => 
            transform.position = new Vector3(value.x, value.y, transform.position.z);
    }

}
