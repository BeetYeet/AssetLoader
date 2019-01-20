using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Pooler: MonoBehaviour
{
	private const float maxExpectedDeltatime = .2f;
	private const float maxExpectedTotalHeavyness = 5f; // in milliseconds
	private const float emptyAdjustment = .1f; // how much extra is pooled if theres not enough


	private Queue<PoolRequest> requests; // stores all spawning requests
	private PoolRequest request; // the current request
	public Dictionary<string, GameObject> poolingReferences;
	public Dictionary<string, int> expectedCount;
	public Dictionary<string, List<GameObject>> pool;

	public static Pooler curr;

	private void Awake()
	{
		if ( curr != null )
			throw new System.Exception( "Cannot have multiple instances of Pooler" );
		curr = this;
		requests = new Queue<PoolRequest>();
		poolingReferences = new Dictionary<string, GameObject>();
		pool = new Dictionary<string, List<GameObject>>();
		expectedCount = new Dictionary<string, int>();
	}

	public GameObject GetFromPool( string identifier )
	{
		if ( !pool.ContainsKey( identifier ) )
		{
			throw new System.NullReferenceException();
		}
		if ( pool[identifier].Count == 0 )
		{
			SpawnObjects( identifier, (int) Mathf.Ceil( expectedCount[identifier] * emptyAdjustment ) );
		}
		GameObject go = pool[identifier][0];
		pool[identifier].RemoveAt( 0 );
		go.SetActive( true );
		return go;
	}

	public void Request( PoolRequest pr )
	{
		requests.Enqueue( pr );
	}

	public void TryAddReference( GameObject go, string identifier )
	{
		if ( poolingReferences.ContainsKey( identifier ) || go == null )
		{
			return;
		}
		poolingReferences.Add( identifier, go );
	}

	public void ReturnToPool( GameObject go, string identifier )
	{
		if ( !pool.ContainsKey( identifier ) || go == null )
		{
			throw new System.NullReferenceException();
		}
		go.SetActive( false );
		pool[identifier].Add( go );
	}

	private void ProcessPool()
	{
		if ( request == null )
		{
			// we don't have a request
			if ( requests.Count == 0 )
			{
				// there aren't any at all
				return;
			}
			else
			{
				request = requests.Dequeue(); // get new request
			}
		}

		// we do have a request
		int numToSpawn = (int) ( Mathf.Clamp( maxExpectedDeltatime / request.heavyness, 1f, (float) request.numToPool ) );
		SpawnObjects( request.identifier, numToSpawn );
		if ( !expectedCount.ContainsKey( request.identifier ) )
		{
			expectedCount.Add( request.identifier, 0 );
		}
		expectedCount[request.identifier] += numToSpawn;
		request.numToPool -= numToSpawn;
		if ( request.numToPool == 0 )
		{
			request = null;
		}
	}

	public void SpawnObjects( string identifier, int count )
	{

		GameObject go = poolingReferences[identifier];
		if ( !pool.ContainsKey( identifier ) )
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
		if ( Input.GetKeyDown( KeyCode.Return ) )
		{
			Debug.Log( pool.Keys.ToString() );
		}
	}

	public static AdvancedHeavyness CalculateHeavyness( GameObject go, int referenceSpawning )
	{
		System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
		List<GameObject> objects = new List<GameObject>();

		long before = System.GC.GetTotalMemory( true );
		watch.Start();

		for ( int i = 0; i < referenceSpawning; i++ )
		{
			objects.Add( Instantiate( go ) );
		}

		watch.Stop();

		long size = System.GC.GetTotalMemory( true ) - before;

		objects.ForEach( ( x ) => { Destroy( x ); } );

		Debug.Log( "Tested spawning time and memory use: \nTime was:\n\tTotal:\t" + watch.ElapsedMilliseconds / 1000f + "s\n\tAverage:\t" + watch.ElapsedMilliseconds / 1000f / referenceSpawning + "\nMemory use was:\n\tTotal:\t" + size + "b\n\tAverage:\t" + size / referenceSpawning + "b" );

		return new AdvancedHeavyness( watch.ElapsedMilliseconds / referenceSpawning, size / referenceSpawning );
	}
	public static AdvancedHeavyness CalculateHeavyness( GameObject go )
	{
		return CalculateHeavyness( go, 100 );
	}
}
