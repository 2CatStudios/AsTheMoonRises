using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Written by Gibson Bethke
public class GUIManager : MonoBehaviour
{

	Manager manager;
	NotificationManager notificationManager;

	internal Vector2 scrollPosition;
	internal List<String> messageList = new List<String>();
	
	public GUISkin guiskin;

	void Start ()
	{

		manager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<Manager>();
		notificationManager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<NotificationManager>();
	}


	void OnGUI ()
	{

		GUI.skin = guiskin;

		if ( manager.connecting == false )
			manager.willPlay = GUI.Toggle ( new Rect ( 100, 200, 110, 20 ), manager.willPlay, "Enable Hosting" );

		if ( manager.connecting == false && manager.hosting == false )
			GUI.Label ( new Rect ( 140, 215, 30, 20), "Or" );

		if ( manager.hosting == false )
		{

			manager.textfieldIP = GUI.TextField ( new Rect ( 100, 235, 100, 20 ), manager.textfieldIP );
			if ( GUI.Button ( new Rect ( 100, 255, 100, 20 ), "Connect to IP" ))
			{

				if ( manager.textfieldIP == "192.168.1.1" || String.IsNullOrEmpty ( manager.textfieldIP.Trim ()) )
				{

					notificationManager.notificationText = "Please enter an IP address and try again.";
					notificationManager.error = true;
				} else {

					if ( manager.moniker == "Moniker" || String.IsNullOrEmpty ( manager.moniker.Trim ()))
					{

						notificationManager.notificationText = "Please enter a moniker ( name ) and try again.";
						notificationManager.error = true;
					} else {

						Network.Connect ( manager.textfieldIP, 25565 );
						manager.connecting = true;
					}
				}
			}
		}

		if ( manager.hosting == true || manager.connecting == true )
		{

			GUI.Window ( 3, new Rect ( 125, 426, 575, 180 ), ChatBar, "Chat with friends in the same game." );
		}

		if ( manager.hosting == false && manager.connecting == false )
			manager.moniker = GUI.TextField ( new Rect ( 100, 290, 100, 20 ), manager.moniker );
	}


	void ChatBar ( int wid )
	{

		scrollPosition = GUILayout.BeginScrollView( scrollPosition, GUILayout.Width( 0 ), GUILayout.Height( 100 ));
		
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