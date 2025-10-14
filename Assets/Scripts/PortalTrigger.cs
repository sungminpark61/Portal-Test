using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PortalTrigger : MonoBehaviour
{
    [Header("URP ������ ����")]
    [Tooltip("������Ʈ�� URP Renderer ������ ������ �����ϼ���. (��: PC_Renderer)")]
    public ScriptableRendererData rendererData;

    [Tooltip("������ ������ Render Objects ����� �̸��� ��Ȯ�� �Է��ϼ���.")]
    public string targetFeatureName = "StencilTarget";

    [Header("�÷��̾� ����")]
    [Tooltip("�÷��̾� �Ǵ� ���� ī�޶� ������Ʈ�� �����ϼ���.")]
    public Transform playerCamera;

    // ���� ����
    private RenderObjects renderObjectsFeature;
    private bool isPlayerInside = false;
    private bool wasInFront;

    void Start()
    {
        if (rendererData == null || playerCamera == null)
        {
            Debug.LogError("PortalTrigger: ������ ������ �Ǵ� �÷��̾� ī�޶� �������� �ʾҽ��ϴ�! Inspector â�� Ȯ�����ּ���.");
            this.enabled = false;
            return;
        }

        renderObjectsFeature = FindRenderFeature(targetFeatureName);

        if (renderObjectsFeature == null)
        {
            Debug.LogError($"PortalTrigger: '{targetFeatureName}' �̸��� ���� RenderObjects ����� '{rendererData.name}' ���¿��� ã�� �� �����ϴ�.");
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

    // [������ �κ�] ���ٽ� ���¸� ��ȯ�ϴ� �Լ�
    private void SwitchStencilState()
    {
        if (renderObjectsFeature == null) return;

        var newSettings = renderObjectsFeature.settings.stencilSettings;

        // 'CompareFunction'�� 'stencilCompareFunction'���� ����
        if (newSettings.stencilCompareFunction == CompareFunction.Equal)
        {
            newSettings.stencilCompareFunction = CompareFunction.NotEqual;
            Debug.Log("���ٽ� ���� ����: NotEqual (��Ż ���� ����)");
        }
        else
        {
            newSettings.stencilCompareFunction = CompareFunction.Equal;
            Debug.Log("���ٽ� ���� ����: Equal (��Ż �ܺη� ����)");
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