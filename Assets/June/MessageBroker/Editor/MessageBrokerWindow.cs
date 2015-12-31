using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Message broker window.
/// </summary>
public class MessageBrokerWindow : EditorWindow {

	private const string _HIGHTLIGHT_TIME_KEY = "MessageBroker_HighlightTime";
	private const string _ACTIVE_COLOUR_KEY = "MessageBroker_ActiveColour";
	private const string _NORMAL_COLOUR_KEY = "MessageBroker_NormalColour";

	[MenuItem("June/MessageBroker")]
	public static void ShowMessageBrokerWindow() {
		GetWindow<MessageBrokerWindow>("MessageBrokerWindow");
	}

	private Vector2 _ScrollPosition;
	private static Dictionary<June.IMessageBroker, Color> _MESSAGE_COLOR = new Dictionary<June.IMessageBroker, Color>();
	private static Dictionary<KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>>, Color> _MESSAGE_QUERY_COLOR = new Dictionary<KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>>, Color>();

	/// <summary>
	/// Gets or sets the HIGHLIGHT TIME.
	/// </summary>
	/// <value>The HIGHLIGH t_ TIM.</value>
	private static float HIGHLIGHT_TIME {
		get {
			return EditorPrefs.HasKey(_HIGHTLIGHT_TIME_KEY) ? EditorPrefs.GetFloat(_HIGHTLIGHT_TIME_KEY) : 2f;
		}
		set {
			EditorPrefs.SetFloat(_HIGHTLIGHT_TIME_KEY, value);
		}
	}

	private static bool _NormalContentColourFetched = false;
	private static Color _NormalContentColour = Color.white;
	private static Color NORMAL_CONTENT_COLOUR {
		get {
			if(false == _NormalContentColourFetched) {
				var values = (EditorPrefs.HasKey(_NORMAL_COLOUR_KEY) ? EditorPrefs.GetString(_NORMAL_COLOUR_KEY) : "1,1,1,1").Split(',');
				_NormalContentColour = new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
				_NormalContentColourFetched = true;
			}
			return _NormalContentColour;
		}
		set {
			_NormalContentColour = value;
			EditorPrefs.SetString(_NORMAL_COLOUR_KEY, 
				string.Join(",", 
		            new string[] { 
						_NormalContentColour.r.ToString(),
						_NormalContentColour.g.ToString(),
						_NormalContentColour.b.ToString(),
						_NormalContentColour.a.ToString()
					}));
		}
	}

	private static bool _ActiveContentColourFetched = false;
	private static Color _ActiveContentColour = Color.red;
	/// <summary>
	/// Gets the ACTIVE CONTENT COLOUR.
	/// </summary>
	/// <value>The ACTIV e_ CONTEN t_ COLOU.</value>
	private static Color ACTIVE_CONTENT_COLOUR {
		get {
			if(false == _ActiveContentColourFetched) {
				var values = (EditorPrefs.HasKey(_ACTIVE_COLOUR_KEY) ? EditorPrefs.GetString(_ACTIVE_COLOUR_KEY) : "1,0,0,1").Split(',');
				_ActiveContentColour = new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
				_ActiveContentColourFetched = true;
			}
			return _ActiveContentColour;
		}
		set {
			_ActiveContentColour = value;
			EditorPrefs.SetString(_ACTIVE_COLOUR_KEY, 
				string.Join(",", 
		            new string[] { 
						_ActiveContentColour.r.ToString(),
						_ActiveContentColour.g.ToString(),
						_ActiveContentColour.b.ToString(),
						_ActiveContentColour.a.ToString()
					}));
		}
	}

	private static List<string> LOG = new List<string>();

