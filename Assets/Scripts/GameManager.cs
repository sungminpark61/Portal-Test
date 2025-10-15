using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inspector â���� Portal ������Ʈ�� �پ��ִ� PortalViewController ��ũ��Ʈ�� ���⿡ ����� ��������.
    public PortalTrigger portalController;

    void Update()
    {
        // VR ü���� �����ϴ� Ű (��: 'R' Ű)�� ������ �� �׽�Ʈ
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndVRExperience();
        }
    }

    public void EndVRExperience()
    {
        // portalController�� ����Ǿ� �ִ��� Ȯ��
        if (portalController != null)
        {
            Debug.Log("VR ü���� �����ϰ� ���� ��� �ǵ����ϴ�.");

            // PortalViewController�� �Լ��� ȣ���Ͽ� �並 ������� ����!
            portalController.ResetToNormalView();
        }
        else
        {
            Debug.LogError("GameManager�� PortalViewController�� ������� �ʾҽ��ϴ�!");
        }
    }
}