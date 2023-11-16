using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace RPG.CinemachineCamera
{
    public class CinemachineFreeLookOnlyWhenRightMouseDown : MonoBehaviour
    {
        void Start()
        {
            // Substitui o m�todo padr�o de entrada do Cinemachine para personalizar a rota��o da c�mera.
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
            {
                // Se o bot�o direito do mouse est� pressionado, use a entrada do mouse X; caso contr�rio, retorne 0.
                if (Input.GetMouseButton(1))
                {
                    return UnityEngine.Input.GetAxis("Mouse X");
                }
                else
                {
                    return 0;
                }
            }
            else if (axisName == "Mouse Y")
            {
                // Se o bot�o direito do mouse est� pressionado, use a entrada do mouse Y; caso contr�rio, retorne 0.
                if (Input.GetMouseButton(1))
                {
                    return UnityEngine.Input.GetAxis("Mouse Y");
                }
                else
                {
                    return 0;
                }
            }
            // Se n�o for "Mouse X" ou "Mouse Y", retorne a entrada padr�o.
            return UnityEngine.Input.GetAxis(axisName);
        }
    }
}