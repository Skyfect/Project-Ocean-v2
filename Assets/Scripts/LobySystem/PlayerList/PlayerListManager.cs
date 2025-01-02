using QFSW.QC;
using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class PlayerListManager : MonoBehaviour
{
    public static PlayerListManager instance;

    public List<PlayerListImage> myChilds;
    public GameObject ChildPrefab;
    public UnityEngine.UI.Image myImage;
    public UnityEngine.UI.Button myButton;
    public Transform Parent;

    private string[] GlobalID = new string[3];
    private string[] GlobalNick = new string[3];

    public bool enableMode = false;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        myImage = GetComponent<UnityEngine.UI.Image>();
    }

    public void CreateChild(int Count)
    {
        if(enableMode != true)
        {
            DestroyChild();
            myImage.enabled = true;

            for (int i = 0; i < Count; i++)
            {
                GameObject child_ = Instantiate(ChildPrefab);
                child_.transform.SetParent(Parent);
                myChilds.Add(child_.GetComponent<PlayerListImage>());
            }

            GiveChildsInfo();
        }
        
    }

    public void DestroyChild()
    {
        foreach (PlayerListImage child in myChilds)
        {
            Destroy(child.gameObject);
        }
        myChilds.Clear();
    }

    [Command]
    private void GiveChildsInfo()
    {
        if(enableMode != true)
        {
            int i = 0;
            foreach (PlayerListImage child in myChilds)
            {
                child.myID = GlobalID[i];
                child.nicktext.text = GlobalNick[i];
                i++;
            }
        }
    }

    public void GetID_Nick(string[] id, string[] nick)
    {
        GlobalID = id;
        GlobalNick = nick;
    }

    public void SetActiveMe(bool updateMode)
    {
        if(updateMode == false)
        {
            DestroyChild();
            myImage.enabled = false;
            myButton.gameObject.SetActive(false);
            enableMode = true;
        }
        else
        {
            CreateChild(LobyManager.Instance.PlayerCountInLobby());
            myImage.enabled = true;
            myButton.gameObject.SetActive(true);
            enableMode = false;
        }
    }

    public int ReturnChildCount()
    {
        if (myChilds.Count > 0) return myChilds.Count;

        else return 0;
    }

    public bool isAnyChildisNull()
    {
        foreach (PlayerListImage child in myChilds)
        {
            if (child.myID.IsNullOrEmpty() || child.nicktext.text.IsNullOrEmpty())
                return true;
        }
        return false;
    }
}
