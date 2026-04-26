using UnityEngine;
using Mirror;

public class NetAgent : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 10f;

    [SerializeField] MeshRenderer meshRenderer;
    bool loggedNonLocalInputBlock;

    // -----------------------------
    // 참고용: SyncVar 방식 (주석 해제해서 사용)
    // NetworkTransform 없이 "위치 값"만 동기화하는 예시.
    // 실제 적용 시에는 Hook에서 transform 반영/보간 처리 필요.
    // -----------------------------
    // [SyncVar(hook = nameof(OnPosXChanged))]
    // float syncedPosX;
    //
    // void OnPosXChanged(float oldValue, float newValue)
    // {
    //     // 클라이언트에서 서버 값을 받아 위치 반영
    //     Vector3 p = transform.position;
    //     p.x = newValue;
    //     transform.position = p;
    // }

    // -----------------------------
    // 참고용: RPC 방식 (주석 해제해서 사용)
    // 서버 이동 후 ClientRpc로 모든 클라에 즉시 반영하는 예시.
    // 네트워크 지연/보간은 별도 처리 필요.
    // -----------------------------
    // [ClientRpc]
    // void RpcApplyPosition(Vector3 serverPosition)
    // {
    //     if (isServer) return; // 호스트 서버는 이미 위치가 맞음
    //     transform.position = serverPosition;
    // }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[NetAgent] OnStartClient netId={netId}, isLocalPlayer={isLocalPlayer}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log($"[NetAgent] OnStartLocalPlayer netId={netId}, isLocalPlayer={isLocalPlayer}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");

        if(netIdentity.isServer == false)
            meshRenderer.material.color = Color.blue;
        else
            meshRenderer.material.color = Color.red;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"[NetAgent] OnStartServer netId={netId}, isLocalPlayer={isLocalPlayer}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");
    }

    void Update()
    {
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1f;
        }

        if (horizontalInput != 0f)
        {
            if(isLocalPlayer == false)
                return;

            Debug.Log($"[NetAgent] CmdMove request from local player. netId={netId}, input={horizontalInput}, isOwned={isOwned}, isServer={isServer}, isClient={isClient}");
            CmdMove(horizontalInput);
        }
    }

    [Command]
    void CmdMove(float horizontalInput)
    {
        Debug.Log($"[NetAgent] CmdMove executed on server. netId={netId}, input={horizontalInput}, connectionId={connectionToClient?.connectionId}");
        Vector3 delta = Vector3.right * horizontalInput * moveSpeed * Time.deltaTime;
        transform.Translate(delta, Space.World);

        // [SyncVar 방식 예시] 서버에서 동기화 값 업데이트
        // syncedPosX = transform.position.x;

        // [RPC 방식 예시] 서버 위치를 모든 클라이언트에 전파
        // RpcApplyPosition(transform.position);
    }
}
