using System;
using Stratego.Api.Models;
using Stratego.TestTools.Builders;

namespace Stratego.Api.Tests.Builders
{
    public class MoveModelBuilder
    {
        private readonly MoveModel _model;

        public MoveModelBuilder()
        {
            _model = new MoveModel
            {
                PieceId = Guid.NewGuid(),
                TargetCoordinate = new BoardCoordinateBuilder().Build()
            };
        }

        public MoveModel Build()
        {
            return _model;
        }
    }
}