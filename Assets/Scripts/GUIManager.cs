using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class GUIManager : MonoBehaviour
{

	Manager manager;

	bool error = false;
	string errorReason = "";

	void Start ()
	{

		manager = GameObject.FindGameObjectWithTag ( "Manager" ).GetComponent<Manager>();
	}

	void OnGUI ()
	{

		manager.willPlay = GUI.Toggle ( new Rect ( 100, 200, 110, 20 ), manager.willPlay, "Enable Hosting" );
		GUI.Label ( new Rect ( 140, 215, 30, 20), "Or" );
		manager.textfieldIP = GUI.TextField ( new Rect ( 100, 235, 100, 20 ), manager.textfieldIP );
		if ( GUI.Button ( new Rect ( 100, 255, 100, 20 ), "Connect to IP" ))
		{

			if ( manager.textfieldIP == "192.168.1.1" )
			{

				errorReason = "Please enter an IP address and try again.";
				error = true;
			} else {

				if ( manager.moniker == "Moniker" )
				{

					errorReason = "Please enter a moniker ( name ) and try again.";
					error = true;
				} else {

					Network.Connect ( manager.textfieldIP, 25565 );
				}
			}
		}
		manager.moniker = GUI.TextField ( new Rect ( 100, 280, 100, 20 ), manager.moniker );

		manager.message = GUI.TextField ( new Rect ( Screen.width/2 - 250, Screen.height/2 + 40, 500, 40 ), manager.message );

		if ( error == true )
		{

			GUI.Window ( 0, new Rect ( Screen.width/2 - 150, Screen.height/2 - 50, 300, 100 ), Error, "An error prevented your desired action!" );
			GUI.FocusWindow (0);
		}
	}

	void Error (int wid)
	{

		GUI.skin.label.alignment = TextAnchor.MiddleCenter;

		GUI.Label ( new Rect ( 0, 15, 300, 40 ), errorReason );
		if ( GUI.Button ( new Rect ( 125, 50, 50, 20 ), "Okay" ))
			error = false;
	}
}