using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooler: MonoBehaviour
{

	public int score;

	private Queue<PoolRequest> toDo;
	private Queue<PoolRequest> requests;
	private PoolRequest request;
	public Dictionary<string, GameObject> PoolingReferences;
	public Dictionary<string, List<GameObject>> pool;

	public static Pooler curr;

	private void Awake()
	{
		if ( curr != null )
			throw new System.Exception( "Cannot have multiple instances of Pooler" );
		curr = this;
		toDo = new Queue<PoolRequest>();
		requests = new Queue<PoolRequest>();
	}


	private void ProcessPool()
	{
		if ( request == null )
		{
			int more = toDo.Count;
			if ( more == 0 )
			{
				if ( requests.Count == 0 )
				{
					return;
				}
				else
				{
					toDo.Enqueue( requests.Dequeue() );
				}
			}

			// there are requests
			request = toDo.Dequeue();
		}

		if ( request.Prioretize )
		{
			SpawnObjects( request.identifier, request.numToPool );
		}
		else
		{
			float timeLeft = request.maxPoolingTime - Time.time;
			if ( timeLeft < .2f )
			{
				SpawnObjects( request.identifier, request.numToPool );
				return;
			}
			int poolNow = request.numToPool;
			SpawnObjects( request.identifier, poolNow );
		}

	}

	public void SpawnObjects( string identifier, int count )
	{

		GameObject go = PoolingReferences[identifier];
		if ( pool[identifier] == null )
		{
			pool.Add( identifier, new List<GameObject>() );
		}
		for ( int i = 0; i < count; i++ )
		{
			GameObject obj = Instantiate( go );
			obj.SetActive( false );
			pool[identifier].Add( obj );
		}
	}

	private void Update()
	{
		ProcessPool();
	}
}
