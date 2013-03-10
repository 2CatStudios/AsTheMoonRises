using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class GUIManager : MonoBehaviour
{

	Manager manager;
	NotificationManager notificationManager;

	public GUISkin guiskin;

	void Start ()
	{

		manager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<Manager>();
		notificationManager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<NotificationManager>();
	}

	void OnGUI ()
	{

		GUI.skin = guiskin;

		manager.willPlay = GUI.Toggle ( new Rect ( 100, 200, 110, 20 ), manager.willPlay, "Enable Hosting" );
		GUI.Label ( new Rect ( 140, 215, 30, 20), "Or" );
		manager.textfieldIP = GUI.TextField ( new Rect ( 100, 235, 100, 20 ), manager.textfieldIP );
		if ( GUI.Button ( new Rect ( 100, 255, 100, 20 ), "Connect to IP" ))
		{

			if ( manager.textfieldIP == "192.168.1.1" )
			{

				notificationManager.notificationText = "Please enter an IP address and try again.";
				notificationManager.error = true;
			} else {

				if ( manager.moniker == "Moniker" )
				{

					notificationManager.notificationText = "Please enter a moniker ( name ) and try again.";
					notificationManager.error = true;
				} else {

					Network.Connect ( manager.textfieldIP, 25565 );
				}
			}
		}

		if ( Event.current.type ==  EventType.KeyDown && ( Event.current.keyCode == KeyCode.Return ))
		{
			
			UnityEngine.Debug.Log ( "Message Sent" );
			manager.networkView.RPC ( "RecieveMessage", RPCMode.All, manager.moniker + ": " + manager.message );
			manager.message = "";
		}

		manager.moniker = GUI.TextField ( new Rect ( 100, 280, 100, 20 ), manager.moniker );
		manager.message = GUI.TextField ( new Rect ( Screen.width/2 - 250, Screen.height/2 + 40, 500, 40 ), manager.message );

		if ( GUI.Button ( new Rect ( Screen.width/2 - 300, Screen.height/2 + 40, 50, 40), "Send" ))
		{

			UnityEngine.Debug.Log ( "Message Sent" );
			manager.networkView.RPC ( "RecieveMessage", RPCMode.All, manager.moniker + ": " + manager.message );
			manager.message = "";
		}
	}
}