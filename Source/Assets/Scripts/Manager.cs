using System;
using System.IO;
using UnityEngine;
using System.Collections;
//Written by Gibson Bethke
public class Manager : MonoBehaviour
{

	string path;
	static string macPath = "/Users/" + Environment.UserName + "/Library/Application Support/2Cat Studios/AsTheMoonRises";
	static string windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\2Cat Studios\\AsTheMoonRises";
	internal string chatLogsPath;
	internal string supportFilesPath;

	NotificationManager notificationManager;
	GUIManager guimanager;

	public float runningVersion = 0.01f;

	internal bool willPlay = false;
	bool lastWillPlay = false;

	internal bool hosting = false;
	internal bool connecting = false;

	internal string textfieldIP = "192.168.1.1";
	internal string moniker = "Moniker";
	internal string message = "Enter smack-talk here";

	internal int totalChats;
	internal string currentChatPath;

	void Start ()
	{

		notificationManager = gameObject.GetComponent<NotificationManager>();
		guimanager = GameObject.FindGameObjectWithTag ( "MainCamera" ).GetComponent<GUIManager>();

		if(Environment.OSVersion.ToString().Substring (0, 4) == "Unix")
			path = macPath;
		else
			path = windowsPath;

		supportFilesPath = path + Path.DirectorySeparatorChar + "SupportFiles";
		chatLogsPath = path + Path.DirectorySeparatorChar + "ChatLogs";

		if ( !Directory.Exists ( supportFilesPath ))
			Directory.CreateDirectory ( supportFilesPath );
		if ( !Directory.Exists ( chatLogsPath ))
			Directory.CreateDirectory ( chatLogsPath );

		if ( !File.Exists ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" ))
		{

			StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt", false );
			textwriter.WriteLine ( "Moniker" + Environment.NewLine + "0" );
			textwriter.Close ();
		}

		moniker = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" )[0];
		totalChats = Convert.ToInt32 ( File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" )[1]);

		if ( !File.Exists ( supportFilesPath + Path.DirectorySeparatorChar + "SavedIPs.txt" ))
		{
			
			File.Create ( supportFilesPath + Path.DirectorySeparatorChar + "SavedIPs.txt" );
		}

		InvokeRepeating ( "ServerControl", 0, 2 );

		currentChatPath = chatLogsPath + Path.DirectorySeparatorChar + DateTime.Today.Day + ":" + DateTime.Today.Month + ":" + DateTime.Today.Year + " " + totalChats + ".txt";
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

					networkView.RPC ( "RecieveMessage", RPCMode.All, "Server created at " + DateTime.Now );

					UnityEngine.Debug.Log ( "Server Enabled" );
				}
			}

		} else {

			if ( lastWillPlay == true )
			{

				networkView.RPC ( "RecieveMessage", RPCMode.All, "Chat ended at " + DateTime.Now );

				Network.Disconnect();
				lastWillPlay = false;
				hosting = false;
				connecting = false;

				totalChats++;

				guimanager.messageList.Clear ();

				UnityEngine.Debug.Log ( "Server Disabled" );
			}
		}
	}

	[RPC]
	void RecieveMessage (string recievedMessage)
	{

		StreamWriter textwriter = new StreamWriter ( currentChatPath, true );
		textwriter.WriteLine ( recievedMessage );
		textwriter.Close ();

		guimanager.messageList.Add ( recievedMessage );
		guimanager.scrollPosition.y += Mathf.Infinity;
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
