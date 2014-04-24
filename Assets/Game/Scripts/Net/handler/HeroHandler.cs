using KXSGCodec;

namespace Assets.Game.Scripts.Net.handler
{
    public class HeroHandler
    {
        public static void OnHeroList(ThriftSCMessage msg)
        {
            HeroModelLocator.Instance.SCHeroList = msg.getContent() as SCHeroList;
            WindowManager.Instance.Show(typeof(UIHerosDisplayWindow), true);
        }
    }
}


