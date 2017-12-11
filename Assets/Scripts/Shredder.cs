using UnityEngine;

public class Shredder : MonoBehaviour
{
    const string ProjectileIdentifierTag = "Projectile";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == ProjectileIdentifierTag)
            Destroy(other.gameObject);
    }
}
