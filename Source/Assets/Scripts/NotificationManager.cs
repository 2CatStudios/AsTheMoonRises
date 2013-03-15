using System;
using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class NotificationManager : MonoBehaviour
{

	public GUISkin guiskin;
	Manager manager;

	internal bool error = false;
	internal bool prompt = false;
	internal bool textfieldPrompt = false;

	internal string notificationText = "";

	void Start ()
	{

		manager = gameObject.GetComponent<Manager>();
	}


	void OnGUI ()
	{

		GUI.skin = guiskin;

		if ( error == true )
		{
			
			GUI.Window ( 0, new Rect ( Screen.width/2 - 150, 150, 300, 75 ), Error, "An error prevented your desired action!" );
			GUI.BringWindowToFront ( 0 );
			GUI.FocusWindow ( 0 );
		}

		if ( prompt == true )
		{

			GUI.Window ( 1, new Rect ( Screen.width/2 - 150, 150, 300, 75 ), Prompt, "Note:" );
			GUI.BringWindowToFront ( 1 );
			GUI.FocusWindow ( 1 );
		}

		if ( textfieldPrompt == true )
		{
			
			GUI.Window ( 2, new Rect ( Screen.width/2 - 150, 150, 300, 75 ), TextfieldPrompt, "Enter a number ranging from 0 to 5" );
			GUI.BringWindowToFront ( 2 );
			GUI.FocusWindow ( 2 );
		}
	}


	void Error ( int wid )
	{

		GUI.Label ( new Rect ( 0, 15, 300, 40 ), notificationText );
		if ( GUI.Button ( new Rect ( 125, 50, 50, 20 ), "Okay" ))
		{
			
			error = false;
			notificationText = "";
		}
	}


	void Prompt ( int wid )
	{

		GUI.Label ( new Rect ( 0, 15, 300, 40 ), notificationText );
		if ( GUI.Button ( new Rect ( 125, 50, 50, 20 ), "Okay" ))
		{

			prompt = false;
			notificationText = "";
		}
	}


	void TextfieldPrompt ( int wid )
	{

		manager.guessedNumber = GUI.TextField ( new Rect ( 140, 20, 20, 20 ), manager.guessedNumber );
		if ( manager.guessedNumber.Length > 1 )
			manager.guessedNumber = manager.guessedNumber.Substring ( 0, manager.guessedNumber.Length - 1 );

		if ( GUI.Button ( new Rect ( 120, 50, 60, 20 ), "Ready" ) && String.IsNullOrEmpty ( manager.guessedNumber ) == false )
		{

			textfieldPrompt = false;
			manager.SendMessage ( "SetupNetworkRound", 1 );
		}
	}
}