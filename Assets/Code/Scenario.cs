using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Player").transform.position = GameObject.Find("Phantasia").GetComponent<CelestialObject>().launchSide.position;       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
