using System;
using System.Collections.Generic;

namespace Stratego.Domain.BoardDomain
{
    /// <summary>
    /// A coordinate (Row, Column) on a game board.
    /// </summary>
    public class BoardCoordinate
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public BoardCoordinate() //Do not change this constructor (needed in the Api layer)
        {
            Row = 0;
            Column = 0;
        }

        public BoardCoordinate(int row, int column)
        {

        }

        public bool IsOutOfBounds(int boardSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of coordinates that indicate how to move to a target coordinate in a straight line.
        /// If there is no horizontal or vertical path to the target, then an empty list is returned.
        /// The start coordinate (this) is not included. The target coordinate is included in the resulting path.
        /// </summary>
        public IList<BoardCoordinate> GetStraightPathTo(BoardCoordinate target)
        {
            throw new NotImplementedException();
        }

        #region overrides
        //DO NOT TOUCH THIS METHODS IN THIS REGION!

        public override string ToString()
        {
            return $"({Row},{Column})";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BoardCoordinate);
        }

        protected bool Equals(BoardCoordinate other)
        {
            if (ReferenceEquals(other, null)) return false;
            return Row == other.Row && Column == other.Column;
        }

        public static bool operator ==(BoardCoordinate a, BoardCoordinate b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(BoardCoordinate a, BoardCoordinate b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        #endregion
    }
}