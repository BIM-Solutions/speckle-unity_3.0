using System.Collections.Generic;
using System.Threading.Tasks;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Models;
using Speckle.Core.Credentials;
using Speckle.Core.Models;

namespace Speckle.ConnectorUnity
{
    public static class Projects
    {   
        
        public static async Task<List<Project>> List(int limit = 10)
        {
            var account = AccountManager.GetDefaultAccount();
            if (account == null)
                return new List<Project>();
            var client = new Client(account);

            var res = await client.ActiveUser.GetProjects(limit);

            return res.items;
        }

        public static async Task<Project> Get(string streamId, int limit = 10)
        {
            var account = AccountManager.GetDefaultAccount();
            if (account == null)
                return null;
            var client = new Client(account);

            var res = await client.Project.GetWithModels(streamId, limit);

            //if (res.versions.items != null)
            //{
            //    res.versions.items.Reverse();
            //}

            return res;
        }
    }
}
