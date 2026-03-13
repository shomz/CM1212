using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource Source;
    [SerializeField] private AudioClip FlipSound;
    [SerializeField] private AudioClip MatchSound;
    [SerializeField] private AudioClip MismatchSound;
    [SerializeField] private AudioClip WinSound;


    public void PlayFlip()
    {
        Source.PlayOneShot(FlipSound);
    }

    public void PlayMatch()
    {
        Source.PlayOneShot(MatchSound);
    }

    public void PlayMismatch()
    {
        Source.PlayOneShot(MismatchSound);
    }

    public void PlayVictory()
    {
        Source.PlayOneShot(WinSound);
    }
}
