using UnityEngine;

public class Notice : MonoBehaviour, INotice
{
    [Header("의존성 목록")]
    [Header("팝업 알림 프리펩")]
    [SerializeField] private GameObject m_notice_prefab;

    public void Notify(string notify_string)
    {
        var popup_notice_obj = ObjectPoolManager.Instance.Get(m_notice_prefab);

        var popup_notice_ui = popup_notice_obj.GetComponent<IPopupNoticeView>();
        popup_notice_ui.OpenUI(notify_string);
    }
}
