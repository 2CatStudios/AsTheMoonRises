using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class Manager : MonoBehaviour
{

	internal bool willPlay = false;
	bool lastWillPlay = false;
	internal string textfieldIP = "192.168.1.1";
	internal string moniker = "Moniker";

	void Start ()
	{

		DontDestroyOnLoad ( gameObject );
		InvokeRepeating ( "ServerControl", 0, 2 );
	}

	void ServerControl ()
	{
		
		if ( willPlay == true )
		{

			if ( lastWillPlay == false )
			{

				Network.InitializeServer ( 2, 25000, false );
				lastWillPlay = true;

				UnityEngine.Debug.Log ( "Server Enabled" );
			}
			UnityEngine.Debug.Log ( "willPlay && lastWillPlay == true" );

		} else {

			if ( lastWillPlay == true )
			{

				Network.Disconnect();
				lastWillPlay = false;

				UnityEngine.Debug.Log ( "Server Disabled" );
			}
			UnityEngine.Debug.Log ( "willPlay && lastWillPlay == false" );
		}
	}
}
