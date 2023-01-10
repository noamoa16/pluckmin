using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MyObject : MonoBehaviour
{
    protected virtual int paintedOrder => 0;
    protected virtual bool isFlippable => true;

    [field: SerializeField]
    public float hp { get; protected set; } = -1;

    [field: SerializeField]
    public float maxHp { get; protected set; } = -1;

    protected virtual bool viewHealthBar => true;

    protected virtual bool autoDamage => false;

    protected virtual bool hasInvincibleTime => true;
    public int invincibleTime { get; protected set; } = 0;


    protected virtual void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = paintedOrder;
        if (viewHealthBar)
        {
            CreateChild("HealthBar", (Vector2)transform.position + new Vector2(0, 1.5f));
        }
    }

    protected virtual void Update()
    {
        invincibleTime = Mathf.Max(invincibleTime - 1, 0);

        Color color = GetComponent<SpriteRenderer>().color;
        color.a = invincibleTime % 4 < 2 ? 1 : 0;
        GetComponent<SpriteRenderer>().color = color;

        if (autoDamage && hp > 0)
        {
            Damaged(1);
        }

        if (hp == 0)
        {
            Destroy(gameObject);
            return;
        }

        if (viewHealthBar)
        {
            GameObject healthBar = GetChild("HealthBar");
            if (healthBar != null)
            {
                healthBar.SetActive(hp != maxHp);
                if (hp >= 0 && maxHp >= 0)
                {
                    healthBar.GetComponent<HealthBar>().health = hp / maxHp;
                }
            }
        }
    }

    protected virtual void OnDestroy()
    {

    }

    public /*virtual*/ void Damaged(float damage)
    {
        if (hp != -1 && invincibleTime == 0)
        {
            hp = Mathf.Max(hp - damage, 0);

            if (hasInvincibleTime)
                invincibleTime = (int)(1.0f * Application.targetFrameRate);
        }
    }

    public void Killed()
    {
        if (hp != -1)
        {
            hp = 0;
        }
    }

    public float rotation
    {
        get => transform.eulerAngles.z;
        protected set => transform.eulerAngles = new Vector3(0, 0, value);
    }

    public Vector2 velocity
    {
        get
        {
            Rigidbody2D r = GetComponent<Rigidbody2D>();
            if (r == null)
            {
                throw new System.InvalidOperationException(
                    "This object \"" + GetType().Name + "\" has no component \"Rigidbody2D\""
                );
            }
            return r.velocity;
        }
        protected set
        {
            Rigidbody2D r = GetComponent<Rigidbody2D>();
            if (r == null)
            {
                throw new System.InvalidOperationException(
                    "This object \"" + GetType().Name + "\" has no component \"Rigidbody2D\""
                );
            }
            r.velocity = value;
            GetComponent<SpriteRenderer>().flipX = r.velocity.x > 0;
        }
    }

    public static GameObject Create(string name, Vector2 position, float rotation = 0)
    {
        GameObject prefab = Resources.Load<GameObject>(@"Prefabs/" + name);
        GameObject obj = Instantiate(prefab, position, Quaternion.Euler(0, 0, rotation));
        obj.name = name;
        return obj;
    }

    public GameObject CreateChild(string name, Vector2 position, float rotation = 0)
    {
        GameObject obj = Create(name, position, rotation);
        obj.transform.SetParent(gameObject.transform);
        return obj;
    }

    public GameObject[] children
    {
        get
        {
            GameObject[] ret = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                ret[i] = transform.GetChild(i).gameObject;
            }
            return ret;
        }
    }

    public GameObject GetChild(string name)
    {
        foreach (GameObject child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    public bool HasChild(string name) => GetChild(name) != null;
}