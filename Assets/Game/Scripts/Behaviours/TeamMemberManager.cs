using System.Collections.Generic;
using System.Linq;

public class TeamMemberManager : Singleton<TeamMemberManager>
{
    public sbyte CurTeamIndex;
    public List<long> CurTeam = new List<long>();
    public List<long> Teams = new List<long>();

    /// <summary>
    /// Set the data of team member manager.
    /// </summary>
    /// <param name="teamIndex"></param>
    public void SetValue(sbyte teamIndex)
    {
        var heroList = HeroModelLocator.Instance.SCHeroList;
        if(heroList != null)
        {
            CurTeam.Clear();
            Teams.Clear();
            CurTeamIndex = teamIndex;
            for(var i = 0; i < heroList.TeamList.Count; i++)
            {
                var uUids = heroList.TeamList[i].ListHeroUuid.Where(id => id != HeroConstant.NoneInitHeroUuid).ToList();
                if(teamIndex == i)
                {
                    CurTeam = uUids.ToList();
                }
                Teams.AddRange(uUids);
            }
            Teams = Teams.Distinct().ToList();
        }
    }
}
