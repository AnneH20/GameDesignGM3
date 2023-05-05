using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public int health;
    public int maxHealth;
}

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] public static int health;
    [SerializeField] public static int maxHealth;
    [SerializeField] private PlayerState playerState;
    [SerializeField] public GameObject grid;
    public Rigidbody2D rb;
    public GameObject tilemapVisualizerPrefab;
    private Vector2 moveDirection;
    public Animator animator;
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    void Start()
    {
        health = playerState.health;
        maxHealth = playerState.maxHealth;
        rb.transform.position = new Vector2(0, 0);
        rb.gravityScale = 0f;
        
    }

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(grid);
        }
        else
        {
            Destroy(gameObject);
            Destroy(grid);
        }
       
    }
    
    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        
    }
    private void FixedUpdate()   
    {
        Move();
    }
        
    
    void ProcessInputs() 
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY);

    }
    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BattleSceneTransition battleSceneTransition = FindObjectOfType<BattleSceneTransition>();
            if (battleSceneTransition != null)
            {
                battleSceneTransition.TransitionToBattleScene();
            }
        }
    }
}