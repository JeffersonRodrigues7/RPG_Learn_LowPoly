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
            // Substitui o método padrão de entrada do Cinemachine para personalizar a rotação da câmera.
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
            {
                // Se o botão direito do mouse está pressionado, use a entrada do mouse X; caso contrário, retorne 0.
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
                // Se o botão direito do mouse está pressionado, use a entrada do mouse Y; caso contrário, retorne 0.
                if (Input.GetMouseButton(1))
                {
                    return UnityEngine.Input.GetAxis("Mouse Y");
                }
                else
                {
                    return 0;
                }
            }
            // Se não for "Mouse X" ou "Mouse Y", retorne a entrada padrão.
            return UnityEngine.Input.GetAxis(axisName);
        }
    }
}