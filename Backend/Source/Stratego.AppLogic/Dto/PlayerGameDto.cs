using System;
using System.Collections.Generic;
using Stratego.Domain.BoardDomain;

namespace Stratego.AppLogic.Dto
{
    /// <summary>
    /// Contains all info about a game that a player may need.
    /// Sensitive data about the opponent is excluded.
    /// </summary>
    public class PlayerGameDto
    {
        /// <summary>
        /// Unique identifier of the game.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Indicates that the player's army is positioned and that the player has marked himself as 'Ready'
        /// </summary>
        public bool OwnPlayerIsReady { get; set; }

        /// <summary>
        /// Indicates if the player's color is red.
        /// </summary>
        public bool OwnColorIsRed { get; set; }

        /// <summary>
        /// The pieces of the player's army that are alive
        /// </summary>
        public IList<PieceDto> OwnLivingPieces { get; set; }

        /// <summary>
        /// The pieces of the player's army that are defeated in battle
        /// </summary>
        public IList<PieceDto> OwnFallenPieces { get; set; }

        /// <summary>
        /// Indicates if the army of the player is defeated (flag captured / no moveable pieces anymore).
        /// </summary>
        public bool OwnArmyIsDefeated { get; set; }

        /// <summary>
        /// Indicates that the opponent army is positioned and that the opponent has marked himself as 'Ready'
        /// </summary>
        public bool OpponentIsReady { get; set; }

        /// <summary>
        /// The coordinates of the pieces of the opponent's army that are alive
        /// </summary>
        public IList<BoardCoordinate> OpponentLivingPieceCoordinates { get; set; }

        /// <summary>
        /// The pieces of the opponent's army that are defeated in battle
        /// </summary>
        public IList<PieceDto> OpponentFallenPieces { get; set; }

        /// <summary>
        /// Indicates if the army of the opponent is defeated (flag captured / no moveable pieces anymore).
        /// </summary>
        public bool OpponentArmyIsDefeated { get; set; }

        /// <summary>
        /// Indicates if both players have marked themselves as 'Ready'.
        /// </summary>
        public bool IsStarted { get; set; }

        /// <summary>
        /// Indicates if the player can move a piece
        /// If false, then the player must wait for the opponent to make a move
        /// </summary>
        public bool IsYourTurn { get; set; }

        /// <summary>
        /// Indicates that at least one of the 2 armies is defeated
        /// </summary>
        /// <remarks>
        /// It is possible that both armies are defeated (e.g. last move killed the last moveable piece of both armies).
        /// So a game ends if one of the armies is defeated or when both armies are defeated.
        /// </remarks>
        public bool IsOver { get; set; }
    }
}