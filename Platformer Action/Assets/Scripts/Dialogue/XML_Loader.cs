using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XML_Loader : MonoBehaviour
{
    private TextAsset xmlAsset;
    public string[] xmlsentences;
    // Start is called before the first frame update
    void Start()
    {
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //<NPCTALK>Like everyone else in this &lt;b&gt;cursed land&lt;/b&gt;.</NPCTALK>
    public void StartDialogue()
    {
        xmlAsset = (TextAsset)Resources.Load("test");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlAsset.text);
        XmlNode dialogsRoot = doc.FirstChild.NextSibling.ChildNodes.Item(0);
        for (int i = 0; i < dialogsRoot.ChildNodes.Count; i++)
        {
            Debug.Log(i);
            Debug.Log(dialogsRoot.ChildNodes[i].InnerText);
            xmlsentences[i] = dialogsRoot.ChildNodes[i].InnerText;
        }
    }
}
