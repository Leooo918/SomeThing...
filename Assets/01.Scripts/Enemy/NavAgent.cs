using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NavAgent : MonoBehaviour
{
    private PriorityQueue<AstarNode> _openList; //�̰� ���� �� �� �ִ� �͵��� ��Ƴ��� ��
    private List<AstarNode> _closeList; //�̰� ���� �ѹ��̶� �湮�� ������ ��Ƴ��� ��

    private List<Vector3Int> _routePath; //���� ������ ��θ� Ÿ�� ���������� ������ �ִ� ��

    [SerializeField] private float _speed = 3f;
    [SerializeField] private bool _cornerCheck = false; //�ڳʸ� ���� üũ���� ����

    private bool _isMove = false; //���� �̵����̳�?
    private int _moveIdx = 0; //���Ʈ ����� ���° ��带 ���� ���� �ִ°�?

    private Vector3Int _currentPos; //���� Ÿ�� ������
    private Vector3Int _destinationPos; //��ǥ Ÿ�� ������

    private Vector3 _nextPos; //������ �̵��� ���� ��ǥ

    public Vector3Int Destination
    {
        get => _destinationPos;
        set
        {
            if (_destinationPos == value) return; //�������� ������ �ʾҴٸ� ����
            SetCurrentPosition(); //�� ���� ��ġ�� �������� �����ϰ�
            _destinationPos = value;
            CalculatePath(); //����� ��� ������ְ�
            PrintRoute();
        }
    }

    public bool GetNextPath(out Vector3Int nextPos)
    {
        nextPos = new Vector3Int();
        if (_routePath.Count == 0 || _moveIdx >= _routePath.Count)
        {
            return false;
        }

        nextPos = _routePath[_moveIdx++];
        return true;
    }

    private LineRenderer _lineRenderer;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _closeList = new List<AstarNode>();
        _routePath = new List<Vector3Int>();
        _openList = new PriorityQueue<AstarNode>();
    }

    private void Start()
    {
        SetCurrentPosition();
        transform.position = MapManager.Instance.GetWorldPosition(_currentPos); //���� Ÿ����ǥ�� �߽����� �̵�
    }

    private void PrintRoute()
    {
        _lineRenderer.positionCount = _routePath.Count; //����� ������ŭ ���� �ø���.


        _lineRenderer.SetPositions(_routePath.Select(p => MapManager.Instance.GetWorldPosition(p)).ToArray());

        //uniRX
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mPos = Input.mousePosition;
            mPos.z = 0;
            Vector3 world = Camera.main.ScreenToWorldPoint(mPos);
            Vector3Int cellPos = MapManager.Instance.GetTilePos(world);
            Destination = cellPos;
        }
    }

    private void SetCurrentPosition()
    {
        _currentPos = MapManager.Instance.GetTilePos(transform.position);
    }

    #region Astar �˰���

    private void CalculatePath()
    {
        _openList.Clear();
        _closeList.Clear();

        _openList.Push(new AstarNode
        {
            pos = _currentPos,
            parent = null,
            G = 0,
            F = CalculateH(_currentPos)
        });

        bool result = false; //��θ� ã�Ҵ°�?

        int cnt = 0;
        while (_openList.Count > 0)
        {
            AstarNode node = _openList.Pop(); //�켱���� ť�� ������������ϱ� ���� �켱�� �Ǵ� �ְ� ��������.
            FindOpenList(node);
            _closeList.Add(node); //node�� �ѹ� ��Ծ����� �ٽ� ������ closeList�� ���� �Ѵ�.
            if (node.pos == _destinationPos) //�湮 ��尡 ���������ٸ� �� �°Ŵϱ� break �����.
            {
                result = true;
                break;
            }

            cnt++;
            if (cnt > 10000)
            {
                Debug.Log("�� �̰� ������.");
                break;
            }
        }

        if (result)
        {
            _routePath.Clear();
            AstarNode node = _closeList.Last();
            while (node.parent != null)
            {
                _routePath.Add(node.pos);
                node = node.parent; //�θ�ã�Ƽ� �ö󰣴�. 
            }
            _routePath.Reverse(); //�������� �����´�.
            _moveIdx = 0;
        }

    }

    private void FindOpenList(AstarNode node)
    {
        //�������� �Ͼ���� �𸣰ڴµ� ��·�� �̰� �̷��� ���� �˾Ƽ� �ڵ����ٲ���.
        //��ư �̰� ������ _openList���� �� �� �ִ� ���� �� �� ��������.
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && x == y) continue; //�̰� ���� �� �ڸ��ϱ� ����

                Vector3Int nextPos = node.pos + new Vector3Int(x, y, 0); //�˻��� ��ǥ�� nextPos�� �����.

                AstarNode temp = _closeList.Find(node => node.pos == nextPos);
                if (temp != null) continue; //���� �̹� �ѹ� �������̴ϱ� �� �ʿ䰡 ����.

                if (!MapManager.Instance.CanMove(nextPos)) continue; //�ʿ��� �� �� ���� ���̴ϱ� ����

                //������� ������ ���� �� �� �ִ� ���̴ϱ� ����ؼ� ���¸���Ʈ�� �־�����.
                float g = (node.pos - nextPos).magnitude + node.G;
                AstarNode nextOpenNode = new AstarNode
                {
                    pos = nextPos,
                    parent = node,
                    G = g,
                    F = g + CalculateH(nextPos)
                };

                AstarNode exist = _openList.Contains(nextOpenNode); //���� ���� ������ �ϴ°� �̹� ������ ���¸���Ʈ�� �־����?
                if (exist != null)
                {
                    if (nextOpenNode.G < exist.G)
                    {
                        exist.G = nextOpenNode.G;
                        exist.F = nextOpenNode.F;
                        exist.parent = nextOpenNode.parent;

                        _openList.Recalculation(exist); //�ٽ� ����ؼ� ������ �����ְ� ���� ������ �����.
                    }
                }
                else
                {
                    _openList.Push(nextOpenNode);
                }
            }
        }
    }

    private float CalculateH(Vector3Int pos)
    {
        return (_destinationPos - pos).magnitude;
    }
    #endregion
    // http://gondr99.iptime.org:9090
}