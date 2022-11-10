
using System;
using UnityEngine;
using UnityEngine.UI;

    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private Text _rank = null;
     
        [SerializeField] private Text _username = null;
      
        [SerializeField] private Text _score = null;
   
        [SerializeField] private Button _profileBtn = null;

       public void SetPlayer(string username, int rank, long score, Action onProfileClicked)
        {
            _username.text = username;
            _rank.text = rank + ".";
            _score.text = score.ToString();
            _profileBtn.onClick.AddListener(() => onProfileClicked());
        }

    }
