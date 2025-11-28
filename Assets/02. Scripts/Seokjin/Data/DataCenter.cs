using System; // async/await 사용을 위해 필요
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataCenter : Singleton<DataCenter>
{
    public static Dictionary<string,CardData> card_datas = new Dictionary<string, CardData>();
    public List<string> userDeck = new List<string>();

    private static AsyncOperationHandle<IList<CardData>> loadHandle; // 메모리 관리를 위한 핸들
    private const string ITEM_DATA_LABEL = "CardData";

    public static bool IsDataLoaded { get; private set; } = false;
    private async void Start()
    {
        await AllCardData();
    }
    public async Task AllCardData()
    {
        // Addressables.LoadAssetsAsync<TObject>(key, callback)
        // key는 주소 또는 레이블을 사용할 수 있습니다. 여기서는 레이블을 사용합니다.
        loadHandle = Addressables.LoadAssetsAsync<CardData>(
            ITEM_DATA_LABEL,
            // 로드된 각 Asset에 대한 콜백 (선택 사항)
            (item) =>
            {
                if (item != null)
                {
                    card_datas[item.id] = item;
                }
            }
        );

        // 비동기 작업이 완료될 때까지 대기
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IsDataLoaded = true;
            UnityEngine.Debug.Log($"ItemData 로드 완료: {card_datas.Count}");
        }
        else
        {
            UnityEngine.Debug.LogError($"ItemData 로드 실패: {loadHandle.OperationException}");
        }
    }
    // 로드된 데이터를 딜레이 없이 바로 사용하는 함수
    public void GetCardData(string id, Action<CardData> data)
    {
        if (IsDataLoaded && card_datas.TryGetValue(id, out CardData itemData))
        {
            data?.Invoke(itemData);
        }
        else
        {
            UnityEngine.Debug.Log($"ID {id}에 해당하는 아이템 데이터가 로드되지 않았습니다.");
            data?.Invoke(null);
        }
    }

    private void OnDisable()
    {
        //Addressables.Release(loadHandle);
        UnityEngine.Debug.Log("Addressables 핸들 해제 완료.");
    }
}
