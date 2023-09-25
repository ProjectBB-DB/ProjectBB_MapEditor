using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// region Ű����� endregion�� ���� ���� Ư�� �κ��� �ڵ带 ��� ������ �� �ֽ��ϴ�.
// �ý��ۻ� ��ȭ�Ǵ°� ������ �ڵ带 ���� �� ����ϰ� �� �� �ְ� �Ͽ��ݴϴ�.
#region GAMEDATA



// Ÿ�� �� ������
// �� ������ �ƴ� ���� ������ ����� ���
// JSON���� InstanceID�� ���� �ǰ� �˴ϴ�.

//�̴� Unity ������ ��Ÿ�� ���� �޶����� ������
// Index(int)�� Ȥ�� Key("string")�� �־��ְ�
// �̸� ���� �Ŵ����� �ȿ� �ִ� �迭 Ȥ�� ��ųʸ����� ã�ư��� �սô�
[System.Serializable]
public class TilemapData
{
    public string name;
    public List<Vector3Int> positions = new List<Vector3Int>();
    public List<string> tiles = new List<string>();
}


// ��� �� ������
[System.Serializable]
public class SpecialBlockData
{
    public string name;
    public Vector3 position;

    // �پ��� ������ �� ���� �Դϴ�.
    // Switch ���� ��� Switch ���� ������ "���� �� ��ü ��"�� �˴ϴ�.
    public int specialData;

}


// �� ���׷����� ������
[System.Serializable]
public class BlockGeneratorData
{
    public string name;
    public int amount;
}


// ���� ������
[System.Serializable]
public class HeroData
{
    public string name;
    public Vector3 position;
    public bool flipX;
    public int hp;

}


// ���� ������
[System.Serializable]
public class MonsterData
{
    public string name;
    public Vector3 position;
    public bool flipX;

}



// ��ǥ ������
[System.Serializable]
public class GoalData
{
    public string name;
    public Vector3 position;
}


// ī�޶� ������
[System.Serializable]
public class CameraData
{
    // ī�޶��� ������ġ
    public Vector3 position;

    // ������ ó�� �÷����� ��
    // ���� ī�޶� ��� ����� ���� ������ ũ�ٸ�,
    // ���� ��ġ -> '�߰� ����' -> ��ǥ ��ġ�� �� �� ������ �÷��̾�� �Ͽ��� ��θ� Ȯ���ϰ� �մϴ�.
    // (���� ��Ź �帱 ����)
    // '�߰� ����'�� �ش� �ϴ� ��ġ ��ǥ�� �迭
    public List<Vector3> stopOverPoints = new List<Vector3>();
}


// Round�� ������ ��� Storage �Դϴ�.
// JSON �����͸� �ҷ��� �⺻ ���� �Դϴ�.
// �̺��� ū ����(��������, Ȥ�� ��ü ������)�� ����غ�������

// �׸� �� ��� �� ���ӿ��� �ش� Ŭ������ ��(heap) �������� �����ϴ� �����Ͱ�
// �������� * ���� ��ŭ�� �����Ͱ� ���� ������ ������ ��û�� ��ŭ Ŀ���� �Ǿ�
// ���� ���� JSON ���ϵ��� ������ �����ϰ� ���尡 �ٲ� ������ �ҷ������� �Ͽ����ϴ�.
[System.Serializable]
public class RoundDataStorage
{
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


/*// Stage�� ������ ��� Storage �Դϴ�.
[System.Serializable]
public class StageDataStorage
{
    public int stage;

    public List<RoundDataStorage> roundData = new List<RoundDataStorage>();
}


// ��ü �����͸� ���� Stroage �Դϴ�.
[System.Serializable]
public class AllDataStorage
{
    public List<StageDataStorage> stageData = new List<StageDataStorage>();
}*/

#endregion


// Inspector â���� ������ �������� �Է��Ͽ��ְ� 
// Ÿ�� �ʰ� �Բ� �����մϴ�.
public class SaveStageRoundData : MonoBehaviour
{
    #region DataVariablesForSaveMap
    // �Ʒ� ���� ���� �� ���ӿ��� ���� ������ �ƴմϴ�.
    // ���� �����ϱ� ���� '�ν�����'���� ������־���� ������ �Դϴ�.
    //���� '��������' �� '����'
    [SerializeField] private int stage;
    [SerializeField] private int round;

    // ���� ������ �ٴ� ����
    [SerializeField] private float floorDepth;

    // ���� ������ ���� ���� (��ǥ�� ������ �������� ���� ��)
    [SerializeField] private int succedStandard;

    // �����͸� �޾ƿ� ���� Ÿ�� ��
    [SerializeField] private Tilemap[] tilemapsForRound;


