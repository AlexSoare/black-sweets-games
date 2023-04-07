using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField] private OpenRoomPanel openRoomPanel;
    [SerializeField] private LobbyPanel lobbyPanel;

    [SerializeField] private DrawingGame drawingGame;

    private string roomCode;

    private void Awake()
    {
        Application.runInBackground = true;

        instance = this;
    }

    private void Start()
    {
        openRoomPanel.Show();
    }

    #region Room connection
    public void RoomHasOpened(string roomCode)
    {
        this.roomCode = roomCode;
        openRoomPanel.Hide();

        ServerAPI.ConnectToWebSocket(roomCode, WebSocketConnected);
    }
    private void WebSocketConnected(bool success)
    {
        Debug.LogError(success);
        if (success)
            drawingGame.gameObject.SetActive(true);
    }
    #endregion

    private void Update()
    {
        ServerAPI.Tick();
    }
}
