using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Pooler: MonoBehaviour
{
	private const float maxExpectedDeltatime = .2f;
	private const float maxExpectedTotalHeavyness = 5f; // in milliseconds


	private Queue<PoolRequest> requests; // stores all spawning requests
	private PoolRequest request; // the current request
	[SerializeField] public Dictionary<string, GameObject> PoolingReferences;
	[SerializeField] public Dictionary<string, List<GameObject>> pool;

	public static Pooler curr;

	private void Awake()
	{
		if ( curr != null )
			throw new System.Exception( "Cannot have multiple instances of Pooler" );
		curr = this;
		requests = new Queue<PoolRequest>();
	}

	public GameObject GetFromPool(string identifier)
	{
		GameObject go = pool[identifier][0];
		pool[identifier].RemoveAt(0);
		go.SetActive( true );
		return go;
	}

	public void Request( PoolRequest pr )
	{
		requests.Enqueue( pr );
	}

	public void TryAddReference( GameObject go, string identifier )
	{
		if ( PoolingReferences[identifier] == null || go == null )
		{
			return;
		}
		PoolingReferences.Add( identifier, go );
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
		int numToSpawn = (int) ( maxExpectedDeltatime / request.heavyness ) + 1;
		SpawnObjects( request.identifier, numToSpawn );

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

	public static AdvancedHeavyness CalculateHeavyness( GameObject go, int referenceSpawning )
	{
		Stopwatch watch = new Stopwatch();
		GameObject[] objects = new GameObject[referenceSpawning];

		watch.Start();

		for ( int i = 0; i < referenceSpawning; i++ )
		{
			objects[i] = Instantiate( go );
		}

		watch.Stop();

		long size = 0;
		using ( Stream s = new MemoryStream() )
		{
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize( s, objects );
			size = s.Length;
		}

		foreach ( GameObject g in objects )
		{
			Destroy( g );
		}

		return new AdvancedHeavyness( watch.ElapsedMilliseconds / referenceSpawning, size / referenceSpawning );
	}
	public static AdvancedHeavyness CalculateHeavyness( GameObject go )
	{
		return CalculateHeavyness( go, 100 );
	}
}
