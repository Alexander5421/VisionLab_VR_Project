using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Stimulus : MonoBehaviour
{
    [SerializeField]
    private GameObject left;
    [SerializeField]
    private GameObject right;

    

    public void SetSphere(bool isRight)
    {
        right.SetActive(isRight);
        left.SetActive(!isRight);
    }
}