
/// <summary>
/// SelectAccount used to generate specific game event.
/// </summary>
public class SelectAccountEvent : GameEvent
{
    public static string TYPE_CHANGE = "change";
    public static string TYPE_DELETE = "delete";
    public string type;
    public AccountVO account;
}
