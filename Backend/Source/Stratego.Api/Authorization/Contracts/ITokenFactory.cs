using System.Collections.Generic;
using System.Security.Claims;
using Stratego.Domain;

namespace Stratego.Api.Authorization.Contracts
{
    //DO NOT TOUCH THIS FILE!!

    public interface ITokenFactory
    {
        string CreateToken(User user, IList<Claim> currentUserClaims);
    }
}