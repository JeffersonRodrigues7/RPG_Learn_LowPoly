using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scroolTexto : MonoBehaviour
{
    // velocidade de scroll na 'distância'
    public float scrollSpeed = 8;
    public float callNextSceneTime = 130;
    public SceneManagerRPG sceneManagerRPG;
    public string nextSceneName = "Vila";

    private void Start()
    {
        StartCoroutine(callVillage());
    }

    // Update is called once per frame
    void Update()
    {
        // pega a posição atual GameObject pai
        Vector3 pos = transform.position;

        // posiciona o vetor apontando para a distância
        Vector3 localVectorUp = transform.TransformDirection(0,1,0);
 
        // move o objeto de texto na distância para o efeito e scrolling 3D
        pos += localVectorUp * scrollSpeed *Time.deltaTime;
        transform.position = pos;
    }

    public IEnumerator callVillage()
    {
        yield return new WaitForSeconds(callNextSceneTime);
        sceneManagerRPG.loadScene(nextSceneName);
    }
}
