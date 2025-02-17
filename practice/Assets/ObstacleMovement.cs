using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime; // Move left

        // If the obstacle goes off-screen, deactivate it
        if (transform.position.x < -10f)
        {
            gameObject.SetActive(false);
        }
    }
}
