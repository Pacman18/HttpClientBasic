using UnityEngine;
using Mirror;

public class Starter : MonoBehaviour
{
    
    void Start()
    {
        NetworkServer.RegisterHandler<TestMessage>(OnTestMessage);
        NetworkClient.RegisterHandler<TestMessage>(OnClientTestMessage);        
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            NetworkManager.singleton.StartHost();
        }

        if(Input.GetKeyDown(KeyCode.F2))
        {
            NetworkManager.singleton.StartClient();
        }

        if(Input.GetKeyDown(KeyCode.F3))
        {
            NetworkClient.Send(new TestMessage
            {
                color = Color.red
            });
        }

        if(Input.GetKeyDown(KeyCode.F4))
        {
            if(NetworkServer.active)
            {
                NetworkServer.SendToAll(new TestMessage
                {
                    color = Color.green
                });                
            }
            else
            {
                Debug.Log("Server is not active");
            }
        }
        
    }

    private void OnTestMessage(NetworkConnectionToClient conn, TestMessage msg)
    {
        Debug.Log($"OnTestMessage TestMessage received: {msg.color}" + " from " + conn.connectionId);
    }

    private void OnClientTestMessage(TestMessage msg)
    {
        Debug.Log($"OnClientTestMessage TestMessage received: {msg.color}");
    }
}


public struct TestMessage : NetworkMessage
{
    public Color color;
}




