using UnityEngine;
using System.Collections;

public class AstranautController : MonoBehaviour {

    static int atakState = Animator.StringToHash("RunRight");
  
    // Use this for initialization
    void Start () {
	
	}
    float d = 0.0f;
	// Update is called once per frame
	void Update () {
        d = Input.GetAxis("Horizontal");
        
        this.GetComponent<Animator>().SetFloat("Run", d);

        if (d < 0)
        {
            this.transform.Translate(Vector3.back * d * 0.2f);
            this.transform.rotation = Quaternion.AngleAxis(-90.0f, Vector3.up);
        }
        if (d > 0)
        {
            this.transform.Translate(Vector3.forward * d * 0.2f);
            this.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.up);
        }
        if (d == 0.0f)
        {
            this.GetComponent<Animator>().CrossFade("Idle", 0.0f);
        }
    }
}
