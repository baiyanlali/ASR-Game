using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PCMConvertor :MonoBehaviour
{
    public static float [] PCM2Floats(byte [] bytes)
    {
        float max = -(float)System.Int16.MinValue;
        float [] samples = new float [bytes.Length / 2];

        for (int i = 0 ; i < samples.Length ; i++)
        {
            short int16sample = System.BitConverter.ToInt16(bytes, i * 2);
            samples [i] = (float)int16sample / max *10;
        }

        return samples;
    }
    public static float [] PCM2Floats(string path)
    {
        FileStream fs = File.Open(path, FileMode.Open);
        byte [] bytes=new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        //#region
        //int a = 0;
        //foreach (var item in bytes)
        //{
        //    a++;
        //    if (a>=1000)
        //    {
        //        Debug.Log(item);
        //        a = 0;
        //    }
        //}
        //#endregion
        fs.Close();
        
        float max = -(float)System.Int16.MinValue;
        float [] samples = new float [bytes.Length / 2];

        for (int i = 0 ; i < samples.Length ; i++)
        {
            short int16sample = System.BitConverter.ToInt16(bytes, i * 2);
            samples [i] = (float)int16sample/ max;
            //Debug.Log((float)int16sample / max);
        }

        return samples;
    }

    public static void PCM2WAV(string inPCMFilePath,string outWAVFilePath,int sampleRate,int channels,int bitNum)
    {
        FileStream fin=new FileStream(inPCMFilePath,FileMode.Open);
        FileStream fout=new FileStream(outWAVFilePath,FileMode.OpenOrCreate,FileAccess.ReadWrite);

        long byteRate = sampleRate * channels * bitNum / 8;

        //获取文件大小
        long totalAudioLen = fin.Length;
        //总文件大小
        long totalDataLen = totalAudioLen + 36;//36;
        writeWaveFileHeader(fout, totalAudioLen, totalDataLen, sampleRate, channels, byteRate);

        byte [] data = new byte [(int)totalAudioLen+44];

        fin.Read(data, 0, (int)totalAudioLen);

        fout.Write(data, 44, (int)totalAudioLen);
        
    }


    private static void writeWaveFileHeader(FileStream fout , long totalAudioLen, long totalDataLen, int sampleRate, int channels, long byteRate)
    {
        List<byte> byteList = new List<byte>();
        byte [] riff = Encoding.ASCII.GetBytes(new char [] {'R','I','F','F' });

        byte [] riffChunk = new byte [] {(byte)(totalDataLen & 0xff),
                                         (byte)(totalDataLen>>8 & 0xff),
                                         (byte)(totalDataLen>>16 & 0xff),
                                         (byte)(totalDataLen>>24 & 0xff)};


        byte [] wave = Encoding.ASCII.GetBytes(new char [] { 'W', 'A', 'V', 'E' });

        byte [] fmt = Encoding.ASCII.GetBytes(new char [] { 'f', 'm', 't', ' ' });
        
        byte [] sizeOfData = new byte [] { 16, 0, 0, 0 };
        
        byte [] waytoCode = new byte [] { 1, 0 };
        
        byte [] numOfChannel = new byte [] { (byte)channels, 0 };
        

        byte [] sampleRateByte = new byte [] {(byte)(sampleRate & 0xff),
                                         (byte)(sampleRate>>8 & 0xff),
                                         (byte)(sampleRate>>16 & 0xff),
                                         (byte)(sampleRate>>24 & 0xff)};

        byte [] byteRateByte = new byte [] {(byte)(byteRate & 0xff),
                                         (byte)(byteRate>>8 & 0xff),
                                         (byte)(byteRate>>16 & 0xff),
                                         (byte)(byteRate>>24 & 0xff)};

        byte [] dataAtATime = new byte[] { BitConverter.GetBytes(channels * 16 / 8)[0], 0 };//BitConverter.GetBytes(channels * 16 / 8);

        byte [] bitOfSample = new byte [] { 16, 0 };

        byte[] dataChunk= Encoding.ASCII.GetBytes(new char [] { 'd', 'a', 't', 'a' });

        byte [] dataChunkByte = new byte [] {(byte)(totalAudioLen & 0xff),
                                         (byte)(totalAudioLen>>8 & 0xff),
                                         (byte)(totalAudioLen>>16 & 0xff),
                                         (byte)(totalAudioLen>>24 & 0xff)};
        byteList.AddRange(riff);
        byteList.AddRange(riffChunk);
        byteList.AddRange(wave);
        byteList.AddRange(fmt);
        byteList.AddRange(sizeOfData);
        byteList.AddRange(waytoCode);
        byteList.AddRange(numOfChannel);
        byteList.AddRange(sampleRateByte);
        byteList.AddRange(byteRateByte);
        byteList.AddRange(dataAtATime);
        byteList.AddRange(bitOfSample);
        byteList.AddRange(dataChunk);
        byteList.AddRange(dataChunkByte);

        fout.Write(byteList.ToArray(), 0, 44);
        //throw new NotImplementedException();
    }





}
