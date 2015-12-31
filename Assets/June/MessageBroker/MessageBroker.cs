using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace June {

	/// <summary>
	/// Message broker.
	/// </summary>
	public class MessageBroker : MessageBrokerFragment {

		#if UNITY_EDITOR
		/// <summary>
		/// The on broker publish callback for editor window.
		/// </summary>
		public static Action<IMessageBroker> _OnBrokerPublish;
		public static Action<KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>>> _OnQueryCallbackPublish;
		public static Action<string> _OnPublish;
		#endif


		public static MessageBrokerFragment _Instance;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static MessageBrokerFragment Instance {
			get {
				if(null == _Instance) {
					_Instance = new MessageBroker();
				}
				return _Instance;
			}
		}

		#region Instance Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="June.MessageBroker"/> class.
		/// </summary>
		private MessageBroker() : base(null, MessageBrokerFragment.SEPARATOR.ToString()) {

		}

		/// <summary>
		/// Gets the URI path.
		/// </summary>
		/// <value>The URI path.</value>
		public override string UriPath {
			get {
				return UriFragment;
			}
		}
		#endregion

		/// <summary>
		/// Subscribe the specified messageUri and callback.
		/// </summary>
		/// <param name="messageUri">Message URI.</param>
		/// <param name="callback">Callback.</param>
		public static void Subscribe(string messageUri, Action<string, IDictionary<string, object>> callback) {
			Instance.Subscribe(messageUri, callback);
		}

		/// <summary>
		/// Unsubscribe the callback.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="messageUri">Message URI.</param>
		public static void UnSubscribe(string messageUri, Action<string, IDictionary<string, object>> callback) {
			Instance.UnSubscribe(messageUri, callback);
		}

		/// <summary>
		/// Publish the specified messageUri.
		/// </summary>
		/// <param name="messageUri">Message URI.</param>
		public static void Publish(string messageUri) {
			Publish(messageUri, null);
		}

		/// <summary>
		/// Publish the specified messageUri, paramKey1 and paramValue1.
		/// </summary>
		/// <param name="messageUri">Message URI.</param>
		/// <param name="paramKey1">Parameter key1.</param>
		/// <param name="paramValue1">Parameter value1.</param>
		public static void Publish(string messageUri, string paramKey1, string paramValue1) {
			Publish(messageUri, new Dictionary<string, object>() { { paramKey1, paramValue1 } });
		}

		/// <summary>
		/// Publish the specified message and msgParameters.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="msgParameters">Message parameters.</param>
		/// <param name="messageUri">Message URI.</param>
		public static void Publish(string messageUri, IDictionary<string, object> msgParameters) {
			//Debug.Log("****************** [MessageBroker] Publish: " + messageUri + " Parameters: \n " + SimpleJson.SimpleJson.SerializeObject(msgParameters));
			#if UNITY_EDITOR
			if(null != _OnPublish) {
				_OnPublish("=> " + messageUri);
			}
			#endif
			Instance.Publish(messageUri, msgParameters);
		}
	}
}