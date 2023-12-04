using Assets.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    private List<AudioClip> clips;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clips = Resources.LoadAll<AudioClip>("mp3").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.clip = clips.PickRandom();
            audioSource.Play();
        }
    }
}
