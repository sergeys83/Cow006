using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Scripts.DataStorage;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Profile;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerInLobby : MonoBehaviourPunCallbacks
{
  public Text RoomPlayersText;
  public Image icon;
  public int pId;
  public Photon.Realtime.Player info;
  private bool isPlayerReady;
  public new string name;
 public virtual void Init(Photon.Realtime.Player player)
 {
    info = player;
    pId = info.ActorNumber;
    info.NickName = player.NickName;
    name = info.NickName;
    RoomPlayersText.text = player.NickName;
    icon.sprite = AvatarManager.Instance.LoadAvatar(player.CustomProperties["icon"].ToString());
  }
 public new void OnEnable()
 {
     PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
 }
 public new void OnDisable()
 {
     PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
 }
 private void OnPlayerNumberingChanged()
 {
     foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
     {
         if (p.ActorNumber == pId)
         {
             icon.sprite = AvatarManager.Instance.LoadAvatar(p.CustomProperties["icon"].ToString());
         }
     }
 }
 
 public void SetPlayerReady(bool playerReady)
 {
   //  PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Ready!" : "Ready?";
    // PlayerReadyImage.enabled = playerReady;
 }
 

}
