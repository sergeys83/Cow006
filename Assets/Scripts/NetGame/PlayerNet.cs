using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Scripts.DataStorage;
using Scripts.Profile;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerNet : PlayerInLobby
{
    public Text score;
    private string SceneName = "Level_Net";
    [Space] 
    private string IconName;
    public void Awake()
    {
        if (SceneManager.GetActiveScene().name==SceneName)
        {
            transform.SetParent(GameManagerNet.S.root.transform);  
        }
    }

   public override void Init(Photon.Realtime.Player player)
    {
       base.Init(player);
       score.text = "0";
    }
   public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
   {
      if (changedProps!=null)
        {
            RoomPlayersText.text = targetPlayer.NickName;
            IconName = targetPlayer.CustomProperties["icon"].ToString();
            icon.sprite = AvatarManager.Instance.LoadAvatar(targetPlayer.CustomProperties["icon"].ToString());
            pId = targetPlayer.ActorNumber;
            if (!targetPlayer.CustomProperties["score"].Equals(null))
            {
                score.text = targetPlayer.CustomProperties["score"].ToString();  
            }
        }
    }
   
    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
     {
         if (requestingPlayer != PhotonNetwork.LocalPlayer)
         {
             targetView.TransferOwnership(requestingPlayer);
         }
     }

    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
     {
         Debug.Log("TRANSFERED");
     }
 
     public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
     {
         Debug.Log("TRANSFERED Failed "+senderOfFailedRequest.NickName);
     }
}
