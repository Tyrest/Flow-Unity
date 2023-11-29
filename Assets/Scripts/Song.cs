using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Class to load and the store the information for a beatMap
public class Song
{
    private const string InfoPath = "info.txt";
    private const string AudioPath = "audio.mp3";

    private readonly string _path;
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public string Mapper { get; private set; }
    public float Bpm { get; private set; }
    public float Offset { get; private set; }

    private AudioClip _audio;
    private List<BeatMap> _songBeatMaps;

    public Song(string path)
    {
        _path = path;
        LoadSongInfo();
    }

    private void LoadSongInfo()
    {
        var songInfoLines = File.ReadAllLines(Path.Combine(_path, InfoPath));
        foreach (var line in songInfoLines)
        {
            var songInfoData = line.Split(':');
            if (songInfoData.Length == 2)
            {
                switch (songInfoData[0])
                {
                    case "Title":
                        Title = songInfoData[1];
                        break;
                    case "Artist":
                        Artist = songInfoData[1];
                        break;
                    case "Mapper":
                        Mapper = songInfoData[1];
                        break;
                    case "BPM":
                        Bpm = float.Parse(songInfoData[1]);
                        break;
                    case "Offset":
                        Offset = float.Parse(songInfoData[1]);
                        break;
                }
            }
        }
    }

    private static async Task<AudioClip> LoadClip(string path)
    {
        AudioClip clip = null;
        using var uwr = UnityWebRequestMultimedia.GetAudioClip(new Uri(path), AudioType.MPEG);
        uwr.SendWebRequest();

        try
        {
            while (!uwr.isDone) await Task.Delay(5);

            if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                Debug.Log($"{uwr.error}");
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(uwr);
            }
        }
        catch (Exception err)
        {
            Debug.Log($"{err.Message}, {err.StackTrace}");
        }

        return clip;
    }

    public async Task<AudioClip> GetAudio()
    {
        _audio ??= await LoadClip(Path.Combine(_path, AudioPath));
        return _audio;
    }

    // Lazy load the beat maps
    private IEnumerable<BeatMap> GetBeatMaps()
    {
        _songBeatMaps ??= Directory.GetFiles(_path, "*.txt")
            .Select(beatMapFile => new BeatMap(beatMapFile))
            .ToList();
        return _songBeatMaps;
    }

    public List<string> GetDifficulties()
    {
        return GetBeatMaps().Select(beatMap => beatMap.GetDifficulty()).ToList();
    }

    public BeatMap GetBeatMap(string difficulty)
    {
        return GetBeatMaps().FirstOrDefault(beatMap => beatMap.GetDifficulty() == difficulty);
    }
}