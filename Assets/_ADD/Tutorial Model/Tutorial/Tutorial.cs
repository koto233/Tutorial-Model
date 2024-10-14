using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    // 单例实例
    public static Tutorial Instance { get; private set; }
    [SerializeField] RectTransform pointerParent;  // 手指提示

    [SerializeField] RectTransform tipParent;  // 提示文本
    [SerializeField] RectTransform tipParentDown;  // 靠下的提示文本
    [SerializeField] Text tipText; // 提示文本
    [SerializeField] Text tipTextDown; // 靠下的提示文本
    [SerializeField] RectTransform nextButton; // 下一步按钮
    [SerializeField] List<TutorialStep> tutorialSteps = new List<TutorialStep>(); // 教程步骤
    Button nextButtonComponent; // 下一步按钮组件
    Animator pointerAnimator; // 手指提示动画
    int step = -1; // 当前步骤
    private void Awake()
    {
        // 如果单例实例已经存在，并且不是当前对象，则销毁当前对象
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // 保持单例实例，确保不会被销毁
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        pointerAnimator = pointerParent.GetComponent<Animator>();
        nextButtonComponent = nextButton.GetComponent<Button>();
    }

    private void Start()
    {
        nextButtonComponent.onClick.AddListener(() => TutorialNextStep());

    }
    public void StartTutorial()
    {
        Invoke("ShowTutorial", 2f);
    }

    void ShowTutorial()
    {
        step = 0;
        pointerParent.gameObject.SetActive(true);
        tipParent.gameObject.SetActive(true);
        // tipText.text = tutorialSteps[step].tipString;
        tipText.text = GetNstring(tutorialSteps[step].tipString, tutorialSteps[step].textWidth);
        Vector3 targetPos = GetUIPointFromWorld(transform.GetComponent<RectTransform>(), tutorialSteps[step].position);
        tipParent.localPosition = targetPos;
        pointerParent.localPosition = targetPos;
        GuideMask.Instance.CreateCircleMaskoffset(pointerParent.gameObject, tutorialSteps[step].circleRad, null);
    }

    public void TutorialNextStep()
    {
        step += 1;
        if (step <= 5)
        {

            tipText.text = GetNstring(tutorialSteps[step].tipString, tutorialSteps[step].textWidth);
            Vector3 targetPos = tutorialSteps[step].position;
            if (!tutorialSteps[step].isUi)
            {
                targetPos = GetUIPointFromWorld(transform.GetComponent<RectTransform>(), tutorialSteps[step].position);

            }
            pointerParent.gameObject.SetActive(!tutorialSteps[step].isUi);
            nextButton.gameObject.SetActive(tutorialSteps[step].isUi);
            nextButton.localPosition = targetPos + Vector3.right * 150;
            if (step == 5)
            {
                tipParent.gameObject.SetActive(false);
                tipParentDown.gameObject.SetActive(true);
                tipTextDown.text = GetNstring(tutorialSteps[step].tipString, tutorialSteps[step].textWidth);
                tipParentDown.localPosition = targetPos;
            }
            tipParent.localPosition = targetPos;
            pointerParent.localPosition = targetPos;
            GuideMask.Instance.CreateCircleMaskoffset(pointerParent.gameObject, tutorialSteps[step].circleRad, null);
            pointerAnimator.SetBool("Left", tutorialSteps[step].isLeft);
        }
        else
        {
            EndTutorial();
        }

    }




    public Vector3 GetUIPointFromWorld(RectTransform uiElement, Vector3 worldPoint)
    {
        return GetUIPointFromScreen(uiElement, Camera.main.WorldToScreenPoint(worldPoint));
    }

    public Vector3 GetUIPointFromScreen(RectTransform uiElement, Vector3 screenPoint)
    {
        // 将屏幕坐标转换为UI元素的本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(uiElement, screenPoint, null, out Vector2 localPoint))
        {
            return localPoint;
        }
        return Vector3.zero;
    }
    public void EndTutorial()
    {
        GuideMask.Instance.CloseGuideMask();
        gameObject.SetActive(false);

    }
    public string GetNstring(string str, int maxWithth)
    {
        string result = "";
        int point = 0;
        for (int i = 0; i < str.Length; i++)
        {
            if (point == maxWithth)
            {
                result += '\n';
                point = 0;
            }
            result += str[i];
            point++;
        }
        return result;
    }

}

[Serializable]
// 定义一个教程步骤类
public class TutorialStep
{
    // 教程步骤的位置
    [Header("提示位置")]
    public Vector3 position;
    // 教程步骤的提示字符串
    [Header("提示文本")]
    public string tipString;
    [Header("是否是UI")]
    // 点击是否在UI上
    public bool isUi;
    [Header("提示圆半径")]
    // 圆的半径
    public int circleRad;
    [Header("是否向左滑动")]
    // 是否朝左
    public bool isLeft;
    [Header("提示文本最大宽度")]
    public int textWidth; // 文本宽度
}