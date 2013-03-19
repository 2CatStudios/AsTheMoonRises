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
	internal string supportFilesPath;

	AudioSource audioSource;
	NotificationManager notificationManager;
	GUIManager guimanager;
	GUIText countdownText;

	public float runningVersion = 1;
	
	internal bool hosting = false;
	internal bool connected = false;

	internal string textfieldIP = "192.168.1.1";
	internal string moniker;
	internal string opponentMoniker;
	internal string message = "Enter smack-talk here";
	public AudioClip messageNotificationSound;
	public AudioClip[] tauntSound = new AudioClip[4];
	int tauntInt = 0;
	public AudioClip countdownTimerSound;
	public AudioClip countdownTimerFinalSound;

	internal bool roundInProgress = false;
	internal string guessedNumber = "";

	internal bool ready = false;
	bool opponentReady = false;

	internal int randNumber;

	internal int gamesWon = 0;
	internal int gamesLost = 0;


	void Start ()
	{

		notificationManager = gameObject.GetComponent<NotificationManager>();
		guimanager = GameObject.FindGameObjectWithTag ( "MainCamera" ).GetComponent<GUIManager>();
		countdownText = GameObject.FindGameObjectWithTag ( "CountdownText" ).guiText;

		if(Environment.OSVersion.ToString().Substring (0, 4) == "Unix")
			path = macPath;
		else
			path = windowsPath;

		supportFilesPath = path + Path.DirectorySeparatorChar + "SupportFiles";

		if ( !Directory.Exists ( supportFilesPath ))
			Directory.CreateDirectory ( supportFilesPath );

		if ( !File.Exists ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" ))
		{

			StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt", false );
			textwriter.WriteLine ( "Moniker" + Environment.NewLine + "0" + Environment.NewLine + DateTime.Today );
			textwriter.Close ();
		}

		String[] tempSettingsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" );
		moniker = tempSettingsString[0];

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

		audioSource = GameObject.FindGameObjectWithTag ( "MainCamera" ).GetComponent<AudioSource>();
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
				connected = false;

				String[] tempSettingsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" );
				tempSettingsString[0] = moniker;
				
				StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt", false );
				textwriter.WriteLine ( tempSettingsString[0] );
				textwriter.Close ();

				networkView.RPC ( "RecieveMessage", RPCMode.AllBuffered, "Chat started at " + DateTime.Now, false );
			}
		} else {

			networkView.RPC ( "RecieveMessage", RPCMode.All, "Chat ended at " + DateTime.Now, true );

			Network.Disconnect();
			hosting = false;
			connected = false;

			guimanager.messageList.Clear ();

			gamesWon = 0;
			gamesLost = 0;
			opponentMoniker = "";
			opponentReady = false;
			ready = false;
			randNumber = 0;
			message = "Enter smack-talk here";
		}
	}


	void ConnectionControl ()
	{

		if ( connected == false )
		{

			if ( textfieldIP == "192.168.1.1" || String.IsNullOrEmpty ( textfieldIP.Trim ()) )
			{

				notificationManager.notificationText = "Please enter an IP address and try again.";
				notificationManager.error = true;
			} else {
				if ( moniker == "Moniker" || String.IsNullOrEmpty ( moniker.Trim ()))
				{


					notificationManager.notificationText = "Please change your moniker ( name ) and try again.";
					notificationManager.error = true;
				} else {

					Network.Connect ( textfieldIP, 25565 );
					hosting = false;
					connected = true;
									
					String[] tempSettingsString = File.ReadAllLines ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt" );
					tempSettingsString[0] = moniker;
				
					StreamWriter textwriter = new StreamWriter ( supportFilesPath + Path.DirectorySeparatorChar + "Settings.txt", false );
					textwriter.WriteLine ( tempSettingsString[0] );
					textwriter.Close ();
				}
			}
		} else {

			networkView.RPC ( "RecieveMessage", RPCMode.All, moniker + " has left this chat.", false );

			Network.Disconnect();
			hosting = false;
			connected = false;

			guimanager.messageList.Clear ();

			gamesWon = 0;
			gamesLost = 0;
			opponentMoniker = "";
			opponentReady = false;
			ready = false;
			randNumber = 0;
			message = "Enter smack-talk here";
		}
	}


	[RPC]
	void RecieveMessage ( string recievedMessage, bool playSound )
	{
		
		if ( playSound == true && guimanager.focusedWindow != 3 )
			audioSource.PlayOneShot ( messageNotificationSound );
		guimanager.messageList.Add ( recievedMessage );
		guimanager.chatScrollPosition.y += Mathf.Infinity;
	}


	[RPC]
	void RecieveTaunt ()
	{

		audioSource.PlayOneShot ( tauntSound[tauntInt] );
		tauntInt += 1;

		if ( tauntInt >= tauntSound.Length )
			tauntInt = 0;
	}


	[RPC]
	void SetupNetworkRound ( int switchCase )
	{

		switch ( switchCase )
		{

			case 0:
				notificationManager.textfieldPrompt = true;
			break;

			case 1:

				if ( connected == true )
				{

					ready = true;
					networkView.RPC ( "ConnectedReady", RPCMode.Server );
				}

				if ( hosting == true )
				{
				
					ready = true;

					if ( opponentReady == true )
						SetupNetworkRound ( 2 );
				}	
				break;

			case 2:
				networkView.RPC ( "CountdownTimer" , RPCMode.All );
			break;

			default:
				UnityEngine.Debug.Log ( "ERROR IN SWITCH-CASE" );
			break;
		}
	}

	[RPC]
	IEnumerator ConnectedReady ()
	{

		opponentReady = true;

		if ( ready == true )
			SetupNetworkRound ( 2 );
		else
		{

			yield return new WaitForSeconds ( 0.2f );
			ConnectedReady ();
		}
	}


	[RPC]
	IEnumerator CountdownTimer ()
	{

		roundInProgress = true;
		countdownText.text = "3";
		audioSource.PlayOneShot ( countdownTimerSound );
		yield return new WaitForSeconds ( 1 );
		countdownText.text = "2";
		audioSource.PlayOneShot ( countdownTimerSound );
		yield return new WaitForSeconds ( 1 );
		countdownText.text = "1";
		audioSource.PlayOneShot ( countdownTimerSound );
		yield return new WaitForSeconds ( 1 );
		countdownText.text = "";
		audioSource.PlayOneShot ( countdownTimerFinalSound );

		randNumber = UnityEngine.Random.Range ( 0, 5 );

		if ( connected == true )
			networkView.RPC ( "FindWinner", RPCMode.Server, randNumber, Convert.ToInt32 ( guessedNumber ));

		ready = false;
		opponentReady = false;
		roundInProgress = false;
	}


	[RPC]
	void FindWinner ( int oppNumber, int oppGuess )
	{

		if ( hosting == true )
			networkView.RPC ( "FindWinner", RPCMode.Others, randNumber, Convert.ToInt32 ( guessedNumber ));

		if ( oppNumber == randNumber )
		{

			gamesWon++;
			gamesLost++;
			notificationManager.notificationText = oppNumber + " - " + randNumber + " It's a tie!";
		} else {
			if ( oppNumber > randNumber )
			{
				
				if ( Convert.ToInt32 ( guessedNumber ) == oppNumber )
				{

					gamesLost++;
					gamesWon++;
					notificationManager.notificationText = "You would have lost, but you guessed their number!";
				} else {

					gamesLost++;
					notificationManager.notificationText = oppNumber + " - " + randNumber + " You've lost!";
			}
		} else {

				if ( oppGuess == randNumber )
				{

					gamesLost++;
					gamesWon++;
					notificationManager.notificationText = "You would have won, but they guessed your number!";
				} else {
				gamesWon++;
				notificationManager.notificationText = randNumber + " - " + oppNumber + " You've won!";
				}
			}
		}
		notificationManager.prompt = true;
	}


	[RPC]
	void ExchangeNames ( string opponentName )
	{

		opponentMoniker = opponentName;

		if ( hosting == true )
			networkView.RPC ( "ExchangeNames", RPCMode.Others, moniker );
	}


	void OnConnectedToServer()
	{

		networkView.RPC ( "RecieveMessage", RPCMode.All, moniker + " has joined this chat.", true );
		networkView.RPC ( "ExchangeNames", RPCMode.Others, moniker );
	}


	void OnPlayerDisconnected ( NetworkPlayer player )
	{

		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		gamesWon = 0;
		gamesLost = 0;
		opponentMoniker = "";
		opponentReady = false;
		ready = false;
		randNumber = 0;
		message = "Enter smack-talk here";
	}
	
	
	void OnFailedToConnect ( NetworkConnectionError error )
	{

		Debug.LogWarning ( "Couldn't connect to server| " + error );
	}
}
