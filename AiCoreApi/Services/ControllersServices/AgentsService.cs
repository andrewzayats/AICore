using System.IO.Compression;
using AiCoreApi.Models.ViewModels;
using AiCoreApi.Data.Processors;
using AiCoreApi.Models.DbModels;
using AiCoreApi.SemanticKernel.Agents;
using AutoMapper;
using AiCoreApi.Common.Extensions;
using static AiCoreApi.SemanticKernel.Agents.CompositeAgent;
using Microsoft.Extensions.Caching.Distributed;
using AiCoreApi.Common;

namespace AiCoreApi.Services.ControllersServices;

public class AgentsService : IAgentsService
{
    private readonly IMapper _mapper;
    private readonly IAgentsProcessor _agentsProcessor;
    private readonly IDistributedCache _distributedCache;
    private readonly ILoginProcessor _loginProcessor;
    private readonly RequestAccessor _requestAccessor;

    public AgentsService(
        IAgentsProcessor agentsProcessor, 
        IMapper mapper,
        IDistributedCache distributedCache,
        ILoginProcessor loginProcessor,
        RequestAccessor requestAccessor)
    {
        _agentsProcessor = agentsProcessor;
        _mapper = mapper;
        _distributedCache = distributedCache;
        _loginProcessor = loginProcessor;
        _requestAccessor = requestAccessor;
    }

    public async Task<AgentViewModel> AddAgent(AgentViewModel agentViewModel)
    {
        if (agentViewModel.AgentId != 0)
            throw new ArgumentException("Value should be 0.", nameof(AgentViewModel.AgentId));

        var agentModel = _mapper.Map<AgentModel>(agentViewModel);
        var savedModel = await _agentsProcessor.Add(agentModel);
        var result = _mapper.Map<AgentViewModel>(savedModel);
        return result;
    }

    public async Task<AgentViewModel> UpdateAgent(AgentViewModel agentViewModel)
    {
        if (agentViewModel.AgentId == 0)
            throw new ArgumentException("Value should be not 0.", nameof(AgentViewModel.AgentId));

        var agentModel = _mapper.Map<AgentModel>(agentViewModel);
        var savedModel = await _agentsProcessor.Update(agentModel);
        var result = _mapper.Map<AgentViewModel>(savedModel);
        return result;
    }

    public async Task<List<AgentViewModel>> ListAgents()
    {
        var agents = await _agentsProcessor.List();
        var agentsViewModelList = _mapper.Map<List<AgentViewModel>>(agents);
        return agentsViewModelList.OrderBy(agent => agent.AgentId).ToList();
    }

    public async Task DeleteAgent(int agentId)
    {
        await _agentsProcessor.Delete(agentId);
    }

