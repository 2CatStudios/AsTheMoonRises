using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class Manager : MonoBehaviour
{

	internal bool willPlay = false;
//	bool hosting = false;
//	bool connected = false;

	bool lastWillPlay = false;
	internal string textfieldIP = "192.168.1.1";
	internal string moniker = "Moniker";
	internal string message = "DebugLog Chat!";

	void Start ()
	{

		InvokeRepeating ( "ServerControl", 0, 2 );
	}

	void ServerControl ()
	{
		
		if ( willPlay == true )
		{

			if ( lastWillPlay == false )
			{

				Network.InitializeServer ( 2, 25565, false );
				lastWillPlay = true;

				UnityEngine.Debug.Log ( "Server Enabled" );
			}

		} else {

			if ( lastWillPlay == true )
			{

				Network.Disconnect();
				lastWillPlay = false;

				UnityEngine.Debug.Log ( "Server Disabled" );
			}

		}
	}

	void Update ()
	{

		if ( Input.GetButtonDown ( "Return" ))
		{

			UnityEngine.Debug.Log ( "Message Sent" );
			networkView.RPC ( "RecieveMessage", RPCMode.All, moniker + ": " + message );
			message = "";

		}
	}

	[RPC]
	void RecieveMessage (string recievedMessage)
	{

		Debug.Log ( recievedMessage );
	}

	void OnPlayerConnected ( NetworkPlayer player )
	{

		Debug.Log ( player.ipAddress + " Connected through " + player.port );
	}

	void OnFailedToConnect ( NetworkConnectionError error )
	{

		Debug.LogWarning ( "Could not connect to server: " + error );
	}
}
