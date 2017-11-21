using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProfileController : MonoBehaviour {

    /// <summary>
    /// Start this instance.
    /// </summary>




    class UsersPlayer
    {
        private string namePlayer;
        private int userID;
        private int wins;
        private int loses;

    };


    private UsersPlayer userPLayer;
    public GameObject emptyList;
    public Text player1;
    public Text Connect;




	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		



	}


    public void NameUser(string name)
    {
        
        this.player1.text = name;

    }


    public void Connection(bool connected)
    {

        if (connected)
            this.Connect.text = "Connected";

        else
            this.Connect.text = "No connected";
        
    }

    public void CopyPlayer(){
        this.emptyList.GetPhotonViewsInChildren();


    }





}
