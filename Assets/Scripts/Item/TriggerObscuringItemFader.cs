using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObscuringItemFader[] obscurantItemFolders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if (obscurantItemFolders.Length > 0)
        {
            for(int i = 0;i< obscurantItemFolders.Length;i++)
            {
                obscurantItemFolders[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ObscuringItemFader[] obscurantItemFolders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if (obscurantItemFolders.Length > 0)
        {
            for (int i = 0;i< obscurantItemFolders.Length; i++)
            {
                obscurantItemFolders[i].FadeIn();
            }
        }
    }
}
