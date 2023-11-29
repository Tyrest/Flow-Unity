using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public struct Beat
{
    public float x;
    public float y;
    public float distance;
    public float beatTime;
}

// Holds the information for a map which is a list of beats
public class BeatMap
{
    public List<Beat> Beats => GetBeats();
    public float offset;
    public float approachPeriod;
    
    private readonly string _path;
    private List<Beat> _beats;

    public BeatMap(string path)
    {
        _path = path;
        approachPeriod = 0.5f;
    }

    // Get the difficulty from the file name
    public string GetDifficulty()
    {
        return Path.GetFileNameWithoutExtension(_path);
    }

    // Parse a line from the file into a beat
    private static Beat? ReadLine(string line)
    {
        var beatMapData = line.Split(',');
        if (beatMapData.Length != 4)
        {
            return null;
        }

        var beat = new Beat
        {
            x = float.Parse(beatMapData[0]),
            y = float.Parse(beatMapData[1]),
            distance = float.Parse(beatMapData[2]),
            beatTime = float.Parse(beatMapData[3])
        };
        return beat;
    }

    // Read the file and parse the beats
    private List<Beat> ReadFile()
    {
        var beatMapLines = File.ReadAllLines(_path);
        return beatMapLines
            .Select(ReadLine)
            .Where(beat => beat != null)
            .Select(beat => beat.Value)
            .OrderBy(beat => beat.beatTime)
            .ToList();
    }

    // Lazy load the beats
    private List<Beat> GetBeats()
    {
        _beats ??= ReadFile();
        return _beats;
    }
}