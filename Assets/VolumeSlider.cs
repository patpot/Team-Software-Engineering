using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    public List<Slider> Sliders;
    public AudioMixer Mixer;

    private void Start()
        => Sliders.ForEach(sl => sl.onValueChanged.AddListener(SetVolume));
    public void SetVolume(float value)
    {
        Mixer.SetFloat("MasterVolume", value);
        Sliders.ForEach(sl => sl.value = value);
    }
}
