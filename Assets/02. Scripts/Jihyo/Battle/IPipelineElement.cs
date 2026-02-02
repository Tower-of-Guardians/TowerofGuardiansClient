using System;

public interface IPipelineElement
{
    /// <summary>
    /// 파이프라인 요소를 실행합니다.
    /// </summary>
    /// <param name="context">파이프라인 요소들이 사용할 의존성들이 담긴 컨텍스트</param>
    /// <param name="onComplete">요소 실행이 완료되면 호출할 콜백</param>
    void execute(ElementContext context, Action onComplete);
}
