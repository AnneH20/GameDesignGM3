using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   private Vector3 _offset;
   [SerializeField] private Transform target;
   [SerializeField] private float smoothingTime;
   [SerializeField] private Vector3 _veolicty = Vector3.zero;

   private void Awake() {
      transform.position = transform.position + target.position;
      _offset = transform.position - target.position;
    
   }

   private void LateUpdate() {
    Vector3 targetPos = target.position + _offset;
    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _veolicty, smoothingTime);
   }
    
}
