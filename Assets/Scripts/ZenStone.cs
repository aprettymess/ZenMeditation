using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ZenStone : MonoBehaviour
{
    [Header("Physics Settings")]
    public float stabilityThreshold = 0.1f;
    public float settleTime = 2f;
    public AudioClip[] stoneSounds;
    
    [Header("Visual Feedback")]
    public Material normalMaterial;
    public Material glowMaterial;
    
    private Rigidbody rb;
    private AudioSource audioSource;
    private XRGrabInteractable grabInteractable;
    private Renderer stoneRenderer;
    private bool isStable = false;
    private float stableTimer = 0f;
    private Vector3 lastPosition;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        stoneRenderer = GetComponent<Renderer>();
        
        // Add audio source if not present
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.volume = 0.5f;
        }
        
        // Set up grab events
        if (grabInteractable)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        
        lastPosition = transform.position;
    }
    
    void Update()
    {
        CheckStability();
    }
    
    void CheckStability()
    {
        if (rb && !grabInteractable.isSelected)
        {
            float movement = Vector3.Distance(transform.position, lastPosition);
            
            if (movement < stabilityThreshold)
            {
                stableTimer += Time.deltaTime;
                if (stableTimer >= settleTime && !isStable)
                {
                    BecomeStable();
                }
            }
            else
            {
                stableTimer = 0f;
                if (isStable)
                {
                    BecomeUnstable();
                }
            }
            
            lastPosition = transform.position;
        }
    }
    
    void BecomeStable()
    {
        isStable = true;
        // Subtle glow effect when stable
        if (glowMaterial && stoneRenderer)
        {
            stoneRenderer.material = glowMaterial;
        }
        
        // Play gentle settling sound
        if (stoneSounds.Length > 0 && audioSource)
        {
            audioSource.PlayOneShot(stoneSounds[Random.Range(0, stoneSounds.Length)]);
        }
    }
    
    void BecomeUnstable()
    {
        isStable = false;
        if (normalMaterial && stoneRenderer)
        {
            stoneRenderer.material = normalMaterial;
        }
    }
    
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isStable = false;
        stableTimer = 0f;
        if (normalMaterial && stoneRenderer)
        {
            stoneRenderer.material = normalMaterial;
        }
    }
    
    void OnReleased(SelectExitEventArgs args)
    {
        stableTimer = 0f;
        lastPosition = transform.position;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Play collision sound based on impact force
        if (stoneSounds.Length > 0 && audioSource && collision.relativeVelocity.magnitude > 0.5f)
        {
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 5f);
            audioSource.volume = volume * 0.3f; // Keep it subtle
            audioSource.PlayOneShot(stoneSounds[Random.Range(0, stoneSounds.Length)]);
        }
    }
}