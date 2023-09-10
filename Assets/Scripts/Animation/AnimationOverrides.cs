using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] animationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    // Start is called before the first frame update
    void Start()
    {
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();
        foreach(SO_AnimationType item in animationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();
        foreach(SO_AnimationType item in animationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() +
                item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {
        foreach(CharacterAttribute characterAttribute in characterAttributesList)
        {
            Animator currentAnimator = null;
            //������һ��Dictionary��KeyValuePair�ṹ�����͵��б��洢�ı��Ķ���
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();
            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            //��ȡ��ǰ������ƥ��Ķ���������Animator������Ӧ��״̬
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();
            foreach(Animator animator in animatorsArray)
            {
                if(animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }
            //animator�Ƕ���������(״̬��)��animationclip�Ƕ�������Ƭ��(�洢���ڹؼ�֡�Ķ���)��animation���ڲ��Ŷ���
            //��ʼ���������ǿ�����AOC����������״̬����ǰ״̬��ͨ��AOC����������ʹ�õ�����Clip
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);
            
            foreach(AnimationClip animationClip in animationsList)
            {
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);
                //�жϵ�ǰclip�Ƿ���Dictionary1��
                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();
                    SO_AnimationType swapTMP;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapTMP);
                    //�жϸı��Ķ����Ƿ����Dictionary2�У�������ھ͸ı�
                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapTMP.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            //���µ��б���AOC�ľ��б��滻��ǰ���е�animationΪAOC
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