	private static GUIStyle[] _LogStyle = null;
	private static GUIStyle[] LogStyle {
		get {
			if(null == _LogStyle) {
				_LogStyle = new GUIStyle[2];
				_LogStyle[0] = new GUIStyle();
				//_LogStyle[0].normal.background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				_LogStyle[0].normal.textColor = Color.white;
				//Debug.Log("[EDITOR COLOR] " + GUI.skin.label.normal.background.GetPixel(0,0).ToString());
				//_LogStyle[0].normal.background.SetPixel(0, 0, Color.blue); 
				_LogStyle[1] = new GUIStyle();
				//_LogStyle[1].normal.background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
				//_LogStyle[1].normal.background.SetPixel(0, 0, Color.black); 
				_LogStyle[1].normal.textColor = Color.grey;

			}
			return _LogStyle;
		}
	}

	private static GUIStyle GetLableStyle(Color colour) {
		GUIStyle s = new GUIStyle(EditorStyles.boldLabel);
		s.normal.textColor = colour;
		return s;
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	private void OnEnable() {
		InitCallback();
		_ResizeCurrentHeight = this.position.height/2;
		_ResizeRect = new Rect(0,_ResizeCurrentHeight,this.position.width,5f);
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void InitCallback() {
		Debug.Log("[MessageBrokerWindow] Setting callback");
		June.MessageBroker._OnBrokerPublish = SetBrokerActive;
		June.MessageBroker._OnQueryCallbackPublish = SetQueryParameterActive;
		June.MessageBroker._OnPublish = LogPublish;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update() {
		// Callbacks
		var keys = _MESSAGE_COLOR.Keys.ToList();
		foreach(var key in keys) {
			_MESSAGE_COLOR[key] = Color.Lerp(_MESSAGE_COLOR[key], NORMAL_CONTENT_COLOUR, Time.deltaTime * (2f/HIGHLIGHT_TIME));

			if(_MESSAGE_COLOR[key] == NORMAL_CONTENT_COLOUR) {
				_MESSAGE_COLOR.Remove(key);
			}
		}

		// Parameter Query Callbacks
		var qkeys = _MESSAGE_QUERY_COLOR.Keys.ToList();
		foreach(var key in qkeys) {
			_MESSAGE_QUERY_COLOR[key] = Color.Lerp(_MESSAGE_QUERY_COLOR[key], NORMAL_CONTENT_COLOUR, Time.deltaTime * (2f/HIGHLIGHT_TIME));
			
			if(_MESSAGE_QUERY_COLOR[key] == NORMAL_CONTENT_COLOUR) {
				_MESSAGE_QUERY_COLOR.Remove(key);
			}
		}
		Repaint();
	}

	private bool _ToggleLog = false;
	private Vector2 _LogScroll;
	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	private void OnGUI() {
		bool notRegisted = (null == June.MessageBroker._OnBrokerPublish);
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar); {
			EditorGUILayout.LabelField("Message Broker (Subscribers)", EditorStyles.boldLabel, GUILayout.MaxWidth(150f), GUILayout.ExpandWidth(true));
			if(position.width >= 500) {
				RenderMessageBrokerToolgBar (notRegisted);
			}
		} EditorGUILayout.EndHorizontal();
		if(position.width < 500) {
			RenderMessageBrokerToolgBar (notRegisted);
		}


		EditorGUILayout.BeginVertical(); {
			_ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);
			RenderIMessageBroker(June.MessageBroker.Instance);
			EditorGUILayout.EndScrollView();
		} EditorGUILayout.EndVertical();

		RenderLog();
	}

	void RenderMessageBrokerToolgBar (bool notRegisted) {
		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, (position.width > 500f ? GUILayout.MaxWidth (200f) : GUILayout.ExpandWidth(true))); {
			EditorGUILayout.LabelField ("Default", GUILayout.MaxWidth (50f));
			NORMAL_CONTENT_COLOUR = EditorGUILayout.ColorField (NORMAL_CONTENT_COLOUR);
			EditorGUILayout.LabelField ("Active", GUILayout.MaxWidth (50f));
			ACTIVE_CONTENT_COLOUR = EditorGUILayout.ColorField (ACTIVE_CONTENT_COLOUR);
			EditorGUILayout.LabelField ("Duration", GUILayout.MaxWidth (50f));
			HIGHLIGHT_TIME = EditorGUILayout.FloatField (HIGHLIGHT_TIME, EditorStyles.toolbarTextField, GUILayout.MaxWidth (20f));
			GUI.contentColor = notRegisted ? ACTIVE_CONTENT_COLOUR : NORMAL_CONTENT_COLOUR;
			if (GUILayout.Button ("Refresh", EditorStyles.toolbarButton, GUILayout.MinWidth (60f), GUILayout.MaxWidth (70f))) {
				LOG.Clear ();
				_MESSAGE_COLOR.Clear ();
				InitCallback ();
			}
			GUI.contentColor = NORMAL_CONTENT_COLOUR;
		}
		EditorGUILayout.EndHorizontal ();
	}

