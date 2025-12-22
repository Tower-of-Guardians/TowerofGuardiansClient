using Mono.Cecil;
using System; // async/await 사용을 위해 필요
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataCenter : Singleton<DataCenter>
{
    ////// 플레이어 관련 /////
    public PlayerState playerstate = new PlayerState();
    //////////////////////////
    ////// 카드 관련 //////
    public static Dictionary<string, CardData> card_datas = new Dictionary<string, CardData>(); // 카드데이터
    private static AsyncOperationHandle<IList<CardData>> carddata_loadHandle; // 메모리 관리를 위한 핸들
    public List<CardData> userDeck = new List<CardData>();// 유저 소지 카드 데이터
    public static List<string> random_card_datas = new List<string>(); // 랜덤하게 카드 뽑기위한 데이터
    SortType sortType;
    bool sortType_oder = true; // true = 오름차순, false = 내림차순
    //////////////////////

    ////// 리절트 관련 //////
    public static Dictionary<int, ResultPercentData> result_datas = new Dictionary<int, ResultPercentData>(); // 카드데이터
    private static AsyncOperationHandle<IList<ResultPercentData>> resultdata_loadHandle; // 메모리 관리를 위한 핸들
    //////////////////////

    public static bool IsCardDataLoaded { get; private set; } = false;
    public static bool IsResultDataLoaded { get; private set; } = false;
    private async void Start()
    {
        await AllCardData();
        await AllResultPercentData();
        LoadPlayerData();
        StartCoroutine(SetStartDeck());
    }
    public void LoadPlayerData()
    {
        // TODO 석진
        // SaveLoad 만들어서 로드 데이터 세팅해주는 부분 추가

        playerstate.level = 1;
        playerstate.experience = 0;
        playerstate.hp = 70;
        playerstate.lhp = 5;
        playerstate.atk = 4;
        playerstate.latk = 0;
        playerstate.maxmagic = 2;
    }
    IEnumerator SetStartDeck()
    {
        yield return new WaitUntil(() => IsCardDataLoaded == true);
        string[] startdecks = { "11000001", "11000002", "11010003", "11010004", "11010005", "11010006", "11010007", "11010008", "11010009", "11010025" };
        foreach (string id in startdecks)
        {
            GetCardData(id, (data) => userDeck.Add(Instantiate(data)));
        }
    }
    public async Task AllCardData()
    {
        // Addressables.LoadAssetsAsync<TObject>(key, callback)
        // key는 주소 또는 레이블을 사용할 수 있습니다. 여기서는 레이블을 사용합니다.
        carddata_loadHandle = Addressables.LoadAssetsAsync<CardData>(
            "CardData",
            // 로드된 각 Asset에 대한 콜백 (선택 사항)
            (item) =>
            {
                if (item != null)
                {
                    card_datas[item.id] = item;
                    random_card_datas.Add(item.id);
                }
            }
        );

        // 비동기 작업이 완료될 때까지 대기
        await carddata_loadHandle.Task;

        if (carddata_loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IsCardDataLoaded = true;
            UnityEngine.Debug.Log($"ItemData 로드 완료: {card_datas.Count}");
        }
        else
        {
            UnityEngine.Debug.LogError($"ItemData 로드 실패: {carddata_loadHandle.OperationException}");
        }
    }
    public async Task AllResultPercentData()
    {
        // Addressables.LoadAssetsAsync<TObject>(key, callback)
        // key는 주소 또는 레이블을 사용할 수 있습니다. 여기서는 레이블을 사용합니다.
        resultdata_loadHandle = Addressables.LoadAssetsAsync<ResultPercentData>(
            "ResultPercentData",
            // 로드된 각 Asset에 대한 콜백 (선택 사항)
            (item) =>
            {
                if (item != null)
                {
                    result_datas[item.level] = item;
                }
            }
        );

        // 비동기 작업이 완료될 때까지 대기
        await resultdata_loadHandle.Task;

        if (resultdata_loadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            IsResultDataLoaded = true;
            UnityEngine.Debug.Log($"ResultPercentData 로드 완료: {result_datas.Count}");
        }
        else
        {
            UnityEngine.Debug.LogError($"ResultPercentData 로드 실패: {resultdata_loadHandle.OperationException}");
        }
    }


    /// <summary>
    /// 아이템 id 를 사용한 데이터 받기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    public void GetCardData(string id, Action<CardData> data)
    {
        if (IsCardDataLoaded && card_datas.TryGetValue(id, out CardData itemData))
        {
            data?.Invoke(itemData);
        }
        else
        {
            UnityEngine.Debug.Log($"ID {id}에 해당하는 아이템 데이터가 로드되지 않았습니다. IsCardDataLoaded = {IsCardDataLoaded}.");
            data?.Invoke(null);
        }
    }


    /// <summary>
    /// 레벨당 리절트 확률 데이터 받기
    /// </summary>
    /// <param name="level"></param>
    /// <param name="data"></param>
    public void GetResultPercentData(int level, Action<ResultPercentData> data)
    {
        if (IsResultDataLoaded && result_datas.TryGetValue(level, out ResultPercentData itemData))
        {
            data?.Invoke(itemData);
        }
        else
        {
            UnityEngine.Debug.Log($"ID {level}에 해당하는 아이템 데이터가 로드되지 않았습니다.");
            data?.Invoke(null);
        }
    }

    /// <summary>
    /// 정렬 타입에 따른 userDeck 리스트 정렬
    /// </summary>
    /// <param name="type"></param>
    public void SortUserCards(SortType type)
    {
        List<CardData> sortedDescending = new List<CardData>();
        if (type != sortType)
        {
            sortType_oder = true;
            if (type == SortType.Grade)
            {
                sortType_oder = false;
            }
        }

        switch (type)
        {
            case SortType.Grade:
                if (!sortType_oder)
                {
                    sortedDescending = userDeck.OrderByDescending(data => data.star).ToList();
                    sortedDescending = userDeck.OrderByDescending(data => data.grade).ToList();
                }
                else
                {
                    sortedDescending = userDeck.OrderBy(data => data.star).ToList();
                    sortedDescending = userDeck.OrderBy(data => data.grade).ToList();
                }
                break;
            case SortType.Time:
                if (sortType_oder)
                {
                    sortedDescending = userDeck.OrderBy(data => data.time).ToList();
                }
                else
                {
                    sortedDescending = userDeck.OrderByDescending(data => data.time).ToList();
                }
                break;
            case SortType.Attack:
                if (sortType_oder)
                {
                    sortedDescending = userDeck.OrderBy(data => data.ATK).ToList();
                }
                else
                {
                    sortedDescending = userDeck.OrderByDescending(data => data.ATK).ToList();
                }
                break;
            case SortType.Defense:
                if (sortType_oder)
                {
                    sortedDescending = userDeck.OrderBy(data => data.DEF).ToList();
                }
                else
                {
                    sortedDescending = userDeck.OrderByDescending(data => data.DEF).ToList();
                }
                break;
        }

        sortType = type;
        sortType_oder = !sortType_oder;
        userDeck = sortedDescending;
        foreach (CardData data in userDeck)
        {
            UnityEngine.Debug.Log(data.itemName);
        }
    }

    private void OnDisable()
    {
        Addressables.Release(carddata_loadHandle);
        Addressables.Release(resultdata_loadHandle);
        UnityEngine.Debug.Log("Addressables 핸들 해제 완료.");
    }
}
