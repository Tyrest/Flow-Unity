using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public struct Beat
{
    public float beatTime;
    public int beatType;
    public int beatLane;
}

// Holds the information for a map which is a list of beats
public class BeatMap
{
    private readonly string _path;
    private List<Beat> _beats;

    public BeatMap(string path)
    {
        _path = path;
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
        if (beatMapData.Length != 3)
        {
            return null;
        }

        var beat = new Beat
        {
            beatTime = float.Parse(beatMapData[0]),
            beatType = int.Parse(beatMapData[1]),
            beatLane = int.Parse(beatMapData[2])
        };
        return beat;
    }

    // Read the file and parse the beats
    private List<Beat> ReadFile()
    {
        var beatMapLines = Resources.Load<TextAsset>(_path).text.Split('\n');
        return beatMapLines
            .Select(ReadLine)
            .Where(beat => beat != null)
            .Select(beat => beat.Value)
            .ToList();
    }

    // Lazy load the beats
    public List<Beat> GetBeats()
    {
        _beats ??= ReadFile();
        return _beats;
    }
}