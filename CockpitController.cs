using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CockpitController : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Transform cockpitTransform;
    private Vector3 initialRotation;
    private Vector3 initialPosition;
    private Quaternion initialRotation1;

    private Transform controllerTransform;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        cockpitTransform = this.transform;
        initialPosition = cockpitTransform.localPosition;
        initialRotation1 = cockpitTransform.localRotation;
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        controllerTransform = args.interactorObject.transform;
        initialRotation = cockpitTransform.localEulerAngles;

    }

    void OnRelease(SelectExitEventArgs args)
    {
        cockpitTransform.localPosition = initialPosition;
        cockpitTransform.localRotation = initialRotation1;
    }

  

}
