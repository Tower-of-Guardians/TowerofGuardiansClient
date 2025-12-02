using System;
using System.Collections;
using UnityEngine;

public class TemporaryCardController : MonoBehaviour
{
    [Header("임시 카드 애니메이터")]
    [SerializeField] private TemporaryCardAnimator m_animator;

    public event Action<BattleCardData> OnAnimationEnd;

    private void Awake()
        => m_animator.OnAnimationEnd += AnimationEndHandler;

    public void PlayAnime(BattleCardData[] card_datas,
                          Vector3 start_position,
                          Vector3 end_position,
                          float scale,
                          float arc_power,
                          float duration,
                          float interval)
    {
        StartCoroutine(Co_Anime(card_datas,
                                start_position,
                                end_position,
                                scale,
                                arc_power,
                                duration,
                                interval));
    }

    public void PlayAnime(BattleCardData card_data,
                          Vector3 start_position,
                          Vector3 end_position,
                          float scale,
                          float arc_power,
                          float duration)
    {
        m_animator.Animate(card_data,
                           start_position,
                           end_position,
                           scale,
                           arc_power,
                           duration);  
    }

    public void PlayAnimeFromThis(BattleCardData[] card_datas,
                                  Vector3[] start_positions,
                                  Vector3 end_position,
                                  float scale,
                                  float arc_power,
                                  float duration,
                                  float interval)
    {
        StartCoroutine(Co_AnimeFromThis(card_datas,
                                        start_positions,
                                        end_position,
                                        scale,
                                        arc_power,
                                        duration,
                                        interval));
    }

    private IEnumerator Co_Anime(BattleCardData[] card_datas,
                                 Vector3 start_position,
                                 Vector3 end_position,
                                 float scale,
                                 float arc_power,
                                 float duration,
                                 float interval)
    {
        for(int i = 0; i < card_datas.Length; i++)
        {
            yield return new WaitForSeconds(interval);

            m_animator.Animate(card_datas[i],
                               start_position,
                               end_position,
                               scale,
                               arc_power,
                               duration);      
        }  
    }

    private IEnumerator Co_AnimeFromThis(BattleCardData[] card_datas,
                                         Vector3[] start_positions,
                                         Vector3 end_position,
                                         float scale,
                                         float arc_power,
                                         float duration,
                                         float interval)
    {
        for(int i = 0; i < card_datas.Length; i++)
        {
            yield return new WaitForSeconds(interval);

            m_animator.Animate(card_datas[i],
                               start_positions[i],
                               end_position,
                               scale,
                               arc_power,
                               duration);      
        }          
    }

    private void AnimationEndHandler(BattleCardData card_data)
    {
        OnAnimationEnd?.Invoke(card_data);
    }
}
