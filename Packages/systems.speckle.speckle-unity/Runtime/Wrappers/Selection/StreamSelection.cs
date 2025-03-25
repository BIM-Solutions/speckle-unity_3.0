using System;
using System.Collections.Generic;
using Speckle.Core.Api;
using Speckle.Core.Api.GraphQL.Models;
using UnityEngine;

#nullable enable
namespace Speckle.ConnectorUnity.Wrappers.Selection
{
    [Serializable]
    public sealed class StreamSelection : OptionSelection<Project>
    {
        private const int DefaultRequestLimit = 50;

        [field: SerializeField, Range(1, 100), Tooltip("Number of streams to request")]
        public int StreamsLimit { get; set; } = DefaultRequestLimit;

        [field: SerializeReference]
        public AccountSelection AccountSelection { get; private set; }

        public StreamSelection(AccountSelection accountSelection)
        {
            AccountSelection = accountSelection;
            Initialise();
        }

        public void Initialise()
        {
            AccountSelection.OnSelectionChange = RefreshOptions;
        }

        public override Client? Client => AccountSelection.Client;

        protected override string? KeyFunction(Project? value) => value?.id;

        public override void RefreshOptions()
        {
            if (Client == null)
                return;
            IReadOnlyList<Project> projects;
            try
            {
                var projectCollection = Client.ActiveUser.GetProjects(StreamsLimit).GetAwaiter().GetResult();
                projects = projectCollection.items;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to refresh {this}\n{e}");
                projects = Array.Empty<Project>();
            }
            GenerateOptions(projects, (_, i) => i == 0);
        }
    }
}
