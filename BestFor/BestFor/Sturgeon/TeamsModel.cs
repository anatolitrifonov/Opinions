using System.Collections.Generic;

namespace BestFor.Sturgeon
{
    public class TeamsModel
    {
        public List<TeamModel> Teams { get; set; }

        public TeamsModel()
        {
            Teams = new List<TeamModel>();
        }
    }
}
