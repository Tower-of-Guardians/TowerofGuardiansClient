using UnityEngine;

[RequireComponent(typeof(TemporaryCardController))]
public abstract class CardEffector : MonoBehaviour
{
    [Header("시작점")]
    [SerializeField] Transform m_start_transform;

    [Header("도착점")]
    [SerializeField] Transform m_end_transform;

    private TemporaryCardController m_temp_card_controller;

    private void Awake()
    {
        m_temp_card_controller = GetComponent<TemporaryCardController>();
        m_temp_card_controller.OnAnimationEnd += OnTempCardAnimeEnd;
    }

    private void OnDestroy()
    {
        if(m_temp_card_controller != null)
            m_temp_card_controller.OnAnimationEnd -= OnTempCardAnimeEnd;
    }

    public void CreateCardStartToEnd(BattleCardData[] card_data_list, float scale, float arc_power, float duration, float interval)
    {
        m_temp_card_controller.PlayAnime(card_data_list, m_start_transform.position, m_end_transform.position, scale, arc_power, duration, interval);
    }

    public void CreateCardStartToEndWithRandomArc(BattleCardData[] card_data_list, float scale, float duration, float interval)
    {
        m_temp_card_controller.PlayAnimeWithRandomArc(card_data_list, m_start_transform.position, m_end_transform.position, scale, duration, interval);
    }

    public void CreateCardEachToEnd(BattleCardData[] card_data_list, Vector3[] card_position_list, float scale, float arc_power, float duration, float interval)
    {
        m_temp_card_controller.PlayAnimeFromThis(card_data_list, card_position_list, m_end_transform.position, scale, arc_power, duration, interval);
    }

    protected abstract void OnTempCardAnimeEnd(BattleCardData card_data);
}