	/// <summary>
	/// Gets the broker colour.
	/// </summary>
	/// <returns>The broker colour.</returns>
	/// <param name="broker">Broker.</param>
	private static GUIStyle GetBrokerColour(June.IMessageBroker broker) {
		if(_MESSAGE_COLOR.ContainsKey(broker)) {
			//return _MESSAGE_COLOR[broker];
			return GetLableStyle(_MESSAGE_COLOR[broker]);
		}
		return GetLableStyle(NORMAL_CONTENT_COLOUR);
	}

	/// <summary>
	/// Gets the color of the query callback.
	/// </summary>
	/// <returns>The query callback color.</returns>
	/// <param name="queryCallback">Query callback.</param>
	private static GUIStyle GetQueryCallbackColor(KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>> queryCallback) {
		if(_MESSAGE_QUERY_COLOR.ContainsKey(queryCallback)) {
			//return _MESSAGE_COLOR[broker];
			return GetLableStyle(_MESSAGE_QUERY_COLOR[queryCallback]);
		}
		return GetLableStyle(NORMAL_CONTENT_COLOUR);
	}

	/// <summary>
	/// Sets the broker active.
	/// </summary>
	/// <param name="broker">Broker.</param>
	public static void SetBrokerActive(June.IMessageBroker broker) {
		//if(_MESSAGE_COLOR.ContainsKey(broker)) {
			_MESSAGE_COLOR[broker] = ACTIVE_CONTENT_COLOUR;
		//}
		LogPublish(broker.UriPath);
	}

	/// <summary>
	/// Sets the query parameter active.
	/// </summary>
	/// <param name="queryCallback">Query callback.</param>
	public static void SetQueryParameterActive(KeyValuePair<KeyValuePair<string, object>[], Action<string, IDictionary<string, object>>> queryCallback) {
		_MESSAGE_QUERY_COLOR[queryCallback] = ACTIVE_CONTENT_COLOUR;
		LogPublish("?" + queryCallback.Key[0].Key);
	}

	/// <summary>
	/// Logs the publish.
	/// </summary>
	/// <param name="uri">URI.</param>
	public static void LogPublish(string uri) {
		LOG.Add(string.Format("[{0:hh:mm:ss.fff}] {1}", DateTime.Now, uri));
	}

