using UnityEngine;
using WebSocketSharp;
using System;

public class WsSingleton : MonoBehaviour
{
    private static WsSingleton instance;
    private WebSocket webSocket;
    
    public event Action<string> OnMessageReceived;
    public event Action OnConnectionOpen;
    public event Action OnConnectionClosed;

    public static WsSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WsSingleton>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("WebSocketManager");
                    instance = obj.AddComponent<WsSingleton>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Connect(string playerName)
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            Debug.LogWarning("WebSocket is already connected.");
            return;
        }

        webSocket = new WebSocket("ws://192.168.18.77:4000/connect?id=" + playerName);

        webSocket.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened.");
            OnConnectionOpen?.Invoke();
        };

        webSocket.OnMessage += (sender, e) =>
        {
            Debug.Log($"WebSocket message received: {e.Data}");
            OnMessageReceived?.Invoke(e.Data);
            // Handle the incoming message here.
        };

        webSocket.OnError += (sender, e) =>
        {
            Debug.LogError($"WebSocket error: {e.Message}");
            OnConnectionClosed?.Invoke();
        };

        webSocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed.");
            OnConnectionClosed?.Invoke();
            webSocket = null;
        };

        webSocket.Connect();
    }

    public void Send(string message)
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Send(message);
        }
        else
        {
            Debug.LogWarning("WebSocket is not connected.");
        }
    }

    public void Close()
    {
        if (webSocket != null && webSocket.IsAlive)
        {
            webSocket.Close();
        }
    }

    private void OnDestroy()
    {
        Close();
    }

}
