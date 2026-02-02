using UnityEngine;

[RequireComponent(typeof(TemporaryCardController))]
public abstract class CardEffector : MonoBehaviour
{
    [Header("시작점")]
    [SerializeField] protected Transform m_start_transform;

    [Header("도착점")]
    [SerializeField] protected Transform m_end_transform;

    protected TemporaryCardController m_temp_card_controller;
    protected TemporaryCardSettings m_temp_card_settings;
    protected TemporaryCardAnimeRequest m_temp_card_anime_request;

    private void Awake()
    {
        m_temp_card_controller = GetComponent<TemporaryCardController>();

        m_temp_card_settings = new();
        m_temp_card_anime_request = new();

        m_temp_card_controller.OnCardAnimationBegin += OnTempCardAnimeStart;
        m_temp_card_controller.OnCardAnimationEnd += OnTempCardAnimeEnd;
        m_temp_card_controller.OnFinalAnimationEnd += OnFinalAnimeEnd;
    }

    private void OnDestroy()
    {
        if(m_temp_card_controller != null)
        {
            m_temp_card_controller.OnCardAnimationBegin -= OnTempCardAnimeStart;
            m_temp_card_controller.OnCardAnimationEnd -= OnTempCardAnimeEnd;
            m_temp_card_controller.OnFinalAnimationEnd -= OnFinalAnimeEnd;
        }
    }

    public virtual void Execute()
    {
        m_temp_card_controller.Play(m_temp_card_anime_request);
    }

    protected virtual void OnTempCardAnimeStart(BattleCardData card_data) {}
    protected virtual void OnTempCardAnimeEnd(BattleCardData card_data) {}
    protected virtual void OnFinalAnimeEnd() {}
}
