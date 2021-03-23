using AutoMapper;
using Stratego.Domain.BoardDomain;

namespace Stratego.Api.Models
{
    public class BoardCoordinateModel
    {
        public int Row { get; set; }
        public int Column { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<BoardCoordinateModel, BoardCoordinate>()
                    .ConstructUsing(model => new BoardCoordinate(model.Row, model.Column));
                CreateMap<BoardCoordinate, BoardCoordinateModel>();
            }
        }
    }
}