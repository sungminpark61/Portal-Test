using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalTrigger : MonoBehaviour
{
    [Header("URP 렌더러 설정")]
    [Tooltip("프로젝트의 URP Renderer 데이터 에셋을 연결하세요.")]
    public ScriptableRendererData rendererData;

    [Tooltip("설정을 변경할 Render Objects 기능의 이름을 정확히 입력하세요.")]
    public string targetFeatureName = "StencilTarget";

    [Header("포탈 오브젝트 설정")]
    [Tooltip("충돌 시 비활성화시킬 포탈 게임 오브젝트 자체를 여기에 연결하세요.")]
    public GameObject portalObjectToDisable;

    private RenderObjects renderObjectsFeature;
    private bool isPortalViewActive = false;

    void Start()
    {
        if (rendererData == null)
        {
            Debug.LogError("PortalViewController: 렌더러 데이터가 설정되지 않았습니다!");
            this.enabled = false;
            return;
        }

        renderObjectsFeature = FindRenderFeature(targetFeatureName);

        if (renderObjectsFeature == null)
        {
            Debug.LogError($"PortalViewController: '{targetFeatureName}' 기능을 찾을 수 없습니다.");
            this.enabled = false;
            return;
        }

        // 시작할 때 원래 상태로 초기화
        ResetToNormalViewOnStart();
    }

    // [핵심 변경] 스크립트(오브젝트)가 파괴될 때 (주로 게임 종료 시) 호출됩니다.
    void OnDestroy()
    {
        // 렌더러 데이터가 메모리에서 해제되지 않았을 경우에만 실행 (오류 방지)
        if (rendererData != null && renderObjectsFeature != null)
        {
            SetStencilState(CompareFunction.Equal, "오브젝트 파괴 시 Equal로 초기화");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") && !isPortalViewActive)
        {
            ActivatePortalView();

            if (portalObjectToDisable != null)
            {
                portalObjectToDisable.SetActive(false);
                Debug.Log($"'{portalObjectToDisable.name}' 포탈을 비활성화했습니다.");
            }
            else
            {
                Debug.LogWarning("비활성화할 포탈 오브젝트가 지정되지 않아, 현재 오브젝트를 비활성화합니다.");
                this.gameObject.SetActive(false);
            }
        }
    }

    public void ActivatePortalView()
    {
        if (renderObjectsFeature == null || isPortalViewActive) return;

        isPortalViewActive = true;
        SetStencilState(CompareFunction.NotEqual, "활성화: NotEqual");
    }

    public void ResetToNormalView()
    {
        if (renderObjectsFeature == null) return;

        isPortalViewActive = false;
        SetStencilState(CompareFunction.Equal, "복구: Equal");

        if (portalObjectToDisable != null)
        {
            portalObjectToDisable.SetActive(true);
            Debug.Log($"'{portalObjectToDisable.name}' 포탈을 다시 활성화했습니다.");
        }
    }

    private void ResetToNormalViewOnStart()
    {
        isPortalViewActive = false;
        SetStencilState(CompareFunction.Equal, "시작 시 Equal로 초기화");

        if (portalObjectToDisable != null && !portalObjectToDisable.activeSelf)
        {
            portalObjectToDisable.SetActive(true);
        }
    }

    private void SetStencilState(CompareFunction state, string message)
    {
        var newSettings = renderObjectsFeature.settings.stencilSettings;
        newSettings.stencilCompareFunction = state;
        renderObjectsFeature.settings.stencilSettings = newSettings;
        rendererData.SetDirty();
        Debug.Log($"[PortalViewController] 스텐실 상태 {message}");
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