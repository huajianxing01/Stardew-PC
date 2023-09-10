using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//�Զ��������������Ϊ����������ֶ����
[RequireComponent(typeof(SpriteRenderer))]

public class ObscurantItemFolder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    //���룬��ǳ����
    public void FadeIn()
    {
        //��ʼһ��Э��
        StartCoroutine(FadeInRoutine());
    }

    //�����������ǳ
    public void FadeOut()
    {
        //��ʼһ��Э��
        StartCoroutine(FadeOutRoutine());
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
            //�ݻ�һ֡����һ֡��ִ��
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetAlpha);
    }
}
