using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesHolder : MonoBehaviour
{
    private static SpritesHolder instance;

    [SerializeField] private List<Sprite> playerAvatars;

    public static List<Sprite> PlayerAvatars {get {return instance.playerAvatars; }}
    
    private void Awake()
    {
        instance = this;
    }
}
