using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonFollowVisual : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 localaxis;
    public float resetSpeed = 5f;
    public float followAngle = 45;
    private Vector3 initialLocalPosition;
    private Vector3 offset;
    private bool isfreeze;
     Transform pokeAttachTransform;
    XRBaseInteractable interactable;
    private bool isFolllowing = false;
    void Start()
    {
        initialLocalPosition = visualTarget.localPosition;
        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(Reset);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Reset(BaseInteractionEventArgs args)
    {
        if (args.interactorObject is XRPokeInteractor poke)
        {
            isFolllowing = false;
        }
    }
    public void Follow(BaseInteractionEventArgs args)
    {
        Debug.Log($"Interactor Object Type: {args.interactorObject.GetType()}"); // Debug log to check the type

        if (args.interactorObject is XRPokeInteractor poke)
        {
            isfreeze = false;
            Debug.Log("Hello world");

            XRPokeInteractor interactor  = poke.GetComponent<XRPokeInteractor>();
            isFolllowing = true;

            pokeAttachTransform = interactor.transform;

            offset = visualTarget.position - pokeAttachTransform.position;
            float PokeAngle = Vector3.Angle(offset, visualTarget.TransformDirection(localaxis));
            if (PokeAngle < followAngle)
            {
                isFolllowing = true;
                isfreeze = true;    
            }
        }
    }
    public void Freeze(BaseInteractionEventArgs args)
    {
        if(args.interactorObject is XRPokeInteractor)
        {
            isfreeze = true;  
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isfreeze) return;
   
        if (isFolllowing) {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocaTargetPosition = Vector3.Project(localTargetPosition, localaxis);    
            visualTarget.position = visualTarget.TransformPoint(constrainedLocaTargetPosition);

        }
     
    }
}
