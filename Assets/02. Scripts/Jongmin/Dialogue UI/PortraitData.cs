using UnityEngine;

[System.Serializable]
public class PortraitData
{
    [Header("캐릭터 코드")]
    [SerializeField] private CharacterCode m_code;
    public CharacterCode Code => m_code;

    [Header("초상화 이미지")]
    [SerializeField] private Sprite m_sprite;
    public Sprite Sprite => m_sprite;
}
