using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernityAnalyzer
{
    public class Repositories
    {

        public Dictionary<string, string> listOfRepos;
        
        public Repositories() 
        {
            // Files repo
            listOfRepos = new Dictionary<string, string>();
            //listOfRepos.Add("Files", "https://github.com/files-community/Files.git");
            //listOfRepos.Add("ShareX", "https://github.com/ShareX/ShareX.git");
            listOfRepos.Add("aspire", "https://github.com/dotnet/aspire.git");


        }

    }

}
