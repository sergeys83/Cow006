using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class BtnLobby : MonoBehaviour
{
 // public Text RoomNameText;
  public Text RoomPlayersText;
  private RoomInfo info;

  public void Init(RoomInfo info)
  {
    this.info = info;
   // RoomNameText.text = info.Name;
    RoomPlayersText.text = info.Name + "   " + info.PlayerCount + " / " + info.MaxPlayers;
  }
  public void Join()
  {
    PhotonNetwork.JoinRoom(info.Name);
  }

}
