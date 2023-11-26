using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerRPG : MonoBehaviour
{
    public string sceneName;

    public void loadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
