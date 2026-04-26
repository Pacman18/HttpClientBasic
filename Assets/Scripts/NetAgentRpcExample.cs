using Mirror;
using UnityEngine;

public class NetAgentRpcExample : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    // 중요:
    // 이 스크립트는 [Command] + [ClientRpc] 예시만 보여줍니다.
    // 자동 Transform 동기화를 원하면 NetworkTransform 컴포넌트를 함께 붙이세요.
    // (없어도 RpcApplyPosition으로 위치 반영은 가능하지만, 보간/동기화 품질은 직접 처리해야 함)

    void Update()
    {
        if (!isLocalPlayer) return;

        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > 0.01f)
            CmdMove(h);
    }

    [Command]
    void CmdMove(float horizontal)
    {
        Vector3 p = transform.position;
        p.x += horizontal * moveSpeed * Time.deltaTime;
        transform.position = p;

        // 서버에서 이동 후 모든 클라이언트에게 위치 전파
        RpcApplyPosition(p);
    }

    [ClientRpc]
    void RpcApplyPosition(Vector3 serverPos)
    {
        // 호스트 서버는 이미 적용됨
        if (isServer) return;

        // RPC로 받은 서버 위치를 클라이언트에 수동 반영
        transform.position = serverPos;
    }
}
