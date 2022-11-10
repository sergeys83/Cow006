
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Cow006;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID

    public class LeaderboardPanel : MonoBehaviour
    {
        private ILeaderboard lb;
        private LeaderboardScoreData lbData;
        public CanvasGroup canvasGroup;
        public CanvasGroup gameMenu;
        [SerializeField] private RectTransform _userList = null;

        [SerializeField] private LeaderboardEntry _leaderboardEntryPrefab = null;
       
        [SerializeField] private int _recordsPerPage = 20;

        [SerializeField] private Button _nextPageButton = null;

        [SerializeField] private Button _prevPageButton = null;

        private void Awake()
        {
            _nextPageButton.onClick.AddListener(NextPage);
            _prevPageButton.onClick.AddListener(Hide);
            
            lb = PlayGamesPlatform.Instance.CreateLeaderboard();
            lb.userScope = UserScope.Global;
            lb.id = GPGSIds.leaderboard;
        }

        IEnumerator Delay(ILeaderboard lb)
        {
            while (lb.loading)
            {
                yield return null;
            }
        }

        public void Show()
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            gameMenu.blocksRaycasts = false;
        }
        
        public void Hide()
        {
            foreach (Transform item in _userList.transform)
            {
                Destroy(item.gameObject);
            }
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            gameMenu.blocksRaycasts = true;
        }
        
        public void Load()
        {
                foreach (Transform item in _userList.transform)
                {
                    Destroy(item.gameObject);
                }
       
            List<string> userIds = new List<string>();
            PlayGamesPlatform.Instance.LoadScores(lb.id, LeaderboardStart.TopScores,
                        _recordsPerPage,
                        LeaderboardCollection.Public,
                        LeaderboardTimeSpan.AllTime,
                        (data) =>
                        {
                           switch(data.Status)
                            {
                                case ResponseStatus.Success:
                                case ResponseStatus.SuccessWithStale:
                                    StartCoroutine(Delay(lb));
                                    
                                    lbData = data;
                                    
                                    foreach(IScore score in data.Scores) 
                                    {
                                        userIds.Add(score.userID);
                                   
                                    }
                                
                                    PlayGamesPlatform.Instance.LoadUsers(userIds.ToArray(), (users) =>
                                    {
                                        foreach(IScore score in data.Scores)
                                        {
                                            IUserProfile user = FindUser(users, score.userID);
                                            LeaderboardEntry entry = Instantiate(_leaderboardEntryPrefab, _userList);
                                            entry.transform.SetParent(_userList);
                                            entry.SetPlayer(user.userName, score.rank, score.value, () => OnProfileClicked(user.userName));
                                        }
                                        
                                    });
                                break;
                               
                            }
                                             
            });
            
        }

        public void NextPage()
        {
            foreach (Transform item in _userList.transform)
            {
                Destroy(item.gameObject);
            }
            GetNextPage(lbData);
        }
       
      public void GetNextPage(LeaderboardScoreData data)
      {
            List<string> userIds = new List<string>();
            
            PlayGamesPlatform.Instance.LoadMoreScores(data.NextPageToken, _recordsPerPage,
                (results) =>
                {
                    StartCoroutine(Delay(lb));
                    lbData = results;
                    
                    foreach(IScore score in results.Scores) 
                    {
                        userIds.Add(score.userID);
                   
                    }
                                
                    PlayGamesPlatform.Instance.LoadUsers(userIds.ToArray(), (users) =>
                    {
                        foreach(IScore score in results.Scores)
                        {
                            IUserProfile user = FindUser(users, score.userID);
                            LeaderboardEntry entry = Instantiate(_leaderboardEntryPrefab, _userList);
                            entry.SetPlayer(user.userName, score.rank, score.value, () => OnProfileClicked(user.userName));
                        }
                      
                    });
                 
             });
      }
      
     
        private IUserProfile FindUser(IUserProfile[] users, string userid)
        {
            foreach (IUserProfile user in users)
            {
                if (user.id == userid)
                {
                    return user;
                }
            }
            return null;
        }
      
        
        private void OnProfileClicked(object ownerId)
        {
            Debug.Log("User clicked");
        }
  
    }
#endif