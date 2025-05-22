using UnityEngine;
using UnityEngine.Events;

public class BalanceDetector : MonoBehaviour
{
    [Header("Balance Settings")]
    public float detectionRadius = 2f;
    public int minimumStones = 3;
    public float balanceHeight = 0.5f;
    
    [Header("Events")]
    public UnityEvent onBalanceAchieved;
    public UnityEvent onBalanceLost;
    
    private bool currentlyBalanced = false;
    private ZenStone[] allStones;
    
    void Start()
    {
        // Find meditation controller to trigger completion
        MeditationController medController = FindObjectOfType<MeditationController>();
        if (medController)
        {
            onBalanceAchieved.AddListener(() => {
                Debug.Log("Beautiful balance achieved!");
            });
        }
    }
    
    void Update()
    {
        CheckBalance();
    }
    
    void CheckBalance()
    {
        allStones = FindObjectsOfType<ZenStone>();
        int stableStones = 0;
        float totalHeight = 0f;
        
        foreach (ZenStone stone in allStones)
        {
            if (Vector3.Distance(stone.transform.position, transform.position) <= detectionRadius)
            {
                if (stone.transform.position.y > balanceHeight)
                {
                    stableStones++;
                    totalHeight += stone.transform.position.y;
                }
            }
        }
        
        bool isBalanced = stableStones >= minimumStones && totalHeight > balanceHeight * minimumStones;
        
        if (isBalanced && !currentlyBalanced)
        {
            currentlyBalanced = true;
            onBalanceAchieved.Invoke();
        }
        else if (!isBalanced && currentlyBalanced)
        {
            currentlyBalanced = false;
            onBalanceLost.Invoke();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.up * balanceHeight, 
                           new Vector3(detectionRadius * 2, 0.1f, detectionRadius * 2));
    }
}