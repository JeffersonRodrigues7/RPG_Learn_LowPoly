using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider; // Arraste o objeto Slider para esta variável no Inspector

    private void Start()
    {
        // Adiciona um listener para chamar a função UpdateVolume quando o valor do slider mudar
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    // Função chamada quando o valor do slider muda
    private void UpdateVolume(float volume)
    {
        // Altera o volume do áudio do jogo usando o valor do slider (0 a 1)
        AudioListener.volume = volume;
    }
}
