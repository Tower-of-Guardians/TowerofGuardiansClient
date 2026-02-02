using System;
using System.Collections;
using UnityEngine;

public class TemporaryCardController : MonoBehaviour
{
    [SerializeField] private TemporaryCardAnimator m_animator;

    public event Action<BattleCardData> OnCardAnimationBegin; 
    public event Action<BattleCardData> OnCardAnimationEnd;
    public event Action OnFinalAnimationEnd;

    private Coroutine m_running;

    public void Play(TemporaryCardAnimeRequest req)
    {
        if (m_running != null) StopCoroutine(m_running);
        m_running = StartCoroutine(Co_Play(req));
    }

    private IEnumerator Co_Play(TemporaryCardAnimeRequest req)
    {
        for (int i = 0; i < req.CardDatas.Length; i++)
        {
            if (req.Interval > 0f)
                yield return new WaitForSeconds(req.Interval);

            var s = req.GetSettings(i);

            m_animator.AnimateOne(
                req.CardDatas[i],
                req.GetStartPosition(i),
                req.EndPosition,
                s,
                d => OnCardAnimationBegin?.Invoke(d),
                d => OnCardAnimationEnd?.Invoke(d)
            );
        }

        OnFinalAnimationEnd?.Invoke();

        m_running = null;
    }
}
