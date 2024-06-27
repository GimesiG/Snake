
using System;
using System.Collections.Generic;

namespace Snake
{
    public class Position
    {
        public int row { get;}
        public int column { get;}

        public  Position(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public Position translate(Direction dir)
        {
            return new Position(row + dir.rowOffset, column + dir.colOffset);
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   row == position.row &&
                   column == position.column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(row, column);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
