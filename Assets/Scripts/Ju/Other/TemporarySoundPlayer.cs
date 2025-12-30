using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class TemporarySoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public string ClipName { get { return audioSource.clip.name; } }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioMixerGroup audioMixer, float delay, bool isLoop)
    {
        audioSource.outputAudioMixerGroup = audioMixer;
        audioSource.loop = isLoop;
        audioSource.Play();

        if(!isLoop)
        {
            //여기에 코루틴
            StartCoroutine(CoDestroyWhenFinish(audioSource.clip.length));
        }
    }

    public void InitSound(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    private IEnumerator CoDestroyWhenFinish(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        Destroy(gameObject);
    }
}
