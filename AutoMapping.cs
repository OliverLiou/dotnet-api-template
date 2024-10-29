using AutoMapper;
using TemplateApi.Models;
// using TemplateApi.ViewModels;

namespace TemplateApi
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            #region  --Model--
            // CreateMap<, >().ReverseMap();
            #endregion

            #region  --ModelLog--
            CreateMap<Table1, Table1Log>().ReverseMap();
            #endregion
        }
    }
}