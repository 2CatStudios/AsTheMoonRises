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

	Vector2 savedIPsScrollPosition;
	internal List<String> savedIPs = new List<String>();
	
	public GUISkin guiskin;


	/*

		GUI.Window 0 is Error
		GUI.Window 1 is Prompt
		GUI.Window 2 is Message
		GUI.Window 3 is ChatBar
		GUI.Window 4 is SavedIPs
	*/


	void Start ()
	{

		manager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<Manager>();
		notificationManager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<NotificationManager>();
	}


	void OnGUI ()
	{

		GUI.skin = guiskin;

		if ( manager.connecting == false && manager.hosting == false )
		{

			if ( GUI.Button ( new Rect ( 100, 200, 100, 20 ), "Enable Hosting" ))
				manager.SendMessage ( "ServerControl" );

			GUI.Label ( new Rect ( 140, 218, 30, 20), "Or" );

			manager.textfieldIP = GUI.TextField ( new Rect ( 100, 255, 100, 20 ), manager.textfieldIP );
			if ( GUI.Button ( new Rect ( 100, 235, 100, 20 ), "Connect to IP" ))
			{
				
				if ( manager.textfieldIP == "192.168.1.1" || String.IsNullOrEmpty ( manager.textfieldIP.Trim ()) )
				{
					
					notificationManager.notificationText = "Please enter an IP address and try again.";
					notificationManager.error = true;
				} else {
					
					if ( manager.moniker == "Moniker" || String.IsNullOrEmpty ( manager.moniker.Trim ()))
					{
						
						notificationManager.notificationText = "Please change your moniker ( name ) and try again.";
						notificationManager.error = true;
					} else {
						
						Network.Connect ( manager.textfieldIP, 25565 );
						manager.connecting = true;
					}
				}
			}

			if ( GUI.Button ( new Rect ( 100, 275, 100, 20 ), "Save IP" ))
				manager.SendMessage ( "SaveIP" );

			manager.moniker = GUI.TextField ( new Rect ( 100, 315, 100, 20 ), manager.moniker );
			GUI.Window ( 4, new Rect ( 300, 250, 400, 180 ), SavedIPs, "Saved public IPs" );
		}

		if ( manager.hosting == true || manager.connecting == true )
		{

			GUI.Window ( 3, new Rect ( 125, 426, 575, 180 ), ChatBar, "Chat with friends in the same game." );
		}

		if ( manager.hosting == true )
		{

			if ( GUI.Button ( new Rect ( 130, 400, 110, 20 ), "Disable Hosting" ))
				manager.SendMessage ( "ServerControl" );
		}
	}


	void SavedIPs ( int wid )
	{

		savedIPsScrollPosition = GUILayout.BeginScrollView( savedIPsScrollPosition, GUILayout.Width( 0 ), GUILayout.Height( 148 ));

		int tempInt = 0;
		while ( tempInt < savedIPs.Count )
		{

			if ( GUILayout.Button ( savedIPs[tempInt] ))
			{

				if ( manager.moniker == "Moniker" || String.IsNullOrEmpty ( manager.moniker.Trim ()))
				{
					
					notificationManager.notificationText = "Please enter a moniker ( name ) and try again.";
					notificationManager.error = true;
				} else {
					
					Network.Connect ( manager.textfieldIP, 25565 );
					manager.connecting = true;
				}
			}
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

		if ( GUI.Button ( new Rect ( 12, 130, 50, 40), "Send" ) || Event.current.type ==  EventType.KeyDown && ( Event.current.keyCode == KeyCode.Return && String.IsNullOrEmpty ( manager.message.Trim ()) == false ))
		{
			
			manager.networkView.RPC ( "RecieveMessage", RPCMode.All, manager.moniker + "| " + manager.message.Trim ());
			manager.message = "";
		}
		
		manager.message = GUI.TextField ( new Rect ( 65, 130, 500, 40 ), manager.message );
	}
}