using UnityEngine;

public class Enemy : MonoBehaviour
{
 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the trigger zone!");
            // Add further logic for when the player enters the trigger
        }
    }

    
}
