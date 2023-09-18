using UnityEngine;
using WebSocketSharp;
using System;
using System.Collections;

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
                    Debug.Log(MatchData.Instance.roundId);

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

        webSocket = new WebSocket("wss://fast-math-ws-3lsl5v5pjq-rj.a.run.app:443/connect?id=" + playerName);
        webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        webSocket.EnableRedirection = true;

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
            StartCoroutine("closeConnection");
        };

        webSocket.Connect();
    }

    IEnumerator closeConnection()
    {
        yield return new WaitForSeconds(1);
        OnConnectionClosed?.Invoke();
        webSocket = null;
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
