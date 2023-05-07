using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public int damage;
    [SerializeField] public int defense;
    [SerializeField] public GameObject grid;
    public InventoryScript playerInventory = new InventoryScript();
    public GameObject sceneCamera;
    private TilemapCollider2D tilemapCollider;
    public static bool initialized = false;
    public Rigidbody2D rb;
    public GameObject tilemapVisualizerPrefab;
    private Vector2 moveDirection;
    public Animator animator;
    public GameObject BGM;
    public static bool isBoss = false;
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }
    float moveX;
    float moveY;

    void Start()
    {
        sceneCamera = GameObject.FindGameObjectWithTag("MainCamera");
        BGM = GameObject.Find("BGM");
        playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Potion", itemDescription = "Heals 10 HP.", itemEffect = 10, itemAmount = 0 });
	    playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Super Potion", itemDescription = "Heals 20 HP.", itemEffect = 20, itemAmount = 0 });
        rb.transform.position = new Vector2(0, 0);
        rb.gravityScale = 0f;
        tilemapCollider = grid.GetComponentInChildren<TilemapCollider2D>();
    }

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
        }
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();

        // check for left and right arrow keys
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            animator.SetBool("horizontalWalk", true);
        }
        else
        {
            animator.SetBool("horizontalWalk", false);
        }
    }

    private void FixedUpdate()
    {
        Move();
        rb.transform.position = rb.transform.position;
    }


    void ProcessInputs()
    {
        moveX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        moveY = Input.GetAxisRaw("Vertical") * moveSpeed;

        moveDirection = new Vector2(moveX, moveY);

        if (moveX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            animator.SetBool("verticalDown", false);
            animator.SetBool("verticalUp", false);
        }
        else if (moveX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            animator.SetBool("verticalDown", false);
            animator.SetBool("verticalUp", false);
        }
        if (moveY < 0)
        {
            animator.SetBool("verticalUp", false);
            animator.SetBool("verticalDown", true);
        }
        else if (moveY > 0)
        {
            animator.SetBool("verticalDown", false);
            animator.SetBool("verticalUp", true);
        }
        else
        {
            animator.SetBool("verticalDown", false);
            animator.SetBool("verticalUp", false);
        }
        animator.SetFloat("horizontalSpeed", Mathf.Abs(moveX));
        animator.SetFloat("verticalSpeed", Mathf.Abs(moveY));
    }
    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            BattleSceneTransition battleSceneTransition = FindObjectOfType<BattleSceneTransition>();
            if (battleSceneTransition != null)
            {
                if (collision.gameObject.CompareTag("Boss"))
                {
                    isBoss = true;
                }
                Destroy(collision.gameObject);
                BattleSystem.battleExit = false;
                battleSceneTransition.TransitionToBattleScene();
            }
        }
    }

}