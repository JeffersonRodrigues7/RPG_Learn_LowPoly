using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class chamaMenu : MonoBehaviour
{

    bool primeira;

    float t;

    // Start is called before the first frame update
    void Start()
    {
        primeira = true;
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        print(t);
        t += Time.deltaTime;
        if (t > 45 && primeira) {
            primeira = false;
            SceneManager.LoadScene("MainMenu");
        }
    }
}


