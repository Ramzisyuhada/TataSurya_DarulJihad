using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static Cinemachine.CinemachineTargetGroup;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;

public class GoToPlanetController : MonoBehaviour
{
    [Header("Ayat Qur an")]
    [SerializeField] Texture2D[] Ayat;
    [SerializeField] GameObject Quran;
    [SerializeField] GameObject Artinya;

    [Header("GUI")]
    [SerializeField] Transform[] Button;
    [SerializeField] GameObject canvas_daftarplanet;
    [SerializeField] GameObject CountDowncanvas;
    [SerializeField] GameObject Canvas_tentang;
    [SerializeField] GameObject Canvas_Alquran;
    [SerializeField] Texture2D[] NumberCountDown;
    private GameObject Image;

    [Header("Planet")]
    [SerializeField] Transform[] PlanentPosition;


    [Header("Rigid Body")]
    [SerializeField] Rigidbody Model;



    [Header("Audio")]
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip[] source;



    [Header("Button")]
    [SerializeField] Transform Button_pysic;
    [SerializeField] Transform Button_pysic_UI;
    [SerializeField] Transform Button_pysic_Memutar;
    [SerializeField] Transform Button_pysic_Orbit;
    [SerializeField] Transform Button_Pusic_Ayat;

    [Header("Stear")]
    [SerializeField] Transform LeftSteer;
    [SerializeField] Transform RightSteer;
    [SerializeField] Transform Gas;


    [Header("UI Tentang")]
    [SerializeField] Sprite[] Tentang;
    Transform planet;
   


    public GameObject cube;
    CameraShake shake;
     enum Kondisi{
        Sampai ,
        Perjalanan,
        EngineStart,
        Menghadap,
        Countdown,
        Default,
        Memutar

      }
    Kondisi kondisi = Kondisi.Default;

    private float forwardForce = 30f;
    private float speed = 50f;
    private float targetSpeed;
    private float smoothSpeed = 0.5f;
    private float hTurn = 0f;
    public float TurnForce = 180f;

    private bool menghadap = false;
    private bool sampai = false;
    private bool enginestart = false;
    private bool menghitung = false;
    private bool berjalan = false;
    private Vector3 initialposition ;
    float jarak;
    private float slowingDistance = 200f; // Distance to start slowing down
    float time = 5f;
    float speed_kapal = 0f;
    private Vector3 initialpositionR;


    void FindAyat()
    {
            int i = 0;
        Texture2D obj;
            String[] Guilds = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Image/Ayat-Quran" });
          Ayat = new Texture2D[Guilds.Length]; 

        foreach (string guild in Guilds)
        {
                String path = AssetDatabase.GUIDToAssetPath(guild);
                obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                Ayat[i] = obj;
                i++;
            

        }
        
    }

   /// <summary>
   /// Tampil UI Sesuai Button
   /// </summary>
   /// 

    public void TampilAyat(int index)
    {
        Quran.GetComponent<RawImage>().texture = Ayat[index].GetComponent<Texture2D>();
    //    Ayat[index];
    }
    void Start()
    {
        ResetUI();
        FindAyat();
        initialposition = LeftSteer.localPosition;
        initialpositionR = RightSteer.localPosition;
        shake = GetComponent<CameraShake>();
        initialDirection = RightSteer.position - LeftSteer.position;

        menghitung = true;
        Image = CountDowncanvas.transform.GetChild(0).gameObject;
        Dictionary<XRBaseInteractable, int> buttonIndices = new Dictionary<XRBaseInteractable, int>();
        InitButton();

        canvas_daftarplanet.SetActive(false);
        for (int i = 0; i < Button.Length; i++)
        {

            int index = i;
            Debug.Log(i);
            Button[i].GetComponent<Button>().onClick.AddListener(() => ClickButton(index));
        }
    }

    private void InitButton()
    {
        XRBaseInteractable interactable = Button_pysic.GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(PushButton);
        XRBaseInteractable interactable1 = Button_pysic_UI.GetComponent<XRBaseInteractable>();
        interactable1.hoverEntered.AddListener(PushButton1);

        XRBaseInteractable interactable2 = Button_pysic_Memutar.GetComponent<XRBaseInteractable>();
        interactable2.hoverEntered.AddListener(PushButton2);
        
        XRBaseInteractable interactable3 = Button_pysic_Orbit.GetComponent<XRBaseInteractable>();
        interactable3.hoverEntered.AddListener(PushButton3);


        XRBaseInteractable interactable4 = Button_Pusic_Ayat.GetComponent<XRBaseInteractable>();
        interactable4.hoverEntered.AddListener(PushButton4);
    }
    /// <summary>
    /// Menampilkan UI Ayat Quran
    /// </summary>
    /// 
    
