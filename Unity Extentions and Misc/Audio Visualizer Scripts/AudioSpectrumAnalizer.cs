using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioSpectrumAnalizer : MonoBehaviour
{
    [System.Serializable]
    public struct Colors
    {
        public Color topColor;//top
       
        public Color c2;
        public Color c3;
        public Color bottomColor;//bottom
        

    }

    public Colors DeanasColors;
    public enum SampleRates
    {
        Hz64 = 64,
        Hz128 = 128,
        Hz256 = 256,
        Hz512 = 512,
        Hz1024 = 1024
    }

    public enum GainTypes { Normal, Square, Exp, InverseExp, LogX, Log10, LogEInverse }

    
    public enum Shapes
    {
        HalfCircle = 0,
        Circle = 1,
        Wall = 2,
        SlowWall = 3,
        Torus = 4,
    }

    public enum PillarType
    {
        Cylinder = 0,
        LightBeam = 1,
        Box = 2,
        RockPillar =3
    }
    public FFTWindow FffWindowType;
    public GainTypes gainType;
    public PillarType pillarType;
    public Shapes shape;
    public static Shapes Shape;
    public static bool isBuilding;

    public List<GameObject> pillars;
    private int currentLength; //store wall length
    //Check sliders moved
    private int currentPillars; //store how many pillars

   
    public GameObject CylinderPf;
    public GameObject BoxPf;
    public GameObject LightBeamPf;
    public GameObject RockpillarPf;
    public List<ParticleSystem> partSyslist;
    public GameObject partSysPf;
    public float partSysHeightMulti = 1f;
    public Vector3 partSysOffset;
    public float radius;

    [Space, Header("Gets audio from AudioListener.GetSpectrumData (sr,0,FFFWindowType) ")]
    public SampleRates sampleRate = SampleRates.Hz128; //power of 2

    public bool evenSamplesOnly;
   
    private float[] spectrum; //Spectrum streamed data from audSource or listener
    public TorusCreator torus;
    //UI Dynamic Sliders
    public float PillarAmount { get; set; }
    public float WallLength { get; set; }
    public float Sensitivity { get; set; }
    public float PillarSmooth { get; set; }
    public float pillarEventThreshold = 30;
  

    private void CreateParticles()
    {
        
        foreach (var part in partSyslist)
        {
            DestroyObject(part.gameObject);
        }
        partSyslist.Clear();
        partSyslist = new List<ParticleSystem>();
        foreach (var pillar in pillars)
        {
            GameObject ps = (GameObject) Instantiate(partSysPf);
            partSyslist.Add(ps.GetComponent<ParticleSystem>());
            ps.transform.position = pillar.transform.position;
        }
    }

    private void Start()
    { 
        //Set some default values
        WallLength = 40;
        Sensitivity = 1000;
        PillarAmount = 32;
        PillarSmooth = 80;
        //Rebuild();
    }
    private void CreatePillarsinCircle()
    {
        pillars = MathB.CircleOfGameObjects(CurrentPrefabType(),radius,(int)PillarAmount,true);//new Method
        SetPillarsParent();
        isBuilding = false;
    }
    private void CreatePillarsinHalfCircle()
    {
        pillars = MathB.CircleOfGameObjects(CurrentPrefabType(), radius, (int)PillarAmount, false);//new Method
        SetPillarsParent();
        isBuilding = false;
    }
    private void SetPillarsParent()
    {
        foreach (GameObject pillar in pillars)
        {
            pillar.transform.SetParent(transform, false);
        }
    }
    private GameObject CurrentPrefabType()
    {
        
        if (pillarType == PillarType.Cylinder) return  CylinderPf;
        if (pillarType == PillarType.Box) return BoxPf;
        if (pillarType == PillarType.LightBeam) return LightBeamPf;
        if (pillarType == PillarType.RockPillar) return RockpillarPf;
        return null;
    }
    private void CreatePillarWall()
    {
        for (int i = 0; i < PillarAmount; i++)
        {
            
            float wallPos = -WallLength/2 + i*WallLength/PillarAmount;
            var pos = new Vector3(wallPos, 0, radius);
            //Instantiate
            var block = (GameObject) Instantiate(CurrentPrefabType(), pos, Quaternion.identity);
            block.transform.SetParent(transform, false);
            pillars.Add(block);
        }

        isBuilding = false;
    }
    /// <summary>
    /// /// this i for the UI buttons to swith shape types
    /// </summary>
    /// <param name="i">ShapeType as 0-4 int</param>
    public void StartBuilding(int i)
    {
        if (isBuilding) return;

        isBuilding = true;

        if (i == 0)
        {
            Shape = Shapes.HalfCircle;
        }
        if (i == 1)
        {
            Shape = Shapes.Circle;
        }
        if (i == 2)
        {
            Shape = Shapes.Wall;
        }
        if (i == 4)
        {
            Shape = Shapes.Torus;
        }

        CreateNewSpectrum();
    }
    private void CreateNewSpectrum()
    {
        DestroySpectrum();//destroy old one first

        if (Shape == Shapes.Circle) CreatePillarsinCircle();
        if (Shape == Shapes.HalfCircle) CreatePillarsinHalfCircle();
        if (Shape == Shapes.Wall) CreatePillarWall();
        if (Shape == Shapes.Torus)torus.StartTorus();//Make Torus

        isBuilding = false;
    }
    private void DestroySpectrum()
    {
        foreach (var go in pillars)
        {
            Destroy(go);
        }
        pillars.Clear();
        
    }
    public void Rebuild()
    {
        isBuilding = true;

       CreateNewSpectrum();
        CreateParticles();
    }
    private float GetGain(float data)
    {
        //Normal is the only viable option really, the rest is for fun /experimentb
        float gain=0;
        if (gainType == GainTypes.Normal)       gain = data*Sensitivity;
        if (gainType == GainTypes.Square)       gain = data*data*Sensitivity;//small number gets small wont work properly
        if (gainType == GainTypes.LogX)         gain = 1/Mathf.Log(data*Sensitivity);
        if (gainType == GainTypes.Log10)        gain = 1/Mathf.Log10(data* Sensitivity) ;
        if (gainType == GainTypes.LogEInverse)  gain = -1/Mathf.Log(data* Sensitivity) ;//this is not bad
        if (gainType == GainTypes.Exp)          gain = Mathf.Exp(data) * Sensitivity;//terrible
        if (gainType == GainTypes.InverseExp)   gain = 1/Mathf.Exp(data) * Sensitivity;
        return gain;
    }

   
    private void PillarListener(GameObject pillar, int i,Vector3 scale)
    {
        var position = pillar.transform.position + (pillar.transform.up*scale.y * RmsToScale.RMSvalue * partSysHeightMulti);
        position += partSysOffset;
        partSyslist[i].transform.position = position;
        partSyslist[i].startSize = (1* RmsToScale.RMSvalue);
        
        if (position.y>30) { partSyslist[i].startColor = DeanasColors.topColor;}
        else if (position.y > 20) partSyslist[i].startColor = DeanasColors.c2;
        else if (position.y > 10) partSyslist[i].startColor = DeanasColors.c3;
        else if (position.y > 5) partSyslist[i].startColor = DeanasColors.bottomColor;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) Rebuild();
        
        if (isBuilding) return;
        if (partSyslist==null || partSyslist.Count != pillars.Count)
        {
            CreateParticles();
        }
        int s_rate = (int) sampleRate;//1024/512/256/128
        //Reads the final master output
        spectrum = AudioListener.GetSpectrumData(s_rate, 0, FffWindowType); //say 512 srate
        //Scale Y only
        int n = 0;
        for (int i = 0; i < pillars.Count; ++i)//needs to be <= s_rate or error or 2 times s_rate in evenSamplesOnly
        {
            //gain type
            float gain = GetGain(spectrum[i]);
            
            //scale pillar --if i want to add  move enum instead of scale do it here
            var previousScale = pillars[i].transform.localScale;
            previousScale.y = Mathf.Lerp(previousScale.y, gain , PillarSmooth*.001f);
            pillars[i].transform.localScale = previousScale;
            if (previousScale.y > pillarEventThreshold) PillarListener(pillars[i],i,previousScale);
            
            
            
        }

        //If slider changed rebuild
        int pillarsamt = Mathf.RoundToInt(PillarAmount);
        if (pillarsamt != currentPillars)
        {
            currentPillars = pillarsamt;
            Rebuild();
        }
        if ((int) Shape == 2) //Only on walls do we adjust length
        {
            //Length update on slider
            int length = Mathf.RoundToInt(WallLength);
            if (length != currentLength)
            {
                currentLength = length;
                Rebuild();
            }
        }
    }
}