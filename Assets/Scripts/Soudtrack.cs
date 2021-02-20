using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soudtrack : MonoBehaviour
{
    [SerializeField]
    AudioClip[] soundtracks;
    AudioSource audio_source;
    // Start is called before the first frame update
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        audio_source.clip = soundtracks[Random.Range(0, soundtracks.Length)];
        audio_source.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
