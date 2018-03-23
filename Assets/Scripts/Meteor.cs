using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Meteor : MonoBehaviour 
{
	[SerializeField] float movementSpeed = 10f;
	[SerializeField] float rotationSpeed = 5f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () 
	{
		transform.rotation = Random.rotation;
        transform.localScale *= Random.Range(0.85f, 1.15f);

		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.down * movementSpeed;
		rb.angularVelocity = Random.onUnitSphere * rotationSpeed;
	}

	void OnCollisionEnter() 
	{
		// TODO: Explode meteor and spawn rubble
	}


}
