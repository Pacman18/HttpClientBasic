using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 로컬 HTTPS 서버 연결 테스트. 인스펙터에서 URL(포트·경로)만 맞추면 됩니다.
/// 자체 서명 인증서는 개발용으로만 검증 생략합니다.
/// </summary>
public class HttpClient : MonoBehaviour
{
    [SerializeField] private string url = "http://127.0.0.1:8443";
    [SerializeField] private KeyCode testKey = KeyCode.T;

    private bool _requestInProgress;

    private void Update()
    {
        if (!Input.GetKeyDown(testKey))
            return;
        if (_requestInProgress)
            return;

        StartCoroutine(TestLocalHttps());
    }

    private IEnumerator TestLocalHttps()
    {
        _requestInProgress = true;
        using (var request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new DevBypassCertificate();
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[HTTPS] 실패: {request.error} (responseCode={request.responseCode})");
            }
            else
            {
                Debug.Log($"[HTTPS] 성공: HTTP {request.responseCode}\n본문:\n{request.downloadHandler.text}");
            }
        }

        _requestInProgress = false;
    }

    /// <summary>로컬 개발용 자체 서명 인증서 허용. 배포 빌드에서는 제거하세요.</summary>
    private sealed class DevBypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}
