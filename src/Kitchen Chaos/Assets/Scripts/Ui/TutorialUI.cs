using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private RectTransform arrowIndicator;
    [SerializeField] private Camera gameCamera;

    private Transform targetTransform;
    private void Awake()
    {
        TutorialManager.Instance.OnStepChanged += OnStepChanged;
        TutorialManager.Instance.OnTutorialComplete += OnTutorialComplete;
    }

    private void Start()
    {
        instructionText.text = TutorialManager.Instance.GetCurrentInstruction();
        targetTransform = TutorialManager.Instance.GetCurrentTargetTransform();
    }

    private void OnDestroy()
    {
        if (TutorialManager.Instance == null) 
        return;
        TutorialManager.Instance.OnStepChanged -= OnStepChanged;
        TutorialManager.Instance.OnTutorialComplete -= OnTutorialComplete;
    }

    private void LateUpdate()
    {
        if (targetTransform == null || arrowIndicator == null) 
        return;

        Vector3 screenPos = gameCamera.WorldToScreenPoint(targetTransform.position + Vector3.up * 2f);
        arrowIndicator.gameObject.SetActive(screenPos.z > 0f);
        arrowIndicator.position = screenPos;
    }
    private void OnStepChanged(object sender, TutorialManager.OnStepChangedEventArgs e)
    {
        instructionText.text = e.instruction;
        targetTransform  = e.targetTransform;
    }

    private void OnTutorialComplete(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
}