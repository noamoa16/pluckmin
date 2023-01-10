using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MyObject
{
    public virtual bool isAggressive => false; // ピクミンが触るとダメージを受けるか

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Main.isQuitting)
            Create("EnemySoul", transform.position);
    }
}