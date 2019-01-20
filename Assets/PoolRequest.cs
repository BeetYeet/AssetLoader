using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolRequest
{
	public readonly string identifier;
	public readonly int numToPool;
	public readonly float maxPoolingTime;
	public bool Prioretize
	{
		get
		{
			return maxPoolingTime == 0f;
		}
	}

	public PoolRequest( string identifier, int numToPool, float maxPoolingTime )
	{
		this.identifier = identifier;
		this.numToPool = numToPool;
		this.maxPoolingTime = Time.time + maxPoolingTime;
	}

	public PoolRequest( string identifier, int numToPool )
	{
		this.identifier = identifier;
		this.numToPool = numToPool;
		maxPoolingTime = 0f;
	}

	public PoolRequest( string identifier )
	{
		this.identifier = identifier;
		numToPool = 1;
		maxPoolingTime = 0f;
	}
}
