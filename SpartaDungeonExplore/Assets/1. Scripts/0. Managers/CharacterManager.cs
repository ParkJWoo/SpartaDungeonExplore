using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    #region CharacterManager ΩÃ±€≈Ê»≠
    private static CharacterManager instance;
    public static CharacterManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject("CharacterManager").AddComponent<CharacterManager>();
            }

            return instance;
        }
    }

    #endregion

    private Player player;
    public Player Player
    {
        get { return player;}
        set { player = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
