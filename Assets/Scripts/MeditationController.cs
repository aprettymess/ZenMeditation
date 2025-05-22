using UnityEngine;
using System.Collections;

public class MeditationController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip welcomeClip;
    public AudioClip[] breathingGuides;
    public AudioClip completionClip;
    public AudioClip ambientSounds;
    
    [Header("Audio Sources")]
    public AudioSource meditationVoice;
    public AudioSource ambientAudio;
    
    [Header("Settings")]
    public float guidanceInterval = 5f; // Time between guidance clips
    public bool autoStart = true;
    
    private int currentGuideIndex = 0;
    private bool sessionActive = false;
    
    void Start()
    {
        // Set up ambient audio
        if (ambientAudio && ambientSounds)
        {
            ambientAudio.clip = ambientSounds;
            ambientAudio.Play();
        }
        
        if (autoStart)
        {
            StartCoroutine(DelayedStart());
        }
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(8f); 
        StartMeditationSession();
    }

    public void StartMeditationSession()
    {
        sessionActive = true;
        StartCoroutine(MeditationSequence());
    }
    
    IEnumerator MeditationSequence()
    {
        // Play welcome message
        if (welcomeClip)
        {
            PlayVoiceClip(welcomeClip);
            yield return new WaitForSeconds(welcomeClip.length + 3f);
        }
        
        // Main meditation loop
        while (sessionActive)
        {
            if (currentGuideIndex < breathingGuides.Length)
            {
                PlayVoiceClip(breathingGuides[currentGuideIndex]);
                currentGuideIndex++;
                yield return new WaitForSeconds(guidanceInterval);
            }
            else
            {
                // Session complete
                if (completionClip)
                {
                    PlayVoiceClip(completionClip);
                }
                sessionActive = false;
            }
        }
    }
    
    void PlayVoiceClip(AudioClip clip)
    {
        if (meditationVoice && clip)
        {
            meditationVoice.clip = clip;
            meditationVoice.Play();
        }
    }
    
    public void RestartSession()
    {
        StopAllCoroutines();
        currentGuideIndex = 0;
        sessionActive = false;
        StartMeditationSession();
    }
}