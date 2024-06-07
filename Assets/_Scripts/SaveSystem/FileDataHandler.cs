using System;
using System.IO;
using UnityEngine;

namespace Blobber
{
    public class FileDataHandler
    {
        private string _dataDirPath = "";
        private string _dataFileName = "";

        private bool _useEncryption = false;
        private readonly string _encryptionCodeWord = "word";

        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption) {
            _dataDirPath = dataDirPath;
            _dataFileName = dataFileName;
            _useEncryption = useEncryption;
        }

        public GameData Load() {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath)) {
                try {
                    string dataToLoad = "";

                    using FileStream stream = new FileStream(fullPath, FileMode.Open);
                    using StreamReader reader = new StreamReader(stream);
                    dataToLoad = reader.ReadToEnd();

                    // decrypt
                    if (_useEncryption) {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }

                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e) {
                    Debug.LogError("Error occured when trying to load from \n" + fullPath + "\n" + e);
                }
            }
            return loadedData;
        }

        public void Save(GameData data) {
            string fullPath = Path.Combine(_dataDirPath, _dataFileName);
            try {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);

                // encrypt
                if (_useEncryption) {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                using FileStream stream = new FileStream(fullPath, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(dataToStore);
            }
            catch (Exception e) {
                Debug.LogError("Error occured when trying to save to \n" + fullPath + "\n" + e);
            }
        }

        // implementation of XOR encryption
        private string EncryptDecrypt(string data) {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++) {
                modifiedData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);
            }
            return modifiedData;
        }
    }
}