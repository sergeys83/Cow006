using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;
using Scripts.Profile;

public class CloudSaveManager : MonoBehaviour
{
    private bool isSaving;
    private static CloudSaveManager sInstance= new CloudSaveManager();

    // Activate the Play Games platform. This will make it the default
    // implementation of Social.Active
    public static CloudSaveManager Instance
    {
        get
        {
            return sInstance;
        }
    }

  
    int GrabInt(string intName)
    {
        int score = 0;
        if (PlayerPrefs.HasKey(intName))
        {
            score = PlayerPrefs.GetInt(intName);
        }
        else
        {
            Debug.Log("Failed to grab " + intName);
        }
        return score;
    }

    string MakeSaveString()//paid:lastScore:highScore:totalScore:shapeNumber
    {
        string s = GrabInt("paid") + ":" +
                    GrabInt("lastScore") + ":" +
                    GrabInt("highScore") + ":" +
                    GrabInt("totalScore") + ":" +
                    GrabInt("shapeNumber");
        return s;
    }

    void LoadFromSaveString(string s)//paid:lastScore:highScore:totalScore:shapeNumber
    {
        string[] data = s.Split(new char[] { ':' });
        PlayerPrefs.SetInt("paid", int.Parse(data[0]));
        PlayerPrefs.SetInt("lastScore", int.Parse(data[1]));
        PlayerPrefs.SetInt("highScore", int.Parse(data[2]));
        PlayerPrefs.SetInt("totalScore", int.Parse(data[3]));
        PlayerPrefs.SetInt("shapeNumber", int.Parse(data[4]));
        PlayerPrefs.Save();
    }
  /*  byte[] ToBytes()
    {
        return System.Text.ASCIIEncoding.Default.GetBytes(MakeSaveString());
    }
    string FromBytes(byte[] b)
    {
        return System.Text.ASCIIEncoding.Default.GetString(b);
    }*/

    public void SaveToCloud()
    {
        if (Authenticated)
        {
            // Cloud save is not in ISocialPlatform, it's a Play Games extension,
            // so we have to break the abstraction and use PlayGamesPlatform:
            Debug.Log("Saving progress to the cloud...");
            isSaving = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                     "saveFile",
                     DataSource.ReadCacheOrNetwork,
                     ConflictResolutionStrategy.UseOriginal,
                     SaveFileOpened);
        }
    }
    public void LoadFromCloud()
    {
        if (Authenticated)
        {
            // Cloud save is not in ISocialPlatform, it's a Play Games extension,
            // so we have to break the abstraction and use PlayGamesPlatform:
           Debug.Log("Loading progress from cloud");
            isSaving = false;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                     "saveFile",
                     DataSource.ReadCacheOrNetwork,
                     ConflictResolutionStrategy.UseOriginal,
                     SaveFileOpened);
        }
    }
    void SaveFileOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //if we have opened the file then we are either saving or loading
        if (status == SavedGameRequestStatus.Success)
        {
            if (isSaving) //writting save
            {
                byte[] data = DataSaver.Instance.GetByteData(); // ToBytes();

                //Can update metadata with played time description and screenshoot
                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata, data, WroteSavedGame);
            }
            else // loading save
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, LoadedSavedGame);
            }

        }
        else
        {
            Debug.LogWarning("Error opening game: " + status);
        }
    }

    public void LoadedSavedGame(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success && data.Length>0)
        {
            Debug.Log("SaveGameLoaded, success=" + status);
            DataSaver.Instance. LoadByteData(data);
        }
        else
        {
            Debug.LogWarning("Error reading game: " + status);
        }
    }

    public void WroteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Game " + game.Description + " written");
        }
        else
        {
            Debug.LogWarning("Error saving game: " + status);
        }
    }

    public bool Authenticated
    {
        get
        {
            return Social.Active.localUser.authenticated;
        }
    }

    public void ShowSavedGameUI()
    {
      //  ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
      //  savedGameClient.ShowSelectSavedGameUI("These are the saves", 3, true, true, OnSavedGameSelected);
    }
    public void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
    {
        if (status == SelectUIStatus.SavedGameSelected)
        {
            // handle selected game save
            Debug.Log("we got a save");
        }
        else
        {
            // handle cancel or error
        }
    }
}
