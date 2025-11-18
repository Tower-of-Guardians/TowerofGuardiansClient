using UnityEngine;

public class TemporaryCardFactory : MonoBehaviour
{
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Space(30f), Header("의존성 목록")]
    [Header("임시 카드 프리펩")]
    [SerializeField] private GameObject m_temp_card_prefab;

    public GameObject InstantiateCard(CardData card_data)
    {
        var m_temp_card_obj = ObjectPoolManager.Instance.Get(m_temp_card_prefab);
        m_temp_card_obj.transform.SetParent(m_canvas.transform);

        // TODO: 카드 데이터 주입

        return m_temp_card_obj;
    }
}