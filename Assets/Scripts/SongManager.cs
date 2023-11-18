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
    public Song currentSong;
    public int currentSongIndex;

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
            Directory.GetDirectories(Path.Combine(songDirectory.Split('/')));
    }
}