using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Scripts.DataStorage;
using Scripts.Profile;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public string nickName;
    public Sprite icon;
    public static PlayerManager Instance;
    public Dictionary<int, PlayerNet> players = new Dictionary<int, PlayerNet>();
    private string SceneName = "Level_Net";

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        if (SceneManager.GetActiveScene().name == SceneName)
        {
            transform.SetParent(GameManagerNet.S.root.transform);
        }
    }
    public void SetupNetPlayer(Photon.Realtime.Player pl)
    {
        nickName = DataSaver.Instance.playerData.playerName;
        pl.NickName = nickName;
        Hashtable hash = new Hashtable();
        hash.Add("icon", DataSaver.Instance.playerData.playerAvatar);
        hash.Add("nickName", nickName);
        hash.Add("score", 0);
        icon = AvatarManager.Instance.LoadAvatar(DataSaver.Instance.playerData.playerAvatar);
        pl.SetCustomProperties(hash);
    }
    public void CreatePlayer(Photon.Realtime.Player item, out Transform hand)
    {
        GameObject pl;
        string pName = "Players/Player" + item.ActorNumber;

        if (item.IsLocal && item.ActorNumber != 1)
        {
            pl = Instantiate(Resources.Load<GameObject>(pName), GameManagerNet.S.TargetPlayer(1).position,
                GameManagerNet.S.TargetPlayer(item.ActorNumber).rotation);
            hand = GameManagerNet.S.PlayerHand;
        }
        else
        {
            pl = Instantiate(Resources.Load<GameObject>(pName), GameManagerNet.S.TargetPlayer(2).position,
                GameManagerNet.S.TargetPlayer(item.ActorNumber).rotation);
            hand = GameManagerNet.S.AI2;
        }

        pl.name = pName;

        Debug.Log(pName + " Player " + item.NickName);
        pl.transform.SetParent(GameManagerNet.S.root.transform, false);
        PlayerNet playerNet = pl.GetComponent<PlayerNet>();

        players.Add(item.ActorNumber, playerNet);
        playerNet.Init(item);
    }

}
