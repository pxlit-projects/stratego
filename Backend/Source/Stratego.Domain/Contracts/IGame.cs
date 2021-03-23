using System;
using Stratego.Common;
using Stratego.Domain.BoardDomain;
using Stratego.Domain.BoardDomain.Contracts;

namespace Stratego.Domain.Contracts
{
    public interface IGame
    {
        Guid Id { get; }
        IPlayer RedPlayer { get; }
        IPlayer BluePlayer { get; }
        IBoard Board { get; }

        /// <summary>
        /// Indicates if the game is started.
        /// This is the case when both players have indicated that they are ready to start.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// Indicates if the game is over.
        /// This is the case when an army of at least one of the players is defeated (It is possible that both armies are defeated). 
        /// </summary>
        bool IsOver { get; }

        /// <summary>
        /// Contains the last move that was made.
        /// If no move was made yet, null is returned.
        /// </summary>
        Move LastMove { get; }

        /// <summary>
        /// Retrieves a player by id and tries to mark the player as 'ready'.
        /// </summary>
        /// <param name="playerId">The identifier of the player.</param>
        void SetPlayerReady(Guid playerId);

        IPlayer GetPlayerById(Guid playerId);
        IPlayer GetOpponent(IPlayer player);

        /// <summary>
        /// Tries to position a piece on the board.
        /// This method is used when the game is not started yet and the players are positioning their army pieces on the board.
        /// </summary>
        /// <param name="playerId">The identifier of the player.</param>
        /// <param name="pieceId">The identifier of the piece.</param>
        /// <param name="targetCoordinate">The target coordinate on the board on which the piece should be positioned.</param>
        /// <returns>
        /// A success result if the positioning succeeded.
        /// A failure result (with a reason) if the positioning did not succeed.
        /// </returns>
        Result PositionPiece(Guid playerId, Guid pieceId, BoardCoordinate targetCoordinate);

        /// <summary>
        /// Tries to move a piece on the board.
        /// This method is used when the game is started and a player wants to make a move.
        /// </summary>
        /// <param name="playerId">The identifier of the player.</param>
        /// <param name="pieceId">The identifier of the piece.</param>
        /// <param name="targetCoordinate">The target coordinate on the board to which the piece should be moved.</param>
        /// <returns>
        /// A move object that contains details about the move (if the move succeeded).
        /// A failure result (with a reason) if the move did not succeed.
        /// </returns>
        Result<Move> MovePiece(Guid playerId, Guid pieceId, BoardCoordinate targetCoordinate);

        /// <summary>
        /// Indicates of a player can make a move.
        /// The <see cref="LastMove"/> should be used to determine if a player is allowed to move.
        /// </summary>
        /// <remarks>When no move has been made yet, it is the red player's turn.</remarks>
        bool IsPlayersTurn(Guid playerId);
    }
}