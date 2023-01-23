using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip[] rallyAudio;
    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
    }

    public void Footstep()
    {
        int footstep = Random.Range(0, footsteps.Length);
        audioSource.PlayOneShot(footsteps[footstep]);
    }
    public void RallyAudio()
    {
        int rallyClip = Random.Range(0, rallyAudio.Length);
        audioSource.PlayOneShot(rallyAudio[rallyClip], 0.5f);
    }
}