    public async Task<List<ParameterModel>?> GetParameters(int agentId)
    {
        var agentViewModel = (await ListAgents()).FirstOrDefault(agent => agent.AgentId == agentId);
        if (agentViewModel == null)
            return null;        
        if(!agentViewModel.Content.ContainsKey("parameterDescription") || string.IsNullOrEmpty(agentViewModel.Content["parameterDescription"].Value))
            return null;
        var parameterId = 1;
        var result = agentViewModel.Content["parameterDescription"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(parameter => new ParameterModel
        {
            Name = $"parameter{parameterId++}",
            Description = parameter
        }).ToList();
        return result;

    }

    public async Task SwitchEnableAgent(int agentId, bool isEnabled)
    {
        var agent = await _agentsProcessor.GetById(agentId);
        if (agent == null)
        {            
            return;
        }
        agent.IsEnabled = isEnabled;
        await _agentsProcessor.Update(agent);
    }

    public async Task<bool> IsAgentEnabled(string agentName)
    {
        var agents = await _agentsProcessor.List();
        var agent = agents.FirstOrDefault(a => a.Name == agentName);
        if (agent == null)
            return true;
        var allUserTags = await _loginProcessor.GetTagsByLogin(_requestAccessor.Login, _requestAccessor.LoginType);
        return agent.IsEnabled && (agent.Tags.Count == 0 || agent.Tags.Select(x => x.TagId).Any(allUserTags.Select(x => x.TagId).Contains));
    }

    public async Task<byte[]> ExportAgents(List<int> agentIdsList)
    {
        var agents = await _agentsProcessor.List();
        // Add Agents to export list if they are not composite
        var agentsToExport = agents.Where(agent => 
                agentIdsList.Contains(agent.AgentId) 
                && agent.Type != Models.DbModels.AgentType.Composite
            ).ToList();

        // Handle Composite Agents
        var compositeAgents = agents.Where(agent => 
                agentIdsList.Contains(agent.AgentId) 
                && agent.Type == Models.DbModels.AgentType.Composite
            ).ToList();
        await HandleCompositeAgents(agentsToExport, agents, compositeAgents);

        var agentsToExportResult = _mapper.Map<List<AgentExportModel>>(agentsToExport);
        using (var zipStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var agent in agentsToExportResult)
                {
                    var entry = archive.CreateEntry($"{agent.Name}.json");
                    using (var entryStream = entry.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        await streamWriter.WriteAsync(agent.ToJson());
                    }
                }
            }
            var zipBytes = zipStream.ToArray();
            return zipBytes;
        }
    }

    private async Task HandleCompositeAgents(List<AgentModel> agentsToExport, List<AgentModel> allAgents, List<AgentModel> compositeAgents)
    {
        foreach (var compositeAgent in compositeAgents)
        {
            var subAgentIds = compositeAgent.Content[CompositeAgent.AgentContentParameters.AgentsList]
                .Value.JsonGet<Dictionary<string, bool>>()!
                .Select(item => item.Key).ToList();

            var subAgentsNames = subAgentIds.Select(subAgentId => allAgents.FirstOrDefault(agent => agent.AgentId.ToString() == subAgentId)?.Name).ToList();
            compositeAgent.Content[AgentContentParameters.AgentsList].Value = subAgentsNames.ToJson();
            agentsToExport.Add(compositeAgent);

            // Add Agents to export list if they are not composite
            var subAgentsToExport = allAgents.Where(agent => 
                    subAgentIds.Contains(agent.AgentId.ToString()) 
                    && agent.Type != Models.DbModels.AgentType.Composite
                    && !agentsToExport.Select(item => item.AgentId).Contains(agent.AgentId)
                )
                .ToList();
            agentsToExport.AddRange(subAgentsToExport);

            // Handle Composite Agents
            var subCompositeSubAgents = allAgents.Where(agent => 
                    subAgentIds.Contains(agent.AgentId.ToString()) 
                    && agent.Type == Models.DbModels.AgentType.Composite
                    && !agentsToExport.Select(item => item.AgentId).Contains(agent.AgentId)
                ).ToList();

            await HandleCompositeAgents(agentsToExport, allAgents, subCompositeSubAgents);
        }
    }

    public async Task<ImportAgentsResultModel> ImportAgents(IFormFile file, Dictionary<Models.ViewModels.AgentType, int> agentVersions)
    {
        var agentExportModels = new List<AgentExportModel>();
        using (var stream = file.OpenReadStream())
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries)
            {
                using (var entryStream = entry.Open())
                {
                    using (var streamReader = new StreamReader(entryStream))
                    {
                        var agentJson = await streamReader.ReadToEndAsync();
                        var agent = agentJson.JsonGet<AgentExportModel>();
                        if(agent != null)
                            agentExportModels.Add(agent);
                    }
                }
            }
        }

        var agents = await _agentsProcessor.List();
        var overridingAgents = agentExportModels
            .Where(agent => agents
                .Any(item => item.Name == agent.Name))
            .ToList();
        var agentsWithOldVersion = agentExportModels
            .Where(agent => agentVersions
                .FirstOrDefault(item => item.Key.ToString() == agent.Type).Value > agent.Version)
            .ToList();

        if(overridingAgents.Count == 0 && agentsWithOldVersion.Count == 0)
        {
            await ImportAgentsConfirmed(agentExportModels);
            return new ImportAgentsResultModel
            {
                IsSuccess = true
            };
        }
        var agentImportId = Guid.NewGuid().ToString();

        await _distributedCache.SetStringAsync(GetAgentImportCacheKey(agentImportId), agentExportModels.ToJson(), 
            new DistributedCacheEntryOptions {SlidingExpiration = new TimeSpan(0, 20,  0) });

        var confirmationMessages = new List<string>();
        if (overridingAgents.Count > 0)
        {
            confirmationMessages.Add($"The following agents are already present in the system:");
            foreach (var overridingAgent in overridingAgents)
                confirmationMessages.Add($"- {overridingAgent.Name} ({overridingAgent.Type})");
            confirmationMessages.Add("");
        }
        if (agentsWithOldVersion.Count > 0)
        {
            confirmationMessages.Add($"The following agents have older versions format:");
            foreach (var agentWithOldVersion in agentsWithOldVersion)
                confirmationMessages.Add($"- {agentWithOldVersion.Name} ({agentWithOldVersion.Type})");
            confirmationMessages.Add("");
        }
        confirmationMessages.Add($"Do you want to proceed?");
        return new ImportAgentsResultModel
        {
            IsSuccess = false,
            ConfirmationId = agentImportId,
            ConfirmationText = confirmationMessages,
        };
    }

    public async Task ConfirmImportAgents(string confirmationId)
    {
        var importAgents = await _distributedCache.GetStringAsync(GetAgentImportCacheKey(confirmationId));
        if (string.IsNullOrEmpty(importAgents))
            throw new ArgumentException("ConfirmationId is not valid.", nameof(confirmationId));
        var importAgentsList = importAgents.JsonGet<List<AgentExportModel>>();
        await ImportAgentsConfirmed(importAgentsList);
    }

    private string GetAgentImportCacheKey(string confirmationId) => $"agentImport-{confirmationId}";

    private async Task ImportAgentsConfirmed(List<AgentExportModel> agentExportModels)
    {
        var agents = (await _agentsProcessor.List()).Select(agent => agent.Name).ToList();
        var nonCompositeAgents = agentExportModels.Where(agent => agent.Type != Models.ViewModels.AgentType.Composite.ToString()).ToList();
        var compositeAgents = agentExportModels.Where(agent => agent.Type == Models.ViewModels.AgentType.Composite.ToString()).ToList();
        foreach (var agent in nonCompositeAgents)
        {
            await ImportAgentConfirmed(agent);
            agents.Add(agent.Name);
        }
        var lastLoopWasNonEmpty = true;
        while(compositeAgents.Count > 0 && lastLoopWasNonEmpty)
        {
            lastLoopWasNonEmpty = false;
            foreach (var agent in compositeAgents.ToList())
            {
                var subAgentNames = agent.Content[AgentContentParameters.AgentsList].Value.JsonGet<List<string>>();
                // Check if all subagents are present in the system
                if (subAgentNames.Any(subAgent => !agents.Contains(subAgent)))
                    continue;

                var subAgentsIds = (await _agentsProcessor.List())
                    .Where(subAgent => subAgentNames.Contains(subAgent.Name))
                    .Select(subAgent => subAgent.AgentId)
                    .Distinct()
                    .ToDictionary(key => key, value => true);
                agent.Content[AgentContentParameters.AgentsList].Value = subAgentsIds.ToJson();

                await ImportAgentConfirmed(agent);
                agents.Add(agent.Name);
                compositeAgents.Remove(agent);
                lastLoopWasNonEmpty = true;
            }
        }
    }

    private async Task ImportAgentConfirmed(AgentExportModel agentExportModel)
    {
        var agentModel = _mapper.Map<AgentModel>(agentExportModel);
        var agent = await _agentsProcessor.GetByName(agentExportModel.Name);
        if (agent == null)
        {
            await _agentsProcessor.Add(agentModel);
        }
        else
        {
            agentModel.AgentId = agent.AgentId;
            await _agentsProcessor.Update(agentModel);
        }
    }
}

public interface IAgentsService
{
    Task<List<AgentViewModel>> ListAgents();
    Task<AgentViewModel> AddAgent(AgentViewModel agentViewModel);
    Task<AgentViewModel> UpdateAgent(AgentViewModel agentViewModel);
    Task DeleteAgent(int agentId); 
    Task<List<ParameterModel>?> GetParameters(int agentId); 
    Task SwitchEnableAgent(int agentId, bool isEnabled);
    Task<bool> IsAgentEnabled(string agentName);
    Task<byte[]> ExportAgents(List<int> agentIdsList);
    Task<ImportAgentsResultModel> ImportAgents(IFormFile file, Dictionary<Models.ViewModels.AgentType, int> agentVersions);
    Task ConfirmImportAgents(string confirmationId);
}