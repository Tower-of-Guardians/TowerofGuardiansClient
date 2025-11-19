using UnityEngine;

[CreateAssetMenu(fileName = "Throw UI Designer", menuName = "SO/Design/Throw UI Designer")]
public class ThrowUIDesigner : ScriptableObject
{
    [Header("기획 옵션")]
    [Header("카드 간 간격")]
    [SerializeField] private float m_space;
    public float Space => m_space;

    [Header("애니메이션 시간")]
    [SerializeField] private float m_anime_duration;
    public float AnimeDuration => m_anime_duration;
}
