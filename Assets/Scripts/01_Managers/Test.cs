using System;
using UnityEngine;

namespace _01_Managers
{
    public class Test : MonoBehaviour
    {
        public C_Look viewer;
        public C__Character target;

        private void Start()
        {
            print(viewer.HasSightOn(target.tile));
        }
    }
}