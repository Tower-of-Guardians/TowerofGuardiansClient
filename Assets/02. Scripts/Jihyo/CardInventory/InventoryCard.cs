using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryCard : MonoBehaviour
{
    [Header("UI 관련 컴포넌트")]
    [Header("카드 테두리 이미지")]
    [SerializeField] private Image m_outline_image;

    [Header("카드 이미지")]
    [SerializeField] private Image m_card_image;

    [Header("카드 이름 텍스트")]
    [SerializeField] private TMP_Text m_name_label;

    [Header("카드 설명 텍스트")]
    [SerializeField] private TMP_Text m_description_label;

    [Header("카드 공격력 텍스트")]
    [SerializeField] private TMP_Text m_atk_label;

    [Header("카드 방어력 텍스트")]
    [SerializeField] private TMP_Text m_def_label;

    public void InitUI(CardData card_data)
    {
        // TODO: 카드 테두리 이미지 설정
        // TODO: 카드 초상화 이미지 설정

        if (card_data == null)
            return;

        m_card_image.sprite = card_data.iconimage;
        m_name_label.text = card_data.itemName;
        m_description_label.text = card_data.effectDescription;

        m_atk_label.text = card_data.ATK.ToString();
        m_def_label.text = card_data.DEF.ToString();
    }


    // TODO: 카드 테두리 이미지 설정 (등급에 따라)
    // TODO: 카드 등급, 강화 상태 등 추가 UI 요소 설정
}