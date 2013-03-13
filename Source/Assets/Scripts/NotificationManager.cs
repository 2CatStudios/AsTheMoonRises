using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class NotificationManager : MonoBehaviour
{

	public GUISkin guiskin;

	internal bool error = false;
	internal bool prompt = false;
	internal bool message = false;

	internal string notificationText = "";

	void OnGUI ()
	{

		GUI.skin = guiskin;

		if ( error == true )
		{
			
			GUI.Window ( 0, new Rect ( Screen.width/2 - 150, 150, 300, 75 ), Error, "An error prevented your desired action!" );
			GUI.FocusWindow ( 0 );
			GUI.BringWindowToFront ( 0 );
		}

		if ( prompt == true )
		{

			GUI.Window ( 1, new Rect ( Screen.width/2 - 150, 150, 300, 75 ), Prompt, "Note:" );
			GUI.FocusWindow ( 1 );
			GUI.BringWindowToFront ( 1 );
		}

		if ( message == true )
		{
			
			GUI.Window ( 2, new Rect ( Screen.width/2 - 150, 150, 300, 75 ), TempWindow, "An error prevented your desired action!" );
			StartCoroutine ( "TempWindowTimeOut" );
			GUI.FocusWindow ( 2 );
			GUI.BringWindowToFront ( 2 );
		}
	}

	void Error ( int wid )
	{

		GUI.Label ( new Rect ( 0, 15, 300, 40 ), notificationText );
		if ( GUI.Button ( new Rect ( 125, 50, 50, 20 ), "Okay" ))
			error = false;
	}

	void Prompt ( int wid )
	{

		GUI.Label ( new Rect ( 0, 15, 300, 40 ), notificationText );
		if ( GUI.Button ( new Rect ( 125, 50, 50, 20 ), "Okay" ))
			prompt = false;
	}

	void TempWindow ( int wid )
	{

		GUI.Label ( new Rect ( 0, 15, 300, 40 ), notificationText );
	}

	bool timeoutOnce = true;
	IEnumerator TempWindowTimeOut ()
	{

		if ( timeoutOnce == true )
		{
		
			timeoutOnce = false;
			yield return new WaitForSeconds ( 5 );
			message = false;
			timeoutOnce = true;
		}
	}
}