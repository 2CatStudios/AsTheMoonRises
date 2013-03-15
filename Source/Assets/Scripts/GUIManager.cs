using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Gibson Bethke
public class GUIManager : MonoBehaviour
{

	Manager manager;
	NotificationManager notificationManager;

	internal Vector2 chatScrollPosition;
	internal List<String> messageList = new List<String>();

	int zero = 0;

	Vector2 savedIPsScrollPosition;
	internal List<String> savedIPs = new List<String>();
	
	public GUISkin guiskin;
	public GUIText gamesWon;
	public GUIText gamesLost;

	/*
		GUI.Window 0 is Error
		GUI.Window 1 is Prompt
		GUI.Window 2 is TextfieldPrompt
		GUI.Window 3 is ChatBar
		GUI.Window 4 is SavedIPs
		GUI.Window 5 is GameWindow
	*/


	void Start ()
	{

		manager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<Manager>();
		notificationManager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<NotificationManager>();
	}


	void OnGUI ()
	{

		GUI.skin = guiskin;

		if ( manager.connected == false && manager.hosting == false )
		{

			if ( GUI.Button ( new Rect ( 100, 200, 100, 20 ), "Enable Hosting" ))
				manager.SendMessage ( "ServerControl" );

			GUI.Label ( new Rect ( 140, 218, 30, 20), "Or" );

			manager.textfieldIP = GUI.TextField ( new Rect ( 100, 255, 100, 20 ), manager.textfieldIP );

			if ( GUI.Button ( new Rect ( 100, 235, 100, 20 ), "Connect to IP" ))
				manager.SendMessage ( "ConnectionControl" );

			if ( GUI.Button ( new Rect ( 100, 275, 100, 20 ), "Save IP" ))
				manager.SendMessage ( "SaveIP" );

			manager.moniker = GUI.TextField ( new Rect ( 100, 315, 100, 20 ), manager.moniker );
			GUI.Window ( 4, new Rect ( 300, 250, 400, 180 ), SavedIPs, "Saved IP Addresses" );

			gamesWon.text = "";
			gamesLost.text = "";
		}

		if ( manager.hosting == true || manager.connected == true )
		{

			GUI.Window ( 3, new Rect ( 125, 426, 575, 180 ), ChatBar, "Chat with friends in the same game." );
			gamesWon.text = manager.moniker + "'s games: " + manager.gamesWon;
			if ( Network.connections.Length > 0 )
				gamesLost.text = manager.opponentMoniker + "'s games: " + manager.gamesLost;
			else
				gamesLost.text = "No player connected";
		}

		if ( manager.hosting == true )
		{

			if ( GUI.Button ( new Rect ( 130, 400, 110, 20 ), "Disable Hosting" ))
				manager.SendMessage ( "ServerControl" );

			if ( GUI.Button ( new Rect ( 250, 400, 85, 20 ), "Start Round" ) && manager.roundInProgress == false )
				if ( Network.connections.Length > 0 )
					manager.networkView.RPC ( "SetupNetworkRound" , RPCMode.All, zero );
				else
				{

					notificationManager.notificationText = "There are no connected players!";
					notificationManager.error = true;
				}
		}

		if ( manager.connected == true )
		{
			
			if ( GUI.Button ( new Rect ( 130, 400, 110, 20 ), "Disconnect" ))
				manager.SendMessage ( "ConnectionControl" );
		}
	}


	void SavedIPs ( int wid )
	{

		savedIPsScrollPosition = GUILayout.BeginScrollView( savedIPsScrollPosition, GUILayout.Width( 0 ), GUILayout.Height( 148 ));

		int tempInt = 0;
		while ( tempInt < savedIPs.Count )
		{

			if ( GUILayout.Button ( savedIPs[tempInt] ))
				manager.textfieldIP = savedIPs[tempInt];
			tempInt++;
		}

		GUILayout.EndScrollView ();
	}


	void ChatBar ( int wid )
	{

		chatScrollPosition = GUILayout.BeginScrollView( chatScrollPosition, GUILayout.Width( 0 ), GUILayout.Height( 100 ));
		
		if ( messageList.Count > 0 )
		{
			
			int tempInt = 0;
			while ( tempInt < messageList.Count )
			{
				
				GUILayout.Label ( messageList[tempInt] ); 
				tempInt += 1;
			}
		}
		GUILayout.EndScrollView ();

		if ( GUI.Button ( new Rect ( 12, 130, 50, 40), "Send" ) && String.IsNullOrEmpty ( manager.message.Trim ()) == false || Event.current.type == EventType.KeyDown && ( Event.current.keyCode == KeyCode.Return && String.IsNullOrEmpty ( manager.message.Trim ()) == false ))
		{
			
			manager.networkView.RPC ( "RecieveMessage", RPCMode.All, manager.moniker + "| " + manager.message.Trim (), true);
			manager.message = "";
		}
		
		manager.message = GUI.TextField ( new Rect ( 65, 130, 500, 40 ), manager.message );
	}
}