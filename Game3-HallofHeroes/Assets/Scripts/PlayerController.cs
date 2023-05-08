using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private int defense;
    [SerializeField] private int currentLevel;
    [SerializeField] public GameObject grid;
    [SerializeField] public int isBossDead = 0;
    public static bool isDead = false;
    public InventoryScript playerInventory = new InventoryScript();
    public GameObject sceneCamera;
    private TilemapCollider2D tilemapCollider;
    public static bool initialized = false;
    public Rigidbody2D rb;
    public GameObject tilemapVisualizerPrefab;
    private Vector2 moveDirection;
    public Animator animator;
    public GameObject BGM;
    public bool isBoss = false;
    public bool hasCollided = false;
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }
    float moveX;
    float moveY;

    void Start()
    {
        if (SceneManager.GetSceneByName("Level1").isLoaded)
        {
            // Set the player's stats on the current scene state
            PlayerPrefs.SetInt("Health", health);
            PlayerPrefs.SetInt("MaxHealth", maxHealth);
            PlayerPrefs.SetInt("Damage", damage);
            PlayerPrefs.SetInt("Defense", defense);
            PlayerPrefs.SetInt("PlayerLevel", 1);
            PlayerPrefs.SetInt("PlayerXP", 0);
        }
        PlayerPrefs.SetInt("Boss Dead", isBossDead);
        playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Potion", itemDescription = "Heals 10 HP.", itemEffect = 10, itemAmount = 1 });
	    playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Super Potion", itemDescription = "Heals 20 HP.", itemEffect = 20, itemAmount = 0 });
        playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Hyper Potion", itemDescription = "Heals 50 HP.", itemEffect = 50, itemAmount = 0 });
        playerInventory.inventory.items.Add(new InventoryScript.Item { itemName = "Max Potion", itemDescription = "Heals to full HP.", itemEffect = 9999, itemAmount = 0 });
        rb.transform.position = new Vector2(0, 0);
        rb.gravityScale = 0f;
        tilemapCollider = grid.GetComponentInChildren<TilemapCollider2D>();
        hasCollided = false;
        Debug.Log("Start");
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        sceneCamera = GameObject.FindGameObjectWithTag("MainCamera");
        BGM = GameObject.Find("BGM");
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
        health = PlayerPrefs.GetInt("Health");
        maxHealth = PlayerPrefs.GetInt("MaxHealth");
        damage = PlayerPrefs.GetInt("Damage");
        defense = PlayerPrefs.GetInt("Defense");
        isBossDead = PlayerPrefs.GetInt("Boss Dead");
        currentLevel = PlayerPrefs.GetInt("PlayerLevel");
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
        if (!hasCollided && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss")))
        {
            BattleSceneTransition battleSceneTransition = FindObjectOfType<BattleSceneTransition>();
            if (battleSceneTransition != null)
            {
                if (collision.gameObject.CompareTag("Boss"))
                {
                    isBoss = true;
                    Debug.Log("Boss");
                }
                Destroy(collision.gameObject);
                BattleSystem.battleExit = false;
                hasCollided = true;
                battleSceneTransition.TransitionToBattleScene();
            }
        }
        hasCollided = false;
    }

}