using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// region 키워드는 endregion과 같이 쓰며 특정 부분의 코드를 접어서 관리할 수 있습니다.
// 시스템상 변화되는건 없으며 코드를 조금 더 깔끔하게 볼 수 있게 하여줍니다.
#region GAMEDATA



// 타일 맵 데이터
// 값 변수가 아닌 참조 변수를 사용할 경우
// JSON에는 InstanceID가 저장 되게 됩니다.

//이는 Unity 엔진의 런타임 마다 달라지게 됨으로
// Index(int)나 혹은 Key("string")를 넣어주고
// 이를 각각 매니저들 안에 있는 배열 혹은 딕셔너리에서 찾아가게 합시다
[System.Serializable]
public class TilemapData
{
    public string name;
    public List<Vector3Int> positions = new List<Vector3Int>();
    public List<string> tiles = new List<string>();
}


// 기믹 블럭 데이터
[System.Serializable]
public class SpecialBlockData
{
    public string name;
    public Vector3 position;

    // 다양한 값들이 들어갈 예정 입니다.
    // Switch 블럭의 경우 Switch 감지 조건인 "감지 될 객체 수"가 됩니다.
    public int specialData;

}


// 블럭 제네레이터 데이터
[System.Serializable]
public class BlockGeneratorData
{
    public string name;
    public int amount;
}


// 영웅 데이터
[System.Serializable]
public class HeroData
{
    public string name;
    public Vector3 position;
    public bool flipX;
    public int hp;

}


// 몬스터 데이터
[System.Serializable]
public class MonsterData
{
    public string name;
    public Vector3 position;
    public bool flipX;

}



// 목표 데이터
[System.Serializable]
public class GoalData
{
    public string name;
    public Vector3 position;
}


// 카메라 데이터
[System.Serializable]
public class CameraData
{
    // 카메라의 시작위치
    public Vector3 position;

    // 게임을 처음 플레이할 시
    // 맵이 카메라에 모두 담기지 않을 정도로 크다면,
    // 시작 위치 -> '중간 지점' -> 목표 위치로 한 번 움직여 플레이어로 하여금 경로를 확인하게 합니다.
    // (컨펌 부탁 드릴 예정)
    // '중간 지점'에 해당 하는 위치 좌표의 배열
    public List<Vector3> stopOverPoints = new List<Vector3>();
}


// Round의 정보가 담길 Storage 입니다.
// JSON 데이터를 불러올 기본 단위 입니다.
// 이보다 큰 단위(스테이지, 혹은 전체 데이터)를 고려해보았으나

// 그리 할 경우 인 게임에서 해당 클래스가 힙(heap) 영역에서 차지하는 데이터가
// 스테이지 * 라운드 만큼의 데이터가 들어가게 됨으로 굉장해 엄청나 만큼 커지게 되어
// 라운드 별로 JSON 파일들을 나누어 저장하고 라운드가 바뀔 때마다 불러오도록 하였습니다.
[System.Serializable]
public class RoundDataStorage
{
    // 이전 스테입지와 다음 스테이지 체이닝을 위함
    // 이전 스테이지 라운드
    public int previousStage;
    public int previousRound;

    // 다음 스테이지 라운드
    public int nextStage;
    public int nextRound;

    // 스테이지 - 라운드
    public int stage;
    public int round;


    public float floorDepth;
    public int succeedStandard;

    public List<TilemapData> tilemapData = new List<TilemapData>();

    public List<SpecialBlockData> specialBlockData = new List<SpecialBlockData>();

    public List<BlockGeneratorData> bgData = new List<BlockGeneratorData>();

    public List<HeroData> heroData = new List<HeroData>();

    public List<MonsterData> monData = new List<MonsterData>();

    public List<GoalData> GoalData = new List<GoalData>();

    public CameraData cameraData;
}


/*// Stage의 정보가 담길 Storage 입니다.
[System.Serializable]
public class StageDataStorage
{
    public int stage;

    public List<RoundDataStorage> roundData = new List<RoundDataStorage>();
}


// 전체 데이터를 담을 Stroage 입니다.
[System.Serializable]
public class AllDataStorage
{
    public List<StageDataStorage> stageData = new List<StageDataStorage>();
}*/

#endregion


// Inspector 창에서 게임의 정보들을 입력하여주고 
// 타일 맵과 함께 저장합니다.
public class SaveStageRoundData : MonoBehaviour
{
    #region DataVariablesForSaveMap
    [Header("<맵 생성을 위한 변수 등록>")]

    [Header("====라운드 속성====")]

    // 이전 스테입지와 다음 스테이지 체이닝을 위함
    // 이전 스테이지 라운드
    [Tooltip("이전 '스테이지' 와 '라운드'")]
    public int previousStage;
    public int previousRound;

