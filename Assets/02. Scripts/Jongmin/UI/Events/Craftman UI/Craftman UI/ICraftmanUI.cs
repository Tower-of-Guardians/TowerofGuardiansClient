using System;

public interface ICraftmanUI : IOpenableUI
{
    /// <summary>
    /// 테스트 버튼 연결을 위한 임시 인터페이스
    /// </summary>
    /// <param name="presenter"></param>
    [Obsolete]
    void Construct(CraftmanPresenter presenter);
}