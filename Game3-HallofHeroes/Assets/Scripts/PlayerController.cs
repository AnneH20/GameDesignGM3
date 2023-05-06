using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private TilemapCollider2D tilemapCollider;
    public static bool initialized = false;
    public Rigidbody2D rb;
    public GameObject tilemapVisualizerPrefab;
    private Vector2 moveDirection;
    public Animator animator;
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }
    float moveX;
    float moveY;

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
        if (tilemapCollider.OverlapPoint(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime))
        {
            Move();
        }
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
                Destroy(collision.gameObject);
                battleSceneTransition.TransitionToBattleScene();
            }
        }
    }

}