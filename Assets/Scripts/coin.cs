using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isAttracted = false; // Check if the coin is attracted by a magnet
    private Transform magnetSource; // Reference to the magnet source (the player with a magnet)

    void Start()
    {
        // Initialize magnetSource to null
        magnetSource = null;
    }

    void Update()
    {
        if (isAttracted && magnetSource != null)
        {
            // Move the coin towards the magnet
            transform.position = Vector3.MoveTowards(transform.position, magnetSource.position, Time.deltaTime * 5f);
        }
        else
        {
            // Rotate the coin as usual
            transform.Rotate(0, 20 * Time.deltaTime, 0);
        }
    }

    public void SetAttracted(Transform source)
    {
        // Called when the coin is attracted by a magnet
        isAttracted = true;
        magnetSource = source;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isAttracted)
            {
                PlayerController.numberOfCoins += 1;
                Debug.Log(PlayerController.numberOfCoins);
            }

            Destroy(gameObject);
        }
    }
}
