using System.Collections;
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
}
