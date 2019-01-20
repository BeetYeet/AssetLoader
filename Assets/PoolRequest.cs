using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolRequest
{
	public readonly string identifier;
	public readonly int numToPool;
	public readonly float heavyness;


	public PoolRequest( string identifier, int numToPool )
	{
		this.identifier = identifier;
		this.numToPool = numToPool;
	}

	public PoolRequest( string identifier )
	{
		this.identifier = identifier;
		numToPool = 1;
	}
}
