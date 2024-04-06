using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomMove : MonoBehaviour
{
    // Public variables that can be adjusted in the Unity editor
    public float speed = 5f; // Controls the speed of the object's movement.
    public float frequency = 0.5f; // Adjusts the rate at which the object changes direction.
    public float neighborDetectionRadius = 2f; // The radius within which this object will detect neighboring objects.

    // Private variables for internal use
    private Vector3 startPosition; // To remember the initial position of the object.
    private float perlinNoiseWalkX = 0f; // Perlin noise offset for X-axis movement.
    private float perlinNoiseWalkZ = 0f; // Perlin noise offset for Z-axis movement.
    private float boundarySize = 5f; // Defines a boundary area for movement to keep the object within a specific area.

    // A nested dictionary to track when each object starts being next to another object
    private Dictionary<GameObject, Dictionary<GameObject, float>> proximityStartTimes = new Dictionary<GameObject, Dictionary<GameObject, float>>();

    void Start()
    {
        startPosition = transform.position; // Store the object's starting position.
        perlinNoiseWalkX = Random.Range(0f, 100f); // Initialize a random value for X-axis Perlin noise.
        perlinNoiseWalkZ = Random.Range(0f, 100f); // Initialize a random value for Z-axis Perlin noise.
    }

    void Update()
    {
        // Use Perlin noise to create a smooth, random movement path
        float xPosition = Mathf.PerlinNoise(perlinNoiseWalkX, Time.time * frequency) * 2 - 1; // Compute X-axis position
        float zPosition = Mathf.PerlinNoise(perlinNoiseWalkZ, Time.time * frequency) * 2 - 1; // Compute Z-axis position

        // Combine the computed X and Z positions into a movement direction, and normalize it to maintain consistent speed
        Vector3 moveDirection = new Vector3(xPosition, 0, zPosition).normalized;
        Vector3 newPosition = transform.position + moveDirection * speed * Time.deltaTime; // Calculate new position

        // Keep the object within the defined boundary
        newPosition.x = Mathf.Clamp(newPosition.x, startPosition.x - boundarySize, startPosition.x + boundarySize);
        newPosition.z = Mathf.Clamp(newPosition.z, startPosition.z - boundarySize, startPosition.z + boundarySize);

        transform.position = newPosition; // Update the object's position

        MoveAndDetectNeighbors(); // Call the function to detect nearby objects

        DetectAndProcessInteractions(); // Call the function to track interactions with other entit(ies)
    }

    private void DetectAndProcessInteractions()
    {
        // Set to store the current neighbours of this entity.
        HashSet<GameObject> currentNeighbors = new HashSet<GameObject>();
        
        // Check for other entities within the neighbour detection radius.
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborDetectionRadius);
        foreach (Collider neighbor in neighbors)
        {
            // Ensure we don't detect ourselves or the 'Floor' object.
            if (neighbor.gameObject != this.gameObject && neighbor.gameObject.name != "Floor")
            {
                currentNeighbors.Add(neighbor.gameObject); // Add the neighbour to the current neighbours set.
            }
        }
        
        // Update the GlobalInteractionTracker.cs with the current set of neighbours.
        GlobalInteractionTracker.Instance.UpdateInteractions(this.gameObject, currentNeighbors);
    }

    void MoveAndDetectNeighbors()
    {
        // Check for objects within the neighbor detection radius
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborDetectionRadius);
        foreach (Collider neighbor in neighbors)
        {
            // Ensure we don't detect ourselves or the 'Floor' object
            if (neighbor.gameObject != this.gameObject && neighbor.gameObject.name != "Floor")
            {
                // Log when a neighboring object is detected generate a rondam hold duration between 3 to 7 seconds
                float randomHoldDuration = Random.Range(3f, 7f);
                StartCoroutine(HoldPosition(randomHoldDuration)); // Call a coroutine to temporarily halt movement
            }
        }
    }

    // Converts a duration in seconds to a formatted time string
    string FormatDuration(float duration)
    {
        var timeSpan = System.TimeSpan.FromSeconds(duration);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }

    // Coroutine to temporarily stop the object's movement
    IEnumerator HoldPosition(float holdTime)
    {
        speed = 0f; // Reduce speed to zero, stopping the object
        yield return new WaitForSeconds(holdTime); // Wait for the specified duration
        speed = 5f; // Resume normal speed
    }
}
