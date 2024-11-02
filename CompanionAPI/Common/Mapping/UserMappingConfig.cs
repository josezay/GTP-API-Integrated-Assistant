using CompanionAPI.Contracts.OnboardingContracts;
using CompanionAPI.Entities;
using Mapster;

namespace CompanionAPI.Common.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserOnboardingResponse>()
            .Map(dest => dest.Calories, src => src.Goals.Last().Calories)
            .Map(dest => dest.Proteins, src => src.Goals.Last().Proteins);

    }
}
