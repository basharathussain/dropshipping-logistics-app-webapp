using Logistics.Application.Abstractions;
using Logistics.Application.Abstractions.Ai;
using Logistics.Shared.Models;

namespace Logistics.Application.Modules.Platform.AiSettings.Commands;

internal sealed class TestAiKeyHandler(ILlmClient llmClient)
    : IAppRequestHandler<TestAiKeyCommand, Result<AiKeyTestResultDto>>
{
    public async Task<Result<AiKeyTestResultDto>> Handle(TestAiKeyCommand req, CancellationToken ct)
    {
        var result = await llmClient.TestConnectionAsync(req.Model, req.ApiKey, ct);
        if (!result.IsSuccess)
            return Result<AiKeyTestResultDto>.Fail(result.Error!);

        var r = result.Value!;
        return Result<AiKeyTestResultDto>.Ok(new AiKeyTestResultDto
        {
            Valid = r.Valid,
            Message = r.Message,
            Model = r.Model
        });
    }
}
