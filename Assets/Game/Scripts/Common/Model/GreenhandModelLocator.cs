using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Template.Auto.Greenhand;

namespace Assets.Game.Scripts.Common.Model
{
    class GreenhandModelLocator
    {
        #region Private Fields
        private static volatile GreenhandModelLocator instance;
        private static  readonly  object SynRoot = new object();

        private Greenhand greenhandTemplate;
        private Dictionary<int, GreenhandTemplate> typeDictionary;
        #endregion

        #region Public Fields

        public static readonly int FIRST_BATTLE = 1;
        public static readonly int FIRST_RAID = 2;
        #endregion

        #region Public Methods

        public static GreenhandModelLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SynRoot)
                    {
                        if (instance == null)
                        {
                            instance = new GreenhandModelLocator();;
                        }
                    }
                }
                return instance;
            }
        }

        public GreenhandTemplate BattleGreenhandTemplate()
        {
            return TemplateByType(FIRST_BATTLE);
        }

        public GreenhandTemplate TemplateByType(int type)
        {
            if (typeDictionary.Count == 0)
            {
                initTypeTemplate();
            }
            return typeDictionary[type];
        }



        public Greenhand Greenhand
        {
            get
            {
                return greenhandTemplate ??
                       (greenhandTemplate = Utils.Decode<Greenhand>(ResourcePath.FileGreenhandConfig));
            }
        }

        #endregion

        #region Private Methods

        private GreenhandModelLocator()
        {
            typeDictionary = new Dictionary<int, GreenhandTemplate>();
        }

        private void initTypeTemplate()
        {
            typeDictionary.Clear();
            Dictionary<int, GreenhandTemplate> _allTemp = Greenhand.GreenhandTmpls;
            foreach (KeyValuePair<int, GreenhandTemplate> pair in _allTemp)
            {
                typeDictionary.Add(pair.Value.GreenhandType, pair.Value);
            }
        }
        #endregion

    }
}