    [Tooltip("다음 '스테이지' 와 '라운드'")]
    // 다음 스테이지 라운드
    public int nextStage;
    public int nextRound;

    [Tooltip("현재 '스테이지' 와 '라운드'")]
    // 스테이지 - 라운드
    public int stage;
    public int round;

    [Tooltip("현재 라운드의 바닥 깊이")]
    [SerializeField] private float floorDepth;

    [Tooltip("현재 라운드의 성공 조건 (목표에 도달한 영웅들의 기준 수)")]
    [SerializeField] private int succedStandard;
    // [Space(3)]

    [Header("====타일 맵====\n" +
            "Hierarchy에 등록된 [GridSet]의 자녀로 등록된\n" +
            "Tilemap 들의 정보를 저장합니다.")]
    [Tooltip("데이터를 받아올 현재 타일 맵")]
    [SerializeField] private Tilemap[] tilemapsForRound;

    // [Tooltip("")]
    /// <summary>
    /// 해당 라운드 안에서 등장하는 미리 설치될 특별한 기믹 블럭 리스트 입니다.
    /// </summary>
    [Header("====라운드에서 등장할 {기믹 블럭} 리스트====\n" +
            "속성 들이 등록되는 순서에 유의해야 합니다.")]
    [Tooltip("해당 라운드 안에서 등장하는 미리 설치될 특별한 기믹 블럭 리스트 입니다.")]
    [SerializeField] private GameObject[] specialBlockForRound;


    /// <summary>
    /// 해당 라운드 안에서 등장하는 미리 설치될 특별한 기믹 블럭의 좌표 리스트 입니다.
    /// </summary>
    [Tooltip("해당 라운드 안에서 등장하는 미리 설치될 특별한 기믹 블럭의 좌표 리스트 입니다.")]
    [SerializeField] private Vector3[] specialBlockPositionForRound;

    /// <summary>
    /// 해당 라운드 안에서 등장하는 미리 설치될 특별한 기믹 블럭의 특별한 데이터 입니다.
    /// ex). 스위치 블럭의 경우, 감지될 콜라이더의 임계치
    /// 
    /// 추후 String으로 받아올 수 도 있습니다.
    /// </summary>
    [Tooltip("해당 라운드 안에서 등장하는 미리 설치될 특별한 기믹 블럭의 특별한 데이터 입니다.\r\n  ex). 스위치 블럭의 경우, 감지될 콜라이더의 임계치\r\n   \r\n  추후 String으로 받아올 수 도 있습니다.")]
    [SerializeField] private int[] specialBlockSpecialDataForRound;
    // [Space(3)]



    /// <summary>
    /// 현재 라운드에서 사용할 블럭 정보 index<br/>
    /// 
    /// 배열의 길이가 해당 라운드에서 사용 가능한 블럭의 수가 됩니다.
    /// </summary>
    [Header("====라운드에서 사용할 {블럭} 리스트====\n" +
            "속성 들이 등록되는 순서에 유의해야 합니다.")]
    [Tooltip("현재 라운드에서 사용할 블럭 정보 index\r\n   \r\n 배열의 길이가 해당 라운드에서 사용 가능한 블럭의 수가 됩니다.")]
    [SerializeField] private Button[] blockGeneratorForRoundData;

    /// <summary>
    /// 라운드에서 사용될 블럭의 갯수<br/>
    /// blockGeneratorForRoundData에 등록된 순서대로 동일하게 위치를 등록해주어야 합니다.
    /// </summary>
    [Tooltip("라운드에서 사용될 블럭의 갯수<br/>\r\n blockGeneratorForRoundData에 등록된 순서와 동일하게 '블럭의 수' 속성을 등록해주어야 합니다.")]
    [SerializeField] private int[] blockAmountForRoundData;
    // [Space(3)]



    /// <summary>
    /// 라운드에서 사용될 영웅의 종류 <br/>
    /// 배열의 길이가 해당 라운드에서 사용 될 영웅의 수가 됩니다. 
    /// </summary>
    ///
    [Header("====라운드에서 등장할 {영웅} 리스트====\n" +
            "속성 들이 등록되는 순서에 유의해야 합니다.")]
    [Tooltip("해당 라운드 안에서 등장하는 영웅 리스트 입니다.")]
    [SerializeField] private GameObject[] heroForRoundData;

    /// <summary>
    /// 라운드에서 사용될 영웅의 초기 위치 변수<br/>
    /// heroForRoundData에 등록된 순서대로 동일하게 위치를 등록해주어야 합니다.
    /// </summary>
    [Tooltip("라운드에서 사용될 영웅의 초기 위치 변수<br/>\r\nheroForRoundData에 등록된 순서대로 동일하게 등록해주어야 합니다.")]
    [SerializeField] private Vector3[] heroPositionForRoundData;