    private void PushButton4(BaseInteractionEventArgs args)
    {
        ResetUI();
        Canvas_Alquran.SetActive(true);
    }
    int index_tentang;
    public void ShowOrbitPlanet()
    {
        GameObject[] orbit = GameObject.FindGameObjectsWithTag("AstronomicalBody");
        Debug.Log(PlanentPosition[index_tentang].name);
        foreach (Transform o in PlanentPosition)
        {
            if (o.GetComponent<FollowOrbit>() != null)
            {
                if (o.GetComponent<FollowOrbit>().orbitToFollow.gameObject.GetComponent<LineRenderer>().enabled == true && PlanentPosition[index_tentang].name == o.name)
                {

                    PlanentPosition[index_tentang].GetComponent<FollowOrbit>().orbitToFollow.gameObject.GetComponent<LineRenderer>().enabled = false;
                    break;
                }
                if (o.GetComponent<FollowOrbit>().orbitToFollow.gameObject.GetComponent<LineRenderer>().enabled == false && PlanentPosition[index_tentang].name == o.name)
                {

                    PlanentPosition[index_tentang].GetComponent<FollowOrbit>().orbitToFollow.gameObject.GetComponent<LineRenderer>().enabled = true;
                    break;
                }
            }
        }

       // if (orbit[index_tentang].GetComponent<LineRenderer>().enabled == true && PlanentPosition[index_tentang+1].GetComponent<FollowOrbit>().orbitToFollow.name == orbit[index_tentang].name) orbit[index_tentang].GetComponent<LineRenderer>().enabled = false;
      //  else orbit[index_tentang].GetComponent<LineRenderer>().enabled = true;

    }
    private void PushButton3(BaseInteractionEventArgs args)
    {
        GameObject[] orbit = GameObject.FindGameObjectsWithTag("Orbit");
        foreach (GameObject o in orbit) { 
            if(o.GetComponent<LineRenderer>().enabled == true)o.GetComponent<LineRenderer>().enabled = false;
            else o.GetComponent<LineRenderer>().enabled = true;
        }
    }
    private void ClickButton(int index)
    {
        index_gambar = 0;
        planet = PlanentPosition[index];
        index_tentang = index;
        enginestart = true;
        sampai = false;
        kondisi = Kondisi.EngineStart;
        if (planet.GetComponent<FollowOrbit>() != null) planet.GetComponent<FollowOrbit>().orbitSpeed = 0f;
        audio.clip = source[1];
        audio.Play();
        
        shake.shakeDuration = 60f;
        
        canvas_daftarplanet.SetActive(false);
    }

    private void PushButton2(BaseInteractionEventArgs args)
    {
        if (kondisi == Kondisi.Default && jarak < 50f) {

            kondisi = Kondisi.Memutar;
            Canvas_tentang.SetActive(false);

            canvas_daftarplanet.SetActive(false) ;
        }


    }
    /// <summary>
    ///  Ketika Push Button Tentang akan muncul dan canvas lain hide
    /// </summary>
    /// <param name="args"></param>
    private void PushButton1(BaseInteractionEventArgs args)
    {
        if (kondisi == Kondisi.Default && sampai)
        {
            ResetUI();  
            Canvas_tentang.SetActive(true);
            Canvas_tentang.transform.GetChild(2).GetComponent<Image>().sprite = Tentang[index_tentang];
        }
    }

    private void ResetUI()
    {

        Canvas_Alquran.SetActive(false);
        // setactive false UI planets
        canvas_daftarplanet.SetActive(false);
        // setactive false UI deskripsi
        Canvas_tentang.SetActive(false);
        // setactive false UI alquran
        Canvas_Alquran.SetActive(false);
    }

    /// <summary>
    /// fungsi untuk interaksi push button ke canvas daftar planet
    /// </summary>
    /// <param name="args"></param>
    public void PushButton(BaseInteractionEventArgs args)
    {
        ResetUI();
        canvas_daftarplanet.SetActive(true);
    }
    public float rotationSpeed = 10f;

    // Arah dasar saat steer tidak bergerak
    private Vector3 initialDirection;
    private float vertikal = 0f;
    private float horizontal = 0f;

    public float dragCoefficient = 0.02f;
    private void FixedUpdate()
    {
       // vertikal = Input.GetAxis("Vertical");
       
        Vector3 push = LeftSteer.localPosition ;
        vertikal = Math.Clamp(push.z , -1, 1);
        Vector3 kanankiri = RightSteer.localPosition;
        horizontal = Math.Clamp(kanankiri.x, -1, 1);
        // horizontal = Math.Clamp(push.x * 2f, -1, 1);
        Debug.Log("Nilai VERTIKAL : " + vertikal * 2f);
        if (initialposition == LeftSteer.localPosition)
        {
            vertikal = 0f;
           // horizontal = 0f;
        }
        if(initialpositionR == RightSteer.localPosition)
        {
            horizontal = 0f;
        }
        if (vertikal != 0f) Model.AddRelativeForce(Vector3.forward * 30f * Model.mass * vertikal * 0.5f);

        Model.rotation = Quaternion.Euler(0, Model.rotation.eulerAngles.y, 0f);
      

        // Stabilize rotation (reset minor unwanted roll/pitch)
       // Model.rotation = Quaternion.Euler(Model.rotation.eulerAngles.x, Model.rotation.eulerAngles.y, 0);

        

    }



