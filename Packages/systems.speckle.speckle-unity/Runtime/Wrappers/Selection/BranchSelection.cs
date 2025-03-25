using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Models;
using UnityEngine;

#nullable enable
namespace Speckle.ConnectorUnity.Wrappers.Selection
{
    [Serializable]
    public sealed class BranchSelection : OptionSelection<Model>
    {
        [field:
            SerializeField,
            Range(1, ServerLimits.BRANCH_GET_LIMIT),
            Tooltip("Number of branches to request")
        ]
        public int BranchesLimit { get; set; } = ServerLimits.OLD_BRANCH_GET_LIMIT;

        [field: SerializeField, Range(1, 100), Tooltip("Number of commits to request")]
        public int CommitsLimit { get; set; } = 25;

        [field: SerializeReference]
        public StreamSelection StreamSelection { get; private set; }
        public override Client? Client => StreamSelection.Client;

        public BranchSelection(StreamSelection streamSelection)
        {
            StreamSelection = streamSelection;
            Initialise();
        }

        public void Initialise()
        {
            StreamSelection.OnSelectionChange = RefreshOptions;
        }

        protected override string? KeyFunction(Model? value) => value?.id;

        public override void RefreshOptions()
        {
            Project? project = StreamSelection.Selected;
            if (project == null)
                return;
            IReadOnlyList<Model> models;
            try
            {
                var modelsCollection = Client!.Model.GetModels(project.id, BranchesLimit).GetAwaiter().GetResult();
                models = modelsCollection.items;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to refresh {this}\n{e}");
                models = Array.Empty<Model>();
            }
            GenerateOptions(models, (b, _) => b.name == "main");
        }
    }
}
