using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int iLoveMath = 92;
    public float moveSpeed;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector2(moveSpeed, 0));
    

    }
    
    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0, 500));
        }
    }
}
