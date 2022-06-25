using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot_tg
{
    public class Information
    {
        private Dictionary<int, Dictionary<string, Dictionary<string, List<long>>>> cours = new Dictionary<int, Dictionary<string, Dictionary<string, List<long>>>>();
        public Information()
        {
        }
        public Information(Dictionary<int, Dictionary<string, Dictionary<string, List<long>>>> _cours)
        {
            Course = _cours;
        }
        public Dictionary<int, Dictionary<string, Dictionary<string, List<long>>>> Course
        {
            get => cours;
            set => cours = value;
        }
        public void AddCours(int _cours)
        {
            cours.Add(_cours, new Dictionary<string, Dictionary<string, List<long>>>());
        }
        public void AddEducationProgram(int _cours, string EP)
        {
            if (cours.ContainsKey(_cours))
            {
                var _ep = cours[_cours];
                _ep.Add(EP, new Dictionary<string, List<long>>());
            }
            else
            {
                cours.Add(_cours, new Dictionary<string, Dictionary<string, List<long>>>());
                AddEducationProgram(_cours, EP);
            }
        }
        public void AddGroup(int _cours, string EP, string group)
        {
            bool ok = cours.ContainsKey(_cours) && cours[_cours].ContainsKey(EP);
            if (ok)
                cours[_cours][EP].Add(group, new List<long>());
            else
            {
                AddEducationProgram(_cours, EP);
                AddGroup(_cours, EP, group);
            }
        }
        public void AddId(int _cours, string EP, string group, int _id)
        {
            bool ok = cours.ContainsKey(_cours) && cours[_cours].ContainsKey(EP) && cours[_cours][EP].ContainsKey(group);
            if (ok)
                cours[_cours][EP][group].Add(_id);
            else
            {
                AddGroup(_cours, EP, group);
                AddId(_cours, EP, group, _id);
            }
        }
        public List<long> GetId(int _cours, string EP, string group)
        {
            bool ok = cours.ContainsKey(_cours) && cours[_cours].ContainsKey(EP) && cours[_cours][EP].ContainsKey(group);
            if (ok) return cours[_cours][EP][group];
            return null;
        }
    }
}
