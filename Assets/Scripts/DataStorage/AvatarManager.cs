using System.Collections.Generic;
using UnityEngine;

namespace Scripts.DataStorage
{
 public class AvatarManager : Singleton<AvatarManager>
 {
        [SerializeField] private List<Icon> _avatars = null;
        public string NextAvatar(string current)
        {
            return NextIcon(current, _avatars);
        }

      public string NextIcon(string current, List<Icon> iconList)
        {

            if (iconList.Count == 0)
            {
                Debug.LogError("Couldn't get next icon: No icons found");
                return null;
            }

            if (string.IsNullOrEmpty(current) == true)
            {
                return iconList[0].Name;
            }

            for (int i = 0; i < iconList.Count; i++)
            {
                if (iconList[i].Name == current)
                {
                    return iconList[(i + 1) % iconList.Count].Name;
                }
            }

            Debug.Log("Current icon [" + current + "] not found. Returning first icon.");
            return iconList[0].Name;
        }

        public Sprite LoadAvatar(string name)
        {
            return LoadIcon(name, _avatars);
        }

        private Sprite LoadIcon(string name, List<Icon> iconList)
        {
            if (iconList.Count == 0)
            {
                Debug.LogError("Couldn't load icon: No icons found.");
                return null;
            }
            if (string.IsNullOrEmpty(name) == true)
            {
                return iconList[iconList.Count - 1].Sprite;
            }
            else
            {
                foreach (Icon avatar in iconList)
                {
                    if (avatar.Name == name)
                    {
                        return avatar.Sprite;
                    }
                }
                Debug.LogError("Icon with name " + name + " not found");
                return iconList[iconList.Count - 1].Sprite;
            }
        }

    }

}