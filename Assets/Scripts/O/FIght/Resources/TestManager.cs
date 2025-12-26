//using UnityEngine;

//public class TestManager : MonoBehaviour
//{
//    [SerializeField] private GameObject player;
//    private PlayerAbility playerAbility;

//    private void Start()
//    {
//        playerAbility = player.GetComponent<PlayerAbility>();
//    }

//    private void Update()
//    {
//        // 1번 키: 수리검 획득/레벨업
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            playerAbility?.AcquireAbility("shuriken");
//            Debug.Log("수리검 획득/레벨업");
//        }

//        // 2번 키: 수호자 획득/레벨업
//        if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            playerAbility?.AcquireAbility("defender");
//            Debug.Log("수호자 획득/레벨업");
//        }

//        // 3번 키: 보호막 획득/레벨업
//        if (Input.GetKeyDown(KeyCode.Alpha3))
//        {
//            playerAbility?.AcquireAbility("forcefield");
//            Debug.Log("보호막 획득/레벨업");
//        }

//        // E키: 진화 체크
//        if (Input.GetKeyDown(KeyCode.E))
//        {
//            playerAbility?.CheckEvolution("shuriken");
//            Debug.Log("진화 체크");
//        }
//    }
//}