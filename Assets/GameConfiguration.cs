using System.Xml.Linq;
using UnityEngine;

public class GameConfiguration : Singleton<GameConfiguration>
{
    private const string GameConfigurationFile = "GameConfiguration.xml";
    private string gameConfigurationPath;

    public ClickEffectHandler ClickEffect;

    private void WriteToXml()
    {
        var doc = new XDocument(new XElement("Root",
                                       new XElement("ClickBehaviour",
                                           new XElement("ContinousMode", "false"),
                                           new XElement("TimeInterval", "0"))));
        doc.Save(gameConfigurationPath);
    }

    private void ReadXml()
    {
        var doc = XElement.Load(gameConfigurationPath);
        var clickElement = doc.Element("ClickBehaviour");
        var continousMode = clickElement.Element("ContinousMode").Value;
        var timeInterval = clickElement.Element("TimeInterval").Value;
        ClickEffect.ContinousMode = bool.Parse(continousMode);
        ClickEffect.TimeInterval = float.Parse(timeInterval);
    }

    #region Mono
    private void Start()
    {
        gameConfigurationPath = string.Format("{0}/{1}", Application.streamingAssetsPath, GameConfigurationFile);

        ReadXml();
    }

    #endregion
}
