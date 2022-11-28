using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnComplete : MonoBehaviour
{
    public AudioSource AudioSource{ get; set; }
    void Awake()
    {
        if (TryGetComponent(out AudioSource audioSource))
        {
            AudioSource = audioSource;
        }
        else
            Debug.LogError("No AudioSource found on " + gameObject.name + transform);
    }

    public void PlayAudioOneShot(AudioClip clip)
    {
        if (AudioSource != null)
        {
            AudioSource.clip = clip;
            AudioSource.Play();
            StartCoroutine("DisableAfterAudio");
        }
        else
            Debug.LogError("No AudioSource found on" +gameObject.name + transform);
    }
    IEnumerator DisableAfterAudio()
    {
        yield return new WaitForSeconds(AudioSource.clip.length);
        AudioSource.clip = null;
    }
}