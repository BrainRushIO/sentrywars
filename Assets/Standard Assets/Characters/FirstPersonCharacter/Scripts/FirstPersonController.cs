using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {

        [SerializeField] private MouseLook m_MouseLook;
         // the sound played when character touches back on ground.

        private Camera m_Camera;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;

        // Use this for initialization
        private void Start()
        {
            m_Camera = Camera.main;

			m_MouseLook.Init(transform , m_Camera.transform);
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();
        }
        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }
    }
}
