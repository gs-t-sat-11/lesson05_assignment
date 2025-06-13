
namespace Firebase.Sample.FirebaseAI {
  using Firebase;
  using Firebase.AI;
  using Firebase.Extensions;
  using System;
  using System.Threading.Tasks;
  using UnityEngine;

  public class FirebaseAILogicManager : MonoBehaviour {
    // シングルトンインスタンス
    private static FirebaseAILogicManager _instance;
    public static FirebaseAILogicManager Instance {
      get {
        if (_instance == null) {
          GameObject go = new GameObject("FirebaseAILogicManager");
          _instance = go.AddComponent<FirebaseAILogicManager>();
          DontDestroyOnLoad(go);
        }
        return _instance;
      }
    }
    
    // Firebase依存関係のステータス
    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    
    // Unityエディタで設定可能なモデル名
    public string ModelName = "gemini-2.0-flash";
    
    // メッセージイベント
    public delegate void MessageEvent(string message);
    public event MessageEvent OnMessageReceived;
    
    private void Awake() {
      if (_instance != null && _instance != this) {
        Destroy(gameObject);
        return;
      }
      
      _instance = this;
      DontDestroyOnLoad(gameObject);
      
      InitializeFirebase();
    }
    
    // Firebaseの初期化
    private void InitializeFirebase() {
      FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
        dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
          Debug.Log("Firebase Ready: " + FirebaseApp.DefaultInstance);
        } else {
          Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }
      });
    }
    
    // Google AI Backendのモデルを取得
    private GenerativeModel GetModel() {
      var backend = FirebaseAI.Backend.GoogleAI();
      return FirebaseAI.GetInstance(backend).GetGenerativeModel(ModelName);
    }
    
    // メッセージを送信する
    public async Task SendMessage(string message) {
      Debug.Log("Sending message to model: " + message);
      try {
        var response = await GetModel().GenerateContentAsync(message);
        Debug.Log("Response: " + response.Text);
        
        // イベント通知
        OnMessageReceived?.Invoke(response.Text);
      } catch (Exception ex) {
        Debug.LogError("Error sending message: " + ex.Message);
      }
    }
  }
}
