using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefabs : MonoBehaviour 
{
	[SerializeField] GameObject prefab;

	[SerializeField] float startDelay;
	[SerializeField] float spawnDelay;
	[SerializeField] float spawnOffset;

	[Range(1, 50)]
	[SerializeField] int maxNumberOfPrefabs;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(MakeItRain ());
	}
	
	// Update is called once per frame
	IEnumerator MakeItRain() 
	{
		yield return new WaitForSeconds(startDelay);

		int numberOfPrefabs = 0;

		while (numberOfPrefabs < maxNumberOfPrefabs) 
		{
			if (Random.value > 0.5f) 
			{
				Vector3 spawnPos = transform.position + new Vector3(Random.Range(-spawnOffset, spawnOffset), 0f, 0f);
				Instantiate(prefab, spawnPos, Quaternion.identity, transform);
				numberOfPrefabs++;
			}

			yield return new WaitForSeconds(spawnDelay);
		}
	}
}
