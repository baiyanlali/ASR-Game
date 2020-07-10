using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.Text;
public class ASR : MonoBehaviour
{
    [DllImport("asr")]
    private static extern void createSLEngine(int rate, int framesPerBuf, int nChannels);
    [DllImport("asr")]
    private static extern void deleteSLEngine();
    [DllImport("asr")]
    private static extern bool createSLBufferQueueAudioPlayer();
    [DllImport("asr")]
    private static extern void deleteSLBufferQueueAudioPlayer();
    [DllImport("asr")]
    private static extern bool createAudioRecorder(string graph_dir_path, int factor, float min_trailing_silence);
    [DllImport("asr")]
    private static extern void deleteAudioRecorder();
    [DllImport("asr")]
    public static extern void startPlay();
    [DllImport("asr")]
    public static extern void catchPlay(int game_flag, StringBuilder result);
    [DllImport("asr")]
    public static extern void stopPlay(int gameflag, StringBuilder result);
    [DllImport("asr")]
    public static extern string offlineDecode(string filename, int para);

    public static Text text;

    private static bool hasSetUp;
    public static bool HasSetUp => hasSetUp;



    private const int audioRecorderFactor = 1;
    private const float audioRecorderTrailingSilence = 2f;
    public static int slEngineSampleRate = 16000, slEngineBufferSize = 1600/*400*/, slEngineChannels = 1;
    //[DllImport("native")]
    //private static extern float add(float x, float y);


    /// <summary>
    /// set up the recorder
    /// </summary>
    /// <param name="lang">0 for Cantonese,1 for English</param>
    public static void recorderSetUp(int lang)
    {
        text = GameObject.Find("DebugText").GetComponent<Text>();
        if (hasSetUp) return;
        else hasSetUp = true;
        try
        {
            print("yes");
            createSLEngine(slEngineSampleRate, slEngineBufferSize, slEngineChannels);
            if (createSLBufferQueueAudioPlayer())
            {

                Debug.Log("CreateSLBufferQueuedAudioPlayer Success");
                text.text = "CreateSLBufferQueuedAudioPlayer Success";
            }
            else
            {
                text.text = "Problems";
                return;
            }
            try
            {
                createAudioRecorder($"{Application.persistentDataPath}/{configManager.instance.directories [lang]}", audioRecorderFactor, audioRecorderTrailingSilence);
                //createAudioRecorder(Application.persistentDataPath + "/models", audioRecorderFactor, audioRecorderTrailingSilence);
                text.text = "createAudioRecorder Success";
            }
            catch (System.Exception o)
            {
                text.text = "Problems:" + o.GetType().ToString() + "\n" + o.Message;
                throw;
            }
            //result.Capacity = asrResultBufferSize;
        }
        catch (System.Exception o)
        {
            text.text = "Problems:" + o.GetType().ToString() + "\n" + o.Message;
            throw;
        }

    }

    public static void recorderSetDown(int lang)
    {
        if (!hasSetUp) return;
        else hasSetUp = true;
        deleteSLBufferQueueAudioPlayer();
        deleteAudioRecorder();
    }

}
