using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenRoomPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomCodeInput;
    [SerializeField] private GameObject loadingPanel;

    private string currentRoomCode;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnOpenButton()
    {
        if (string.IsNullOrEmpty(roomCodeInput.text))
            return;

        currentRoomCode = roomCodeInput.text;
        var openRoomRequest = new OpenRoomQuery.Request(currentRoomCode);
        ServerAPI.OpenRoom(openRoomRequest, OpenRoomCallback);

        loadingPanel.SetActive(true);
    }

    private void OpenRoomCallback(OpenRoomQuery.Response res)
    {
        loadingPanel.SetActive(false);
        if (res.success)
        {
            GameManager.Instance.RoomHasOpened(currentRoomCode);
        }
    }
}
