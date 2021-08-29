using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePainter
{
    // Template magic to allow custom types to be stored inside the Event Channels
    public interface IEventRequestInfo
    {
        string path { get; }
        object sender { get; }
    }

    public class EventRequestInfo : IEventRequestInfo
    {
        // Event Channel Name
        // 频道的名词
        public string path { get; private set; }

        // Event Request Sender (Can be typecasted)
        // 消息的发件人 （能被类型转换）
        public object sender { get; private set; }

        public EventRequestInfo(string channelpath, object senderobject)
        {
            path = channelpath;
            sender = senderobject;
        }
    }

    public class EventRequestInfo<T> : IEventRequestInfo
    {
        // Event Channel Name
        // 频道的名词
        public string path { get; private set; }

        // Event Request Sender (Can be typecasted)
        // 消息的发件人 （能被类型转换）
        public object sender { get; private set; }

        // Contains the custom data of the info
        // 自订资料
        public T body;

        public EventRequestInfo(string channelpath, object senderobject, T requestbody)
        {
            path = channelpath;
            sender = senderobject;
            body = requestbody;
        }

        public EventRequestInfo(string channelpath, object senderobject)
        {
            path = channelpath;
            sender = senderobject;
        }
    }

}
