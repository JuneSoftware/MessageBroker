using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace June.MessageBrokers.Providers {

	/// <summary>
	/// IFastBroker.
	/// This uses arrays to store fragments improving access times
	/// TODO: Compare performance of IFastBroker[] vs List<IFastBroker>
	/// </summary>
	public abstract class IFastBroker : IIMessageBroker<int, IFastBroker[], IDictionary<string, object>> {

		#region IIMessageBroker implementation
		public abstract void RemoveSubscriber(int message);

		public abstract void Subscribe(int message, Action<int, IDictionary<string, object>> callback);

		public abstract void UnSubscribe(int message, Action<int, IDictionary<string, object>> callback);

		public abstract void Publish(int message, IDictionary<string, object> msgParameters);

		protected IFastBroker _Parent;
		public IIMessageBroker<int, IFastBroker[], IDictionary<string, object>> Parent {
			get {
				return _Parent;
			}
		}

		protected int _UriFragment;
		public virtual int UriFragment {
			get {
				return _UriFragment;
			}
		}

		public abstract int UriPath { get; }

		public abstract IFastBroker[] SUBSCRIBERS {
			get;
		}

		protected List<Action<int, IDictionary<string, object>>> _CALLBACKS = new List<Action<int, IDictionary<string, object>>>();
		public List<Action<int, IDictionary<string, object>>> CALLBACKS {
			get {
				return _CALLBACKS;
			}
		}
		#endregion

		public IFastBroker(IFastBroker parent, int uriFragment) {
			this._Parent = parent;
			this._UriFragment = uriFragment;
		}
	}
}