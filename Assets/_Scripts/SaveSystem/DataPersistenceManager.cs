using RoboRiftRush.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using TarodevController;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blobber
{
    public class DataPersistenceManager : Singleton<DataPersistenceManager>
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        [SerializeField] private bool saveGame;
        [SerializeField] private bool useEncryption;

        private GameData _gameData;
        private List<IDataPersistance> _dataPersistanceObjects;
        private FileDataHandler _dataHandler;

        public GameData GetGameData() {
            return _gameData;
        }

        private void Start() {
            SceneManager.sceneUnloaded += BeforeSceneChange;
            SceneManager.activeSceneChanged += AfterSceneChange;

            LoadGame();
        }

        private void BeforeSceneChange(Scene arg0) {
            SaveGame();
        }

        private void AfterSceneChange(Scene arg0, Scene arg1) {
            LoadGame();
        }

        private void NewGame() {
            _gameData = new GameData();
        }

        private void LoadGame() {
            if (!saveGame) return;

            _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
            _dataPersistanceObjects = FindAllDataPersistanceObjects();

            _gameData = _dataHandler.Load();

            // if no game data, then create it
            if (_gameData == null) {
                Debug.Log("No data found. Making new game data");
                NewGame();
            }

            foreach (IDataPersistance dataPersistanceObj in _dataPersistanceObjects) {
                dataPersistanceObj.LoadData(_gameData);
            }
        }

        private void SaveGame() {
            if (!saveGame) return;

            foreach (IDataPersistance dataPersistanceObj in _dataPersistanceObjects) {
                dataPersistanceObj.SaveData(ref _gameData);
            }

            _dataHandler.Save(_gameData);
        }

        protected override void OnApplicationQuit() {
            base.OnApplicationQuit();
            SaveGame();
        }

        private List<IDataPersistance> FindAllDataPersistanceObjects() {
            return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>().ToList();
        }
    }
}