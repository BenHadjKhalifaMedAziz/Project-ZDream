using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class coin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerController.numberOfCoins += 1;
            Debug.Log(PlayerController.numberOfCoins);
            Destroy(gameObject);
        }
    }
}
