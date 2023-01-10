using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulbear : Enemy
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
            // ƒvƒŒƒCƒ„[‚ª‹ß‚­‚É‚¢‚ê‚Î
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && (player.transform.position - transform.position).magnitude < searchRange)
            {
                moveVector = (player.transform.position - transform.position).normalized * maxSpeed;
                moveCount = (int)(actionCycle * Application.targetFrameRate);
            }

            // ‹ß‚­‚É‚¢‚È‚¯‚ê‚Î
            else
            {
                float deg = Main.random.NextFloat(360);
                moveVector = Quaternion.Euler(0, 0, deg) * Vector3.right * maxSpeed / 2.0f;
                moveCount = (int)(Main.random.NextFloat(1, 2) * actionCycle * Application.targetFrameRate);
            }
        }

        velocity = moveVector;
        moveCount--;
    }
}
