using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace June {

	/// <summary>
	/// Generic IMessageBroker interface.
	/// </summary>
	public interface IIMessageBroker<TUri, TUriMap, TParameter> {

		IIMessageBroker<TUri, TUriMap, TParameter> Parent { get; }

		TUri UriFragment { get; }

		TUri UriPath { get; }

		TUriMap SUBSCRIBERS { get; }

		List<Action<TUri, TParameter>> CALLBACKS { get; }

		void RemoveSubscriber(TUri message);

		void Subscribe(TUri message, Action<TUri, TParameter> callback);

		void UnSubscribe(TUri message, Action<TUri, TParameter> callback);

		void Publish(TUri message, TParameter msgParameters);
	}

	/// <summary>
	/// I message broker.
	/// </summary>
	public abstract class IMessageBroker {

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public virtual IMessageBroker Parent { get; protected set; }
		
		/// <summary>
		/// Gets or sets the URI fragment.
		/// </summary>
		/// <value>The URI fragment.</value>
		public virtual string UriFragment { get; protected set; }
		
		/// <summary>
		/// Gets the URI path.
		/// </summary>
		/// <value>The URI path.</value>
		public abstract string UriPath {
			get;
		}
		
		/// <summary>
		/// The SUBSCRIBERS for child fragments.
		/// </summary>
		#if UNITY_EDITOR
		public
		#else
		protected 
		#endif
		Dictionary<string, IMessageBroker> _SUBSCRIBERS = new Dictionary<string, IMessageBroker>();
		
		/// <summary>
		/// The CALLBACKS.
		/// </summary>
		#if UNITY_EDITOR
		public
		#else
		protected 
		#endif
		List<Action<string, IDictionary<string, object>>> _CALLBACKS = new List<Action<string, IDictionary<string, object>>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="June.MessageBrokerFragment"/> class.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <param name="uriFragment">URI fragment.</param>
		public IMessageBroker(IMessageBroker parent, string uriFragment) {
			this.Parent = parent;
			this.UriFragment = uriFragment;
		}

		/// <summary>
		/// Removes the subscriber.
		/// </summary>
		/// <param name="message">Message.</param>
		internal virtual void RemoveSubscriber(string message) {
			_SUBSCRIBERS.Remove(message);
		}

		/// <summary>
		/// Subscribe the specified message and callback.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		public abstract void Subscribe(string message, Action<string, IDictionary<string, object>> callback);

		/// <summary>
		/// Unsubscribe the callback.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="callback">Callback.</param>
		public abstract void UnSubscribe(string message, Action<string, IDictionary<string, object>> callback);
		
		/// <summary>
		/// Publish the specified message and msgParameters.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="msgParameters">Message parameters.</param>
		public abstract void Publish(string message, IDictionary<string, object> msgParameters);
		
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="June.IMessageBroker"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="June.IMessageBroker"/>.</returns>
		public override string ToString () {
			return string.Format ("IMessageBroker: Callbacks:{1} Subscribers:{2} UriPath={0}\n{3}", 
			                      UriPath,
			                      _CALLBACKS.Count,
								  _SUBSCRIBERS.Count,
			                      string.Join("\n", _SUBSCRIBERS.Values.ToList().ConvertAll<string>(b => b.ToString()).ToArray()));
		}
	}
}