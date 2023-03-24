using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener( () => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load( Loader.Scene.MainMenuScene );
        } );
        readyButton.onClick.AddListener( () => {
            CharacterSelectReady.Instance.SetPlayerRead();
        } );
    }
}
