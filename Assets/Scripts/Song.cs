using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Beat
{
    public float beatTime;
    public int beatType;
    public int beatLane;
}

// Class to load and the store the information for a beatMap
public class Song
{
    public const string InfoPath = @"\info.txt";
    public const string AudioPath = @"\audio.ogg";

    public string path;
    public string title;
    public string artist;
    public string mapper;
    public float bpm;
    public float offset;

    private AudioClip audio;
    private List<BeatMap> songBeatMaps;

    public Song(string path)
    {
        Debug.Log(path);
        this.path = path;
        LoadSongInfo();
    }

    private void LoadSongInfo()
    {
        Debug.Log(path + InfoPath);
        var songInfoLines = File.ReadAllLines(path + InfoPath);
        foreach (string line in songInfoLines)
        {
            var songInfoData = line.Split(':');
            if (songInfoData.Length == 2)
            {
                switch (songInfoData[0])
                {
                    case "Title":
                        title = songInfoData[1];
                        break;
                    case "Artist":
                        artist = songInfoData[1];
                        break;
                    case "Mapper":
                        mapper = songInfoData[1];
                        break;
                    case "BPM":
                        bpm = float.Parse(songInfoData[1]);
                        break;
                    case "Offset":
                        offset = float.Parse(songInfoData[1]);
                        break;
                }
            }
        }
    }
}