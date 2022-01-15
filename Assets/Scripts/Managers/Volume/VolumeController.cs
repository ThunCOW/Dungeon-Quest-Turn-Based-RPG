using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeController : MonoBehaviour
{
    [SerializeField] private string VolumeParameter = "MasterVolume";
    [SerializeField] AudioMixer audioMixer = null;
    [SerializeField] Slider volumeSlider = null;
    //[SerializeField] float multiplier = 30f;
    
    [Space]
    [Range(0, 30)]
    [SerializeField] private int volume;

    [Range(0, 10)]
    [SerializeField] private int shownValue;
    void Awake()
    {
        audioMixer.GetFloat("MasterVolume", out StaticClass.MasterVolume);
        audioMixer.GetFloat("BackgroundVolume", out StaticClass.BackgroundVolume);
        audioMixer.GetFloat("SoundEffectVolume", out StaticClass.SoundEffectVolume);

        volumeSlider.onValueChanged.AddListener(HandleSliderValueChanged);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleSliderValueChanged(float value)
    {
        audioMixer.SetFloat(VolumeParameter, value);

        audioMixer.GetFloat("MasterVolume", out StaticClass.MasterVolume);
        audioMixer.GetFloat("BackgroundVolume", out StaticClass.BackgroundVolume);
        audioMixer.GetFloat("SoundEffectVolume", out StaticClass.SoundEffectVolume);
    }
    /*
    1 = 20
    0.5 = 0
    0 = -20
        0.1 = 4
        0.025 = 1
    */
}
