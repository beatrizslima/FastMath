using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;

public class WebSocketSingleton : MonoBehaviour
{
    private static WebSocketSingleton instance;
    private WebSocket webSocket;

    public static WebSocketSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WebSocketSingleton>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("WebSocketSingleton");
                    instance = singletonObject.AddComponent<WebSocketSingleton>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void CreateConn(string playerName)
    {
        // Inicialize sua conexão WebSocket aqui
        webSocket = new WebSocket("ws://192.168.18.77:4000/connect?id=" + playerName);
        webSocket.Connect();

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };
        webSocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);
        };
        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };
    }

    // Você pode adicionar métodos públicos para interagir com a conexão WebSocket, como enviar mensagens, fechar a conexão, etc.
    public void SendMessage(string message)
    {
        if (webSocket?.State == WebSocketState.Open)
        {
            webSocket.SendText(message);
        }
    }

    // Lembre-se de adicionar métodos para fechar a conexão WebSocket quando o jogo for encerrado.
    private void OnApplicationQuit()
    {
        if (webSocket?.State == WebSocketState.Open)
        {
            webSocket.Close();
        }
    }
}
