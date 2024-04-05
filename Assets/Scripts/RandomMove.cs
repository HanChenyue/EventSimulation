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




/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomMove : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public float frequency = 0.5f; // Controls the smoothness of the movement path
    public float neighborDetectionRadius = 2f; // Radius of the circle for detecting neighbors

    
    private Vector3 startPosition;
    private float perlinNoiseWalkX = 0f;
    private float perlinNoiseWalkZ = 0f;
    private float boundarySize = 5f; // Half the size of the boundary, for a 10x10 square

    void Start()
    {
        startPosition = transform.position; // Save start position for Perlin noise offset
        perlinNoiseWalkX = Random.Range(0f, 100f); // Random offset for X
        perlinNoiseWalkZ = Random.Range(0f, 100f); // Random offset for Z
    }

    void Update()
    {
        // Calculate Perlin noise based positions
        float xPosition = Mathf.PerlinNoise(perlinNoiseWalkX, Time.time * frequency) * 2 - 1; // Adjust Perlin noise to range between -1 and 1
        float zPosition = Mathf.PerlinNoise(perlinNoiseWalkZ, Time.time * frequency) * 2 - 1; // Adjust for Z

        Vector3 moveDirection = new Vector3(xPosition, 0, zPosition).normalized; // Normalize to ensure consistent speed
        Vector3 newPosition = transform.position + moveDirection * speed * Time.deltaTime; // Calculate new position

        // Enforce boundary conditions
        newPosition.x = Mathf.Clamp(newPosition.x, startPosition.x - boundarySize, startPosition.x + boundarySize);
        newPosition.z = Mathf.Clamp(newPosition.z, startPosition.z - boundarySize, startPosition.z + boundarySize);

        // Move the object
        transform.position = newPosition;
        
        MoveAndDetectNeighbors(); // Detect neighbors
    }
    
    void MoveAndDetectNeighbors()
    {
        // Detect neighbors
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborDetectionRadius);
        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject != this.gameObject && neighbor.gameObject.name != "Floor") // Exclude self
            {
                // neighbor.gameObject is a neighbor within the circle
                // Implement your logic here for what to do with the neighbors
                Debug.Log(neighbor.gameObject.name + " is a neighbor!");

                // Hold position for 3 seconds
                StartCoroutine(HoldPosition(3f));
                
            }
        }
    }

    IEnumerator HoldPosition(float holdTime)
    {
        speed = 0f; // Stop moving
        yield return new WaitForSeconds(holdTime); // Wait for holdTime seconds
        speed = 5f; // Resume moving
    }
}*/


// My simple version of the RandomMove script

/*
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

        DetectNeighboursAndLogInteractionStartTimesAndEndTimes(); // Call the function to track interactions with neighbors
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
                // Log when a neighboring object is detected (this part is currently commented out)
                // StartCoroutine(HoldPosition(3f)); // Call a coroutine to temporarily halt movement
            }
        }
    }

    void DetectNeighboursAndLogInteractionStartTimesAndEndTimes()
    {
        // Check for objects within the neighbor detection radius again
        Collider[] neighbors = Physics.OverlapSphere(transform.position, neighborDetectionRadius);
        var currentNeighbors = new HashSet<GameObject>(); // A temporary set to store current neighbors

        // Ensure an entry exists in the proximity tracking dictionary for this object
        if (!proximityStartTimes.ContainsKey(this.gameObject))
        {
            proximityStartTimes[this.gameObject] = new Dictionary<GameObject, float>();
        }

        // Iterate through detected neighbors
        foreach (Collider neighbor in neighbors)
        {
            if (neighbor.gameObject != this.gameObject && neighbor.gameObject.name != "Floor")
            {
                currentNeighbors.Add(neighbor.gameObject); // Add to the current neighbors set

                // If this is a new proximity interaction, record its start time
                if (!proximityStartTimes[this.gameObject].ContainsKey(neighbor.gameObject))
                {
                    proximityStartTimes[this.gameObject][neighbor.gameObject] = Time.time;
                    Debug.Log($"{Time.time:HH:mm:ss} {this.gameObject.name} and {neighbor.gameObject.name} next to one another");
                }
            }
        }

        // Check for neighbors that are no longer nearby
        foreach (var prevNeighbor in new List<GameObject>(proximityStartTimes[this.gameObject].Keys))
        {
            if (!currentNeighbors.Contains(prevNeighbor))
            {
                // Calculate the duration of the contact and log it
                var contactTime = Time.time - proximityStartTimes[this.gameObject][prevNeighbor];
                Debug.Log($"{Time.time:HH:mm:ss} {this.gameObject.name} left {prevNeighbor.name}. {this.gameObject.name} was in contact with {prevNeighbor.name} for {FormatDuration(contactTime)}");
                proximityStartTimes[this.gameObject].Remove(prevNeighbor); // Remove the neighbor from tracking
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
*/
