using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

[RequireComponent(typeof(DialogueRunner))]
public class YarnDialogueUI : MonoBehaviour, IDialogueUI
{
    [Header("UI 관련 컴포넌트")]
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup m_canvas_group;

    [Header("플레이어 초상화")]
    [SerializeField] private Image m_player_portrait;

    [Header("NPC 초상화")]
    [SerializeField] private Image m_npc_portrait;

    [Space(30f), Header("의존성 목록")]
    [SerializeField] private PortraitDataBase m_portrait_db;

    private DialogueRunner m_dialogue_runner;

    public DialogueRunner Runner => m_dialogue_runner;

    private void Awake()
        => m_dialogue_runner = GetComponent<DialogueRunner>();

    public void InitPortrait(CharacterCode player_code, CharacterCode npc_code)
    {
        m_player_portrait.sprite = m_portrait_db.GetPortrait(player_code);
        m_npc_portrait.sprite = m_portrait_db.GetPortrait(npc_code);
    }

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
