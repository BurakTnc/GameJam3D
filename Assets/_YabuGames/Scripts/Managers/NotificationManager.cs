using System;
using System.Collections;
using UnityEngine;
using Unity.Notifications.iOS;
using Unity.Notifications.Android;
using UnityEngine.Android;

namespace _YabuGames.Scripts.Managers
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance;

        private void Awake()
        {
            #region Singleton

            if (Instance != this && Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            #endregion
        }

        private void Start()
        {
            RequestAuthorization();
        }

        private void AndroidAuthorization()
        {
            if (!Permission.HasUserAuthorizedPermission("android.permissions.POST_NOTIFICATIONS")) 
                Permission.RequestUserPermission("android.permissions.POST_NOTIFICATIONS");
        }

        private IEnumerator IOSAuthorization()
        {
            using var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true);
            while (!req.IsFinished)
            {
                yield return null;
            }

            var result = "\n RequestAuthorization: \n";
            result += "\n finished: " + req.IsFinished;
            result += "\n granted :  " + req.Granted;
            result += "\n error:  " + req.Error;
            result += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log("IOS AUTHORIZATION STATUS: " + result);
        }
        private void RequestAuthorization()
        {
            #if UNITY_ANDROID
            AndroidAuthorization();
            #elif UNITY_IOS
            StartCoroutine(IOSAuthorization());
            #endif
        }
        
        public void ScheduleNotification(int hours,int minutes)
        {
            #if UNITY_IOS
            IOSNotification(hours, minutes);
            #elif UNITY_ANDROID
            AndroidNotification(hours, minutes);
            #elif UNITY_EDITOR
            Debug.Log($"Notification is scheduled on {hours} Hours, {minutes} Min later");
            #endif
            
        }

        private void AndroidNotification(int hours, int minutes)
        {
            if (hours > 0)
                minutes += (hours * 60);
            
            var channel = new AndroidNotificationChannel() 
            {
                Id = "channel_id",
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            var notification = new AndroidNotification();
            notification.Title = "Your Title";
            notification.Text = "Your Text";
            notification.FireTime = System.DateTime.Now.AddMinutes(minutes);
            notification.SmallIcon = "small_icon";
            notification.LargeIcon = "large_icon";
            
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
        
        private void IOSNotification(int hours, int minutes)
        {
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(hours, minutes, 0),
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                // You can specify a custom identifier which can be used to manage the notification later.
                // If you don't provide one, a unique string will be generated automatically.
                Identifier = "_notification_01",
                Title = "Game Name",
                Body = "Come Here and Play!",
                Subtitle = "Your lives are full!",
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }
    }
}