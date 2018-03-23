using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject 
{
	public List<T> Items = new List<T>();

	// Indexer to allow RuntimeSet[i] without accessing Items
	public T this[int i]
	{
		get { return Items[i]; }
	}

	// Property to allow RuntimeSet.Count 
	public int Count 
	{
		get { return Items.Count; }
	}

	// TODO: Optimize this
	public void Add(T t) 
	{
		if (!Items.Contains(t))
			Items.Add(t);
	}

	// TODO: Optimize this
	public void Remove(T t)
	{
		if (Items.Contains(t))
			Items.Remove(t);
	}
}
