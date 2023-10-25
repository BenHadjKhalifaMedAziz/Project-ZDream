using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public bool magnetIsOn = false;
    private List<Transform> attractedCoins = new List<Transform>();
    private CapsuleCollider coinCollider; // Reference to the player's coin collider

    void Start()
    {
        coinCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);

        if (magnetIsOn)
        {
            AttractCoins();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Activate the magnet effect
            Destroy(gameObject);
            magnetIsOn = true;
            StartCoroutine(TurnOffMagnet(10f));
            Debug.Log("Magnet is on");
        }
        else if (other.CompareTag("Coin"))
        {
            attractedCoins.Add(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            attractedCoins.Remove(other.transform);
        }
    }

    IEnumerator TurnOffMagnet(float duration)
    {
        yield return new WaitForSeconds(duration);
        magnetIsOn = false;
        Debug.Log("Magnet is off");
        // Reset the player's coin collider properties here (e.g., radius and height)
        coinCollider.radius = 1.29f;
        coinCollider.height = 3.49f;
    }

    void AttractCoins()
    {
        foreach (Transform coin in attractedCoins)
        {
            if (coin != null)
            {
                Vector3 direction = transform.position - coin.position;
                float distance = direction.magnitude;

                if (distance > 0.1f)
                {
                    coin.GetComponent<Rigidbody>().AddForce(direction.normalized * 5f);
                }
            }
        }
    }
}


