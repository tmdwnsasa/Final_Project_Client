using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private static ResolutionManager instance;

    private int lastScreenWidth;
    private int lastScreenHeight;
    private float targetAspectRatio;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 이 오브젝트가 신 전환 시 파괴되지 않도록 함
            InitializeResolution();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeResolution()
    {
        // 초기 화면 크기 및 비율 설정
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        targetAspectRatio = (float)Screen.width / Screen.height;
    }

    void Update()
    {
        // 창 크기가 변경되었는지 확인
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            MaintainAspectRatio();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    void MaintainAspectRatio()
    {
        // 현재 화면 비율 계산
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (currentAspectRatio > targetAspectRatio)
        {
            // 현재 비율이 목표 비율보다 크다면 너비를 조정
            int width = Mathf.RoundToInt(Screen.height * targetAspectRatio);
            Screen.SetResolution(width, Screen.height, false);
        }
        else if (currentAspectRatio < targetAspectRatio)
        {
            // 현재 비율이 목표 비율보다 작다면 높이를 조정
            int height = Mathf.RoundToInt(Screen.width / targetAspectRatio);
            Screen.SetResolution(Screen.width, height, false);
        }
    }
}
