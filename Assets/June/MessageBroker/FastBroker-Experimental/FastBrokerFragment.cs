using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace June.MessageBrokers.Providers {

	/// <summary>
	/// Fast broker fragment.
	/// </summary>
	public class FastBrokerFragment : IFastBroker {

		protected int _SubscriberCount = 0;

		#region implemented abstract members of IFastBroker
		/// <summary>
		/// Gets the URI path.
		/// </summary>
		/// <value>The URI path.</value>
		public override int UriPath {
			get {
				return (Parent.UriPath << 8) | UriFragment;
			}
		}
			
		protected IFastBroker[] _SUBSCRIBERS = new IFastBroker[255];
		/// <summary>
		/// Gets the SUBSCRIBER.
		/// </summary>
		/// <value>The SUBSCRIBER.</value>
		public override IFastBroker[] SUBSCRIBERS {
			get {
				return _SUBSCRIBERS;
			}
		}
			
		/// <summary>
		/// Removes the subscriber.
		/// </summary>
		/// <param name="message">Message.</param>
		public override void RemoveSubscriber(int message) {
			_SubscriberCount--;	// NOTE: Wrap inside Interlocked.Decrement for Multi-Threaded scenarios
			_SUBSCRIBERS[message] = null;
		}

		/// <summary>
		/// Subscribe the specified message and callback.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		public override void Subscribe (int message, Action<int, IDictionary<string, object>> callback) {
			SubscribeFragments(
				fragments: GetFragmentsFromMessage(message),
				fragmentIndex: 0,
				callback: callback);
		}

		/// <summary>
		/// Unsubscribe.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		public override void UnSubscribe(int message, Action<int, IDictionary<string, object>> callback) {
			UnSubscribeFragments(
				fragments: GetFragmentsFromMessage(message), 
				callback: callback);
		}

		/// <summary>
		/// Publish the specified message and msgParameters.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="msgParameters">Message parameters.</param>
		public override void Publish(int message, IDictionary<string, object> msgParameters) {
			PublishFragments(
				message: message,
				fragments: GetFragmentsFromMessage(message),
				fragmentIndex: 0,
				msgParameters: msgParameters);
		}
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="June.FastBrokerFragment"/> class.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="uriFragment">URI fragment.</param>
		public FastBrokerFragment(FastBrokerFragment parent, int uriFragment) : base(parent, uriFragment) { }

		/// <summary>
		/// Gets the fragments from message.
		/// 
		///   Byte     Byte     Byte     Byte 
		/// +--------+--------+--------+--------+
		/// |AAAAAAAA|BBBBBBBB|CCCCCCCC|DDDDDDDD|
		/// +--------+--------+--------+--------+
		/// MSB									LSB
		/// 
		/// This method converts the integer into a byte array with a max length of 4
		/// 
		/// This allows the broker to have : 255 * 255 * 255 messages
		/// 
		/// The inital ROOT fragment will be constant !
		/// 
		/// With a maximum of 3 fragments
		/// </summary>
		/// <returns>The fragments from message.</returns>
		/// <param name="message">Message.</param>
		private static int[] GetFragmentsFromMessage(int message) {
			const int maxFragments = 4;
			List<int> fragments = new List<int>();
			uint mask = 0x000000FF;

			for(int i=0; i<maxFragments; i++) {
				if(unchecked(message & mask) > 0) {
					fragments.Add((int)((message & mask) >> (8 * i)));
				}
				mask <<= 8;
			}

			return fragments.ToArray();
		}

		/// <summary>
		/// Subscribes the fragments.
		/// </summary>
		/// <param name="fragments">Fragments.</param>
		/// <param name="callback">Callback.</param>
		public void SubscribeFragments(int[] fragments, int fragmentIndex, Action<int, IDictionary<string, object>> callback) {
			if(fragmentIndex >= fragments.Length) {
				_CALLBACKS.Add(callback);
				return;
			}

			int currentFragment = fragments[fragmentIndex];

			if(null == _SUBSCRIBERS[currentFragment]) {
				_SUBSCRIBERS[currentFragment] = new FastBrokerFragment(this, currentFragment);
				_SubscriberCount++;
			}

			((FastBrokerFragment)_SUBSCRIBERS[currentFragment]).SubscribeFragments(fragments, fragmentIndex+1, callback);
		}

		/// <summary>
		/// Uns the subscribe fragments.
		/// </summary>
		/// <param name="fragments">Fragments.</param>
		/// <param name="callback">Callback.</param>
		public void UnSubscribeFragments(int[] fragments, Action<int, IDictionary<string, object>> callback) {
			int currentFragment = fragments[0];

			if(_CALLBACKS.Contains(callback)) {
				_CALLBACKS.Remove(callback);

				if(0 == _CALLBACKS.Count && 0 == _SubscriberCount) {
					Parent.RemoveSubscriber(this.UriFragment);
				}

				return;
			}

			if(null != _SUBSCRIBERS[currentFragment]) {
				((FastBrokerFragment)_SUBSCRIBERS[currentFragment]).UnSubscribeFragments(fragments.Skip(1).ToArray(), callback);
			}
		}

		/// <summary>
		/// Publishs the fragments.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="fragments">Fragments.</param>
		/// <param name="msgParameters">Message parameters.</param>
		public void PublishFragments(int message, int[] fragments, int fragmentIndex, IDictionary<string, object> msgParameters) {

			if(fragmentIndex >= fragments.Length) {
				// Invoke all callbacks subscribed upto this URI
				foreach(var callback in _CALLBACKS) {
					callback(message, msgParameters);
				}
				return;
			}

			// If message published matches current object fragment
			if(UriFragment == fragments[fragmentIndex]) {

				//#if UNITY_EDITOR
				//if(null != MessageBroker._OnBrokerPublish) {
				//	MessageBroker._OnBrokerPublish(this);
				//}
				//#endif

				// Invoke all callbacks subscribed upto this URI
				foreach(var callback in _CALLBACKS) {
					callback(message, msgParameters);
				}

				// Invoke all child subscribers
				if(_SubscriberCount > 0) {
					foreach(var kv in _SUBSCRIBERS) {
						if(null != kv) {
							((FastBrokerFragment)kv).PublishFragments(message, fragments, fragmentIndex+1, msgParameters);
						}
					}
				}
			}
		}
	}
}