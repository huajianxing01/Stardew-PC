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
            //定义了一个Dictionary的KeyValuePair结构体类型的列表，存储改变后的动画
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();
            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            //获取当前场景中匹配的动画控制器Animator，即对应的状态
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();
            foreach(Animator animator in animatorsArray)
            {
                if(animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }
            //animator是动画控制器(状态机)，animationclip是动画剪辑片段(存储基于关键帧的动画)，animation用于播放动画
            //初始化动画覆盖控制器AOC，不会重置状态机当前状态，通过AOC检索控制器使用的所有Clip
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);
            
            foreach(AnimationClip animationClip in animationsList)
            {
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);
                //判断当前clip是否在Dictionary1中
                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();
                    SO_AnimationType swapTMP;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapTMP);
                    //判断改变后的动画是否存在Dictionary2中，如果存在就改变
                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapTMP.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            //用新的列表覆盖AOC的旧列表，替换当前运行的animation为AOC
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }
    }
}
