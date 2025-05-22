using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class StoneSpawner : MonoBehaviour
{
    [Header("Stone Settings")]
    public GameObject stonePrefab;
    public Transform spawnPoint;
    public int maxStones = 10;
    public float spawnHeight = 2f;
    
    private int currentStoneCount = 0;
    
    void Start()
    {
        // Spawn initial stones
        SpawnInitialStones();
    }
    
    void SpawnInitialStones()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnStone();
        }
    }
    
    public void SpawnStone()
    {
        if (currentStoneCount >= maxStones) return;
        
        Vector3 spawnPosition = spawnPoint.position + new Vector3(
            Random.Range(-1f, 1f), 
            spawnHeight, 
            Random.Range(-1f, 1f)
        );
        
        GameObject newStone = Instantiate(stonePrefab, spawnPosition, Random.rotation);
        currentStoneCount++;
        
        // Add listener for when stone is grabbed
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = newStone.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnStoneReleased);
        }
    }
    
    private void OnStoneReleased(SelectExitEventArgs args)
    {
        // Optional: Add sound effect or haptic feedback here
    }
}