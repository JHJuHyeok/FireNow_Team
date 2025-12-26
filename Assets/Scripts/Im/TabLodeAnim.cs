
using UnityEngine;
using DG.Tweening;

public class TabLodeAnim : MonoBehaviour
{
    [Header("MapSelect ÆÐ³Î")]
    [SerializeField] private GameObject MapSelectPanel;
    [SerializeField] private GameObject ScaleShow;
    [SerializeField] private float DeploymentSpeed = 0.1f;
    [SerializeField] private Ease ShowingEase = Ease.InOutBounce;
    private Vector3 _TargetPosition;



    public void OnShowMapPanel()
    {
        ScaleShow.transform.DOScale(0.5f,0f);
        MapSelectPanel.SetActive(true);
        ScaleShow.transform.DOScale(1f, DeploymentSpeed).SetEase(ShowingEase);
    }
}
