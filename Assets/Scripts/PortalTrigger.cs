using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalTrigger : MonoBehaviour
{
    [Header("URP ������ ����")]
    [Tooltip("������Ʈ�� URP Renderer ������ ������ �����ϼ���.")]
    public ScriptableRendererData rendererData;

    [Tooltip("������ ������ Render Objects ����� �̸��� ��Ȯ�� �Է��ϼ���.")]
    public string targetFeatureName = "StencilTarget";

    [Header("��Ż ������Ʈ ����")]
    [Tooltip("�浹 �� ��Ȱ��ȭ��ų ��Ż ���� ������Ʈ ��ü�� ���⿡ �����ϼ���.")]
    public GameObject portalObjectToDisable;

    private RenderObjects renderObjectsFeature;
    private bool isPortalViewActive = false;

    void Start()
    {
        if (rendererData == null)
        {
            Debug.LogError("PortalViewController: ������ �����Ͱ� �������� �ʾҽ��ϴ�!");
            this.enabled = false;
            return;
        }

        renderObjectsFeature = FindRenderFeature(targetFeatureName);

        if (renderObjectsFeature == null)
        {
            Debug.LogError($"PortalViewController: '{targetFeatureName}' ����� ã�� �� �����ϴ�.");
            this.enabled = false;
            return;
        }

        // ������ �� ���� ���·� �ʱ�ȭ
        ResetToNormalViewOnStart();
    }

    // [�ٽ� ����] ��ũ��Ʈ(������Ʈ)�� �ı��� �� (�ַ� ���� ���� ��) ȣ��˴ϴ�.
    void OnDestroy()
    {
        // ������ �����Ͱ� �޸𸮿��� �������� �ʾ��� ��쿡�� ���� (���� ����)
        if (rendererData != null && renderObjectsFeature != null)
        {
            SetStencilState(CompareFunction.Equal, "������Ʈ �ı� �� Equal�� �ʱ�ȭ");
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
                Debug.Log($"'{portalObjectToDisable.name}' ��Ż�� ��Ȱ��ȭ�߽��ϴ�.");
            }
            else
            {
                Debug.LogWarning("��Ȱ��ȭ�� ��Ż ������Ʈ�� �������� �ʾ�, ���� ������Ʈ�� ��Ȱ��ȭ�մϴ�.");
                this.gameObject.SetActive(false);
            }
        }
    }

    public void ActivatePortalView()
    {
        if (renderObjectsFeature == null || isPortalViewActive) return;

        isPortalViewActive = true;
        SetStencilState(CompareFunction.NotEqual, "Ȱ��ȭ: NotEqual");
    }

    public void ResetToNormalView()
    {
        if (renderObjectsFeature == null) return;

        isPortalViewActive = false;
        SetStencilState(CompareFunction.Equal, "����: Equal");

        if (portalObjectToDisable != null)
        {
            portalObjectToDisable.SetActive(true);
            Debug.Log($"'{portalObjectToDisable.name}' ��Ż�� �ٽ� Ȱ��ȭ�߽��ϴ�.");
        }
    }

    private void ResetToNormalViewOnStart()
    {
        isPortalViewActive = false;
        SetStencilState(CompareFunction.Equal, "���� �� Equal�� �ʱ�ȭ");

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
        Debug.Log($"[PortalViewController] ���ٽ� ���� {message}");
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