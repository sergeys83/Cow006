using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;
using System;
using GooglePlayGames.BasicApi;
using Cow006;
using Scripts.Profile;
using UnityEngine.SocialPlatforms;

public class Gpg : MonoBehaviour
{
  //  public LeaderboardPanel lbp;
    private static Gpg _sInstance;
    private int mLevel = 0;
    private int score;
    private bool mAuthenticating = false;
    private string mAuthProgressMessage;

        // list of achievements we know we have unlocked (to avoid making repeated calls to the API)
        private Dictionary<string, bool> mUnlockedAchievements = new Dictionary<string, bool>();

        // achievement increments we are accumulating locally, waiting to send to the games API
        private Dictionary<string, int> mPendingIncrements = new Dictionary<string, int>();

        // what is the highest score we have posted to the leaderboard?
        private int mHighestPostedScore = 0;

        // keep track of saving or loading during callbacks.
        private bool mSaving;

        // auto save
        private string mAutoSaveName;

        private Texture2D mScreenImage;
        public Action<bool> onAuth;
        public static Gpg Instance
        {
            get
            {
                return _sInstance;
            }
        }

        void ReportAllProgress()
        {
            FlushAchievements();
            //   UnlockProgressBasedAchievements();
            PostToLeaderboard();
        }

 
        public void CaptureScreenshot()
        {
            mScreenImage = new Texture2D(Screen.width, Screen.height);
            mScreenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            mScreenImage.Apply();
            Debug.Log("Captured screen: " + mScreenImage);
        }

     /*   public GameProgress Progress
        {
            get
            {
                return mProgress;
            }
        }*/

   
    /*    private void UnlockProgressBasedAchievements()
        {
            int totalStars = mProgress.TotalStars;
            int i;
            for (i = 0; i < GameIds.Achievements.ForTotalStars.Length; i++)
            {
                int starsRequired = GameIds.Achievements.TotalStarsRequired[i];
                if (totalStars >= starsRequired)
                {
                    UnlockAchievement(GameIds.Achievements.ForTotalStars[i]);
                }
            }

            if (mProgress.AreAllLevelsCleared())
            {
                UnlockAchievement(GameIds.Achievements.ClearAllLevels);
            }
        }*/

        public void UnlockAchievement(string achId)
        {
            if (Authenticated && !mUnlockedAchievements.ContainsKey(achId))
            {
                Social.ReportProgress(achId, 100.0f, (bool success) =>
                {
                });
                mUnlockedAchievements[achId] = true;
            }
        }

        public void IncrementAchievement(string achId, int steps)
        {
            if (mPendingIncrements.ContainsKey(achId))
            {
                steps += mPendingIncrements[achId];
            }
            mPendingIncrements[achId] = steps;
        }

        public void FlushAchievements()
        {
            if (Authenticated)
            {
                foreach (string ach in mPendingIncrements.Keys)
                {
                    // incrementing achievements by a delta is a feature
                    // that's specific to the Play Games API and not part of the
                    // ISocialPlatform spec, so we have to break the abstraction and
                    // use the PlayGamesPlatform rather than ISocialPlatform
                 /*   PlayGamesPlatform p = (PlayGamesPlatform)Social.Active;
                    p.IncrementAchievement(ach, mPendingIncrements[ach], (bool success) =>
                    {
                    });*/
                }
                mPendingIncrements.Clear();
            }
        }

        public void Awake()
        {
            _sInstance = this;
            onAuth += DataSaver.Instance.LoadFromCloud;
        }
        public void Start()
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.Activate();
            Authenticate();
            DataSaver.Instance.Preloading();
        }

        public void Authenticate()
        {
            if (Authenticated || mAuthenticating)
            {
                Debug.LogWarning("Ignoring repeated call to Authenticate().");
                return;
            }

            Social.localUser.Authenticate((bool success) =>
            {
                mAuthenticating = false;
                if (success)
                {
                    // if we signed in successfully, load data from cloud                 
                    Egame.InfoMessage = "Login successful!"; 
                    Debug.Log("Login successful!");
                }
                else
                {
                    // no need to show error message (error messages are shown automatically
                    // by plugin)
                    Egame.InfoMessage = "Failed to sign in with Google Play Games." ;
                    Debug.LogWarning("Failed to sign in with Google Play Games." + success);
                    
                }
                onAuth?.Invoke(success);
            });
           
            // Set the default leaderboard for the leaderboards UI
            ((PlayGamesPlatform)Social.Active).SetDefaultLeaderboardForUI(GPGSIds.leaderboard);

            // Sign in to Google Play Games
            mAuthenticating = true;
            //
            
        }

        public bool Authenticating
        {
            get
            {
                return mAuthenticating;
            }
        }

        public bool Authenticated
        {
            get
            {
                return Social.Active.localUser.authenticated;
            }
        }

    /*    public void SignOut()
        {
            ((PlayGamesPlatform)Social.Active).SignOut();
        }*/

        public string AuthProgressMessage
        {
            get
            {
                return mAuthProgressMessage;
            }
        }

        public void ShowLeaderboardUI()
        {
            if (Authenticated)
            {
               // lbp.Show();
               Social.ShowLeaderboardUI();
                
            }
            else
            {
                //показать ошибку аутентификации
            }
        }

        public void ShowAchievementsUI()
        {
            if (Authenticated)
            {
                Social.ShowAchievementsUI();
            }
        }
      public  void SetScore(int sc)
      {
          score = sc;
         
      }
        public void PostToLeaderboard()
        {
           if (Authenticated)
            {
                // post score to the leaderboard
                Social.ReportScore(score, GPGSIds.leaderboard, (bool success) =>
                {
                    
                });
              //  mHighestPostedScore = score;
            }
            else
            {
                Debug.LogWarning("Not reporting score, auth = " + Authenticated + " " +
                    score + " <= " + mHighestPostedScore);
            }
        }

}
