using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CinemachineCamera
{
    public class ShakeCamera : MonoBehaviour
    {
        [SerializeField] private float frequencyDefault; // Frequ�ncia padr�o do shake (n�o est� sendo usada no c�digo)
        [SerializeField] private float decreaseTime; // Tempo para diminuir a intensidade do shake

        private CinemachineFreeLook freeLookCamera; // Refer�ncia para o componente CinemachineFreeLook

        float magnitude; // Intensidade do shake
        bool increase; // Indica se o shake est� aumentando

        public void Start()
        {
            // Encontrar a inst�ncia do CinemachineFreeLook pelo nome
            freeLookCamera = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();

            increase = false;
        }

        public void Update()
        {
            if (increase)
            {
                // Configurar os valores de amplitude e frequ�ncia do shake no componente CinemachineBasicMultiChannelPerlin
                freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = magnitude;
                freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = magnitude;

                // Diminuir a intensidade ao longo do tempo
                magnitude = Mathf.Clamp(magnitude - decreaseTime * Time.deltaTime, 0, Mathf.Infinity);

                if (magnitude <= 0.0f)
                {
                    // Resetar os valores para zero quando a intensidade se torna zero
                    freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                    freeLookCamera.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
                    increase = false;
                }
            }
        }

        public void startShaking(float maxMagnitude)
        {
            // Iniciar o shake configurando a intensidade m�xima e indicando para aumentar
            magnitude = maxMagnitude;
            increase = true;
        }
    }
}
