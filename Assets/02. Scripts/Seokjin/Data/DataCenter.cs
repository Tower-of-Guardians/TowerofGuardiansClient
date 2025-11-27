using System; // async/await 사용을 위해 필요
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataCenter : Singleton<DataCenter>
{
    public List<CardData> cardDatas = new List<CardData>();
    public List<string> userDeck = new List<string>();

    private static ItemData loadedData;
    private static AsyncOperationHandle<ItemData> loadHandle; // 메모리 관리를 위한 핸들

    public static async void GetItemData(string id, Action<ItemData> complete)
    {
        loadHandle = Addressables.LoadAssetAsync<ItemData>(id);

        // 로드가 완료될 때까지 대기 (Unity 2021+에서는 'await' 사용을 권장합니다.)
        await loadHandle.Task;

        if (loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedData = loadHandle.Result;
            UnityEngine.Debug.Log("ItemDataList 로드 성공!");

            // 2. 로드된 데이터 사용
            //UseLoadedData(loadedData.allItems);
        }
        else
        {
            UnityEngine.Debug.LogError(id + "를 찾을수 없습니다.");
        }
        complete?.Invoke(loadedData);
        DataRelease();
    }

    public static void DataRelease()
    {
        if (loadHandle.IsValid())
        {
            // 로드 핸들을 Addressables 시스템에 반환하여 메모리를 해제합니다.
            Addressables.Release(loadHandle);
            loadedData = null;
            UnityEngine.Debug.Log("Addressables 핸들 해제 완료.");
        }
    }
}
