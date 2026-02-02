using System;
using System.Collections.Generic;

/// <summary>
/// 턴 진행을 위한 파이프라인을 관리하는 클래스
/// 파이프라인 요소들을 순차적으로 실행합니다.
/// </summary>
public class TurnPipeline
{
    private readonly List<IPipelineElement> m_pipe_elements = new List<IPipelineElement>();

    /// <summary>
    /// 파이프라인에 요소를 등록합니다.
    /// </summary>
    /// <param name="element">등록할 파이프라인 요소</param>
    public void Register(IPipelineElement element)
    {
        if (element == null) return;
        if (m_pipe_elements.Contains(element)) return;
        m_pipe_elements.Add(element);
    }

    /// <summary>
    /// 파이프라인에서 요소를 삭제합니다.
    /// </summary>
    /// <param name="element">삭제할 파이프라인 요소</param>
    public void Remove(IPipelineElement element)
    {
        if (element == null) return;
        m_pipe_elements.Remove(element);
    }

    /// <summary>
    /// 파이프라인의 모든 요소를 제거합니다.
    /// </summary>
    public void Clear()
    {
        m_pipe_elements.Clear();
    }

    /// <summary>
    /// 파이프라인의 모든 요소를 순차적으로 실행합니다.
    /// </summary>
    /// <param name="context">파이프라인 요소들이 사용할 의존성들이 담긴 컨텍스트</param>
    /// <param name="onPipelineComplete">파이프라인 전체 실행이 완료되면 호출할 콜백</param>
    public void Execute(ElementContext context, Action onPipelineComplete = null)
    {
        if (context == null)
        {
            onPipelineComplete?.Invoke();
            return;
        }

        if (m_pipe_elements.Count == 0)
        {
            onPipelineComplete?.Invoke();
            return;
        }

        ExecuteNext(0, context, onPipelineComplete);
    }

    /// <summary>
    /// 다음 파이프라인 요소를 실행합니다.
    /// </summary>
    /// <param name="index">실행할 요소의 인덱스</param>
    /// <param name="context">파이프라인 요소들이 사용할 의존성들이 담긴 컨텍스트</param>
    /// <param name="onComplete">파이프라인 전체 실행이 완료되면 호출할 콜백</param>
    private void ExecuteNext(int index, ElementContext context, Action onComplete)
    {
        // 모든 요소 실행 완료
        if (index >= m_pipe_elements.Count)
        {
            onComplete?.Invoke();
            return;
        }

        // 현재 요소 실행
        IPipelineElement currentElement = m_pipe_elements[index];
        if (currentElement == null)
        {
            ExecuteNext(index + 1, context, onComplete);
            return;
        }

        // 요소 실행 후 완료 콜백에서 다음 요소로 진행
        currentElement.execute(context, () =>
        {
            ExecuteNext(index + 1, context, onComplete);
        });
    }
}
