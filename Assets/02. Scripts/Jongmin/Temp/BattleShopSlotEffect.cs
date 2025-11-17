using UnityEngine;

public class BattleShopSlotEffect : MonoBehaviour
{
    [Header("에디터 테스트 컴포넌트")]
    [Header("착지 이펙트")]
    [SerializeField] private GameObject m_landing_object;

    public void CallbackToInstantiateEffect()
    {
        var effect = Instantiate(m_landing_object, transform);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.position += Vector3.down;
    }
}
