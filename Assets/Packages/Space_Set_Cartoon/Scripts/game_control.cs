using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class game_control : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

  
    
        // Update is called once per frame
        void Update()
    {
        if(Input.anyKeyDown)
        {
            Scene scene = SceneManager.GetActiveScene();
            switch (scene.buildIndex)
            {
                case 0: SceneManager.LoadScene(1); break;
                case 1: SceneManager.LoadScene(2); break;
                case 2: SceneManager.LoadScene(3); break;
                case 3: SceneManager.LoadScene(4); break;
                case 4: SceneManager.LoadScene(5); break;
                case 5: SceneManager.LoadScene(0); break;

            }
        }
    }
}