    /// <summary>
    /// �ش� ���� �ȿ��� �����ϴ� �̸� ��ġ�� Ư���� ��� �� ����Ʈ �Դϴ�.
    /// </summary>
    [SerializeField] private GameObject[] specialBlockForRound;


    /// <summary>
    /// �ش� ���� �ȿ��� �����ϴ� �̸� ��ġ�� Ư���� ��� ���� ��ǥ ����Ʈ �Դϴ�.
    /// </summary>
    [SerializeField] private Vector3[] specialBlockPositionForRound;

    /// <summary>
    /// �ش� ���� �ȿ��� �����ϴ� �̸� ��ġ�� Ư���� ��� ���� Ư���� ������ �Դϴ�.
    /// ex). ����ġ ���� ���, ������ �ݶ��̴��� �Ӱ�ġ
    /// 
    /// ���� String���� �޾ƿ� �� �� �ֽ��ϴ�.
    /// </summary>
    [SerializeField] private int[] specialBlockSpecialDataForRound;


    /// <summary>
    /// ���� ���忡�� ����� �� ���� index<br/>
    /// 
    /// �迭�� ���̰� �ش� ���忡�� ��� ������ ���� ���� �˴ϴ�.
    /// </summary>
    [SerializeField] private Button[] blockGeneratorForRoundData;

    /// <summary>
    /// ���忡�� ���� ���� ����<br/>
    /// blockGeneratorForRoundData�� ��ϵ� ������� �����ϰ� ��ġ�� ������־�� �մϴ�.
    /// </summary>
    [SerializeField] private int[] blockAmountForRoundData;


    /// <summary>
    /// ���忡�� ���� ������ ���� <br/>
    /// �迭�� ���̰� �ش� ���忡�� ��� �� ������ ���� �˴ϴ�. 
    /// </summary>
    [SerializeField] private GameObject[] heroForRoundData;

    /// <summary>
    /// ���忡�� ���� ������ �ʱ� ��ġ ����<br/>
    /// heroForRoundData�� ��ϵ� ������� �����ϰ� ��ġ�� ������־�� �մϴ�.
    /// </summary>
    [SerializeField] private Vector3[] heroPositionForRoundData;

    /// <summary>
    /// ���忡�� ���� ������ ���� <br/>
    /// heroForRoundData�� ��ϵ� ������� �����ϰ� ��ġ�� ������־�� �մϴ�.
    /// </summary>
    [SerializeField] private bool[] characterDirectionForRoundData;

    /// <summary>
    /// ���忡�� ���� ������ �⺻ ü�� <br/>
    /// heroForRoundData�� ��ϵ� ������� �����ϰ� ��ġ�� ������־�� �մϴ�.
    /// </summary>
    [SerializeField] private int[] characterHpForRoundData;


    /// <summary>
    /// ���忡�� ���� ������ ���� <br/>
    /// �迭�� ���̰� �ش� ���忡�� ��� �� ������ ���� �˴ϴ�. 
    /// </summary>
    [SerializeField] private GameObject[] monsterForRoundData;

    /// <summary>
    /// ���忡�� ���� ������ �ʱ� ��ġ ����<br/>
    /// monsterForRoundData ��ϵ� ������� �����ϰ� ��ġ�� ������־�� �մϴ�.
    /// </summary>
    [SerializeField] private Vector3[] monsterPositionForRoundData;

    /// <summary>
    /// ���忡�� ���� ������ ���� <br/>
    /// monsterForRoundData ��ϵ� ������� �����ϰ� ��ġ�� ������־�� �մϴ�.
    /// </summary>
    [SerializeField] private bool[] monsterDirectionForRoundData;



    /// <summary>
    /// ���忡�� ���� ��ǥ������ ���� <br/>
    /// �迭�� ���̰� �ش� ���忡�� ��� �� ��ǥ�� ���� �˴ϴ�.  (�Ϲ� ������ 1�� �Դϴ�)
    /// </summary>
    [SerializeField] private GameObject[] goalForRoundData;


    /// <summary>
    /// ���忡�� �ش� ������ ��ǥ�� �ִ� ��ġ �Դϴ�.
    /// ���� ���� ������ �� �ֱ⿡ �迭�� ����������
    /// ��ȹ ���⿡ ���� �޶��� �� �ֽ��ϴ�.
    /// </summary>
    [SerializeField] private Vector3[] goalPositionForRound;


    /// <summary>
    /// ���忡�� ī�޶��� �ʱ� ��ġ<br/>
    /// ī�޶�� 1���� �����մϴ�.
    /// </summary>
    [SerializeField] private Vector3 cameraPositionForRound;


