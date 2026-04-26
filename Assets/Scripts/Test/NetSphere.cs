using UnityEngine;
using Mirror;

public class NetSphere : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[NetSphere] OnStartClient netId={netId}, isLocalPlayer={isLocalPlayer}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"[NetSphere] OnStartServer netId={netId}, isLocalPlayer={isLocalPlayer}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log($"[NetSphere] OnStartLocalPlayer netId={netId}, isLocalPlayer={isLocalPlayer}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");        

        Debug.Log($"[NetSphere] Number of players: {NetworkManager.singleton.numPlayers}");

    }
    
}
