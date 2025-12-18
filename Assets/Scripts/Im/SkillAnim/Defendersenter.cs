using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class Defendersenter : MonoBehaviour
{

    [SerializeField] private GameObject prefabDefender;  //수호자/수비수 프리펩
    [SerializeField] private int MaxDefenderCount = 6; //수호자 최대갯수 본게임도 6개임으로 그이상 불필요 예상.

    [Range(2,6)] public int defenderCount = 2;  //수호자갯수

    public float revolutionSpeed = 1f;  //공전 속도 조절
    public float spinradius = 1.5f;  // 공전 범위 조절
    public float LifeTime = 4f;     //지속 시간
    public float coolTime = 4f;     //쿨타임
    
    public bool _eveloution = false;  //진화여부

    private List<GameObject> _pool = new(); //풀링리스트

    private Tween _revolutiontween;
    private Coroutine _lifeCoroutine;

    private void Awake()
    {
        CreatePool();
    }

    private void OnEnable()
    {
        
        ActivateDefenders();
        revolves();
        _lifeCoroutine = StartCoroutine(LifeCycle());
    }
    private void OnDisable()
    {
        _revolutiontween?.Kill();

        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);

        DeactivationDefenders();
    }
    private void CreatePool()
    {  //프리팹 풀에 6개 생성
        _pool.Clear();

        for (int i = 0; i < MaxDefenderCount; i++)
        {
            
           GameObject obj = Instantiate(prefabDefender, transform);
            obj.SetActive(false);
            _pool.Add(obj);
        }
        
    }

    private void ActivateDefenders()
    {   // 프리팹 활성화.
        
        for (int i = 0; i < _pool.Count;i++)
        {   
            GameObject Obj = _pool[i];

            if (i<defenderCount)
            {   //풀 안에있는것 중 활성화 된것만 일정한 간격으로 배치.
                float angel = i * Mathf.PI * 2f / defenderCount;
                Vector2 offset = new Vector2(Mathf.Cos(angel), Mathf.Sin(angel)) * spinradius;
                
                Obj.transform.localPosition = offset;
                Obj.GetComponent<Defender>().setEvolution(_eveloution);
                Obj.SetActive(true);
            }
            else
            {
                Obj.SetActive(false);
            }
            
            
        }
    }
    private void DeactivationDefenders()
    {// 오브젝트 풀로 되돌리기. 
        foreach (var obj in _pool)
        {
            obj.SetActive(false);
        }
    }
    private IEnumerator LifeCycle()
    {  // 나타나고 사라지는 주기 조절.
        while (!_eveloution) //진화 되면 이 주기 사라져, LifeTime/coolTime 의미 없음. 
        {
            yield return new WaitForSeconds(LifeTime);
            DeactivationDefenders();
            yield return new WaitForSeconds(coolTime);
            ActivateDefenders();
        }
    }
    
    private void revolves()
    {   // 해당 프리팹은 자식으로 소환됨으로, 이 스크립트가 있는 오브젝트를 회전 시킴으로 공전한다.
        // 자전은 프리팹에 들어있는 스크립트로 자전한다.
        _revolutiontween?.Kill();
        _revolutiontween = transform.DOLocalRotate(new Vector3(0, 0, -360), revolutionSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

    }

    public void SetEvolutution(bool value)  // 진화 하면 호출하면 즉시 적용됨.
    {
        _eveloution = value;

        foreach (var obj in _pool)
        {
            if (!obj.activeSelf) continue;

            obj.GetComponent<Defender>().setEvolution(_eveloution);
        }
    }
    public void SetDefenderCount(int count) // 갯수가 바뀌면 SetDefenderCount(3); 식으로 호출해줄것.
    {
        defenderCount = count;
        ActivateDefenders();
    }

    
}
