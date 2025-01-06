using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Api.Lxns.Models;
using Assets.Scripts.Constants;

namespace Assets.Scripts.Api.Lxns.Managers
{
    [Serializable]
    public class SongList
    {
        public List<Song> songs = new List<Song>();
    }

    public class SongManager
    {
        private static SongManager _instance;
        public static SongManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SongManager();
                }
                return _instance;
            }
        }

        private Dictionary<int, Song> songDictionary = new Dictionary<int, Song>();
        private readonly string filePath;

        private SongManager()
        {
            filePath = Path.Combine(ApplicationConstants.RecordsPath, "lxns_songs.json");
            LoadSongsFromFile();
        }

        private void LoadSongsFromFile()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    SongList songList = JsonUtility.FromJson<SongList>(json);
                    if (songList != null && songList.songs != null)
                    {
                        foreach (var song in songList.songs)
                        {
                            if (!songDictionary.ContainsKey(song.id))
                            {
                                songDictionary.Add(song.id, song);
                            }
                        }
                        Debug.Log("Loaded songs from local file.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load songs from file: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning("Songs file does not exist. Starting with an empty song dictionary.");
            }
        }

        private void SaveSongsToFile()
        {
            try
            {
                SongList songList = new SongList
                {
                    songs = new List<Song>(songDictionary.Values)
                };
                string directory = Path.GetDirectoryName(filePath);
                if (Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string json = JsonUtility.ToJson(songList, true);
                File.WriteAllText(filePath, json);
                Debug.Log("Saved songs to local file.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save songs to file: {ex.Message}");
            }
        }

        public async Task<Song> GetSongAsync(int songId)
        {
            if (songDictionary.ContainsKey(songId))
            {
                return songDictionary[songId];
            }
            else
            {
                var requests = new Requests(ApplicationConstants.Token);
                Song fetchedSong = await requests.GetSongAsync(songId);
                if (fetchedSong != null)
                {
                    songDictionary.Add(songId, fetchedSong);
                    SaveSongsToFile();
                }
                else
                {
                    Debug.LogWarning($"Failed to fetch Song with ID {songId}.");
                }
                return fetchedSong;
            }
        }

        public Song GetSong(int songId)
        {
            if (songDictionary.ContainsKey(songId))
            {
                return songDictionary[songId];
            }
            else
            {
                var task = GetSongAsync(songId);
                task.Wait();
                return task.Result;
            }
        }
    }
}
