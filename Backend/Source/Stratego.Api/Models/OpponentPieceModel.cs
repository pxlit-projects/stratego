using AutoMapper;
using Stratego.Domain.ArmyDomain.Contracts;

namespace Stratego.Api.Models
{
    public class OpponentPieceModel
    {
        public BoardCoordinateModel Position { get; set; }
        public bool IsAlive { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<IPiece, OpponentPieceModel>();
            }
        }
    }
}