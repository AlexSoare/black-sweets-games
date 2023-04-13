using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DrawingGameStates
{
    Lobby,
    WaitingForDrawings,
    ShowingDrawings,
    ShowingTitles,
    ShowingRoundScore,
    ShowingTotalScore,
}

[Serializable]
public class Player : IPlayerState
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public string State;
    [SerializeField]
    public string Uid;
    [SerializeField]
    public bool Ready;
    [SerializeField]
    public Drawing Avatar;
    [SerializeField]
    public Drawing Drawing;
    [SerializeField]
    public Title TitleToDraw;
    [SerializeField]
    public Title WrittenTitle;
    [SerializeField]
    public Title ChosenTitle;
    [SerializeField]
    public int Score;

    public Player() { }
    public Player(string uid, string name)
    {
        Uid = uid;
        Name = name;
    }

    public string GetUid()
    {
        return Uid;
    }
}

[Serializable]
public class Title
{
    [SerializeField]
    public string TitleText;
    [SerializeField]
    public List<string> WrittedByUid;
    [SerializeField]
    public List<string> ChosenByUid;

    public Title() { }
    public Title(string title, string playerUid)
    {
        TitleText = title;

        WrittedByUid = new List<string>();
        WrittedByUid.Add(playerUid);

        ChosenByUid = new List<string>();
    }

}

[Serializable]
public class Drawing
{
    [SerializeField]
    public string Uid;
    [SerializeField]
    public string Base64Texture;
    [SerializeField]
    public bool Shown;
}

[Serializable]
public class Round
{
    [SerializeField]
    public Drawing Drawing;
    [SerializeField]
    public List<Title> Titles;
}

[Serializable]
public class DrawingGameStateData// : IWebSocketMsg
{
    [SerializeField]
    public string State;
    [SerializeField]
    public int RoundNumber;
    [SerializeField]
    public List<Player> Players;
    [SerializeField]
    public List<Drawing> Drawings;
    [SerializeField]
    public Drawing CurrentDrawing;
    [SerializeField]
    public List<Title> CurrentTitles;
    [SerializeField]
    public List<string> CurrentRoundWinners;

    public bool PlayerInRoom(string uid, out Player player)
    {
        if (Players == null)
        {
            player = null;
            return false;
        }

        player = Players.Find(p => p.Uid == uid);

        return player != null;
    }

    public void AddPlayer(Player player)
    {
        if (Players == null)
            Players = new List<Player>();

        if (Players.Find(p => p.Uid == player.Uid) == null)
            Players.Add(player);
        else
            Debug.LogError("Trying to add an already existing user");
    }

    public void ResetPlayers()
    {
        foreach (var p in Players)
            p.Ready = false;
    }

    public void Clear()
    {
        Players = new List<Player>();
        Drawings = new List<Drawing>();
        CurrentTitles = new List<Title>();
        CurrentRoundWinners = new List<string>();
    }
}

[Serializable]
public class PlayerConnectedMsg// : IWebSocketMsg
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public string Uid;
    [SerializeField]
    public bool Reconnected;
}
[Serializable]
public class PlayerDrawingMsg// : IWebSocketMsg
{
    [SerializeField]
    public string Uid;
    [SerializeField]
    public string DrawingBase64;
}
[Serializable]
public class PlayerCustomMsg// : IWebSocketMsg
{
    [SerializeField]
    public string Uid;
    [SerializeField]
    public string Msg;
}

public class DrawingGame : StateMachineBase<DrawingGameStates, DrawingGameStateData>
{
    [SerializeField] private LobbyPanel lobbyPanel;
    [SerializeField] private WaitingForDrawingsPanel waitingForDrawingsPanel;
    [SerializeField] private ShowingDrawingsPanel showingDrawingPanel;
    [SerializeField] private ShowingTitlesPanel showingTitlesPanel;
    [SerializeField] private ShowingRoundScorePanel showingRoundScorePanel;
    [SerializeField] private ShowingTotalScorePanel showingTotalScorePanel;

