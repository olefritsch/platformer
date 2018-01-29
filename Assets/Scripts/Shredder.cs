using UnityEngine;

public class Shredder : MonoBehaviour
{
    public delegate void OnPlayerDeath(int playerId);
    public static OnPlayerDeath PlayerDeath;

    private void OnTriggerEnter(Collider other)
    {
		if (other.transform.root.tag == TagManager.ProjectileIdentifierTag)
			Destroy (other.gameObject);
		else if (other.transform.root.tag == TagManager.PlayerIdentifierTag) 
		{
			PlayerController player = other.transform.root.GetComponent<PlayerController>();
			PlayerDeath (player.playerId);
		}
		else if (other.transform.root.tag == TagManager.DummyIdentifierTag) 
		{
			other.transform.root.GetComponent<PlayerDummy>().ResetPosition();
		}
    }
}
