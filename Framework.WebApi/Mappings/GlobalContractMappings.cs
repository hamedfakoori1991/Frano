using Framework.Domain.Entities;
using Framework.WebApi.Contracts;
using Mapster;

namespace Framework.WebApi.Mappings;

public partial class GlobalContractMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType(typeof(Result<>), typeof(ApiResult<>))
            .MapToConstructor(true);

        config.ForType<Message, MessageResponse>()
            .Map(z => z.Type, z => z.Code)
            .Map(z => z.Title, z => z.Text)
            .Map(z => z.Detail, z => z.Description)
            .Map(z => z.MessageType, z => z.Type);

    }
}
