using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MyObject
{
    public virtual bool isAggressive => false; // �s�N�~�����G��ƃ_���[�W���󂯂邩

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Main.isQuitting)
            Create("EnemySoul", transform.position);
    }
}