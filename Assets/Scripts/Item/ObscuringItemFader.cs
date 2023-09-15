using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自动将所需的组件添加为依赖项，无需手动添加
[RequireComponent(typeof(SpriteRenderer))]

public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    //淡入，由浅变深
    public void FadeIn()
    {
        if (this.gameObject.activeSelf)
        {
            //开始一个协程，必须物体有效的，否则停止协程
            StartCoroutine(FadeInRoutine());
        }
        else
        {
            StopCoroutine(FadeInRoutine());
        }
        
    }

    //淡出，由深变浅
    public void FadeOut()
    {
        if(this.gameObject.activeSelf)
        {
            StartCoroutine(FadeOutRoutine());
        }
        else
        {
            StopCoroutine(FadeOutRoutine());
        }
        
    }

    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1f - currentAlpha;
        
        while(1f - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / Settings.fadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha =spriteRenderer.color.a;
        float distance = currentAlpha - Settings.targetAlpha;

        while(currentAlpha - distance > 0.01f)
        {
            currentAlpha = currentAlpha - distance / Settings.fadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            //暂缓一帧，下一帧再执行
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha);
    }
}
