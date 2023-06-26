using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public Animator anim;
    public XML_Loader loader;

    private Queue<string> sentences;
    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        anim.SetBool("isOpen", true);
        nameText.text = dialogue.name;

        sentences.Clear();

        /*foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }*/
        foreach (string sentence in loader.xmlsentences)
        {
            sentences.Enqueue(sentence);
        }
        Debug.Log(sentences);

        DisplayNextSentence();

    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        //StopAllCoroutines();
        //StartCoroutine(TypeSentence(sentence));
    }

    /*IEnumerator TypeSentence(string sentence)
    {
        float waittime = 0.1f;
        char[] temp;
        dialogueText.text = "";
        foreach (char letter in sentence)
        {

            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(waittime);
        }
    }*/

    void EndDialogue()
    {
        anim.SetBool("isOpen", false);
        Debug.Log("End of conversation");
    }
}
