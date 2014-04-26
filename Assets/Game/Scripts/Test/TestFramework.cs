using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Test framwork which come with a GUI menu feature testing system.
/// </summary>
public class TestFramework : MonoBehaviour
{
    #region Public Fields

    public BattleEndControl BattenEnd;

    #endregion

    #region Private Fields

    private string assertType = "Ok";
    private string assertMessage = "This is a assert message.";

    #endregion

    #region Mono

    void OnGUI()
    {
        SingletonTest();

        WindowManagerTest();

        EventTest();

        UIEventTest();

        AssetWindowTest();
    }

    void Start()
    {
        GameManager.Instance.Play();
    }

    #endregion

    #region Private Methods

    private static void SingletonTest()
    {
        GUILayout.Label("Singleton Test");

        if (GUILayout.Button("New Scene"))
        {
            Application.LoadLevel("SingletonTestScene");
        }

        if (GUILayout.Button("Back Scene"))
        {
            Application.LoadLevel("BattleScene");
        }

        if (GUILayout.Button("Print"))
        {
            GameManager.Instance.Counter++;
            GameManager.Instance.Print();
        }
    }

    private void WindowManagerTest()
    {
        GUILayout.Label("Window Manager Test");

        //if (GUILayout.Button("Battle End Popup Show."))
        if (GUILayout.Button("Battle Win Popup Show."))
        {
            var currentScreenWindow = WindowManager.Instance.CurrentWindowMap[WindowGroupType.Screen];
            var battle = currentScreenWindow.GetComponent<InitBattleField>();
            battle.enabled = false;

            //var battleEnd = WindowManager.Instance.Show(WindowType.BattleEnd, true);
            //var battleEndController = battleEnd.GetComponent<BattleEndControl>();
            //battleEndController.Show();
            var battleWin = WindowManager.Instance.Show(typeof(BattleWinWindow), true);
            var battleWinControl = battleWin.GetComponent<BattleWinControl>();
            battleWinControl.Show();
        } 
        if (GUILayout.Button("Heros"))
        {
            WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), true);
        }
    }

    private void EventTest()
    {
        GUILayout.Label("Event Manager Test");

        if (GUILayout.Button("Add Listener"))
        {
            EventManager.Instance.AddListener<TestEvent>(OnTestHandler);
        }

        if (GUILayout.Button("Remove Listener"))
        {
            EventManager.Instance.RemoveListener<TestEvent>(OnTestHandler);
        }

        if (GUILayout.Button("Post Event"))
        {
            EventManager.Instance.Post(new TestEvent {Message = "This is a test event."});
        }
    }

    private void UIEventTest()
    {
        if (GUILayout.Button("Show Hero Window"))
        {
            WindowManager.Instance.Show(typeof(HeroWindow), true);
        }

        if (GUILayout.Button("Hide Hero Window"))
        {
            WindowManager.Instance.Show(typeof(HeroWindow), false);
        }
    }

    private static void OnTestHandler(TestEvent e)
    {
        Debug.LogWarning("OnTestHandler get called back, with event-" + e.Message);
    }

    private void AssetWindowTest()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Assert Message: ");
        assertMessage = GUILayout.TextField(assertMessage);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Assert Type: ");
        assertType = GUILayout.TextField(assertType);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Show Assert Window"))
        {
            var assertWindow = WindowManager.Instance.GetWindow(typeof(AssertionWindow)) as AssertionWindow;
            assertWindow.AssertType = (AssertionWindow.Type)Enum.Parse(typeof(AssertionWindow.Type), assertType);
            assertWindow.Message = assertMessage;
            WindowManager.Instance.Show(typeof(AssertionWindow), true);

            Debug.LogWarning("Show assert window.");

            assertWindow.OkButtonClicked += OnOkButtonClicked;
            assertWindow.CancelButtonClicked += OnCancelButtonClicked;
        }

        if (GUILayout.Button("Hide Assert Window"))
        {
            var assertWindow = WindowManager.Instance.Show(typeof(AssertionWindow), false) as AssertionWindow;

            assertWindow.OkButtonClicked -= OnOkButtonClicked;
            assertWindow.CancelButtonClicked -= OnCancelButtonClicked;
        }
    }

    private void OnOkButtonClicked(GameObject sender)
    {
        Debug.LogWarning("Ok button is clicked. " + sender.name);
    }

    private void OnCancelButtonClicked(GameObject sender)
    {
        Debug.LogWarning("Cancel button is clicked. " + sender.name);
    }

    #endregion
}
