using System;
using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Models;
using UnityEngine;
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

        public override void RefreshOptions()
        {
            Model? model = BranchSelection.Selected;
            if (model == null)
                return;
            List<Version> versions = model.versions.items;
            GenerateOptions(versions, (_, i) => i == 0);
        }
    }
}
