using UnityEngine;

public class TankController : MonoBehaviour
{
    private Rigidbody tankRigidbody;

    void Start()
    {
        tankRigidbody = GetComponent<Rigidbody>();

        // Erstellen eines neuen Physics Materials mit höherer Reibung
        PhysicMaterial highFrictionMaterial = new PhysicMaterial();
        highFrictionMaterial.name = "HighFriction";
        highFrictionMaterial.staticFriction = 1.0f;
        highFrictionMaterial.dynamicFriction = 1.0f;

        // Zuweisen des Materials zum Collider des Panzers
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.material = highFrictionMaterial;
        }
    }

}