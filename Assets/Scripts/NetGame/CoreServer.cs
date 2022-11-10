using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

namespace NetGame
{
    public class CoreServer : MonoBehaviour, IPunTurnManagerCallbacks, IOnEventCallback
    {
        public const float TurnTime = 25f;
        public const int moveCardToTarget = 0;

        #region PunManagerCallBack
        
        public void OnTurnBegins(int turn) {}
        public void OnTurnCompleted(int turn) {}
        public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
        {
            PhotonNetwork.RaiseEvent(moveCardToTarget, move ,RaiseEventOptions.Default, SendOptions.SendReliable);
        }
        public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move) {}
        public void OnTurnTimeEnds(int turn)
        {
            throw new System.NotImplementedException();
        }
        
        #endregion

        public void OnEvent(EventData photonEvent)
        {
            Debug.Log("Data REcieved " + photonEvent.CustomData);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}
