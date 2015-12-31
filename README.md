# MessageBroker
Flexible message router based on the URL pattern

![MessageBrokerDemo](https://raw.githubusercontent.com/JuneSoftware/MessageBroker/master/images/MessageBrokerDemo.png)

This message broker is based on the URL pattern, all messages follow the following syntax

```
//home
//home/button
//home/store/powers
//game/level_start
```

The message broker supports query parameters, subscribers can subscribe to events only when certain parameter-values are present, e.g.

```
//home/store/powers?type=SuperRun
                    /\
                    ||
                    This subscriber will only get invoked when the type field contains SuperRun

//game/level_start?level_name=level1

//game/level_end?level_name=level2&position=1
                 /\                /\
                 ||                ||
                 The broker also supports multiple query parameters
```


```csharp
// Demo scene
public class Demo : MonoBehaviour {

	const float _PADDING = 10f;
	StringBuilder _LOG;

	// Messages class holds all the URI's used in this demo
	private class Messages {
		public const string ROOT = "//";
		public const string HOME = "//home";
		public const string HOME_BUTTON = "//home/button";
		public const string STORE = "//store";
		public const string STORE_BUTTON = "//store/button";
		public const string STORE_PURCHASE_SUCCESS = "//store/purchase_success";

		// Parameters class holds the parameters names that are published with certain messages
		public class Parameters {
			public class Store {
				public const string SECTION = "section";
				public class Purchase {
					public const string ITEM = "item";
					public const string PRICE = "price";
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		_LOG = new StringBuilder();

		// Uncommenting this line will invoke this callback for every message sent!
		//June.MessageBroker.Subscribe(Messages.ROOT, GenerateCallbackHandler("Root"));

		// Subscribing to //home 
		June.MessageBroker.Subscribe(Messages.HOME, GenerateCallbackHandler("Home"));
		
		// Subscribing to //home/button
		June.MessageBroker.Subscribe(Messages.HOME_BUTTON, GenerateCallbackHandler("HomeButton"));
		
		// Subscribing to //store
		June.MessageBroker.Subscribe(Messages.STORE, GenerateCallbackHandler("Store"));
		
		// Subscribing to //store/button
		June.MessageBroker.Subscribe(Messages.STORE_BUTTON, GenerateCallbackHandler("StoreButton"));
		
		// Subscribing to //store/purchase_success
		June.MessageBroker.Subscribe(Messages.STORE_PURCHASE_SUCCESS, HandlePurchaseSuccess);
	}

	// Generates the callback handler for messages
	Action<string, IDictionary<string, object>> GenerateCallbackHandler(string subscriberName) {
		return (message, parameters) => {
			Log(string.Format("Name:{0} Message:{1} Parameters:{2}", subscriberName, message, SimpleJson.SimpleJson.SerializeObject(parameters)));
		};
	}

	// Custom callback handler for the purchase success message
	public void HandlePurchaseSuccess(string message, IDictionary<string, object> parameters) {
		Log(string.Format("YAY ! Successfully purchased {0}, for ${1}"
			,parameters[Messages.Parameters.Store.Purchase.ITEM] 
			,parameters[Messages.Parameters.Store.Purchase.PRICE]));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Log(string message) {
		June.DebugLogger.Log(message);
		_LOG.AppendLine(message);
	}

	void OnGUI() {

		using(var canvas = new JuneHorizontalSection(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
			using(var buttonSection = new JuneVerticalSection(GUILayout.ExpandWidth(true))) {

				GUILayout.Label("Publish Messages");

				GUILayout.Space(_PADDING);

				if(GUILayout.Button("Publish " + Messages.HOME)) {
					June.MessageBroker.Publish(Messages.HOME);
				}

				if(GUILayout.Button("Publish " + Messages.HOME_BUTTON)) {
					June.MessageBroker.Publish(Messages.HOME_BUTTON);
				}

				if(GUILayout.Button("Publish " + Messages.STORE)) {
					June.MessageBroker.Publish(Messages.STORE);
				}

				if(GUILayout.Button("Publish " + Messages.STORE_BUTTON)) {
					June.MessageBroker.Publish(Messages.STORE_BUTTON, Messages.Parameters.Store.SECTION, "combos");
				}

				if(GUILayout.Button("Publish " + Messages.STORE_PURCHASE_SUCCESS)) {
					June.MessageBroker.Publish(
						Messages.STORE_PURCHASE_SUCCESS, 
						new Dictionary<string, object>() { 
							{ Messages.Parameters.Store.Purchase.ITEM, "Ultimate Combo" },
							{ Messages.Parameters.Store.Purchase.PRICE, 0.99f }
						});
				}
			}

			GUILayout.Space(_PADDING);

			using(var logSection = new JuneVerticalSection(GUILayout.ExpandWidth(true))) {
				GUILayout.Label("LOG", GUILayout.ExpandWidth(true));

				GUILayout.Space(_PADDING);

				GUILayout.TextArea(_LOG.ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

			}
		}

		GUILayout.Space(_PADDING);

		using(var subscriberSection = new JuneVerticalSection(GUILayout.ExpandWidth(true))) {
			GUILayout.Label("Subscribers");

			GUILayout.Space(_PADDING);

			GUILayout.Label(((June.IMessageBroker)June.MessageBroker.Instance).ToString(), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}
	}
}

```