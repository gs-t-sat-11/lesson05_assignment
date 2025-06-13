namespace Firebase.Sample.FirebaseAI {
  using UnityEngine;
  using UnityEngine.UI;
  using TMPro;

  public class FirebaseAILogicUI : MonoBehaviour {
    // UI要素
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private TextMeshProUGUI messageDisplayText;
    [SerializeField] private Button sendButton;
    [SerializeField] private ScrollRect scrollRect;

    // AIマネージャー
    private FirebaseAILogicManager aiManager;
    
    private void Start() {
      // マネージャーの参照を取得
      aiManager = FirebaseAILogicManager.Instance;
      
      // 初期状態
      //messageDisplayText.text = "";
      
      // ボタンイベント設定
      sendButton.onClick.AddListener(OnSendButtonClick);
      
      // AIからのメッセージ受信イベント設定
      aiManager.OnMessageReceived += OnMessageReceived;
    }
    
    private void OnDestroy() {
      // イベント解除
      if (aiManager != null) {
        aiManager.OnMessageReceived -= OnMessageReceived;
      }
    }

    // 送信ボタンクリック時の処理
    public void OnSendButtonClick() {
      string message = messageInputField.text;
      
      if (string.IsNullOrEmpty(message)) {
        return;
      }
      
      // 自分のメッセージを表示
      DisplayMessage("ユーザー: " + message);
      
      // メッセージを送信
      aiManager.SendMessage(message);
      
      // 入力フィールドをクリア
      messageInputField.text = "";
      messageInputField.ActivateInputField();
    }
    
    // AIからのメッセージ受信時の処理
    private void OnMessageReceived(string message) {
      DisplayMessage("AI: " + message);
    }
    
    // メッセージを表示
    private void DisplayMessage(string message) {
      // メッセージ追加
      messageDisplayText.text += (string.IsNullOrEmpty(messageDisplayText.text) ? "" : "\n") + message;
    }
  }
}