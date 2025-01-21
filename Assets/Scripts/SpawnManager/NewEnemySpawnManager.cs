using Biostart.DayNight;
using UnityEngine;

public class NewEnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToActivate;
    [Range(0, 1)]
    [SerializeField] private float activationStartTime = 0.2f; 

    private DayNightCycle dayNightCycle;

    private bool objectsActivated = false; 

    private void Start()
    {
    
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        if (dayNightCycle == null)
        {
            Debug.LogError("DayNightCycle script not found in the scene!");
            return;
        }

       
        InvokeRepeating("CheckAndActivateObjects", 0f, 1f);
    }

    private void CheckAndActivateObjects()
    {
        if (dayNightCycle == null) return;

        
        float currentTime = dayNightCycle.currentTimeOfDay;

        if (currentTime >= activationStartTime && !objectsActivated)
        {
            ActivateObjects();
            objectsActivated = true; 
        }
    }

    private void ActivateObjects()
    {
        // Loop through each object and set active state to true
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Activated: {obj.name}");
            }
        }
    }
}
