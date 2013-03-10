using System;
using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class Manager : MonoBehaviour
{

	NotificationManager notificationManager;
	GUIManager guimanager;

	public float runningVersion = 0.01f;

	internal bool willPlay = false;
	bool lastWillPlay = false;

	internal bool hosting = false;
	internal bool connecting = false;

	internal string textfieldIP = "192.168.1.1";
	internal string moniker = "Moniker";
	internal string message = "DebugLog Chat!";

	void Start ()
	{

		InvokeRepeating ( "ServerControl", 0, 2 );
		notificationManager = gameObject.GetComponent<NotificationManager>();
		guimanager = GameObject.FindGameObjectWithTag ( "MainCamera" ).GetComponent<GUIManager>();
	}

	void ServerControl ()
	{
		
		if ( willPlay == true )
		{

			if ( lastWillPlay == false )
			{

				if ( moniker == "Moniker" )
				{

					notificationManager.error = true;
					notificationManager.notificationText = "Please enter a moniker ( name ) and try again.";
					willPlay = false;
				} else {

					Network.InitializeServer ( 2, 25565, false );
					lastWillPlay = true;
					hosting = true;
					connecting = false;

					UnityEngine.Debug.Log ( "Server Enabled" );
				}
			}

		} else {

			if ( lastWillPlay == true )
			{

				Network.Disconnect();
				lastWillPlay = false;
				hosting = false;
				connecting = false;

				UnityEngine.Debug.Log ( "Server Disabled" );
			}
		}
	}

	[RPC]
	void RecieveMessage (string recievedMessage)
	{

		guimanager.messageList.Add ( recievedMessage );
		guimanager.scrollPosition.y += 100;
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
