using System.Collections.Generic;
using System.Linq;

public class AccountVO
{
    public string Account = "";
    public string Password = "";
    public List<string> Servers;//后面的是最新的

    public string GetInfo()
    {
        var str = Account + "," + Password;
        return Servers == null ? str : Servers.Aggregate(str, (current, t) => current + ("," + t));
    }

    public void AddServer(string server)
    {
        if (Servers == null)
        {
            Servers = new List<string>();
        }
        for (int i = 0; i < Servers.Count; i++)
        {
            if (Servers[i] == server)
            {
                Servers.RemoveAt(i);
                break;
            }
        }
        Servers.Add(server);
    }

    public static AccountVO CreateData(string str)
    {
        AccountVO data = null;
        var index = str.IndexOf(",", System.StringComparison.Ordinal);
        if (index >= 0)
        {
            var arr = str.Split(',');
            data = new AccountVO
            {
                Account = arr[0], Password = arr[1], Servers = new List<string>()
            };
            for (var i = 2; i < arr.Length; i++)
            {
                data.Servers.Add(arr[i]);
            }
        }
        return data;
    }

    public static AccountVO CreateData(string ac, string pw, string sv)
    {
        return CreateData(ac + "," + pw + "," + sv);
    }

    public override string ToString()
    {
        return string.Format("Account: " + Account + ", Password: " + Password);
    }
}
