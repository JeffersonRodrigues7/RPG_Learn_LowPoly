using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeysController : MonoBehaviour
{
    public GameObject hotkeys;

    //aTIVA/DESATIVA TELA DE ATALHOS
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (hotkeys.active) hotkeys.SetActive(false);
            else hotkeys.SetActive(true);
        }
    }
}
