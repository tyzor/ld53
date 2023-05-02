using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {get; private set; }

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] audioClips;

    private void Awake() {
        if(instance != null && instance != this)
        {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    public void PlaySound(int type)
    {
        audioSource?.PlayOneShot(audioClips[type], 0.3f);
    }

}
