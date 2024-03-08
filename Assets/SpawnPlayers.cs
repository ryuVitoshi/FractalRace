using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject _playerPrefab;
    //public GameObject _cameraPrefab;

    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void Start()
    {
        Vector2 randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        PhotonNetwork.Instantiate(_playerPrefab.name, randomPos, Quaternion.identity);
        //PhotonNetwork.Instantiate(_cameraPrefab.name, randomPos, Quaternion.identity);
        //_cameraPrefab.GetComponent<ThirdPersonCamera>().SetTarget(_playerPrefab);
    }

}
