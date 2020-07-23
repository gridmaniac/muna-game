using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour {

    public new Rigidbody2D rigidbody;
    private int initialTorque = 0;
    private float rot = 0.0f;
    
	// Use this for initialization
	void Start () {
        this.rigidbody = this.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        /*if (initialTorque < 100)
        {
            this.rigidbody.AddTorque(100000.0f);
            initialTorque++;
        }
        else
        {
            print("done");
        }*/
        rot += 0.01f;
        if (rot > 360) rot = 0.0f;
        this.rigidbody.MoveRotation(rot);

        //Debug.Log(this.GetComponent<PointEffector2D>().);

        //this.rigidbody.transform.Rotate(Vector3.forward, Time.deltaTime * 0.1f);
    }
}
