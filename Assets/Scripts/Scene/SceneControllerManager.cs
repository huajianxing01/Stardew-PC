using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    public SceneName startingSceneName;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;

    private IEnumerator Start()
    {
        faderImage.color = new Color(0,0,0,1);
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));
        
        EventHandler.CallAfterSceneLoadEvent();
        //�����������ʱ������Ҫ���أ������һ�Σ����򲻻�����������
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        StartCoroutine(Fade(0f));
        
    }

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();
        //��ʼ�������������ȴ�ת��Ч����ɲż�����һ�д���
        yield return StartCoroutine(Fade(1f));

        //�洢��ǰ��������
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //�����µ�playerλ��
        Player.Instance.gameObject.transform.position = spawnPosition;
        EventHandler.CallBeforeSceneUnloadEvent();

        //�첽ж�ص�ǰscene����������֮������gameobject
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //������scene���ȴ��������
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        EventHandler.CallAfterSceneLoadEvent();

        //�����³�������
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        yield return StartCoroutine(Fade(0f));
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator Fade(float finalAlpha) 
    {
        //���ڵ���״̬��ȷ��canvasgroup�ѱ����߼�⵽�������������벻�ٱ�����
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;
        //��õ����ٶ�
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        //�첽����scene�������ȼ��غ󼤻�ģ������ּ���ģʽ
        //single���ر��Ѿ����ص����г�����ֻ����һ���³�����managerĿ¼��
        //additive�������Ѽ��س������ּ���״̬��ͬʱ����һ���³���
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newLoadScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        //������ص��³���
        SceneManager.SetActiveScene(newLoadScene);
    }
}
