using Standards.POC.Retry.Api.Services;

var scriptGenerationService = new ScriptGenerationService();
scriptGenerationService.GenerateBuildScript();
scriptGenerationService.GenerateProvisionScript();