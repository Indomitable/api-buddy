using System;
using DomainHttpMethod = Api.Buddy.Main.Logic.Models.HttpMethod;
using SystemHttpMethod = System.Net.Http.HttpMethod;

namespace Api.Buddy.Main.Logic.Services;

internal interface IMapper
{
    SystemHttpMethod ToSystemHttpMethod(DomainHttpMethod httpMethod);
}

internal sealed class Mapper : IMapper
{
    public SystemHttpMethod ToSystemHttpMethod(DomainHttpMethod httpMethod)
    {
        return httpMethod switch
        {
            DomainHttpMethod.GET => SystemHttpMethod.Get,
            DomainHttpMethod.POST => SystemHttpMethod.Post,
            DomainHttpMethod.PUT => SystemHttpMethod.Put,
            DomainHttpMethod.DELETE => SystemHttpMethod.Delete,
            DomainHttpMethod.PATCH => SystemHttpMethod.Patch,
            DomainHttpMethod.HEAD => SystemHttpMethod.Head,
            DomainHttpMethod.OPTIONS => SystemHttpMethod.Options,
            _ => throw new NotSupportedException($"HttpMethod is not supported. [Method: {httpMethod}]")
        };
    }
}
