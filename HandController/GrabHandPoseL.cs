using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
public class GrabHandPoseL : MonoBehaviour
{
    public GameObject LeftHandPhysic;
    public InputActionProperty primary;
    public HandData leftHandPose;

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;

    private Quaternion[] startingFingerRotation;
    private Quaternion[] finalFingerRotation;


    [Header("Fire")]
    [SerializeField] Transform Firepoint_right;
    [SerializeField] Transform prefabs;

    [Header("Sound Effect")]
    [SerializeField] AudioSource shoot_effect;
    void TryInitialize()
    {

    }


    void Start()
    {


        XRKnob grabInteractable = GetComponent<XRKnob>();
        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnsetPoses);  
        
        leftHandPose.gameObject.SetActive(false);
    }
    public void Shoot()
    {

        GameObject bullet = ObjectPooling.instance.GetPooledObjectl();
        if (bullet != null)
        {
            bullet.transform.position = Firepoint_right.position; 
            bullet.transform.rotation = Firepoint_right.rotation; 
            bullet.SetActive(true);
            shoot_effect.Play();

        }

    }
    private bool memegang = false;
    public void SetupPose(BaseInteractionEventArgs args)
    {
        Debug.Log($"Interactor Object Type: {args.interactorObject.GetType()}"); // Debug log to check the type

        if (args.interactorObject is XRDirectInteractor rayInteractor)
        {
            Debug.Log("Ada");
            HandData handData = rayInteractor.transform.GetComponentInChildren<HandData>();
            if (handData != null)
            {

                memegang = true;    
                leftHandPose.transform.gameObject.SetActive(true);
                handData.gameObject.SetActive(true);
                handData.animator.enabled = false;
                LeftHandPhysic.SetActive(false);
              //  SetHandDataValue(handData, leftHandPose);
               // SetHandData(handData, finalHandPosition, finalHandRotation, finalFingerRotation);
              
            }
            else
            {
                Debug.LogError("SetupPose: HandData not found on the interactor.");
            }
        }
    }

    public void UnsetPoses(BaseInteractionEventArgs args)
    {
                Debug.Log($"Interactor Object Type: {args.interactorObject.GetType()}"); // Debug log to check the type
        memegang = false;

        if (args.interactorObject is XRDirectInteractor rayInteractor)
        {
            HandData handData = rayInteractor.GetComponentInChildren<HandData>();
            if (handData != null)
            {
                Debug.Log("UnsetPoses: HandData found, resetting pose."); // Debug log for reset
                handData.animator.enabled = true;
                LeftHandPhysic.SetActive(true);
               // SetHandData(handData, startingHandPosition, startingHandRotation, startingFingerRotation);

            }
        }
    }

    public void SetHandDataValue(HandData h1, HandData h2)
    {
        startingHandPosition = h1.root.localPosition;
        finalHandPosition = h2.root.localPosition;

        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;

        startingFingerRotation = new Quaternion[h1.FingerOne.Length];
        finalFingerRotation = new Quaternion[h2.FingerOne.Length]; 

        for (int i = 0; i < h1.FingerOne.Length; i++)
        {
            startingFingerRotation[i] = h1.FingerOne[i].localRotation;
            finalFingerRotation[i] = h2.FingerOne[i].localRotation;
        }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.FingerOne[i].localRotation = newBonesRotation[i];
        }
    }
    private void Update()
    {
        if (memegang)
        {
            if (primary.action.WasPressedThisFrame())
            {
                
                    Shoot();
                
               
            }
        }
    }
}
