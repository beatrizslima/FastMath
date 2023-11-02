using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
     [Header("Sliders")]
 
    [SerializeField] Slider SliderMusica;
    [SerializeField] Slider SliderEfeitos;

    [Header("Audios")]
    [SerializeField] List<AudioSource> Efeitos = new List<AudioSource>();
    [SerializeField] AudioSource Musica;

    // Start is called before the first frame update
    void Start()
    {
        SliderMusica.maxValue = 1;
        SliderMusica.minValue = 0;

        SliderEfeitos.maxValue = 1;
        SliderEfeitos.minValue = 0;

        LoadPlayerPrefsVolumes();

        SliderMusica.onValueChanged.RemoveAllListeners();
        SliderEfeitos.onValueChanged.RemoveAllListeners();

        SliderMusica.onValueChanged.AddListener((float x) => VolumeMusica());
        SliderEfeitos.onValueChanged.AddListener((float x) => VolumeEfeitos());
    }

    public void VolumeMusica()
    {
        PlayerPrefs.SetFloat("VolumeMusica", SliderMusica.value);
        //Debug.Log(SliderMusica.value);
        if(Musica != null)
        {
            Musica.volume = SliderMusica.value;
        }
    }

    public void VolumeEfeitos()
    {
        PlayerPrefs.SetFloat("VolumeEffect", SliderEfeitos.value);

        foreach (AudioSource efeito in Efeitos)
        {
            efeito.volume = SliderEfeitos.value;
        }
    }

    public void LoadPlayerPrefsVolumes()
    {
        if (!PlayerPrefs.HasKey("VolumeEffect"))
        {
            PlayerPrefs.SetFloat("VolumeEffect", 0.5f);
        }

        if (!PlayerPrefs.HasKey("VolumeMusica"))
        {
            PlayerPrefs.SetFloat("VolumeMusica", 0.5f);
        }
        
        SliderMusica.value = PlayerPrefs.GetFloat("VolumeMusica");
        VolumeMusica();

        SliderEfeitos.value = PlayerPrefs.GetFloat("VolumeEffect");
        VolumeEfeitos();
    }
}
