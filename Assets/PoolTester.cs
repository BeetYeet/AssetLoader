using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTester : MonoBehaviour
{
	public string Identifier = "Test/Objects/001";
	public GameObject toPool;
	public List<GameObject> unPooled = new List<GameObject>();

    void Start()
    {
		Pooler.curr.TryAddReference(toPool, Identifier );
		Pooler.curr.Request( new PoolRequest( Identifier, 100) );
    }

   
    void Update()
    {
		if ( Input.GetKey( KeyCode.Space ) )
		{
			GameObject go = Pooler.curr.GetFromPool( Identifier );
			unPooled.Add( go );
			go.transform.position = Vector3.zero;
		}
		else
		{
			if ( unPooled.Count == 0 )
				return;
			GameObject go = unPooled[0];
			unPooled.RemoveAt( 0 );
			Pooler.curr.ReturnToPool( go, Identifier );
		}
    }
}
