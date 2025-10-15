using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inspector 창에서 Portal 오브젝트에 붙어있는 PortalViewController 스크립트를 여기에 끌어다 놓으세요.
    public PortalTrigger portalController;

    void Update()
    {
        // VR 체험을 종료하는 키 (예: 'R' 키)를 눌렀을 때 테스트
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndVRExperience();
        }
    }

    public void EndVRExperience()
    {
        // portalController가 연결되어 있는지 확인
        if (portalController != null)
        {
            Debug.Log("VR 체험을 종료하고 원래 뷰로 되돌립니다.");

            // PortalViewController의 함수를 호출하여 뷰를 원래대로 복구!
            portalController.ResetToNormalView();
        }
        else
        {
            Debug.LogError("GameManager에 PortalViewController가 연결되지 않았습니다!");
        }
    }
}