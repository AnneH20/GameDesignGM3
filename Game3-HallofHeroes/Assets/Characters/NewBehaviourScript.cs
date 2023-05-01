using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody2D rb;

    private Vector2 moveDirection;

    public Animator animator;

    void Start()
    {
        rb.transform.position = new Vector2(0, 0);
        rb.gravityScale = 0f;
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
    
}

