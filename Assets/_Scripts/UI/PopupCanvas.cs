using DG.Tweening;
using SpeedPlatformer.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpeedPlatformer.UI {
    public class PopupCanvas : StaticInstance<PopupCanvas> {

        public static event Action<string> OnObjectActivated;

        [SerializeField] private PopupData[] _popupData;

        // name of popup, is the popup active
        private Dictionary<string, bool> _activePopups = new Dictionary<string, bool>();

        public bool GetPopupActive(string name) {
            return _activePopups[name];
        }

        protected override void Awake() {
            base.Awake();

            // initialize the dictionary as all popups are inactive
            foreach (PopupData data in _popupData) {
                _activePopups.Add(data.Name, data.StartActive);
            }
        }

        public void ActivateUIObject(string name, bool playAudio = true) {
            // if the object is already active, return
            if (_activePopups[name]) return;

            PopupData currentPopup = default;
            foreach (var popupData in from popupData in _popupData
                                      where popupData.Name == name
                                      select popupData) {
                currentPopup = popupData;
            }

            if (currentPopup.Equals(default)) {
                Debug.LogWarning("PopupData not found");
                return;
            }

            currentPopup.Transform.gameObject.SetActive(true);

            currentPopup.Transform.position = currentPopup.HidePos;
            currentPopup.Transform.DOMove(currentPopup.ShowPos, 0.3f).SetEase(Ease.OutSine).SetUpdate(true).OnComplete(() => {
                // set the dictionary to the panel being active
                _activePopups[name] = true;
            });

            if (playAudio) {
                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.UISlide, 0f, 0.1f);
            }

            OnObjectActivated?.Invoke(name);
        }

        public void DeactivateUIObject(string name, bool playAudio = true) {
            // if the object is already deactive, return
            if (!_activePopups[name]) return;

            PopupData currentPopup = default;
            foreach (var popupData in from popupData in _popupData
                                      where popupData.Name == name
                                      select popupData) {
                currentPopup = popupData;
            }

            if (currentPopup.Equals(default)) {
                Debug.LogWarning("PopupData not found");
                return;
            }

            currentPopup.Transform.position = currentPopup.ShowPos;
            currentPopup.Transform.DOMove(currentPopup.HidePos, 0.3f).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() => {
                currentPopup.Transform.gameObject.SetActive(false);

                // set the dictionary to the object being deactive
                _activePopups[name] = false;
            });

            if (playAudio) {
                AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.UISlide, 0f, 0.1f);
            }
        }
    }

    [Serializable]
    public struct PopupData {
        public string Name;
        public Transform Transform;
        public Vector2 ShowPos;
        public Vector2 HidePos;
        public bool StartActive;
    }
}