using System;
using AutoMapper;
using Stratego.Domain;

namespace Stratego.Api.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<User, UserModel>();
            }
        }
    }
}