using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MyObject
{
    [SerializeField]
    private float lifespan;

    [SerializeField]
    private float moveUpVelocity;

    protected override bool autoDamage => true;
    protected override bool viewHealthBar => false;
    protected override bool hasInvincibleTime => false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hp = maxHp = lifespan * Application.targetFrameRate;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.position += (Vector3)Vector2.up * moveUpVelocity / Application.targetFrameRate;

        // Application.targetFrameRate‚©‚ç-1‚ª•Ô‚Á‚Ä‚­‚éƒoƒO‘Îô
        if (hp < 0 || maxHp < 0)
        {
            hp = maxHp = lifespan * Application.targetFrameRate;
        }

        Color color = GetComponent<SpriteRenderer>().color;
        color.a = hp / maxHp * 0.75f;
        GetComponent<SpriteRenderer>().color = color;
    }
}
