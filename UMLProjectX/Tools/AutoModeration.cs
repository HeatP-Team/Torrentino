using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UMLProjectX.DAL;

namespace UMLProjectX.Tools
{
    public class AutoModeration
    {
        private DataContext _db;
        public AutoModeration(DataContext context)
        {
            _db = context;
        }

        public bool PassAutoModeration(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            var regexedText = Regex.Replace(text.ToLower(), "[-.?!)(,:;]", "");
            var splittedText = regexedText.Split(' ');
            var banned = _db.ReadBanWords().Select(x => x.Content).ToList();

            foreach (var word in splittedText)
            {
                if (banned.Contains(word))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
