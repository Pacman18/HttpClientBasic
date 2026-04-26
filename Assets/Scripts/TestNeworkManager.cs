using UnityEngine;
using Mirror;

public struct CustomPingMessage : NetworkMessage
{
    public string text;
    public double clientTime;
}

public struct CustomPongMessage : NetworkMessage
{
    public string text;
    public double serverTime;
}

public class TestNeworkManager : NetworkManager
{
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log($"[Server] 클라이언트 연결됨. connId={conn.connectionId}");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"[Server] 클라이언트 연결 해제. connId={conn.connectionId}");
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log($"[Server] 플레이어 생성 완료. connId={conn.connectionId}, hasIdentity={conn.identity != null}");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CustomPingMessage>(OnServerPingMessage);
        Debug.Log("Server handler registered: CustomPingMessage");
    }

    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<CustomPingMessage>();
        base.OnStopServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<CustomPongMessage>(OnClientPongMessage);
        Debug.Log("Client handler registered: CustomPongMessage");
    }

    public override void OnStopClient()
    {
        NetworkClient.UnregisterHandler<CustomPongMessage>();
        base.OnStopClient();
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log($"[Client] 서버 연결 성공. address={networkAddress}");
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("[Client] 서버 연결 종료");
        base.OnClientDisconnect();
    }

    public override void OnClientError(TransportError error, string reason)
    {
        Debug.LogError($"[Client] 연결 에러. error={error}, reason={reason}");
        base.OnClientError(error, reason);
    }

    void OnServerPingMessage(NetworkConnectionToClient conn, CustomPingMessage msg)
    {
        Debug.Log($"[Server] 받은 Ping: {msg.text}, clientTime={msg.clientTime:F3}");

        conn.Send(new CustomPongMessage
        {
            text = "Pong from server",
            serverTime = NetworkTime.time
        });
    }

    void OnClientPongMessage(CustomPongMessage msg)
    {
        Debug.Log($"[Client] 받은 Pong: {msg.text}, serverTime={msg.serverTime:F3}");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartHost();
            Debug.Log("Host started");
        }        

        if (Input.GetKeyDown(KeyCode.F4))
        {
            StartClient();
            Debug.Log("Client started");
        }

        if(Input.GetKeyDown(KeyCode.F5))
        {
            StopHost();
            Debug.Log("Host stopped");
        }

        // RegisterHandler 예제:
        // F2를 누르면 클라이언트에서 서버로 커스텀 메시지를 보냄.
        // 서버 핸들러가 받아서 해당 클라이언트(conn)로 응답(Pong)을 다시 보냄.
        if (Input.GetKeyDown(KeyCode.F2) && NetworkClient.isConnected)
        {
            NetworkClient.Send(new CustomPingMessage
            {
                text = "Ping from client",
                clientTime = NetworkTime.time
            });            

            Debug.Log("Client sent CustomPingMessage");
        }
    }

    
}