    /// <summary>
    /// ������ ó������ ������ �� ��ǥ�� ���� ī�޶� ������ �ϰ� �Ǵµ�,
    /// �� �� ������ ������ �Դϴ�.
    /// ���� �� �� �ֽ��ϴ�.
    /// </summary>
    [SerializeField] private Vector3[] cameraStopOverPointForRound;





    // ������ ���� �߰��� ���� �Դϴ�.
    #endregion




    // �� ���ӿ��� ���� �� �Լ��� �ƴմϴ�!
    // �� ���ӿ��� ���� �� �Լ��� �ƴմϴ�!
    private RoundDataStorage CreateDataStorage()
    {
        // ��� �����͸� ��ƿ� roundData ������ �����մϴ�.
        RoundDataStorage _roundData = new RoundDataStorage();

        // ���������� ���带 ������Ʈ �Ͽ��ݽô�
        _roundData.stage = stage;
        _roundData.round = round;
        _roundData.floorDepth = floorDepth;
        _roundData.succeedStandard = succedStandard;

        // �ν����Ϳ��� ������ ���� Ÿ�� �� ���� ������Ʈ
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
                    // Tile�� ���� �����ϴ´�� �̸����� �����Ͽ�
                    // MapManager�� TileBase ��ųʸ����� ã�ư��� �սô�
                }
            }
            _roundData.tilemapData.Add(_tilemapData);
        }

        // �ν����Ϳ��� �ۼ��� ���� ��ġ ��� �� ���� ����ȭ
        for (int i = 0; i < specialBlockForRound.Length; i++)
        {
            SpecialBlockData _specialData = new SpecialBlockData();
            _specialData.name = specialBlockForRound[i].name;
            _specialData.position = specialBlockPositionForRound[i];
            _specialData.specialData = specialBlockSpecialDataForRound[i];

            _roundData.specialBlockData.Add(_specialData);
        }

        // �ν����Ϳ��� �ۼ��� �� ���׷����� ���� ����ȭ
        for (int i = 0; i < blockGeneratorForRoundData.Length; i++)
        {
            BlockGeneratorData _bgData = new BlockGeneratorData();
            _bgData.name = blockGeneratorForRoundData[i].name;
            _bgData.amount = blockAmountForRoundData[i];

            _roundData.bgData.Add(_bgData);
        }

        // �ν����Ϳ��� �ۼ��� ���� ���� ����ȭ
        for (int i = 0; i < heroForRoundData.Length; i++)
        {
            HeroData _heroData = new HeroData();
            _heroData.name = heroForRoundData[i].name;
            _heroData.position = heroPositionForRoundData[i];
            _heroData.flipX = characterDirectionForRoundData[i];
            _heroData.hp = characterHpForRoundData[i];

            _roundData.heroData.Add(_heroData);
        }

        // �ν����Ϳ��� �ۼ��� ���� ���� ����ȭ
        for (int i = 0; i < monsterForRoundData.Length; i++)
        {
            MonsterData _monData = new MonsterData();
            _monData.name = monsterForRoundData[i].name;
            _monData.position = monsterPositionForRoundData[i];
            _monData.flipX = monsterDirectionForRoundData[i];

            _roundData.monData.Add(_monData);
        }

        // �ν����Ϳ��� �ۼ��� ��ǥ ���� ����ȭ
        for (int i = 0; i < goalForRoundData.Length; i++)
        {
            GoalData _goalData = new GoalData();
            _goalData.name = goalForRoundData[i].name;
            _goalData.position = goalPositionForRound[i];

            _roundData.GoalData.Add(_goalData);
        }


        // �ν����Ϳ��� �ۼ��� ī�޶� ���� ����ȭ
        CameraData _cameraData = new CameraData();
        _cameraData.position = cameraPositionForRound;

        // �������� ���� �� �� �ֽ��ϴ�.
        foreach (Vector3 _cameraStopOver in cameraStopOverPointForRound)
        {
            _cameraData.stopOverPoints.Add(_cameraStopOver);
        }

        _roundData.cameraData = _cameraData;


        return _roundData;
    }

    // �� �Լ��� �� ���ӿ����� ���� ���� �����ϴ�.
    // ���� �����͵��� ��� ����� ���� ��ϵ� �����͸� ������� JSON ������ �̾Ƴ��� ���� �� ȣ���ϴ� �޼ҵ� �Դϴ�.
    public void Save()
    {
        // StreamingAssets > Datas ���� �Ʒ� �������� ���� �Ʒ� �ش� ���� �����͸� ���� JSON�� ���� �մϴ�.

        //�ش� ��ο� Stage ������ ���ٸ� �����մϴ�.
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
