using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace June {

	/// <summary>
	/// Message broker fragment.
	/// </summary>
	public class MessageBrokerFragment : IMessageBroker {
		/// <summary>
		/// The SEPARATOR.
		/// </summary>
		public const char SEPARATOR = '/';
		public const char QUERY_START = '?';
		public const char QUERY_APPEND = '&';
		public const char QUERY_KEY_VALUE_SEPARATOR = '=';

		public List<
			KeyValuePair<
				KeyValuePair<string, object>[], 
				Action<string, IDictionary<string, object>>>> _QUERY_CALLBACK = new List<KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>>>();

		/// <summary>
		/// Gets the URI path.
		/// </summary>
		/// <value>The URI path.</value>
		public override string UriPath {
			get {
				return string.Concat(Parent.UriPath, SEPARATOR, UriFragment);
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="June.MessageBrokerFragment"/> class.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="uriFragment">URI fragment.</param>
		public MessageBrokerFragment(MessageBrokerFragment parent, string uriFragment) : base(parent, uriFragment) { }

		/// <summary>
		/// Subscribe the specified message and callback.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		public override void Subscribe (string message, Action<string, IDictionary<string, object>> callback) {
			SubscribeFragments(
				fragments: message.Split(new char[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries), 
				query: null,
				callback: callback);
		}

		/// <summary>
		/// Unsubscribe the callback.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		public override void UnSubscribe (string message, Action<string, IDictionary<string, object>> callback) {
			UnSubscribeFragments(
				fragments: message.Split(new char[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries), 
				callback: callback);
		}

		/// <summary>
		/// Subscribe the specified messageUri and callback.
		/// </summary>
		/// <param name="messageUri">Message URI.</param>
		/// <param name="callback">Callback.</param>
		public void SubscribeFragments(IEnumerable<string> fragments, string query, Action<string, IDictionary<string, object>> callback) {
			string currentFragment = fragments.FirstOrDefault();

			if(string.IsNullOrEmpty(currentFragment)) {

				if(!string.IsNullOrEmpty(query)) {
					string[] pairs = query.Split(new char[] { QUERY_APPEND }, StringSplitOptions.RemoveEmptyEntries);
					List<KeyValuePair<string, object>> keypairs = new List<KeyValuePair<string, object>>();
					foreach(var pair in pairs) {
						string[] keyAndValue = pair.Split(new char[] { QUERY_KEY_VALUE_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
						if(2 == keyAndValue.Length) {
							keypairs.Add(new KeyValuePair<string, object>(keyAndValue[0], keyAndValue[1]));
						}
					}
					_QUERY_CALLBACK.Add(
						new KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>>(
						keypairs.ToArray(),
						callback));
				}
				else {
					_CALLBACKS.Add(callback);
				}

				return;
			}

			//Sanitize current fragment
			// - Remove query

			if(false == string.IsNullOrEmpty(currentFragment)) {
				if(currentFragment.Contains(QUERY_START)) {
					query = currentFragment.Substring(currentFragment.IndexOf(QUERY_START)+1);
					currentFragment = currentFragment.Substring(0, currentFragment.IndexOf(QUERY_START));
				}
			}


			if(false == _SUBSCRIBERS.ContainsKey(currentFragment)) {
				_SUBSCRIBERS.Add(currentFragment, new MessageBrokerFragment(this, currentFragment));
			}
			((MessageBrokerFragment)_SUBSCRIBERS[currentFragment]).SubscribeFragments(fragments.Skip(1), query, callback);
		}

		/// <summary>
		/// Uns the subscribe fragments.
		/// </summary>
		/// <param name="fragments">Fragments.</param>
		/// <param name="callback">Callback.</param>
		public void UnSubscribeFragments(IEnumerable<string> fragments, Action<string, IDictionary<string, object>> callback) {
			string currentFragment = fragments.FirstOrDefault();
			if(string.IsNullOrEmpty(currentFragment)) {
				if(_CALLBACKS.Contains(callback)) {
					_CALLBACKS.Remove(callback);

					if(0 == _CALLBACKS.Count && 0 == _SUBSCRIBERS.Count) {
						Parent.RemoveSubscriber(this.UriFragment);
					}

					return;
				}
			}
			if(true == _SUBSCRIBERS.ContainsKey(currentFragment)) {
				((MessageBrokerFragment)_SUBSCRIBERS[currentFragment]).UnSubscribeFragments(fragments.Skip(1), callback);
			}
		}
		
		/// <summary>
		/// Publish the specified message and msgParameters.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="msgParameters">Message parameters.</param>
		public override void Publish(string message, IDictionary<string, object> msgParameters) {

			// If message published matches current object fragment
			if(message.StartsWith(UriPath)) {

				#if UNITY_EDITOR
				if(null != MessageBroker._OnBrokerPublish) {
					MessageBroker._OnBrokerPublish(this);
				}
				#endif

				// Invoke all callbacks subscribed upto this URI
				foreach(var callback in _CALLBACKS) {
					callback(message, msgParameters);
				}

				// Invoke all child subscribers
				foreach(var kv in _SUBSCRIBERS) {
					kv.Value.Publish(message, msgParameters);
				}

				// Invoke parameterized callbacks after checking parameters are valid
				if(null != msgParameters) {
					foreach(var queries in _QUERY_CALLBACK) {
						if(queries.Key.All(kv => (msgParameters.ContainsKey(kv.Key) && msgParameters[kv.Key].Equals(kv.Value)))) {
							#if UNITY_EDITOR
							if(null != MessageBroker._OnQueryCallbackPublish) {
								MessageBroker._OnQueryCallbackPublish(queries);
							}
							#endif
							queries.Value(message, msgParameters);
						}
					}
				}
			}
		}

		public new string ToString () {
			return string.Format ("[MessageBrokerFragment: \n\tBase={0} Query:\n\t{1}]", 
			                      base.ToString(),
			                      string.Join("\n\t", _QUERY_CALLBACK.ConvertAll<string>(
									kv => {
										return string.Format("[{0}]", string.Join(",\n\t", Array.ConvertAll(kv.Key, k => k.ToString())));
									}).ToArray()
			                      ));
		}
	}
}