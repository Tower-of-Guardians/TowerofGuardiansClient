using TMPro;
using UnityEngine;

public class ThrowManualView : MonoBehaviour, IActionManualView
{
    [Header("UI 관련 컴포넌트")]
    [Header("동작 횟수 텍스트")]
    [SerializeField] private TMP_Text m_action_label;

    private ActionManualPresenter m_presenter;

    private void OnDestroy()
        => m_presenter?.Dispose();

    public void Inject(ActionManualPresenter presenter)
    {
        m_presenter = presenter;
    }

    public void UpdateUI(ActionData action_data, bool can_action)
    {
        var action_text = $"{action_data.Current} / {action_data.Max}";

        m_action_label.text = can_action ? action_text
                                         : $"<color=red>{action_text}</color>";
    }
}
