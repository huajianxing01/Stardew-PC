using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds twoSeceonds;
    [SerializeField] private GameObject reapingPrefab = null;

    protected override void Awake()
    {
        base.Awake();
        twoSeceonds = new WaitForSeconds(2);
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectObject,WaitForSeconds waitForSeconds)
    {
        yield return waitForSeconds;
        effectObject.SetActive(false);
    }

    private void DisplayHarvestActionEffect(Vector3 vector, HarvestActionEffect effect)
    {
        switch (effect)
        {
            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, vector, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeceonds));
                break;
            case HarvestActionEffect.deciduousLeavesFalling:
            case HarvestActionEffect.breakingStone:
            case HarvestActionEffect.pineConesFalling:
            case HarvestActionEffect.choppingTreeTrunk:
            case HarvestActionEffect.none:
                break;
            default:
                break;
        }
    }
}