	/// <summary>
	/// Renders the I message broker.
	/// </summary>
	/// <param name="broker">Broker.</param>
	private void RenderIMessageBroker(June.IMessageBroker broker) {

		//Render Subscribers
		foreach(var kv in broker._SUBSCRIBERS) {
			EditorGUILayout.BeginHorizontal(EditorStyles.largeLabel); {
				GUIStyle style = MessageBrokerWindow.GetBrokerColour(kv.Value);
				//GUI.contentColor = MessageBrokerWindow.GetBrokerColour(kv.Value);
				EditorGUILayout.LabelField(
					string.Format("/{0} :: Callbacks:{1} ParamCallbacks:{2} Events:{3}", 
				              kv.Key, 
				              kv.Value._CALLBACKS.Count, 
				              ((June.MessageBrokerFragment)kv.Value)._QUERY_CALLBACK.Count,
				              CountOfEventsSubscribingToMessage(kv.Value.UriPath)), 
                	style);
			} EditorGUILayout.EndHorizontal();

			if(((June.MessageBrokerFragment)kv.Value)._QUERY_CALLBACK.Count > 0) {

				EditorGUI.indentLevel += 1;
				foreach(var query in ((June.MessageBrokerFragment)kv.Value)._QUERY_CALLBACK) {
					GUIStyle style = GetQueryCallbackColor(query);
					EditorGUILayout.LabelField(
						string.Format("{0}{1}", 
							June.MessageBroker.QUERY_START,
							string.Join(June.MessageBroker.QUERY_APPEND.ToString(),
					            Array.ConvertAll(query.Key, 
									qp => string.Format("{0}{1}{2}", 
					                    	qp.Key,  
					                    	June.MessageBroker.QUERY_KEY_VALUE_SEPARATOR,
					                    	qp.Value.ToString())))),
						style);
				}
				EditorGUI.indentLevel -= 1;
			}

			if(kv.Value._SUBSCRIBERS.Count > 0) {
				EditorGUI.indentLevel += 1;
				RenderIMessageBroker(kv.Value);
				EditorGUI.indentLevel -= 1;
			}
		}
	}

	/// <summary>
	/// Counts the of events subscribing to message.
	/// </summary>
	/// <returns>The of events subscribing to message.</returns>
	/// <param name="messageUri">Message URI.</param>
	private int CountOfEventsSubscribingToMessage(string messageUri) {
		return 0;
		//return June.Analytics.AnalyticsManager.EVENTS.Count(ev => 0 == string.Compare(ev.Value.SubscribedMessage, messageUri, true));
	}

	private void RenderLog() { 
		if(_ToggleLog) {
			ResizeScrollView();
		}

		EditorGUILayout.BeginVertical((_ToggleLog ? GUILayout.MinHeight(this.position.height - _ResizeCurrentHeight) : GUILayout.MaxHeight(15f))); {
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar); {
				_ToggleLog = EditorGUILayout.Foldout(_ToggleLog, "Log (Publishers)");
				EditorGUILayout.LabelField(string.Format("{0} messages", LOG.Count), GUILayout.MaxWidth(80f));
				if(GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.MinWidth(50f), GUILayout.MaxWidth(60f))) {
					LOG.Clear();
				}
			} EditorGUILayout.EndHorizontal();
			if(_ToggleLog) {
				_LogScroll = EditorGUILayout.BeginScrollView(_LogScroll); {
					if(null != LOG && LOG.Count > 0) {
						for(int i=LOG.Count-1; i>=0; i--) {
							if(!string.IsNullOrEmpty(LOG[i])) {
								EditorGUILayout.LabelField(LOG[i], LogStyle[i%2], GUILayout.ExpandWidth(true));
							}
							//EditorGUILayout.LabelField(LOG[i], LogStyle[i%2], GUILayout.ExpandWidth(true));
						}
					}
				} EditorGUILayout.EndScrollView();
			}
		} EditorGUILayout.EndVertical();
	}
	
	float _ResizeCurrentHeight;
	bool _IsResize = false;
	Rect _ResizeRect;

	private void ResizeScrollView(){
		_ResizeRect = new Rect(0,_ResizeCurrentHeight,this.position.width,5f);
		GUI.DrawTexture(_ResizeRect,EditorGUIUtility.whiteTexture);
		EditorGUIUtility.AddCursorRect(_ResizeRect,MouseCursor.ResizeVertical);
		
		if( Event.current.type == EventType.mouseDown && _ResizeRect.Contains(Event.current.mousePosition)){
			_IsResize = true;
		}
		if(_IsResize){
			_ResizeCurrentHeight = Event.current.mousePosition.y;
			_ResizeRect.Set(_ResizeRect.x,_ResizeCurrentHeight,_ResizeRect.width,_ResizeRect.height);
		}
		if(Event.current.type == EventType.MouseUp)
			_IsResize = false;        
	}

}
