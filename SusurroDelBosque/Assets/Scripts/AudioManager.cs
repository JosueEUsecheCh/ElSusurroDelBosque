using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource audioSource;
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Debug.Log("COÑO ALGO PASO");
        }
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void ReproducirSonido(AudioClip audio)
    {
        if (audioSource.clip != audio || !audioSource.isPlaying)
        {

            audioSource.clip = audio;
            audioSource.PlayOneShot(audio);
        }
    }
    
    


}
