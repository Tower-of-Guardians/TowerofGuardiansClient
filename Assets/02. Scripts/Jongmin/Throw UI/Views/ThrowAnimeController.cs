using System.Collections.Generic;
using UnityEngine;

public class ThrowAnimeController : MonoBehaviour
{
    [Header("의존성 목록")]
    [Header("임시 카드 컨트롤러")]
    [SerializeField] private TemporaryCardController m_temp_card_controller;

    [Header("손 위치")]
    [SerializeField] private Transform m_hand_transform;

    [Header("교체 패 위치")]
    [SerializeField] private Transform m_throw_deck_transform;

    public void PlayThrowAll(Transform root, CardData[] card_datas)
    {
        var throw_cards = root.GetComponentsInChildren<IThrowCardView>();
        var throw_card_positions = new List<Vector3>();
        foreach(var throw_card in throw_cards)
            throw_card_positions.Add((throw_card as ThrowCardView).transform.position);

        m_temp_card_controller.PlayAnimeFromThis(card_datas,
                                                 throw_card_positions.ToArray(),
                                                 m_throw_deck_transform.position,
                                                 0.3f,
                                                 0f,
                                                 0.75f,
                                                 0.1f);        
    }

    public void PlayRemove(IThrowCardView card_view, CardData card_data)
    {
        var target_card = card_view as ThrowCardView;
        
        m_temp_card_controller.PlayAnime(card_data,
                                         target_card.transform.position,
                                         m_hand_transform.position,
                                         1f,
                                         0f,
                                         0f);  

        ObjectPoolManager.Instance.Return(target_card.gameObject);     
    }

    public void PlayRemoveAll(Transform root, CardData[] card_datas)
    {
        var throw_cards = root.GetComponentsInChildren<IThrowCardView>();
        var throw_card_positions = new List<Vector3>();
        foreach(var throw_card in throw_cards)
            throw_card_positions.Add((throw_card as ThrowCardView).transform.position);

        m_temp_card_controller.PlayAnimeFromThis(card_datas,
                                                 throw_card_positions.ToArray(),
                                                 m_hand_transform.position,
                                                 0.75f,
                                                 100f,
                                                 0.5f,
                                                 0.1f);        
    }
}
