using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Scripts.Profile;
using UnityEngine;
using UnityEngine.UI;
using Player = Photon.Realtime.Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Lobby : MonoBehaviourPunCallbacks
{
    //List<GameObject> roomButtons = new List<GameObject>();
    //List<GameObject> playerButtons = new List<GameObject>();
    public Button CreateBtn;
    public GameObject prefub;
    public GameObject playerPrefub;
    public Text roomText;
    public RectTransform Roomcontent;
    public RectTransform PlayerContent;
    public GameObject LobbyPlayers;
    public GameObject LobbyView;
    public int x;
   
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;
  
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1.0";
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
        playerListEntries = new Dictionary<int, GameObject>();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PlayerManager.Instance.SetupNetPlayer(PhotonNetwork.LocalPlayer);
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }
    
    public override void OnJoinedRoom()
    {
        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        
        LobbyPlayers.SetActive(true);
        LobbyView.SetActive(false);
        if (playerListEntries==null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }
        foreach (var p in PhotonNetwork.PlayerList)
        {           
            GameObject playerAvatar = Instantiate(playerPrefub, PlayerContent.position, PlayerContent.rotation);
            playerAvatar.transform.parent = PlayerContent.transform;
            playerAvatar.transform.localScale = Vector3.one;
            Debug.Log(playerAvatar.name + "COUNT " + PhotonNetwork.CurrentRoom.Players.Values.Count);
            Debug.Log("Player = "+p.ActorNumber + " name = " +p.NickName + "avatar = " +p.CustomProperties["icon"].ToString() + " Nick = "+p.CustomProperties["nickName"]);
            playerAvatar.GetComponent<PlayerInLobby>().Init(p);            
            playerListEntries.Add(p.ActorNumber+1, playerAvatar);
        }
       
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
     
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Errror = {cause}");
    }
   
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        cachedRoomList.Clear();

        foreach (var p in playerListEntries.Values)
        {
            Destroy(p.gameObject);
        }
        playerListEntries.Clear();

        foreach (var p in PhotonNetwork.PlayerList)
        {           
            GameObject playerAvatar = Instantiate(playerPrefub, PlayerContent.position, PlayerContent.rotation);
            playerAvatar.transform.parent = PlayerContent.transform;
            playerAvatar.transform.localScale = Vector3.one;
            Debug.Log(playerAvatar.name + "COUNT " + PhotonNetwork.CurrentRoom.Players.Values.Count);
            playerAvatar.GetComponent<PlayerInLobby>().Init(p);
            playerListEntries.Add(p.ActorNumber+1, playerAvatar);
        }

        if (PhotonNetwork.PlayerList.Length >1)
        {
            Debug.Log(PhotonNetwork.PlayerList.Length + "COUNT " + PhotonNetwork.CurrentRoom.Players.Values.Count);
            PhotonNetwork.LoadLevel("Level_Net");
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);
    }
    
    public override void OnLeftRoom()
    {
        LobbyPlayers.SetActive(false);
        LobbyView.SetActive(true);
        
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public void CreatRoom()
    {
        string roomName = roomText.text+" Test";
        RoomOptions options = new RoomOptions();
        options.PlayerTtl = 20000;
        options.MaxPlayers = 2;
        options.EmptyRoomTtl = -1;
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("New Room Created");
        CreateBtn.gameObject.SetActive(false);
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
        CreateBtn.gameObject.SetActive(true);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }
    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }
    
    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(prefub);
            entry.transform.SetParent(Roomcontent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<BtnLobby>().Init(info);

            roomListEntries.Add(info.Name, entry);
        }
    }
}
