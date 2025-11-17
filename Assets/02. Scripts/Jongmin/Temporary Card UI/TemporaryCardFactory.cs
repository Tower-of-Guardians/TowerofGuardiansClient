using UnityEngine;

public class TemporaryCardFactory : MonoBehaviour
{
    [Header("캔버스")]
    [SerializeField] private Canvas m_canvas;

    [Space(30f), Header("에디터 테스트 옵션")]
    [Header("임시 카드 프리펩")]
    [SerializeField] private GameObject m_temp_card_prefab;

    public GameObject InstantiateCard(CardData card_data)
    {
        // TODO: Object Pool을 통한 생성
        return Instantiate(m_temp_card_prefab, m_canvas.transform);
        // TODO: 카드 데이터 주입
    }
}