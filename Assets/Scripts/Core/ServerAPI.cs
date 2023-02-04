using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;

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
    PlayerData,
    PingPlayer,
    Error
}

[Serializable]
public class WebSocketMessageParam
{
    public string paramName;
    public string paramValue;
    public WebSocketMessageParam(string paramName, string paramValue)
    {
        this.paramName = paramName;
        this.paramValue = paramValue;
    }
}

[Serializable]
public class WebSocketMessage
{
    [SerializeField] public string msgType;
    [SerializeField] public List<WebSocketMessageParam> data;

    public WebSocketMessage(string msgType, List<WebSocketMessageParam> data)
    {
        this.msgType = msgType;
        this.data = data;
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
    private static Dictionary<WebSocketMessageType, List<Action<string>>> webSocketMessageListeners = new Dictionary<WebSocketMessageType, List<Action<string>>>();
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
    public static void AddWebSocketMessageCallback(WebSocketMessageType type, Action<string> callBack)
    {
        if (!webSocketMessageListeners.ContainsKey(type))
            webSocketMessageListeners.Add(type, new List<Action<string>>());

        webSocketMessageListeners[type].Add(callBack);
    }
    public static void RemoveWebSocketMessageCallback(WebSocketMessageType type, Action<string> callBack)
    {
        if (webSocketMessageListeners.ContainsKey(type))
            webSocketMessageListeners[type].Remove(callBack);
    }
    public static void SendToWebSocket(WebSocketMessageType type, List<WebSocketMessageParam> msg)
    {
        var toSend = new WebSocketMessage(type.ToString(), msg);
        Debug.LogError("Send to WS: " + JsonUtility.ToJson(toSend));
        webSocket.SendText(JsonUtility.ToJson(toSend));
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
                    listener(msgObj.msg);
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
