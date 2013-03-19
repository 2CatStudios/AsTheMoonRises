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

			GUI.Label ( new Rect ( 100, 310, 100, 20 ), "Enter your name" );
			manager.moniker = GUI.TextField ( new Rect ( 100, 330, 100, 20 ), manager.moniker );
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


	public int focusedWindow
	{
		
		get
		{
			
			System.Reflection.FieldInfo field = typeof ( GUI ).GetField ( "focusedWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static );
			return ( int ) field.GetValue ( null );	
		}
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

		if ( GUI.Button ( new Rect ( 12, 125, 100, 20), "Send" ) && String.IsNullOrEmpty ( manager.message.Trim ()) == false || Event.current.type == EventType.KeyDown && ( Event.current.keyCode == KeyCode.Return && String.IsNullOrEmpty ( manager.message.Trim ()) == false ))
		{
			
			manager.networkView.RPC ( "RecieveMessage", RPCMode.All, manager.moniker + "| " + manager.message.Trim (), true);
			manager.message = "";
		}

		if ( manager.hosting == true )
		{
			
			if ( GUI.Button ( new Rect ( 122, 125, 110, 20 ), "Disable Hosting" ))
				manager.SendMessage ( "ServerControl" );
			
			if ( GUI.Button ( new Rect ( 242, 125, 100, 20 ), "Start Round" ) && manager.roundInProgress == false )
				if ( Network.connections.Length > 0 )
					manager.networkView.RPC ( "SetupNetworkRound" , RPCMode.All, zero );
				else
				{
				
				notificationManager.notificationText = "There are no connected players!";
				notificationManager.error = true;
				}

			if ( GUI.Button ( new Rect ( 352, 125, 100, 20 ), "Send Taunt" ))
				manager.networkView.RPC ( "RecieveTaunt", RPCMode.All );
		}
		
		if ( manager.connected == true )
		{
			
			if ( GUI.Button ( new Rect ( 122, 125, 110, 20 ), "Disconnect" ))
				manager.SendMessage ( "ConnectionControl" );

			if ( GUI.Button ( new Rect ( 242, 125, 100, 20 ), "Send Taunt" ))
				manager.networkView.RPC ( "RecieveTaunt", RPCMode.All );
		}
		
		manager.message = GUI.TextField ( new Rect ( 12, 150, 551, 20 ), manager.message );
	}
}