    /// <summary>
    /// 라운드에서 사용될 영웅의 방향 <br/>
    /// heroForRoundData에 등록된 순서대로 동일하게 위치를 등록해주어야 합니다.
    /// </summary>
    [Tooltip("라운드에서 사용될 영웅의 방향\nheroForRoundData에 등록된 순서대로 동일하게 등록해주어야 합니다.")]
    [SerializeField] private bool[] characterDirectionForRoundData;

    /// <summary>
    /// 라운드에서 사용될 영웅의 기본 체력 <br/>
    /// heroForRoundData에 등록된 순서대로 동일하게 위치를 등록해주어야 합니다.
    /// </summary>
    [Tooltip("라운드에서 사용될 영웅의 기본 체력\nheroForRoundData에 등록된 순서대로 동일하게 등록해주어야 합니다.")]
    [SerializeField] private int[] characterHpForRoundData;


    /// <summary>
    /// 라운드에서 사용될 몬스터의 종류 <br/>
    /// 배열의 길이가 해당 라운드에서 사용 될 영웅의 수가 됩니다. 
    /// </summary>
    [Header("====라운드에서 등장할 {몬스터} 리스트====\n" +
            "속성 들이 등록되는 순서에 유의해야 합니다.")]
    [Tooltip("라운드에서 등장할 몬스터의 종류")]
    [SerializeField] private GameObject[] monsterForRoundData;

    /// <summary>
    /// 라운드에서 사용될 몬스터의 초기 위치 변수<br/>
    /// monsterForRoundData 등록된 순서와 동일하게 등록해주어야 합니다.
    /// </summary>
    [Tooltip("라운드에서 사용될 몬스터의 초기 위치 변수\nmonsterForRoundData 등록된 순서와 동일하게 등록해주어야 합니다.")]
    [SerializeField] private Vector3[] monsterPositionForRoundData;

    /// <summary>
    /// 라운드에서 사용될 몬스터의 방향 <br/>
    /// monsterForRoundData 등록된 순서대로 동일하게 위치를 등록해주어야 합니다.
    /// </summary>
    [Tooltip("몬스터의 방향\nmonsterForRoundData 등록된 순서와 동일하게 등록해주어야 합니다.")]
    [SerializeField] private bool[] monsterDirectionForRoundData;



    /// <summary>
    /// 라운드에서 사용될 목표지점의 종류 <br/>
    /// 배열의 길이가 해당 라운드에서 사용 될 목표의 수가 됩니다.  (일반 적으론 1개 입니다)
    /// </summary>
    [Header("====라운드에서 등장할 {목표 지점} 리스트====\n" +
            "속성 들이 등록되는 순서에 유의해야 합니다.")]
    [Tooltip("라운드에서 사용될 목표지점의 종류\n" +
             "배열의 길이가 해당 라운드에서 사용 될 목표의 수가 됩니다.  (일반 적으론 1개 입니다)")]
    [SerializeField] private GameObject[] goalForRoundData;


    /// <summary>
    /// 라운드에서 해당 게임의 목표가 있는 위치 입니다.
    /// 여러 곳이 존재할 수 있기에 배열로 선언했으나
    /// 기획 방향에 따라 달라질 수 있습니다.
    /// </summary>
    [Tooltip("라운드에서 해당 게임의 목표가 있는 위치 입니다.\ngoalForRoundData 등록된 순서와 동일하게 등록해주어야 합니다.")]
    [SerializeField] private Vector3[] goalPositionForRound;


    /// <summary>
    /// 라운드에서 카메라의 초기 위치<br/>
    /// 카메라는 1개만 존재합니다.
    /// </summary>
    [Header("====라운드의 {카메라} 속성====")]
    [Tooltip("라운드에서 카메라의 초기 위치")]
    [SerializeField] private Vector3 cameraPositionForRound;


    /// <summary>
    /// 게임이 처음으로 시작할 시 목표를 따라 카메라 무빙을 하게 되는데,
    /// 그 때 경유할 경유지 입니다.
    /// 없을 수 도 있습니다.
    /// </summary>
    [Tooltip("게임이 처음으로 시작할 시 목표를 따라 카메라 무빙을 하게 되는데,\n그 때 경유할 경유지 리스트 입니다.\n" +
             "경유지의 '처음'과 '끝'은 각각 영웅의 초기 위치와 목표 지점 위치 입니다.")]
    [SerializeField] private Vector3[] cameraStopOverPointForRound;


    // 앞으로 더욱 추가될 예정 입니다.
    #endregion




