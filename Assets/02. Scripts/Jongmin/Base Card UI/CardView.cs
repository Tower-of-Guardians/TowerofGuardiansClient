using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour, ICardView
{
    [Header("UI 관련 컴포넌트")]
    [Header("카드 이미지")]
    [SerializeField] private Image m_card_image;

    [Header("카드 아이템 이미지")]
    [SerializeField] private Image m_card_item_image;

    [Header("카드 이름")]
    [SerializeField] private TMP_Text m_card_name_label;

    [Header("카드 설명")]
    [SerializeField] private TMP_Text m_card_description_label;

    [Header("카드 공격력")]
    [SerializeField] protected TMP_Text m_card_atk_label;

    [Header("카드 방어력")]
    [SerializeField] protected TMP_Text m_card_def_label;

    [Header("카드 성급 그룹")]
    [SerializeField] private GameObject m_star_group;

    private Image[] m_star_objects;

    protected virtual void Awake()
        => m_star_objects = m_star_group.GetComponentsInChildren<Image>();

    public virtual void InitUI(CardData card_data)
    {
        m_card_image.sprite = card_data.cardimage;
        m_card_item_image.sprite = card_data.iconimage;
        m_card_name_label.text = card_data.itemName;
        m_card_description_label.text = card_data.effectDescription;
        m_card_atk_label.text = card_data.ATK.ToString();
        m_card_def_label.text = card_data.DEF.ToString();

        for(int i = 0; i < m_star_objects.Length; i++)
            m_star_objects[i].gameObject.SetActive(false);

        for(int i = 0; i < card_data.star; i++)
            m_star_objects[i].gameObject.SetActive(true);
    }

    public virtual void Return()
        => ObjectPoolManager.Instance.Return(gameObject);
}
