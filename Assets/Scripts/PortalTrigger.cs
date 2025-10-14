using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalTrigger : MonoBehaviour
{
    [Header("URP 렌더러 설정")]
    [Tooltip("프로젝트의 URP Renderer 데이터 에셋을 연결하세요. (예: PC_Renderer)")]
    public ScriptableRendererData rendererData;

    [Tooltip("설정을 변경할 Render Objects 기능의 이름을 정확히 입력하세요.")]
    public string targetFeatureName = "StencilTarget";

    [Header("플레이어 설정")]
    [Tooltip("플레이어 또는 메인 카메라 오브젝트를 연결하세요.")]
    public Transform playerCamera;

    // 내부 변수
    private RenderObjects renderObjectsFeature;
    private bool isPlayerInside = false;
    private bool wasInFront;

    void Start()
    {
        if (rendererData == null || playerCamera == null)
        {
            Debug.LogError("PortalTrigger: 렌더러 데이터 또는 플레이어 카메라가 설정되지 않았습니다! Inspector 창을 확인해주세요.");
            this.enabled = false;
            return;
        }

        renderObjectsFeature = FindRenderFeature(targetFeatureName);

        if (renderObjectsFeature == null)
        {
            Debug.LogError($"PortalTrigger: '{targetFeatureName}' 이름을 가진 RenderObjects 기능을 '{rendererData.name}' 에셋에서 찾을 수 없습니다.");
            this.enabled = false;
            return;
        }

        wasInFront = IsPlayerInFront();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Player entered");
            isPlayerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Player exited");
            isPlayerInside = false;
        }
    }

    void Update()
    {
        if (!isPlayerInside) return;

        bool isInFront = IsPlayerInFront();

        if (isInFront != wasInFront)
        {
            SwitchStencilState();
            wasInFront = isInFront;
        }
    }

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = (playerCamera.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToPlayer);
        return dot > 0;
    }

    // [수정된 부분] 스텐실 상태를 전환하는 함수
    private void SwitchStencilState()
    {
        if (renderObjectsFeature == null) return;

        var newSettings = renderObjectsFeature.settings.stencilSettings;

        // 'CompareFunction'을 'stencilCompareFunction'으로 변경
        if (newSettings.stencilCompareFunction == CompareFunction.Equal)
        {
            newSettings.stencilCompareFunction = CompareFunction.NotEqual;
            Debug.Log("스텐실 상태 변경: NotEqual (포탈 내부 진입)");
        }
        else
        {
            newSettings.stencilCompareFunction = CompareFunction.Equal;
            Debug.Log("스텐실 상태 변경: Equal (포탈 외부로 나옴)");
        }

        renderObjectsFeature.settings.stencilSettings = newSettings;
        rendererData.SetDirty();
    }

    private RenderObjects FindRenderFeature(string featureName)
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == featureName)
            {
                return feature as RenderObjects;
            }
        }
        return null;
    }
}