using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NavAgent : MonoBehaviour
{
    private PriorityQueue<AstarNode> _openList; //이게 내가 갈 수 있는 것들을 모아놓은 곳
    private List<AstarNode> _closeList; //이건 내가 한번이라도 방문한 노드들을 모아놓은 곳

    private List<Vector3Int> _routePath; //내가 가야할 경로를 타일 포지션으로 가지고 있는 것

    [SerializeField] private float _speed = 3f;
    [SerializeField] private bool _cornerCheck = false; //코너링 돌때 체크여부 저장

    private bool _isMove = false; //현재 이동중이냐?
    private int _moveIdx = 0; //라우트 경로중 몇번째 노드를 향해 가고 있는가?

    private Vector3Int _currentPos; //현재 타일 포지션
    private Vector3Int _destinationPos; //목표 타일 포지션

    private Vector3 _nextPos; //다음에 이동할 월드 좌표

    public Vector3Int Destination
    {
        get => _destinationPos;
        set
        {
            if (_destinationPos == value) return; //목적지가 변하지 않았다면 굳이
            SetCurrentPosition(); //내 현재 위치를 시작점을 설정하고
            _destinationPos = value;
            CalculatePath(); //라우팅 경로 계산해주고
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
        transform.position = MapManager.Instance.GetWorldPosition(_currentPos); //계산된 타일좌표의 중심으로 이동
    }

    private void PrintRoute()
    {
        _lineRenderer.positionCount = _routePath.Count; //경로의 갯수만큼 점을 늘린다.


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

    #region Astar 알고리즘

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

        bool result = false; //경로를 찾았는가?

        int cnt = 0;
        while (_openList.Count > 0)
        {
            AstarNode node = _openList.Pop(); //우선순위 큐로 만들어져있으니까 가장 우선이 되는 애가 나오겠지.
            FindOpenList(node);
            _closeList.Add(node); //node는 한번 써먹었으니 다시 못가게 closeList로 들어가야 한다.
            if (node.pos == _destinationPos) //방문 노드가 목적지였다면 다 온거니까 break 해줘라.
            {
                result = true;
                break;
            }

            cnt++;
            if (cnt > 10000)
            {
                Debug.Log("야 이건 무리다.");
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
                node = node.parent; //부모찾아서 올라간다. 
            }
            _routePath.Reverse(); //역순으로 뒤집는다.
            _moveIdx = 0;
        }

    }

    private void FindOpenList(AstarNode node)
    {
        //무슨일이 일어나는지 모르겠는데 어쨌건 이건 미래의 내가 알아서 코딩해줄꺼야.
        //암튼 이거 끝나면 _openList에는 갈 수 있는 곳이 다 들어가 있을꺼야.
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && x == y) continue; //이건 현재 내 자리니까 무시

                Vector3Int nextPos = node.pos + new Vector3Int(x, y, 0); //검사할 좌표를 nextPos로 만든다.

                AstarNode temp = _closeList.Find(node => node.pos == nextPos);
                if (temp != null) continue; //여긴 이미 한번 갔던곳이니까 갈 필요가 없다.

                if (!MapManager.Instance.CanMove(nextPos)) continue; //맵에서 갈 수 없는 곳이니까 무시

                //여기까지 왔으면 이제 갈 수 있는 곳이니까 계산해서 오픈리스트에 넣어주자.
                float g = (node.pos - nextPos).magnitude + node.G;
                AstarNode nextOpenNode = new AstarNode
                {
                    pos = nextPos,
                    parent = node,
                    G = g,
                    F = g + CalculateH(nextPos)
                };

                AstarNode exist = _openList.Contains(nextOpenNode); //지금 내가 가려고 하는게 이미 누군가 오픈리스트에 넣어놨냐?
                if (exist != null)
                {
                    if (nextOpenNode.G < exist.G)
                    {
                        exist.G = nextOpenNode.G;
                        exist.F = nextOpenNode.F;
                        exist.parent = nextOpenNode.parent;

                        _openList.Recalculation(exist); //다시 계산해서 힙에서 작은애가 위로 오도록 해줘라.
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