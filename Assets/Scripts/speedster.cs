using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
using static Models;

public class speedster : MonoBehaviour
{
    public bool speedsterIsOn = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 20 * Time.deltaTime, 0);
       // isSpeedster();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
            speedsterIsOn = true;
            StartCoroutine(TurnOffSpeedsterMode(10f));
            Debug.Log("speedster on 1");
        }
    }
    IEnumerator TurnOffSpeedsterMode(float duration)
    {
        yield return new WaitForSeconds(duration);

        speedsterIsOn = false;
    }

    public PlayerSettingsModel settings;
    public PlayerController playercontroller;

    public float oldspeed = 3.65f;
    public float newspeed = 400;
  //  private void isSpeedster()
 //   {   
  //      if (speedsterIsOn)
     //   {
////settings.RunningSpeed = newspeed;
//playercontroller.settings.RunningSpeed = newspeed;
//            Debug.Log("speedster on 2");
                
//        }
//        if ( !speedsterIsOn) 
//        {
//            //settings.RunningSpeed = oldspeed;
//            playercontroller.settings.RunningSpeed = oldspeed;
//            Debug.Log("speedster off");
//        }
  //  }
}
