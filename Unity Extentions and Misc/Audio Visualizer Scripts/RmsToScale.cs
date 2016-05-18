using NAudio.Gui;
using UnityEngine;

public class RmsToScale : MonoBehaviour
{    
    public static float RMSvalue;//Other scripts can get the current audio rms averaged levels
    [Space, Header("This script gets/returns the Averaged Audio Data from OnAudioFilterRead()")]
    public int sensitivity = 1;//10 works nice

    [Range(1f, 10f)] public float gain=1;  
   
    [System.Serializable]
    public struct Thresholds
    {
        [Header("Set Audio Thresholds")]
        [Range(.1f, 1f)] public float low;
        [Range(.1f, 1f)] public float mid;
        [Range(.1f, 1f)] public float high;


        public bool lowOn;
        public bool midOn;
        public bool highOn;

        public bool IsHitting(float value,float threshold)
        {
            bool b = value > threshold;
            return b;
        }
    }

    public Thresholds thresholds;    
    [Header("Current Actual Values")]
    public float lowSum;
    public float midSum;
    public float highSum;
    public float m_Rms;
   
    //for ranges
    [Header("Current Smoothed Values")]
    public float low;
    public float mid;
    public float high;
    public float m_SmoothRms;

    [Range(50, 500)] public float RmsSmoothSpeed = 100f;
    [Range(2, 1000)] public float rmsCalibrateConstant=1;//test only

    [Range(-1f, 1f)]
    public float avgleft;
    [Range(-1f, 1f)]
    public float avgright;
    private void Update()
    {
        float speed = RmsSmoothSpeed*.01f*Time.deltaTime;
        //smooth rms value
        RMSvalue = Mathf.Lerp(RMSvalue, m_Rms*sensitivity, speed);
        m_SmoothRms = RMSvalue*rmsCalibrateConstant;
        //smooth range averages
        low = Mathf.Lerp(low, lowSum * sensitivity, speed);
        mid = Mathf.Lerp(mid, midSum * sensitivity, speed);
        high = Mathf.Lerp(high, highSum * sensitivity, speed);


        thresholds.lowOn = thresholds.IsHitting(low, thresholds.low);
        thresholds.midOn = thresholds.IsHitting(mid, thresholds.mid);
        thresholds.highOn = thresholds.IsHitting(high, thresholds.high);

        avgleft = Mathf.Lerp(avgleft, avgl, speed);
        avgright = Mathf.Lerp(avgright, avgr, speed);
    }

    //runs in seperate thread
    private int m_sampleRate=1;//more than 1 lowers quality
    [Range(-1f, 1f)] public float avgl;
    [Range(-1f, 1f)] public float avgr;
    private void OnAudioFilterRead(float[] data, int channels)//-1 to 1 data
    {
        float squareSum = 0;
        float sampleCount = 0;
        float lowThresholdSum = 0;
        float midThresholdSum = 0;
        float highThresholdSum = 0;

        int dataSize = data.Length;
        

        for (int i = 0; i < dataSize - m_sampleRate*channels; i += m_sampleRate*channels)
        {
            avgl = 0.0f;
            avgr = 0.0f;

            for (int j = 0; j < m_sampleRate*channels; j += channels)
            {
                avgl += data[i + j];
                if (channels > 1)
				{
                    avgr += data[i + j + 1];
                }
            }
        }

        //get and store data
        for (int i = 0; i < dataSize; i += channels)
        {
            float level = data[i]*data[i];//we offset from -1,1 to 1,2
            level *= gain;
            squareSum += level ;// 
            
            //total sumation of all example data = 9
            if (i < 1 * dataSize / 3) lowThresholdSum += level;//0-3
            else if (i < 2 * dataSize / 3 ) midThresholdSum += level ;//3-6
            else if (i < 3 * dataSize / 3) highThresholdSum += level ;//6-9
        }

        sampleCount += data.Length/channels;

        //go do math with data now
        if (sampleCount > 0)
        {
            //do range averages
            if (lowThresholdSum > 0) lowSum = Mathf.Sqrt(lowThresholdSum / sampleCount / 3);
            if (midThresholdSum > 0) midSum = Mathf.Sqrt(midThresholdSum / sampleCount / 3);
            if (highThresholdSum > 0) highSum = Mathf.Sqrt(highThresholdSum / sampleCount / 3);

            //do rms average
            m_Rms = Mathf.Sqrt(squareSum / sampleCount);//all the samples averaged together
           
        }
        
    }
}