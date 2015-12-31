using UnityEngine;
using System.Collections;
using June.MessageBrokers.Providers;

/// <summary>
/// Fast broker.
/// </summary>
public class FastBroker : FastBrokerFragment {

	public static FastBroker _Instance;
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static FastBroker Instance {
		get {
			if(null == _Instance) {
				_Instance = new FastBroker();
			}
			return _Instance;
		}
	}

	#region Instance Methods
	/// <summary>
	/// Initializes a new instance of the <see cref="June.MessageBroker"/> class.
	/// </summary>
	private FastBroker() : base(null, 1) { }

	/// <summary>
	/// Gets the URI path.
	/// </summary>
	/// <value>The URI path.</value>
	public override int UriPath {
		get {
			return UriFragment;
		}
	}
	#endregion

	// TODO: Create static helper methods
}
