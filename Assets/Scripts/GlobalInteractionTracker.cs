// GlobalInteractionTracker.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GlobalInteractionTracker : MonoBehaviour
{
    public static GlobalInteractionTracker Instance;

    // Dictionary to track interactions and start times
    private Dictionary<GameObject, HashSet<GameObject>> currentInteractions = new Dictionary<GameObject, HashSet<GameObject>>();
    private Dictionary<(GameObject, GameObject), float> interactionStartTimes = new Dictionary<(GameObject, GameObject), float>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void UpdateInteractions(GameObject entity, HashSet<GameObject> currentNeighbors)
    {
        // Ensure the entity has an entry in the dictionary.
        if (!currentInteractions.ContainsKey(entity))
        {
            currentInteractions[entity] = new HashSet<GameObject>();
        }

        // Copy the previous neighbors to a new HashSet to avoid modifying the original during iteration.
        var previousNeighbors = new HashSet<GameObject>(currentInteractions[entity]);

        // Handle new interactions.
        foreach (var neighbor in currentNeighbors)
        {
            if (!previousNeighbors.Contains(neighbor))
            {
                interactionStartTimes[(entity, neighbor)] = Time.time;
                Debug.Log($"{FormatTime(Time.time)} {entity.name} and {neighbor.name} are in an interaction");
                currentInteractions[entity].Add(neighbor); // Add to the current interactions.
            }
        }

        // Handle ended interactions.
        foreach (var oldNeighbor in previousNeighbors)
        {
            if (!currentNeighbors.Contains(oldNeighbor))
            {
                var startTime = interactionStartTimes[(entity, oldNeighbor)];
                var duration = Time.time - startTime;
                Debug.Log($"{FormatTime(Time.time)} {entity.name} leaves {oldNeighbor.name}. {entity.name} interacted with {oldNeighbor.name} for {FormatDuration(duration)}");
                interactionStartTimes.Remove((entity, oldNeighbor));
                currentInteractions[entity].Remove(oldNeighbor); // Remove from the current interactions.
            }
        }
    }

    private string FormatTime(float time)
    {
        return System.DateTime.UtcNow.AddSeconds(time - Time.time).ToString("HH:mm:ss");
    }

    private string FormatDuration(float duration)
    {
        var timeSpan = System.TimeSpan.FromSeconds(duration);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
}
