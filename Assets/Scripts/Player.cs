using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MyObject
{
    [SerializeField]
    private float maxSpeed = 4;

    private int treasuresCollected = 0;

    public static readonly string[] COLORS =
    {
        "Red", "Yellow", "Blue"
    };

    private Dictionary<KeyCode, bool> lastKeyState = new Dictionary<KeyCode, bool>();
    private Dictionary<KeyCode, bool> currentKeyState = new Dictionary<KeyCode, bool>();

    private int bombCoolTime = 0;

    public string color
    {
        get
        {
            string spriteName = GetComponent<SpriteRenderer>().sprite.name;
            if (!spriteName.EndsWith("Pikmin"))
            {
                throw new System.InvalidOperationException(
                    "This object \"" + GetType().Name + "\" must have sprite name ends with \"Pikmin\""
                );
            }
            string colorString = spriteName.Substring(0, spriteName.Length - "Pikmin".Length);

            if (!COLORS.Contains(colorString))
            {
                throw new System.InvalidOperationException(
                    "No Pikmin color named \"" + colorString + "\""
                );
            }

            return colorString;
        }
        set
        {
            if (!COLORS.Contains(value))
            {
                throw new System.InvalidOperationException(
                    "No Pikmin color named \"" + value + "\""
                );
            }

            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + value + "Pikmin");
            name = value + "Pikmin";
        }
    }

    protected override int paintedOrder => 2;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        lastKeyState[KeyCode.Return] = false;
        currentKeyState[KeyCode.Return] = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        lastKeyState = new Dictionary<KeyCode, bool>(currentKeyState);
        currentKeyState[KeyCode.Return] = Input.GetKey(KeyCode.Return);

        bombCoolTime = Mathf.Max(bombCoolTime - 1, 0);

        // 移動
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        velocity = new Vector2(h, v) * maxSpeed;

        // お宝数のチェック
        int length = GameObject.FindGameObjectsWithTag("Treasure").Length;
        int treasuresMax = length + treasuresCollected;
        GameObject statusText = GameObject.Find("StatusText");
        statusText.GetComponent<Text>().text = "Treasures: " + treasuresCollected + " / " + treasuresMax;

        // 爆弾を置く
        if (
            hasBomb &&
            !lastKeyState[KeyCode.Return] &&
            currentKeyState[KeyCode.Return] &&
            bombCoolTime == 0
            )
        {
            GameObject bomb = holdingBomb;
            bomb.transform.parent = null;
            bomb.transform.position = transform.position;
            bomb.GetComponent<SpriteRenderer>().sortingOrder = 0;
            bomb.GetComponent<Bomb>().Fired();
            bombCoolTime = (int)(0.1 * Application.targetFrameRate);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Main.isQuitting)
            Create("PikminSoul", transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // お宝取得
        if (collider.gameObject.tag == "Treasure")
        {
            Destroy(collider.gameObject);
            treasuresCollected++;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        // 爆弾を拾う
        if (
            color == "Yellow" &&
            !hasBomb &&
            collider.gameObject.name == "Bomb" &&
            !collider.gameObject.GetComponent<Bomb>().isFired &&
            !lastKeyState[KeyCode.Return] &&
            currentKeyState[KeyCode.Return] &&
            bombCoolTime == 0
            )
        {
            collider.transform.parent = gameObject.transform;
            collider.transform.localPosition = new Vector2(0, -0.5f);
            collider.GetComponent<SpriteRenderer>().sortingOrder = 4;
            bombCoolTime = (int)(0.1 * Application.targetFrameRate);
        }

        // 炎
        if (color != "Red" && collider.gameObject.name == "Flame")
        {
            Killed();
        }

        // 雷
        if (color != "Yellow" && collider.gameObject.name == "Electricity")
        {
            Killed();
        }

        // 水
        if (color != "Blue" && collider.gameObject.name == "Water")
        {
            Killed();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ポンガシグサ
        if (
            collision.gameObject.tag == "Candypop" &&
            !hasBomb
            )
        {
            string name = collision.gameObject.name;
            if (!name.EndsWith("Candypop"))
            {
                throw new System.InvalidOperationException(
                    "The object \"" + collision.gameObject.GetType().Name
                    + "\" must have name ends with \"Candypop\""
                );
            }
            string colorString = name.Substring(0, name.Length - "Candypop".Length);

            // 色が変わる
            if (color != colorString)
            {
                color = colorString;
                Destroy(collision.gameObject);
            }

            // 色が変わらない
            else
            {

            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 敵との衝突
        Enemy enemy;
        if ((enemy = collision.gameObject.GetComponent<Enemy>()) && enemy.isAggressive)
        {
            Damaged(1f);
        }
    }

    public bool hasBomb => HasChild("Bomb");
    public GameObject holdingBomb => GetChild("Bomb");
}