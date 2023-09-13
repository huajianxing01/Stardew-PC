using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds twoSeceonds;
    [SerializeField] private GameObject reapingPrefab = null;
    [SerializeField] private GameObject canyonOakLeavesFallingPrefab = null;
    [SerializeField] private GameObject blueSpruceLeavesFallingPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;

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

    private IEnumerator DisableHarvestActionEffect(GameObject effectObject, WaitForSeconds waitForSeconds)
    {
        yield return waitForSeconds;
        effectObject.SetActive(false);
    }

    private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect effect)
    {
        switch (effect)
        {
            case HarvestActionEffect.reaping:
                GameObject reaping = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                reaping.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(reaping, twoSeceonds));
                break;
            case HarvestActionEffect.deciduousLeavesFalling:
                GameObject canyonOakLeavesFalling = PoolManager.Instance.ReuseObject(canyonOakLeavesFallingPrefab, effectPosition, Quaternion.identity);
                canyonOakLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(canyonOakLeavesFalling, twoSeceonds));
                break;
            case HarvestActionEffect.pineConesFalling:
                GameObject blueSpruceLeavesFalling = PoolManager.Instance.ReuseObject(blueSpruceLeavesFallingPrefab, effectPosition, Quaternion.identity);
                blueSpruceLeavesFalling.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(blueSpruceLeavesFalling, twoSeceonds));
                break;
            case HarvestActionEffect.choppingTreeTrunk:
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                choppingTreeTrunk.SetActive(true);
                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, twoSeceonds));
                break;
            case HarvestActionEffect.breakingStone:
            case HarvestActionEffect.none:
                break;
            default:
                break;
        }
    }
}
