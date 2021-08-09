using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniAudio : MonoBehaviour
{
    AudioSource source;
    PlayerController player;
    AudioClip[] footsteps;
    AudioClip[] swimClips;

    // Start is called before the first frame update
    void Start()
    {
        source = this.GetComponent<AudioSource>();
        player = this.GetComponentInParent<PlayerController>();
        footsteps = player.footstepClips;
        swimClips = player.swimSound;
    }

    void Step()
    {
        if (player._grounded && !player._inWater)
        {
            source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
        }
    }

    void Swim()
    {
        source.PlayOneShot(swimClips[Random.Range(0, swimClips.Length)]);
    }
}