    // 인 게임에서 실행 될 함수가 아닙니다!
    // 인 게임에서 실행 될 함수가 아닙니다!
    private RoundDataStorage CreateDataStorage()
    {
        // 모든 데이터를 담아올 roundData 변수를 생성합니다.
        RoundDataStorage _roundData = new RoundDataStorage();

        // 스테이지와 라운드를 업데이트 하여줍시다

        _roundData.previousStage = previousStage;
        _roundData.previousRound = previousRound;

        _roundData.nextStage = nextStage;
        _roundData.nextRound = nextRound;


        _roundData.stage = stage;
        _roundData.round = round;


        _roundData.floorDepth = floorDepth;
        _roundData.succeedStandard = succedStandard;

        // 인스펙터에서 연결한 현재 타일 맵 정보 업데이트
        foreach (Tilemap _tilemap in tilemapsForRound)
        {
            TilemapData _tilemapData = new TilemapData();
            _tilemapData.name = _tilemap.name;

            foreach (Vector3Int position in _tilemap.cellBounds.allPositionsWithin)
            {
                if (_tilemap.HasTile(position))
                {
                    _tilemapData.positions.Add(position);
                    _tilemapData.tiles.Add(_tilemap.GetTile(position).name);
                    // Tile을 직접 저장하는대신 이름으로 저장하여
                    // MapManager의 TileBase 딕셔너리에서 찾아가게 합시다
                }
            }
            _roundData.tilemapData.Add(_tilemapData);
        }

        // 인스펙터에서 작성한 사전 설치 기믹 블럭 정보 동기화
        for (int i = 0; i < specialBlockForRound.Length; i++)
        {
            SpecialBlockData _specialData = new SpecialBlockData();
            _specialData.name = specialBlockForRound[i].name;
            _specialData.position = specialBlockPositionForRound[i];
            _specialData.specialData = specialBlockSpecialDataForRound[i];

            _roundData.specialBlockData.Add(_specialData);
        }

        // 인스펙터에서 작성한 블럭 제네레이터 정보 동기화
        for (int i = 0; i < blockGeneratorForRoundData.Length; i++)
        {
            BlockGeneratorData _bgData = new BlockGeneratorData();
            _bgData.name = blockGeneratorForRoundData[i].name;
            _bgData.amount = blockAmountForRoundData[i];

            _roundData.bgData.Add(_bgData);
        }

        // 인스펙터에서 작성한 영웅 정보 동기화
        for (int i = 0; i < heroForRoundData.Length; i++)
        {
            HeroData _heroData = new HeroData();
            _heroData.name = heroForRoundData[i].name;
            _heroData.position = heroPositionForRoundData[i];
            _heroData.flipX = characterDirectionForRoundData[i];
            _heroData.hp = characterHpForRoundData[i];

            _roundData.heroData.Add(_heroData);
        }

        // 인스펙터에서 작성한 몬스터 정보 동기화
        for (int i = 0; i < monsterForRoundData.Length; i++)
        {
            MonsterData _monData = new MonsterData();
            _monData.name = monsterForRoundData[i].name;
            _monData.position = monsterPositionForRoundData[i];
            _monData.flipX = monsterDirectionForRoundData[i];

            _roundData.monData.Add(_monData);
        }

        // 인스펙터에서 작성한 목표 정보 동기화
        for (int i = 0; i < goalForRoundData.Length; i++)
        {
            GoalData _goalData = new GoalData();
            _goalData.name = goalForRoundData[i].name;
            _goalData.position = goalPositionForRound[i];

            _roundData.GoalData.Add(_goalData);
        }


        // 인스펙터에서 작성한 카메라 정보 동기화
        CameraData _cameraData = new CameraData();
        _cameraData.position = cameraPositionForRound;

        // 경유지는 없을 수 도 있습니다.
        foreach (Vector3 _cameraStopOver in cameraStopOverPointForRound)
        {
            _cameraData.stopOverPoints.Add(_cameraStopOver);
        }

        _roundData.cameraData = _cameraData;


        return _roundData;
    }

    // 이 함수는 인 게임에서는 사용될 일이 없습니다.
    // 라운드 데이터들을 모두 등록한 이후 등록된 데이터를 기반으로 JSON 파일을 뽑아내길 원할 때 호출하는 메소드 입니다.
    public void Save()
    {
        // StreamingAssets > Datas 폴더 아래 스테이지 폴더 아래 해당 라운드 데이터를 담은 JSON을 저장 합니다.

        //해당 경로에 Stage 폴더가 없다면 생성합니다.
        string folderPath = Application.streamingAssetsPath + $"/Datas/Stage{stage}";
        /*string folderPath = $"/Datas/Stage{stage}";*/
        if (!Directory.Exists(folderPath))
        {
            Debug.Log("None");
            Directory.CreateDirectory(folderPath);
        }

        string path = folderPath + $"/Round[{round}].json";
        Debug.Log(path);
        File.WriteAllText(path, JsonUtility.ToJson(CreateDataStorage()));
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
