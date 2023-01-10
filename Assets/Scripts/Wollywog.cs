using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wollywog : Enemy
{
    public override bool isAggressive => true;

    [SerializeField]
    private float actionCycle;
    [SerializeField]
    private float normalSpeed;
    [SerializeField]
    private float maxJumpSpeed;
    [SerializeField]
    private float homeRange;
    [SerializeField]
    private float searchRange;

    private int moveCount = 0;
    private Vector3 moveVector = Vector3.zero;
    private Vector3 homePosition = Vector3.zero;
    private bool jumped = false;

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
            // ホームから離れすぎたら
            if ((homePosition - transform.position).magnitude > homeRange)
            {
                Vector3 toHomeVector = (homePosition - transform.position).normalized;
                moveVector = Quaternion.Euler(0, 0, Main.random.NextFloat(-15, 15))
                    * toHomeVector * normalSpeed;
                moveCount = (int)(2 * actionCycle * Application.targetFrameRate);
                jumped = false;
            }
            else
            {
                // プレイヤーが近くにいれば
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && (player.transform.position - transform.position).magnitude < searchRange
                    && !jumped)
                {
                    Vector2 toPlayerVector = (player.transform.position - transform.position).normalized;

                    // rad in [0, 2π]
                    float rad = Mathf.Atan2(toPlayerVector.y, toPlayerVector.x);
                    if (rad < 0) rad += Mathf.PI * 2;

                    Vector2 dir;

                    bool nearlyEqual(float a, float b)
                        => b * 0.99 < a && a < b * 1.01;

                    if (nearlyEqual(rad, Mathf.PI / 4.0f))
                    {
                        dir = Main.random.NextBool() ? new Vector2(1, 0) : new Vector2(0, 1);
                    }
                    else if (nearlyEqual(rad, Mathf.PI * 3.0f / 4.0f))
                    {
                        dir = Main.random.NextBool() ? new Vector2(-1, 0) : new Vector2(0, 1);
                    }
                    else if (nearlyEqual(rad, Mathf.PI * 5.0f / 4.0f))
                    {
                        dir = Main.random.NextBool() ? new Vector2(-1, 0) : new Vector2(0, -1);
                    }
                    else if (nearlyEqual(rad, Mathf.PI * 7.0f / 4.0f))
                    {
                        dir = Main.random.NextBool() ? new Vector2(1, 0) : new Vector2(0, -1);
                    }
                    else if (Mathf.PI / 4.0f < rad && rad < Mathf.PI * 3.0f / 4.0f)
                    {
                        dir = new Vector2(0, 1);
                    }
                    else if (Mathf.PI * 3.0f / 4.0f < rad && rad < Mathf.PI * 5.0f / 4.0f)
                    {
                        dir = new Vector2(-1, 0);
                    }
                    else if (Mathf.PI * 5.0f / 4.0f < rad && rad < Mathf.PI * 7.0f / 4.0f)
                    {
                        dir = new Vector2(0, -1);
                    }
                    else
                    {
                        dir = new Vector2(1, 0);
                    }

                    moveVector = dir * maxJumpSpeed;
                    moveCount = (int)(actionCycle * Application.targetFrameRate);
                    jumped = true;
                }

                // 近くにいなければ
                else
                {
                    float deg = Main.random.NextFloat(360);
                    moveVector = Quaternion.Euler(0, 0, deg) * Vector3.right * normalSpeed;
                    moveCount = (int)(Main.random.NextFloat(1, 2) * actionCycle * Application.targetFrameRate);
                    jumped = false;
                }
            }
        }

        velocity = moveVector;
        moveCount--;
    }
}
