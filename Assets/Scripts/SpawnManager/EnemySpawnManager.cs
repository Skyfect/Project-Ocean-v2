using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objects; // Array of objects to activate

    private void Start()
    {
        ActivateRandomObject();
    }

    private void ActivateRandomObject()
    {
        if (objects.Length == 0)
        {
            Debug.LogWarning("No objects assigned to the array!");
            return;
        }

        // Deactivate all objects
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        // Activate a random object
        int randomIndex = Random.Range(0, objects.Length);
        GameObject selectedObject = objects[randomIndex];
        selectedObject.SetActive(true);

        // Debug the activated object
        Debug.Log($"Activated {selectedObject.name}");
    }
}
