using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic_Events : RhythmListener
{
    public delegate void BGMEvents();
    public static event BGMEvents BGM_Events;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (TryGetComponent(out AudioSource audioSource))
        {
            _audioSource = audioSource;
        }
        else
            Debug.LogError("No AudioSource found on " + gameObject.name + transform);
    }
    private void Start()
    {
        //listen for Audio Update.
        AudioManager.UpdateMusicVolumeEvent += UpdateMusicVolume;
    }
    private void UpdateMusicVolume(float musicVolume)
    {
        _audioSource.volume = musicVolume;
    }
    public override void BPMEvent(RhythmEventData data)
    {
        throw new System.NotImplementedException();
    }

    public override void RhythmEvent(RhythmEventData data)
    {
        //Note: layerName keeps resetting to default Channel 0_0
        if (data.layer.layerName == "Channel 0_0")
        {
            data.layer.layerName = "BaseBeat";
        }
        if (data.layer.layerName == "BaseBeat")
        {
            BGM_Events?.Invoke();
        }
    }
    private void OnDisable()
    {
        AudioManager.UpdateMusicVolumeEvent -= UpdateMusicVolume;
    }
}
