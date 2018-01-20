using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkProjectileManager : MonoBehaviour {

    private static NetworkProjectileManager instance;
    public static NetworkProjectileManager Instance { get { return instance; } }

    private PhotonView photonView;
    private Dictionary<short, ProjectileBehaviour> projectileDict = new Dictionary<short, ProjectileBehaviour>();

    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void addProjectile(short id, ProjectileBehaviour behaviour)
    {
        if(!projectileDict.ContainsKey(id))
        {
            projectileDict.Add(id, behaviour);
        }
    }

    public void removeProjectile(short id, ProjectileBehaviour behaviour)
    {
        if(!projectileDict.ContainsKey(id))
        {
            projectileDict.Remove(id);
        }
    }

    public void killProjectile(short projectileId)
    {
        if(!PhotonNetwork.offlineMode)
        {
            photonView.RPC("RPC_KillProjectile", PhotonTargets.Others, (short)projectileId);
        }
    }

    [PunRPC]
    private void RPC_KillProjectile(short id)
    {
        if(projectileDict.ContainsKey(id))
        {
            ProjectileBehaviour behaviour = projectileDict[id];
            if(behaviour != null && !behaviour.IsDead)
            {
                behaviour.Die();
                projectileDict.Remove(id);
            }
        }
    }
        
}
