using Mirror;
using UnityEngine;

public class NetAgentSyncVarExample : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    // 필요 컴포넌트 정리:
    // - 필수: NetworkIdentity
    // - 선택: NetworkTransform
    //   * 이 예제는 SyncVar로 x값을 직접 동기화하므로 NetworkTransform "필수 아님"
    //   * 대신 y/z, rotation, scale, 보간(interpolation)은 직접 구현해야 함
    //   * 위치/회전 전체를 자동 동기화하고 싶으면 NetworkTransform 추가 권장

    // 서버 값이 바뀌면 클라이언트에서 Hook 호출
    [SyncVar(hook = nameof(OnPosXChanged))]
    float syncedPosX;

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

        // SyncVar 값 갱신 -> 모든 클라이언트로 전파
        syncedPosX = p.x;
    }

    void OnPosXChanged(float oldX, float newX)
    {
        // 서버는 이미 값을 알고 있으므로 클라에서만 반영
        if (isServer) return;

        Vector3 p = transform.position;
        p.x = newX;
        transform.position = p;
    }
}