    //  Debug.Log(thrustInput); 





void Update()
    {
        Vector3 kanankiri = RightSteer.localPosition;
        kanankiri = transform.InverseTransformPoint(kanankiri);
        horizontal = Math.Clamp(kanankiri.x * 5f, -1, 1);
        if (initialpositionR == RightSteer.localPosition)
        {
            horizontal = 0f;
        }
        Debug.Log(horizontal);
        float amount = 10f * horizontal * Time.deltaTime;
        transform.Rotate(0,amount,0);
        // Biasanya tombol W/S atau panah atas/bawah
        /*  if (initialposition == LeftSteer.localPosition)
          {
              vertikal = 0f;
              horizontal = 0f;
          }
          Vector3 push = LeftSteer.localPosition - RightSteer.localPosition;
          vertikal = Math.Clamp(push.z * 2f, -1, 1); 
          transform.Translate(Vector3.forward * vertikal * 2f );*/
        /*if (Gas.eulerAngles.x >= 300)
        {
            speed_kapal = Mathf.Lerp(speed_kapal,50f , Time.deltaTime * 0.5f);
            Debug.Log(speed_kapal);
            transform.Translate(Vector3.forward * Time.deltaTime * speed_kapal);
        }*/
        if (kondisi == Kondisi.Default) return;



        if (kondisi == Kondisi.EngineStart || kondisi == Kondisi.Menghadap)
        {
            if (kondisi != Kondisi.Menghadap)
            {
                float angleplayer = LookRotation();
                if (angleplayer < 1.0f)
                {
                    CountDowncanvas.SetActive(true);
                    kondisi = Kondisi.Menghadap;
                    menghadap = true;
                    menghitung = true;
                }
                return;
            }

            if (kondisi == Kondisi.Menghadap)
            {
                CountDown();
                kondisi = Kondisi.Countdown; 
            }
          
           return ;
        }

        if (kondisi == Kondisi.Countdown && index_gambar >= 5)
        {
            shake.Perjalanan = true;

            Berjalan();
        }else if(kondisi == Kondisi.Memutar)
        {
            if(planet != null)
            Memutari_Planet();
        }
        
  
    }
    private float currentRotationY = 0f; 

    int index_gambar = 0;
    float totalrotasi = 0f;
    Vector3 asalposisi;


    private void Memutari_Planet()
    {
        float slowDownFactor = Mathf.Clamp01(totalrotasi / 360f); 
        float speed = Mathf.Lerp(50f, 5f, slowDownFactor * 1.3f); 

        Vector3 asalposisi = transform.position;

        transform.RotateAround(planet.position, Vector3.up, speed * Time.deltaTime);

        Vector3 forwardDirection = transform.forward;
        transform.rotation = Quaternion.LookRotation(forwardDirection, Vector3.up);

        float angleThisFrame = Vector3.Angle(asalposisi - planet.position, transform.position - planet.position);

        totalrotasi += angleThisFrame;

        if (totalrotasi >= 360f)
        {
            totalrotasi = 0f;
            kondisi = Kondisi.Default;
        }
    }

    private void Berjalan()
    {
        CountDowncanvas.SetActive(false);

        sampai = DistanceTarget();
        float speed = Mathf.Lerp(50f, 0f, Mathf.Clamp01((slowingDistance - jarak) / slowingDistance) * 1.3f);
        shake.shakeAmount  = Mathf.Lerp(-0.11f, 0f, Mathf.Clamp01((slowingDistance - jarak) / slowingDistance) * 1.3f);

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
    private void CountDown()
    {
        if (!menghitung) return; 

        StartCoroutine(DisplayCountdown());
        
    }
    IEnumerator DisplayCountdown()
    {
        while (index_gambar < NumberCountDown.Length)
        {
            Image.GetComponent<RawImage>().texture = NumberCountDown[index_gambar];
            index_gambar++;
            yield return new WaitForSeconds(1);
        }



    }

    public bool DistanceTarget()
    {
         jarak = Vector3.Distance(transform.position, planet.position);
        if (jarak < 50f)
        {
            shake.Perjalanan = false;
            sampai = true;
            menghadap = false;
            enginestart = false;
            kondisi = Kondisi.Default;

            return true;
        }

        targetSpeed = 50f;

        return false;
    }

    private void Maju()
    {

    }
    public float LookRotation()
    {
        Quaternion targetrotation = Quaternion.LookRotation(planet.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, Time.deltaTime * 0.5f);
        float angleDifference = Quaternion.Angle(transform.rotation, targetrotation);

        return angleDifference;
    }

    public void Close_Planet()
    {
        ResetUI();
        canvas_daftarplanet.SetActive(false);
    }

    public void Close_Tentang()
    {
        ResetUI();
        Canvas_tentang.SetActive(false);
       
    }
}
