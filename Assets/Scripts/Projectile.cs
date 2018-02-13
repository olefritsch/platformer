using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float initialSpeed = 200f;
    [SerializeField] float lifetime = 5f;
	[SerializeField] float deathtime = 0.5f;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(this.transform.up * initialSpeed);
		StartCoroutine(Die());
    }

	private IEnumerator Die() 
	{
		yield return new WaitForSeconds(lifetime);

		float i = 0;

		while (i < 1) 
		{
			this.transform.localScale = Vector3.Lerp(this.transform.localScale, Vector3.zero, i);
			i += Time.deltaTime / deathtime;
			yield return null;
		}

		Destroy(this.gameObject);
	}

}

