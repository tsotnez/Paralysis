using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkProjectileManager : MonoBehaviour {

    private static NetworkProjectileManager instance;
    public static NetworkProjectileManager Instance { get { return instance; } }

    private PhotonView photonView;
    private Dictionary<int, ProjectileBehaviour> projectileDict = new Dictionary<int, ProjectileBehaviour>();

    private void Awake()
    {
        instance = this;
        photonView = GetComponent<PhotonView>();
    }

    public void addProjectile(int id, ProjectileBehaviour behaviour)
    {
        if(!projectileDict.ContainsKey(id))
        {
            projectileDict.Add(id, behaviour);
        }
    }

    public void removeProjectile(int id, ProjectileBehaviour behaviour)
    {
        if(!projectileDict.ContainsKey(id))
        {
            projectileDict.Remove(id);
        }
    }

    public void killProjectile(int projectileId)
    {
        if(!PhotonNetwork.offlineMode)
        {
            photonView.RPC("RPC_KillProjectile", PhotonTargets.Others, projectileId);
        }
    }

    [PunRPC]
    private void RPC_KillProjectile(int id)
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
