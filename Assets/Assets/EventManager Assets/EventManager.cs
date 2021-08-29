using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EventManagerProject;

namespace EventManagerProject
{
    public class EventChannel : UnityEvent<IEventRequestInfo> { }

    public class EventManager : MonoBehaviour
    {
        // Singleton
        // 单例模式
        static public EventManager Instance
        {
            get;
            private set;
        }
        public void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("Event Manager instance already created. Deleting it and instantiating a new instance...");
                Destroy(Instance);
                Instance = this;
            }
            else
            {
                Instance = this;
            }
        }

        // Stores all the events
        // 一个数据字典，用来保存事件
        Dictionary<string, EventChannel> EventDictionary = new Dictionary<string, EventChannel>();

        // Function to allow an object to listen to a channel, and call a function(s) when a request to said channel is made
        // 让一个物件收消息的函数。当物件收到消息，会调用函数。
        public void Listen(string channelname, UnityAction<IEventRequestInfo> action)
        {
            if (!EventDictionary.ContainsKey(channelname))
            {
                EventDictionary.Add(channelname, new EventChannel());
            }
            EventChannel channel = EventDictionary[channelname];
            channel.AddListener(action);
        }

        // Allows an object to publish info to all listeners of a channel. Can send custom datatypes over.
        // 让一个物件发消息的函数。会把自订资料发给一个频道。
        public void Publish<T>(string channelname, object sender, T body)
        {
            EventChannel channel;
            if (EventDictionary.TryGetValue(channelname, out channel))
            {
                channel.Invoke(new EventRequestInfo<T>(channelname, sender, body));
            }
            else
            {
                Debug.LogError("Tried to publish an event to a non-existent event channel (Did you forget to register a channel, or was it a typo?)");
            }
        }

        // Allows an object to stop listening to a channel
        // 让一个物件停止收消息的函数
        public void Close(string channelname, UnityAction<IEventRequestInfo> action)
        {
            EventChannel channel;
            if (EventDictionary.TryGetValue(channelname, out channel))
            {
                channel.RemoveListener(action);
            }
        }
    }
}