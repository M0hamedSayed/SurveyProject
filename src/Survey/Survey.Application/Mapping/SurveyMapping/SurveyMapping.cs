using Mapster;

namespace Survey.Application.Mapping.SurveyMapping
{
    public partial class SurveyMapping : IRegister
    {

        public void Register(TypeAdapterConfig config)
        {
            AddSurveyMapping(config);
            GetSurveyDetailsMapping(config);
        }
    }
}
