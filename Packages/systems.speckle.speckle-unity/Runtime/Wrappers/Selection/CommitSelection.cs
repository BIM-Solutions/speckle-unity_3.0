using System;
using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Models;
using UnityEngine;
using Project = Speckle.Core.Api.GraphQL.Models.Project;
using Version = Speckle.Core.Api.GraphQL.Models.Version;

#nullable enable
namespace Speckle.ConnectorUnity.Wrappers.Selection
{
    [Serializable]
    public sealed class CommitSelection : OptionSelection<Version>
    {
        [field: SerializeReference]
        public BranchSelection BranchSelection { get; private set; }

        public override Client? Client => BranchSelection.Client;

        public CommitSelection(BranchSelection branchSelection)
        {
            BranchSelection = branchSelection;
            Initialise();
        }

        public void Initialise()
        {
            BranchSelection.OnSelectionChange = RefreshOptions;
        }

        protected override string? KeyFunction(Version? value) => value?.id;

        public override async void RefreshOptions()
        {
            Model? branch = BranchSelection.Selected;
            if (branch == null)
                return;
            List<Version> commits;
            try
            {
                Project? project = this.BranchSelection.StreamSelection.Selected;
                if (project == null)
                {
                    Debug.LogWarning("Project is null");
                    return;
                }
                var model = await Client!.Model.GetWithVersions(branch.id, project.id);
                commits = model.versions.items;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to refresh {this}\n{e}");
                commits = new List<Version>();
            }
            GenerateOptions(commits, (_, i) => i == 0);
        }
    }
}
