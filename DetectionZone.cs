using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Add this for UnityEvent

public class DetectionZone : MonoBehaviour
{
    // Event to trigger when no colliders remain in the zone
    public UnityEvent noColliderRemain;

    // List to store colliders currently in the zone
    public List<Collider2D> collidersInZone = new List<Collider2D>();

    // Reference to the Collider2D component
    private Collider2D detectionCol;

    // Count of colliders in the zone
    public int colliderCount;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Collider2D component attached to this GameObject
        detectionCol = GetComponent<Collider2D>();
    }

    // Called when another Collider2D enters the trigger zone
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Add the collider to the list
        collidersInZone.Add(collider);

        // Update the collider count
        colliderCount = collidersInZone.Count;
    }

    // Called when another Collider2D exits the trigger zone
    private void OnTriggerExit2D(Collider2D collider)
    {
        // Remove the collider from the list
        collidersInZone.Remove(collider);

        // Update the collider count
        colliderCount = collidersInZone.Count;

        // If no colliders remain, invoke the event
        if (collidersInZone.Count <= 0)
        {
            noColliderRemain.Invoke();
        }
    }
}
