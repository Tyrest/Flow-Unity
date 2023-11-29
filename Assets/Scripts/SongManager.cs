using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    private static SongManager _instance;

    public static SongManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SongManager>();
            }

            return _instance;
        }
    }

    public List<string> songDirectories;

    private Song CurrentSong => Songs[currentSongIndex];
    public int currentSongIndex;
    public string currentDifficulty;

    private List<Song> Songs => GetSongs();
    private List<Song> _songs;

    private void Awake()
    {
        _instance = this;
    }

    public List<Song> GetSongs()
    {
        _songs ??= songDirectories
            .SelectMany(GetSubdirectories)
            .Select(songPath => new Song(songPath))
            .OrderBy(song => song.Title)
            .ToList();
        return _songs;

        IEnumerable<string> GetSubdirectories(string songDirectory) =>
            Directory.GetDirectories(Path.GetFullPath(Path.Combine(songDirectory.Split('/'))));
    }

    public Song GetSong()
    {
        return CurrentSong;
    }

    public BeatMap GetBeatMap()
    {
        Debug.Log("Loading: " + CurrentSong.Title + " (" + currentDifficulty + ")");
        return CurrentSong.GetBeatMap(currentDifficulty);
    }
}