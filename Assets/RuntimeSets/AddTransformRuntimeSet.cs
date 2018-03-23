using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTransformRuntimeSet : MonoBehaviour {

	[SerializeField] TransformRuntimeSet runtimeSet;

	void OnEnable() 
	{
		runtimeSet.Add(this.transform);
	}

	void OnDisable() 
	{
		runtimeSet.Remove(this.transform);
	}
}
