using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds the information for a map which is a list of beats
public class BeatMap
{
    public List<Beat> beats;

    public BeatMap(string path)
    {
        beats = new List<Beat>();
        LoadBeatMap(path);
    }
    
    private void LoadBeatMap(string path)
    {
        var beatMapLines = Resources.Load<TextAsset>(path).text.Split('\n');
        foreach (string line in beatMapLines)
        {
            var beatMapData = line.Split(',');
            if (beatMapData.Length == 3)
            {
                Beat beat = new Beat();
                beat.beatTime = float.Parse(beatMapData[0]);
                beat.beatType = int.Parse(beatMapData[1]);
                beat.beatLane = int.Parse(beatMapData[2]);
                beats.Add(beat);
            }
        }
    }
}
