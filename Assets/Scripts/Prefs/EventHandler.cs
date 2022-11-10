using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Prefs
{
    public class EventHandler : MonoBehaviour {
        public enum LevelPlayState {Won, Lost,InProgress}
        private int _players=0;
        private LevelPlayState state = LevelPlayState.InProgress;
        private int score = 0;
        private int money = 0;

        public void SetLevelPlayState(LevelPlayState newState){
            this.state = newState;
        }

        public void IncreaseScore(int points){
            score += points;
        }
        public void IncreaseMoney(int mm){
            money += mm;
        }

        public void GetPlayers(int player){
            _players+=player;
        }

        void OnDestroy(){
            Dictionary<string, object> customParams = new Dictionary<string, object>();
            customParams.Add("state", state);
            customParams.Add("points", score);
            customParams.Add("money", money);
            GetPlayers(GameManagerScr.S.numberPlayers);

           /* switch(this.state){
                case LevelPlayState.Won:
                    AnalyticsEvent.LevelComplete(_players.ToString(), _players, customParams);
                    break;
                case LevelPlayState.Lost:
                    AnalyticsEvent.LevelFail(_players.ToString(), _players, customParams);
                    break;
             
                case LevelPlayState.InProgress:
              
                default:
                    AnalyticsEvent.LevelQuit(_players.ToString(), _players, customParams);
                    break;
            }*/
        }
    }
}
