using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider effectsSlider;
    public Slider masterSliderESP;
    public Slider musicSliderESP;
    public Slider effectsSliderESP;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(SetMaterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
        masterSliderESP.onValueChanged.AddListener(SetMaterVolume);
        musicSliderESP.onValueChanged.AddListener(SetMusicVolume);
        effectsSliderESP.onValueChanged.AddListener(SetEffectsVolume);
    }
    public void SetMaterVolume(float sliderValue)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetEffectsVolume(float sliderValue)
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(sliderValue) * 20);
    }
}
