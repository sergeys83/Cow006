using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Scripts.DataStorage;
using Random = System.Random;


namespace Scripts.Profile
{
    [Serializable]
    public class PlayerData
    {
        public int energy;
        public int curPoints; 
        public int money;
        public int wins;
        public int gamesPlayed;
        public string playerName;
        public string playerAvatar;
    }
    public class DataSaver:Singleton<DataSaver>
    {
        private PlayerData _playerData=new PlayerData();
        public PlayerData playerData => _playerData;

        private string jsonData;
        private int passwd, pwd;
        private const string LOADING_STRING = "Loading_Cloud.json";
        private const string LOADING = "Loading.json";
        private string path,file;
        public Action<bool> onDataLoad;

        public void Preloading()
        {
#if UNITY_ANDROID && !UNITY_EDITOR && !UNITY_EDITOR_WIN
            path = Path.Combine(Application.persistentDataPath, LOADING);
#else
            path = Path.Combine(Application.dataPath, LOADING);
           
#endif
            Debug.Log($"Path = {path}");
            bool loading = this;
            if (File.Exists(path)&&path.Length>0)
            {
                _playerData = Load();
               // LoadStringData(File.ReadAllText(path));
               loading = true;
            }
            else
            { 
                Init(NameGenerator(), AvatarManager.Instance.NextAvatar("0"),  200,  0,  0,  0 );
                Egame.InfoMessage = "New account created ";
                Debug.Log("File does not exist, new acc created");  
            }
            onDataLoad?.Invoke(loading);
        }
        
        public void LoadFromCloud(bool success)
        {
            if (success)
            {
                Debug.Log("Loading Data from Cloud=" +success);
                Egame.InfoMessage = "Loading Data from Cloud ";
               CloudSaveManager.Instance.LoadFromCloud();    

            }
            else
            {
                Debug.Log("Invoke Loading CloudData" + playerData.money);     
                Egame.InfoMessage = "Loading Data from disc ";
            }
           // onDataLoad?.Invoke();
        }
       public void SetData(string n, string p, int en= 0, int cp= 0, int gp= 0, int win= 0 )
        {
            _playerData.playerName = (n == null) ? _playerData.playerName : n;
            _playerData.playerAvatar = (p == null) ? _playerData.playerAvatar : p;
            _playerData.energy = (en == 0) ? _playerData.energy : en;
            _playerData.curPoints = (cp == 0) ? _playerData.curPoints : cp;
            _playerData.money = (gp == 0) ? _playerData.money : gp;
            _playerData.wins = (win == 0) ? _playerData.wins : win;
            Save();
        }

        public void SetData( int cp ,int gp)
        {
            _playerData.curPoints =  cp;
            _playerData.money = gp;
            Save();
        }
        private void Save()
        {
           file =  JsonUtility.ToJson(_playerData,true);
           File.WriteAllText(path,file);
        }
        
        public PlayerData Load()
        {
            _playerData = JsonUtility.FromJson<PlayerData>(File.ReadAllText(path));
            return _playerData;
        }

        public void SaveToCloud()
        {
            file =  JsonUtility.ToJson(_playerData,true);
#if UNITY_ANDROID
            CloudSaveManager.Instance.SaveToCloud();
#endif
        }
        public byte[] GetByteData()
        {
            file = JsonUtility.ToJson(_playerData, true);
            byte[] barray = Encoding.UTF8.GetBytes(file);
            return barray;
        }
        public void LoadStringData(string data)
        { 
            string s = Path.Combine(Application.persistentDataPath, LOADING_STRING);
           File.WriteAllText(path,data);
           File.WriteAllText(s,data);
           _playerData = JsonUtility.FromJson<PlayerData>(data);
        }

        public void LoadByteData(byte[] data)
        {
            LoadStringData(Encoding.UTF8.GetString(data,0,data.Length));
        }
        
        private void Init(string n, string p, int en= 0, int cp= 0, int gp= 0, int win= 0 )
        {
            _playerData.playerName = n;
            _playerData.playerAvatar = p;
            _playerData.energy = en;
            _playerData.curPoints = cp;
            _playerData.money = gp;
            _playerData.wins = win;
            Save();
        }
        public string NameGenerator()
        {
            string nm = null;
            Random r= new Random();
            Random j = new Random();
            var g = new Char[8];
            for (int i = 0; i < 8; i++)
            {
                int ra = j.Next(0, 6);
                if (ra < 4)
                    g[i] = (char) r.Next(0x041, 0x05A);
                else
                    g[i] = (char) r.Next(0x061, 0x07A);
                nm += g[i];
            }
           
            Debug.Log($"name = {nm} at step 9");
            return nm;
        }

    }
}