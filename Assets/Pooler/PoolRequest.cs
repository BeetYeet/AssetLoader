using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolRequest
{
	public readonly string identifier;
	public int numToPool;
	public readonly float heavyness;


	public PoolRequest( string identifier, int numToPool, float heavyness )
	{
		this.identifier = identifier;
		this.numToPool = numToPool;
		this.heavyness = heavyness;
	}

	public PoolRequest( string identifier, int numToPool )
	{
		this.identifier = identifier;
		this.numToPool = numToPool;
		heavyness = CalcHeavyness( identifier );
	}

	public PoolRequest( string identifier, float heavyness )
	{
		this.identifier = identifier;
		this.heavyness = heavyness;
		numToPool = 1;
	}

	public PoolRequest( string identifier )
	{
		this.identifier = identifier;
		heavyness = CalcHeavyness( identifier );
		numToPool = 1;
	}

	private float CalcHeavyness( string identifier )
	{
		if ( Pooler.curr.poolingReferences[identifier] == null )
		{
			throw new System.NullReferenceException();
		}
		return Pooler.CalculateHeavyness( Pooler.curr.poolingReferences[identifier] ).heavyness;
	}
}
