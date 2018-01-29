using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDummy : MonoBehaviour {

	private Vector3 originalPosition;
	private Rigidbody rb;

	void Start() 
	{
		rb = GetComponent<Rigidbody>();
		originalPosition = transform.position;
	}

	public void ResetPosition() 
	{
		rb.velocity = Vector3.zero;
		transform.position = originalPosition;
	}
}
