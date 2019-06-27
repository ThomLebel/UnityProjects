using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class BasicGameLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		NetworkManager.Instance.InstantiatePlayerCube();
    }
}
