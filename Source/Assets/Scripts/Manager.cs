using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	
	internal bool hosting = false;
	internal bool connecting = false;

	internal string textfieldIP = "192.168.1.1";
	internal string moniker;
	internal string message = "Enter smack-talk here";

	DateTime lastChatDate;
	internal int totalChatsToday;
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
			textwriter.WriteLine ( "Moniker" + Environment.NewLine + "0" + Environment.NewLine + DateTime.Today );
			textwriter.Close ();
		}

		String[] tempSettingsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" );
		moniker = tempSettingsString[0];
		totalChatsToday = Convert.ToInt32 ( tempSettingsString[1] );
		lastChatDate = Convert.ToDateTime ( tempSettingsString[2] );

		if ( !File.Exists ( supportFilesPath + Path.DirectorySeparatorChar + "SavedIPs.txt" ))
			File.Create ( supportFilesPath + Path.DirectorySeparatorChar + "SavedIPs.txt" );
		else
		{

			String[] tempSavedIPsString;
			tempSavedIPsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "SavedIPs.txt" );

			if ( tempSavedIPsString.Length != 0 )
			{

				int savedIPsInt = 0;
				while ( savedIPsInt < tempSavedIPsString.Length )
				{

					guimanager.savedIPs.Add ( tempSavedIPsString[savedIPsInt] );
					savedIPsInt++;
				}
			}
		}

		if ( lastChatDate != DateTime.Today )
			totalChatsToday = 0;
	}


	void SaveIP ()
	{

		StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "SavedIPs.txt", true );
		textwriter.WriteLine ( textfieldIP );
		textwriter.Close ();

		guimanager.savedIPs.Add ( textfieldIP );
	}
	
	
	void ServerControl ()
	{

		if ( hosting == false )
		{

			if ( moniker == "Moniker" || String.IsNullOrEmpty ( moniker.Trim ()) )
			{

				notificationManager.error = true;
				notificationManager.notificationText = "Please change your moniker ( name ) and try again.";
			} else {

				Network.InitializeServer ( 2, 25565, false );
				hosting = true;
				connecting = false;

				currentChatPath = chatLogsPath + Path.DirectorySeparatorChar + DateTime.Today.Day + ":" + DateTime.Today.Month + ":" + DateTime.Today.Year + " " + totalChatsToday + ".txt";
				lastChatDate = DateTime.Today;

				String[] tempSettingsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" );
				tempSettingsString[0] = moniker;
				
				StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt", false );
				textwriter.WriteLine ( tempSettingsString[0] + Environment.NewLine + tempSettingsString[1] + Environment.NewLine + DateTime.Today );
				textwriter.Close ();
	
				networkView.RPC ( "RecieveMessage", RPCMode.All, "Chat started at " + DateTime.Now );
			}
		} else {

			networkView.RPC ( "RecieveMessage", RPCMode.All, "Chat ended at " + DateTime.Now );

			Network.Disconnect();
			hosting = false;
			connecting = false;

			totalChatsToday++;
			String[] tempSettingsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" );
			tempSettingsString[1] = totalChatsToday.ToString ();

			StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt", false );
			textwriter.WriteLine ( tempSettingsString[0] + Environment.NewLine + tempSettingsString[1] + Environment.NewLine + DateTime.Today );
			textwriter.Close ();

			guimanager.messageList.Clear ();
		}
	}

	[RPC]
	void RecieveMessage (string recievedMessage)
	{

		StreamWriter textwriter = new StreamWriter ( currentChatPath, true );
		textwriter.WriteLine ( recievedMessage );
		textwriter.Close ();

		guimanager.messageList.Add ( recievedMessage );
		guimanager.chatScrollPosition.y += Mathf.Infinity;
	}

	void OnPlayerConnected ( NetworkPlayer player )
	{

		Debug.Log ( player.ipAddress + " Connected through| " + player.port );
	}

	void OnFailedToConnect ( NetworkConnectionError error )
	{

		Debug.LogWarning ( "Couldn't connect to server| " + error );
	}
}
