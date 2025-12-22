using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

[RequireComponent(typeof(DialogueRunner))]
public class YarnDialogueUI : MonoBehaviour, IDialogueUI
{
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    private DialogueRunner m_dialogue_runner;

    public DialogueRunner Runner => m_dialogue_runner;

    private void Awake()
        => m_dialogue_runner = GetComponent<DialogueRunner>();

    public void StartDialogue(string dialogue_code)
    {
        ToggleInteraction(true);
        m_dialogue_runner.StartDialogue(dialogue_code);
    }

    public void StopDialogue()
    {
        ToggleInteraction(false);
        m_dialogue_runner.Stop();
    }

    private void ToggleInteraction(bool active)
    {
        m_canvas_group.blocksRaycasts = active;
        m_canvas_group.interactable = active;
    }
}
