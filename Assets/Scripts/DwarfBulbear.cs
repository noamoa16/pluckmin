using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfBulbear : Enemy
{
    public override bool isAggressive => true;

    [SerializeField]
    private float actionCycle;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float searchRange;

    private int moveCount = 0;
    private Vector3 moveVector = Vector3.zero;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (moveCount <= 0)
        {
            // プレイヤーが近くにいれば
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && (player.transform.position - transform.position).magnitude < searchRange)
            {
                moveVector = (player.transform.position - transform.position).normalized * maxSpeed;
                moveCount = (int)(actionCycle * Application.targetFrameRate);
            }

            // 近くにいなければ
            else
            {

                // クマチャッピーが近くにいれば
                Bulbear bulbear = FindNearestBulbear();
                if (
                    bulbear != null &&
                    (bulbear.transform.position - transform.position).magnitude > searchRange / 2 &&
                    (bulbear.transform.position - transform.position).magnitude < 2 * searchRange
                    )
                {
                    moveVector = (bulbear.transform.position - transform.position).normalized * maxSpeed;
                    moveCount = (int)(actionCycle * Application.targetFrameRate);
                }
                else
                {
                    float deg = Main.random.NextFloat(360);
                    moveVector = Quaternion.Euler(0, 0, deg) * Vector3.right * maxSpeed / 2.0f;
                    moveCount = (int)(Main.random.NextFloat(1, 2) * actionCycle * Application.targetFrameRate);
                }
            }
        }

        velocity = moveVector;
        moveCount--;
    }

    private Bulbear FindNearestBulbear()
    {
        Bulbear[] bulbears = GameObject.FindObjectsOfType<Bulbear>();

        float minDistance = float.MaxValue;
        Bulbear ret = null;
        foreach (var bulbear in bulbears)
        {
            float distance = (transform.position - bulbear.transform.position).magnitude;
            if (distance < minDistance)
            {
                ret = bulbear;
                minDistance = distance;
            }
        }
        return ret;
    }
}
