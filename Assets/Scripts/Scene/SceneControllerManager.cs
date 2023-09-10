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
        //开始淡出到黑屏，等待转场效果完成才继续下一行代码
        yield return StartCoroutine(Fade(1f));

        //存储当前场景数据
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //设置新的player位置
        Player.Instance.gameObject.transform.position = spawnPosition;
        EventHandler.CallBeforeSceneUnloadEvent();

        //异步卸载当前scene，并销毁与之关联的gameobject
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        //加载新scene，等待加载完成
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        EventHandler.CallAfterSceneLoadEvent();

        //加载新场景数据
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        yield return StartCoroutine(Fade(0f));
        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator Fade(float finalAlpha) 
    {
        //正在淡化状态，确认canvasgroup已被射线检测到，后续其他输入不再被处理
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;
        //获得淡化速度
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
        //异步加载scene，场景先加载后激活的，有两种加载模式
        //single，关闭已经加载的所有场景，只加载一个新场景到manager目录中
        //additive，其他已加载场景保持激活状态，同时加载一个新场景
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newLoadScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        //激活加载的新场景
        SceneManager.SetActiveScene(newLoadScene);
    }
}
