using UnityEngine;

public class TemporaryCardFactory : MonoBehaviour
{
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Space(30f), Header("의존성 목록")]
    [Header("임시 카드 프리펩")]
    [SerializeField] private GameObject m_temp_card_prefab;

    public GameObject InstantiateCard(BattleCardData card_data)
    {
        var m_temp_card_obj = ObjectPoolManager.Instance.Get(m_temp_card_prefab);
        m_temp_card_obj.transform.SetParent(m_canvas.transform);

        var temp_card_view = m_temp_card_obj.GetComponent<TemporaryCardView>();
        var temp_card_presenter = new TemporaryCardPresenter(temp_card_view,
                                                             card_data);

        return m_temp_card_obj;
    }
}