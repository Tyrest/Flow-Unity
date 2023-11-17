using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public List<string> songDirectories;
    public List<Song> songs;
    public Song currentSong;
    public int currentSongIndex;

    private static SongManager _instance;

    public static SongManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SongManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public List<Song> GetSongs()
    {
        songs ??= songDirectories
            .SelectMany(GetSubdirectories)
            .Select(songPath => new Song(songPath))
            .OrderBy(song => song.Title)
            .ToList();
        return songs;

        IEnumerable<string> GetSubdirectories(string songDirectory) =>
            Directory.GetDirectories(Path.Combine(songDirectory.Split('/')));
    }
}