    private void Start()
    {
        InitStates();

        ChangeState(DrawingGameStates.Lobby);
    }

    private void InitStates()
    {
        AddState(DrawingGameStates.Lobby, new LobbyState(lobbyPanel));
        AddState(DrawingGameStates.WaitingForDrawings, new WaitingForDrawingsState(waitingForDrawingsPanel));
        //AddState(DrawingGameStates.ShowingDrawings, new ShowingDrawingsState(showingDrawingPanel));
        //AddState(DrawingGameStates.ShowingTitles, new ShowingTitlesState(showingTitlesPanel));
        //AddState(DrawingGameStates.ShowingRoundScore, new ShowingRoundScoreState(showingRoundScorePanel));
        //AddState(DrawingGameStates.ShowingTotalScore, new ShowingTotalScoreState(showingTotalScorePanel));
    }

    private void OnPlayerConnected(PlayerConnectedMsg newPlayer)
    {
        if (!newPlayer.Reconnected)
            return;

        Player tempPlayer;

        if (!GlobalStatesData.PlayerInRoom(newPlayer.Uid, out tempPlayer))
        {
            tempPlayer = new Player(newPlayer.Uid, newPlayer.Name);
            tempPlayer.State = DrawingGameStates.Lobby.ToString();

            GlobalStatesData.AddPlayer(tempPlayer);
        }

        // type, roomState, playersState
        ServerAPI.SendToWebSocket(WebSocketMessageType.RoomStateUpdate, GlobalStatesData, GlobalStatesData.Players);
    }

    [SerializeField]
    private static List<string> Titles = new List<string>
    {
        "Pisica",
        "Caine",
        "Felie de tort",
        "Ciorapi",
        "Om de zapada",
        "Zmeu",
        "2 prieteni jucandu-se",
        "Masa plina de cadouri",
        "Papusa",
        "Zebra",
        "Urs",
        "Vulpe",
        "Elefant",
    };

    public static List<string> GenerateTitlesToDraw(int titlesToGenerate)
    {
        var titlesCopy = new List<string>();

        foreach (var t in Titles)
            titlesCopy.Add(t);

        var generatedTitles = new List<string>();

        while(generatedTitles.Count< titlesToGenerate)
        {
            var rndInd = UnityEngine.Random.Range(0, titlesCopy.Count);
            generatedTitles.Add(titlesCopy[rndInd]);
            titlesCopy.Remove(titlesCopy[rndInd]);
        }

        return titlesCopy;
    }

    public static Sprite GetDrawingSprite(string drawingBase64, bool save = false)
    {
        byte[] imageBytes = Convert.FromBase64String(drawingBase64);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);

        // Remove white bkg
        Color[] pixels = tex.GetPixels(0, 0, tex.width, tex.height, 0);

        Color averageColor = new Color(0, 0, 0, 0);
        List<int> usedPixelsIndexes = new List<int>();
        for (int p = 0; p < pixels.Length; p++)
        {
            if (pixels[p] == Color.white)
                pixels[p] = new Color(0, 0, 0, 0);
            else
            {
                averageColor += pixels[p];
                usedPixelsIndexes.Add(p);
            }
        }
        averageColor = averageColor * (1f / usedPixelsIndexes.Count);
        foreach (var pi in usedPixelsIndexes)
            pixels[pi] = averageColor;

        tex.SetPixels(0, 0, tex.width, tex.height, pixels, 0);
        tex.Apply();
        // ----

#if UNITY_EDITOR
        if(save)
        {
            // Save the texture as a PNG file
            byte[] bytes = tex.EncodeToPNG();
            string fileName = "Texture_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            System.IO.File.WriteAllBytes(Application.dataPath + "/Graphics/AI/TextureGenerator/" + fileName, bytes);

            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        return sprite;
    }
}
