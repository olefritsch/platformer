using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TagManager.ProjectileIdentifierTag)
            Destroy(other.gameObject);
    }
}
