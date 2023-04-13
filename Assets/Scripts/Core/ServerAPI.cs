using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;
using System.Linq.Expressions;

public interface IQueryRequest
{

}

public interface IQueryRespone
{

}

public class OpenRoomQuery
{
    public class Request : IQueryRequest
    {
        public string roomCode;
        public Request(string roomCode)
        {
            this.roomCode = roomCode;
        }
    }
    public class Response : IQueryRespone
    {
        public bool success;
    }
}

public enum WebSocketMessageType
{
    PlayerConnected,
    RoomStateUpdate,
    PlayerInputUpdate,
    PlayerCustomMessage,
    PingPlayer,
    Error
}

//public interface IWebSocketMsg { }
public interface IWebSocketMsgListener {
    int GetHash();
    void Trigger(string json);
}

public interface IPlayerState
{
    string GetUid();
}

[Serializable]
public class PlayerStateMessage<W> where W : IPlayerState
{
    [SerializeField]
    public string Uid;
    [SerializeField]
    public W State;

    public PlayerStateMessage() { }
    public PlayerStateMessage(W state)
    {
        Uid = state.GetUid();
        State = state;
    }
}

[Serializable]
public class WebSocketMessage<T, W> /*where T : IWebSocketMsg*/ where W : IPlayerState
{
    [SerializeField]
    public string Type;
    [SerializeField]
    public T RoomState;
    [SerializeField]
    public List<PlayerStateMessage<W>> PlayersState;

    public WebSocketMessage() { }

    public WebSocketMessage(WebSocketMessageType type, T roomState, List<W> playersState)
    {
        Type = type.ToString();
        RoomState = roomState;
        PlayersState = new List<PlayerStateMessage<W>>();
        foreach (var p in playersState)
            PlayersState.Add(new PlayerStateMessage<W>(p));
    }
}

public class WebSocketMsgListener<T> : IWebSocketMsgListener
{
    public Action<T> Callback;

    public WebSocketMsgListener(Action<T> callback)
    {
        Callback = callback;
    }

    public void Trigger(string json)
    {
        var msgObj = JsonUtility.FromJson<T>(json);
        Callback(msgObj);
    }

    public int GetHash()
    {
        return Callback.GetHashCode();
    }
}

public static class ServerAPI
{
    private class RawWebSocketMessage
    {
        public string msgType;
        public string msg;
    }

#if UNITY_EDITOR
    private const string baseUrl = "http://localhost:5000";
#else
    private const string baseUrl = "https://black-sweets-games.herokuapp.com";
#endif

    private const string openRoomAPI = "/openRoom";

    private static WebSocket webSocket;
    private static List<string> webSocketEventsQueue = new List<string>();
    private static Dictionary<WebSocketMessageType, List<IWebSocketMsgListener>> webSocketMessageListeners = new Dictionary<WebSocketMessageType, List<IWebSocketMsgListener>>();
    private static Action<bool> webSocketConnectedCallback;
    private static bool clientConnectedToWebSocket;

#region PUBILC
    public static void OpenRoom(OpenRoomQuery.Request request, Action<OpenRoomQuery.Response> responseCallback)
    {
        var url = baseUrl + openRoomAPI;
        CoroutineHelper.Start(PostReuqest(url, request, responseCallback));
    }
    public static void ConnectToWebSocket(string roomCode, Action<bool> connectedCallback)
    {
        webSocketConnectedCallback = connectedCallback;

#if UNITY_EDITOR
        webSocket = new WebSocket($"ws://localhost:5000/?origin=client&roomCode={roomCode}");
#else
        webSocket = new WebSocket($"wss://black-sweets-games.herokuapp.com/?origin=client&roomCode={roomCode}");
#endif

        webSocket.OnOpen += () =>
        {
            Debug.LogError("WS connection open!");
            webSocketConnectedCallback(true);
            clientConnectedToWebSocket = true;
        };
        webSocket.OnError += (e) =>
        {
            Debug.LogError("WS connection error: " + e);
        };
        webSocket.OnClose += (e) =>
        {
            Debug.LogError("WS closed: " + e);
            //webSocketEventsQueue.Add(e.Reason);
        };

        webSocket.OnMessage += (e) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(e);
            Debug.LogError("WS message: " + message);
            webSocketEventsQueue.Add(message);
        };

        webSocket.Connect();
    }
    public static void AddWebSocketMessageCallback<T>(WebSocketMessageType type, Action<T> callback)// where T: IWebSocketMsg
    {
        
        if (!webSocketMessageListeners.ContainsKey(type))
            webSocketMessageListeners.Add(type, new List<IWebSocketMsgListener>());

        var listener = new WebSocketMsgListener<T>(callback);

        webSocketMessageListeners[type].Add(listener);
    }

    public static void RemoveWebSocketMessageCallback<T>(WebSocketMessageType type, Action<T> callback) //where T : IWebSocketMsg
    {
        if (webSocketMessageListeners.ContainsKey(type))
        {
            var toRemove = webSocketMessageListeners[type].Find(l => l.GetHash() == callback.GetHashCode());

            if(toRemove != null)
                webSocketMessageListeners[type].Remove(toRemove);
        }
    }

    public static void SendToWebSocket<T, W>(WebSocketMessageType type, T roomState, List<W> playersState)/* where T : IWebSocketMsg*/ where W : IPlayerState
    {
        var msgToSend = new WebSocketMessage<T, W>(type, roomState, playersState);

        webSocket.SendText(JsonUtility.ToJson(msgToSend));
    }
#endregion

    private static void ProcessWebSocketEvents()
    {
        foreach (var data in webSocketEventsQueue)
        {
            var msgObj = JsonUtility.FromJson<RawWebSocketMessage>(data);

            var msgType = (WebSocketMessageType)Enum.Parse(typeof(WebSocketMessageType),msgObj.msgType);
            
            if (webSocketMessageListeners.ContainsKey(msgType))
                foreach (var listener in webSocketMessageListeners[msgType])
                    listener.Trigger(msgObj.msg);
        }

        webSocketEventsQueue.Clear();
    }

    private static IEnumerator GetReuqest<T>(string url, IQueryRequest request, Action<T> responseCallback)
    {
        using (UnityWebRequest unityReq = UnityWebRequest.Get(url))
        {
            yield return unityReq.SendWebRequest();

            var respObj = JsonUtility.FromJson<T>(unityReq.downloadHandler.text);
            responseCallback(respObj);
        }
    }
    private static IEnumerator PostReuqest<T>(string url, IQueryRequest request, Action<T> responseCallback)
    {
        var json = JsonUtility.ToJson(request);

        using (UnityWebRequest unityReq = UnityWebRequest.Post(url, ""))
        {
            byte[] bytes = Encoding.ASCII.GetBytes(json);

            var uploader = new UploadHandlerRaw(bytes);
            uploader.contentType = "application/json";
            unityReq.uploadHandler = uploader;

            unityReq.SetRequestHeader("Content-Type", "application/json");

            yield return unityReq.SendWebRequest();

            var respObj = JsonUtility.FromJson<T>(unityReq.downloadHandler.text);
            responseCallback(respObj);
        }
    }

    public static void Tick()
    {
        if (webSocket == null) return;

#if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.DispatchMessageQueue();
#endif

        ProcessWebSocketEvents();
    }
}
